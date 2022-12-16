using System.Linq.Expressions;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using CRM.Application.Common.Exceptions;
using CRM.Application.Common.Interfaces;
using CRM.Application.Common.Security;
using CRM.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using static CRM.Application.Constants;

namespace CRM.Application.Companies.Queries.GetCompanyManagers;

[Authorize]
public class GetCompanyManagersQuery : IRequest<GetCompanyManagersResponse>
{
    public GetCompanyManagersQuery(int id)
    {
        Id = id;
    }

    public int Id { get; set; }
}

public class GetCompanyManagersRequestHandler : IRequestHandler<GetCompanyManagersQuery, GetCompanyManagersResponse>
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

    public async Task<GetCompanyManagersResponse> Handle(GetCompanyManagersQuery request, CancellationToken cancellationToken)
    {
        var company = await _dbContext.Companies
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken);

        if (company == null)
        {
            throw new NotFoundException(nameof(Company), request.Id);
        }

        var accessRights = await _accessService.CheckAccessAsync(_currentUserService.UserId!);
        if (!accessRights.Any())
        {
            return new GetCompanyManagersResponse();
        }

        var query = _dbContext.ApplicationUsers.AsNoTracking();
        if (accessRights.Contains(Access.Company.Any.SetManagerFromAnyToAny))
        {
            return await BuildResponseAsync(query, true, cancellationToken);
        }

        var expressions = GetExpressions(company, accessRights, _currentUserService.UserId!);
        if (!expressions.Any())
        {
            return new GetCompanyManagersResponse();
        }

        foreach (var expression in expressions)
        {
            query = query.Where(expression);
        }

        return await BuildResponseAsync(query, false, cancellationToken);
    }

    private static List<Expression<Func<ApplicationUser, bool>>> GetExpressions(Company company, string[] accessRights, string userId)
    {
        var expressions = new List<Expression<Func<ApplicationUser, bool>>>();
        if (company.ManagerId == null && accessRights.Contains(Access.Company.Any.SetManagerFromNoneToSelf))
        {
            expressions.Add(x => x.Id == userId);
        }
        if (company.ManagerId != null && accessRights.Contains(Access.Company.Any.SetManagerFromAnyToSelf))
        {
            expressions.Add(x => x.Id == userId);
        }

        return expressions;
    }

    private async Task<GetCompanyManagersResponse> BuildResponseAsync(IQueryable<ApplicationUser> query, bool includeNullManager, CancellationToken cancellationToken)
    {
        var managers = await query
            .ProjectTo<ManagerDto>(_mapper.ConfigurationProvider)
            .ToListAsync(cancellationToken);
            
        if (includeNullManager)
        {
            managers.Add(new ManagerDto());
        }

        return new GetCompanyManagersResponse
        {
            Managers = managers
        };
    }
}
