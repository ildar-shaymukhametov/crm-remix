using System.Linq.Expressions;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using CRM.Application.Common.Interfaces;
using CRM.Application.Common.Security;
using CRM.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using static CRM.Application.Constants;
using CRM.Application.Utils;
using CRM.Application.Common.Mappings;
using CRM.Application.Common.Extensions;

namespace CRM.Application.Companies.Queries.GetCompanyManagers;

[Authorize]
public record GetCompanyInitDataQuery : IRequest<GetCompanyInitDataResponse>
{
    public int? Id { get; set; }
}

public class GetCompanyManagersRequestHandler : IRequestHandler<GetCompanyInitDataQuery, GetCompanyInitDataResponse>
{
    private readonly IAccessService _accessService;
    private readonly ICurrentUserService _currentUserService;
    private readonly IApplicationDbContext _dbContext;
    private readonly IMapper _mapper;
    private readonly Expression<Func<ApplicationUser, bool>> _emptyManager = x => x.Id == null;
    private readonly Expression<Func<ApplicationUser, bool>> _allManagers = x => true;

    public GetCompanyManagersRequestHandler(IAccessService accessService, ICurrentUserService currentUserService, IApplicationDbContext dbContext, IMapper mapper)
    {
        _accessService = accessService;
        _currentUserService = currentUserService;
        _dbContext = dbContext;
        _mapper = mapper;
    }

    public async Task<GetCompanyInitDataResponse> Handle(GetCompanyInitDataQuery request, CancellationToken cancellationToken)
    {
        var accessRights = await _accessService.CheckAccessAsync(_currentUserService.UserId!);
        if (!accessRights.Any())
        {
            return new GetCompanyInitDataResponse();
        }

        return new GetCompanyInitDataResponse
        {
            Managers = await GetManagersAsync(request, accessRights),
            CompanyTypes = await GetCompanyTypesAsync()
        };
    }

    private async Task<List<CompanyTypeDto>> GetCompanyTypesAsync()
    {
        return await _dbContext.CompanyTypes.ProjectToListAsync<CompanyTypeDto>(_mapper.ConfigurationProvider);
    }

    private async Task<string?> GetManagerIdAsync(GetCompanyInitDataQuery request)
    {
        return request.Id is not null
            ? await _dbContext.Companies
                .Where(x => x.Id == request.Id)
                .Select(x => x.ManagerId)
                .FirstOrDefaultAsync()
            : null;
    }

    private async Task<List<Expression<Func<ApplicationUser, bool>>>> GetManagerExpressionsAsync(GetCompanyInitDataQuery request, string[] accessRights)
    {
        var result = new List<Expression<Func<ApplicationUser, bool>>>();
        if (accessRights.Contains(Access.Company.Any.Manager.SetFromAnyToAny))
        {
            result.Add(_allManagers);
            return result;
        }

        var managerId = await GetManagerIdAsync(request);

        if (accessRights.ContainsAny(Access.Company.Any.Manager.SetFromNoneToAny, Access.Company.Any.Manager.SetFromAnyToAny) && managerId == null
            || accessRights.Contains(Access.Company.Any.Manager.SetFromSelfToAny) && managerId == _currentUserService.UserId)
        {
            result.Add(_allManagers);
            return result;
        }
        if (accessRights.Contains(Access.Company.Any.Manager.SetFromAnyToSelf) && managerId != null
            || accessRights.ContainsAny(Access.Company.Any.Manager.SetFromNoneToSelf,
                Access.Company.Any.Manager.SetFromNoneToAny,
                Access.Company.Any.Manager.SetFromAnyToSelf,
                Access.Company.Any.Manager.SetFromAnyToAny) && managerId == null
            || accessRights.ContainsAny(Access.Company.Any.Manager.SetFromSelfToAny,
                Access.Company.Any.Manager.SetFromAnyToAny,
                Access.Company.Any.Manager.SetFromSelfToNone,
                Access.Company.Any.Manager.SetFromAnyToNone) && managerId == _currentUserService.UserId)
        {
            result.Add(x => x.Id == _currentUserService.UserId);
        }

        if (accessRights.ContainsAny(Access.Company.Any.Manager.SetFromAnyToNone,
                Access.Company.Any.Manager.SetFromAnyToSelf,
                Access.Company.Any.Manager.SetFromAnyToAny) && managerId != null)
        {
            result.Add(x => x.Id == managerId);
        }

        if (accessRights.ContainsAny(Access.Company.Any.Manager.SetFromSelfToAny,
                Access.Company.Any.Manager.SetFromAnyToAny,
                Access.Company.Any.Manager.SetFromSelfToNone,
                Access.Company.Any.Manager.SetFromAnyToNone) && managerId == _currentUserService.UserId
            || accessRights.ContainsAny(Access.Company.Any.Manager.SetFromNoneToAny,
                Access.Company.Any.Manager.SetFromNoneToSelf,
                Access.Company.Any.Manager.SetFromAnyToAny,
                Access.Company.Any.Manager.SetFromAnyToNone,
                Access.Company.Any.Manager.SetFromAnyToSelf) && managerId == null
            || accessRights.Contains(Access.Company.Any.Manager.SetFromAnyToNone) && managerId != null)
        {
            result.Add(_emptyManager);
        }

        return result;
    }

    private async Task<List<ManagerDto>> GetManagersAsync(GetCompanyInitDataQuery request, string[] accessRights)
    {
        var result = new List<ManagerDto>();

        var expressions = await GetManagerExpressionsAsync(request, accessRights);
        if (!expressions.Any())
        {
            return result;
        }

        if (expressions.Contains(_emptyManager) || expressions.Contains(_allManagers))
        {
            expressions.Remove(_emptyManager);
            result.Add(new ManagerDto
            {
                Id = string.Empty
            });
        }

        var expression = expressions.Aggregate(PredicateBuilder.False<ApplicationUser>(), (acc, v) => acc.Or(v));
        var users = await _dbContext.ApplicationUsers
            .AsNoTracking()
            .Where(expression)
            .OrderBy(x => x.LastName)
            .ProjectTo<ManagerDto>(_mapper.ConfigurationProvider)
            .ToListAsync();

        result.AddRange(users);

        return result;
    }
}
