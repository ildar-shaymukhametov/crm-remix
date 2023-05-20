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
public class GetCompanyInitDataQuery : IRequest<GetCompanyInitDataResponse>
{
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

        var query = _dbContext.ApplicationUsers.AsNoTracking();
        if (accessRights.Contains(Access.Company.New.SetManagerToAny))
        {
            return await BuildResponseAsync(query, true, cancellationToken);
        }

        var includeNullManager = accessRights.Contains(Access.Company.New.SetManagerToNone);
        var expressions = GetExpressions(accessRights, _currentUserService.UserId!);
        if (!expressions.Any())
        {
            var result = new GetCompanyInitDataResponse();
            if (includeNullManager)
            {
                result.Managers = new List<ManagerDto>
                {
                    new ManagerDto()
                };
            }

            return result;
        }

        foreach (var expression in expressions)
        {
            query = query.Where(expression);
        }

        return await BuildResponseAsync(query, includeNullManager, cancellationToken);
    }

    private static List<Expression<Func<ApplicationUser, bool>>> GetExpressions(string[] accessRights, string userId)
    {
        var expressions = new List<Expression<Func<ApplicationUser, bool>>>();
        if (accessRights.Contains(Access.Company.New.SetManagerToSelf))
        {
            expressions.Add(x => x.Id == userId);
        }

        return expressions;
    }

    private async Task<GetCompanyInitDataResponse> BuildResponseAsync(IQueryable<ApplicationUser> query, bool includeNullManager, CancellationToken cancellationToken)
    {
        var managers = new List<ManagerDto>();
        if (includeNullManager)
        {
            managers.Add(new ManagerDto());
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