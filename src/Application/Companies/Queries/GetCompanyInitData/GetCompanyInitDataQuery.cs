using System.Linq.Expressions;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using CRM.Application.Common.Interfaces;
using CRM.Application.Common.Security;
using CRM.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using static CRM.Application.Constants;

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

        var includeNullManager = new[]
        {
            Access.Company.Any.SetManagerFromNoneToAny,
            Access.Company.Any.SetManagerFromNoneToSelf,
            Access.Company.Any.SetManagerFromAnyToSelf,
            Access.Company.Any.SetManagerFromAnyToAny,
            Access.Company.Any.SetManagerFromAnyToNone,
            Access.Company.Any.SetManagerFromSelfToAny,
            Access.Company.Any.SetManagerFromSelfToNone,
        }.Any(accessRights.Contains);

        var expressions = await GetExpressionsAsync(accessRights, _currentUserService.UserId!, request.Id);
        if (!expressions.Any())
        {
            var result = new GetCompanyInitDataResponse();
            if (includeNullManager)
            {
                result.Managers = new List<ManagerDto>
                {
                    new ManagerDto { Id = string.Empty }
                };
            }

            return result;
        }

        var query = _dbContext.ApplicationUsers.AsNoTracking();
        foreach (var expression in expressions)
        {
            query = query.Where(expression);
        }

        return await BuildResponseAsync(query, includeNullManager, cancellationToken);
    }

    private async Task<List<Expression<Func<ApplicationUser, bool>>>> GetExpressionsAsync(string[] accessRights, string userId, int? companyId)
    {
        var result = new List<Expression<Func<ApplicationUser, bool>>>();

        if (new[]
        {
            Access.Company.Any.SetManagerFromNoneToAny,
            Access.Company.Any.SetManagerFromSelfToAny,
            Access.Company.Any.SetManagerFromAnyToAny
        }.Any(accessRights.Contains))
        {
            result.Add(x => true);
            return result;
        }

        if (new[]
        {
            Access.Company.Any.SetManagerFromNoneToSelf,
            Access.Company.Any.SetManagerFromAnyToSelf,
            Access.Company.Any.SetManagerFromSelfToNone
        }.Any(accessRights.Contains))
        {
            result.Add(x => x.Id == userId);
        }

        if (new[]
        {
            Access.Company.Any.SetManagerFromAnyToNone,
        }.Any(accessRights.Contains) && companyId is not null)
        {
            var managerId = await _dbContext.Companies
                .Where(x => x.Id == companyId && x.ManagerId != null)
                .Select(x => x.ManagerId)
                .FirstOrDefaultAsync();

            result.Add(x => x.Id == managerId);
        }

        return result;
    }

    private async Task<GetCompanyInitDataResponse> BuildResponseAsync(IQueryable<ApplicationUser> query, bool includeNullManager, CancellationToken cancellationToken)
    {
        var managers = new List<ManagerDto>();
        if (includeNullManager)
        {
            managers.Add(new ManagerDto
            {
                Id = string.Empty
            });
        }

        var users = await query
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
