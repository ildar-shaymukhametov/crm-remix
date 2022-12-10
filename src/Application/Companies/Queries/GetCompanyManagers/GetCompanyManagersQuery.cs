using System.Linq.Expressions;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using CRM.Application.Common.Exceptions;
using CRM.Application.Common.Interfaces;
using CRM.Application.Common.Security;
using CRM.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

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
            .Where(x => x.Id == request.Id)
            .Select(x => new { x.ManagerId })
            .FirstOrDefaultAsync(cancellationToken);

        if (company == null)
        {
            throw new NotFoundException("Company", request.Id);
        }

        var accessRightsToCheck = new[]
        {
            Constants.Access.SetManagerToSelfFromNone,
            Constants.Access.SetManagerToAnyFromNone
        };

        var accessRights = await _accessService.CheckAccessAsync(_currentUserService.UserId!, accessRightsToCheck);
        if (!accessRights.Any())
        {
            return new GetCompanyManagersResponse();
        }

        var expressions = new List<Expression<Func<ApplicationUser, bool>>>();
        if (company.ManagerId == null && !accessRights.Contains(Constants.Access.SetManagerToAnyFromNone))
        {
            if (accessRights.Contains(Constants.Access.SetManagerToSelfFromNone))
            {
                expressions.Add(x => x.Id == _currentUserService.UserId);
            }
        }

        var query = _dbContext.ApplicationUsers.AsNoTracking();
        foreach (var expression in expressions)
        {
            query = query.Where(expression);
        }

        var managers = await query
            .ProjectTo<ManagerDto>(_mapper.ConfigurationProvider)
            .ToArrayAsync(cancellationToken);

        return new GetCompanyManagersResponse
        {
            Managers = managers
        };
    }
}
