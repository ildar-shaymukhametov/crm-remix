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

        var expression = await GetExpressionsAsync(accessRights, _currentUserService.UserId!, request.Id);
        var query = _dbContext.ApplicationUsers.AsNoTracking().Where(expression);
        var includeNullManager = accessRights.Contains(Access.Company.SetManagerToOrFromNone);

        return await BuildResponseAsync(query, includeNullManager, cancellationToken);
    }

    private async Task<Expression<Func<ApplicationUser, bool>>> GetExpressionsAsync(string[] accessRights, string userId, int? companyId)
    {
        if (accessRights.Contains(Access.Company.SetManagerToAny))
        {
            return PredicateBuilder.True<ApplicationUser>();
        }

        var result = PredicateBuilder.False<ApplicationUser>();
        if (accessRights.Contains(Access.Company.SetManagerToSelf))
        {
            result = result.Or(x => x.Id == userId);
        }

        if (companyId is not null)
        {
            if (accessRights.Contains(Access.Company.Old.SetManagerFromAny))
            {
                var managerId = await _dbContext.Companies
                    .Where(x => x.Id == companyId && x.ManagerId != null)
                    .Select(x => x.ManagerId)
                    .FirstOrDefaultAsync();

                result = result.Or(x => x.Id == managerId);
            }
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
