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

        var expressions = await GetExpressionsAsync(request, accessRights);
        if (!expressions.Any())
        {
            return new GetCompanyInitDataResponse();
        }

        return await BuildResponseAsync(expressions, cancellationToken);
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

    private async Task<List<Expression<Func<ApplicationUser, bool>>>> GetExpressionsAsync(GetCompanyInitDataQuery request, string[] accessRights)
    {
        var result = new List<Expression<Func<ApplicationUser, bool>>>();
        if (accessRights.Contains(Access.Company.Any.SetManagerFromAnyToAny))
        {
            result.Add(_allManagers);
            return result;
        }

        var managerId = await GetManagerIdAsync(request);

        if (accessRights.Contains(Access.Company.Any.SetManagerFromNoneToAny) && managerId == null
            || accessRights.Contains(Access.Company.Any.SetManagerFromSelfToAny) && managerId == _currentUserService.UserId)
        {
            result.Add(_allManagers);
            return result;
        }

        if (accessRights.Contains(Access.Company.Any.SetManagerFromAnyToSelf) && managerId != null
            || accessRights.Contains(Access.Company.Any.SetManagerFromNoneToSelf) && managerId == null
            || accessRights.Contains(Access.Company.SetManagerFromSelf) && managerId == _currentUserService.UserId)
        {
            result.Add(x => x.Id == _currentUserService.UserId);
        }

        if (accessRights.Contains(Access.Company.SetManagerFromAny) && managerId != null)
        {
            result.Add(x => x.Id == managerId);
        }

        if (accessRights.Contains(Access.Company.Any.SetManagerFromSelfToNone) && managerId == _currentUserService.UserId
            || accessRights.Contains(Access.Company.SetManagerFromNone) && managerId == null
            || accessRights.Contains(Access.Company.Any.SetManagerFromAnyToNone) && managerId != null)
        {
            result.Add(_emptyManager);
        }

        return result;
    }

    private async Task<GetCompanyInitDataResponse> BuildResponseAsync(List<Expression<Func<ApplicationUser, bool>>> expressions, CancellationToken cancellationToken)
    {
        var managers = new List<ManagerDto>();
        if (expressions.Contains(_emptyManager) || expressions.Contains(_allManagers))
        {
            expressions.Remove(_emptyManager);
            managers.Add(new ManagerDto
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
            .ToListAsync(cancellationToken);

        managers.AddRange(users);

        return new GetCompanyInitDataResponse
        {
            Managers = managers
        };
    }
}
