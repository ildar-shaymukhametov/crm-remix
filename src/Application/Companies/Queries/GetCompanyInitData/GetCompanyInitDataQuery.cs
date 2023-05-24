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

        if (accessRights.Contains(Access.Company.SetManagerToAny))
        {
            var query = _dbContext.ApplicationUsers.AsNoTracking();
            return await BuildResponseAsync(query, true, cancellationToken);
        }
        else if (request.Id is null)
        {
            var expression = GetExpression(accessRights);
            var query = _dbContext.ApplicationUsers.AsNoTracking().Where(expression);

            return await BuildResponseAsync(query, true, cancellationToken);
        }
        else
        {
            var managerId = await _dbContext.Companies
                .Where(x => x.Id == request.Id)
                .Select(x => x.ManagerId)
                .FirstOrDefaultAsync(cancellationToken);

            var expression = GetExpression(accessRights, managerId);
            var query = _dbContext.ApplicationUsers.AsNoTracking().Where(expression);
            var includeEmptyManager = accessRights.Contains(Access.Company.SetManagerToNone) || accessRights.Contains(Access.Company.SetManagerFromNone) && managerId == null;

            return await BuildResponseAsync(query, includeEmptyManager, cancellationToken);
        }
    }

    private Expression<Func<ApplicationUser, bool>> GetExpression(string[] accessRights, string? managerId)
    {
        var result = PredicateBuilder.False<ApplicationUser>();
        if (accessRights.Contains(Access.Company.SetManagerFromAny))
        {
            result = result.Or(x => x.Id == managerId);
        }

        if (accessRights.Contains(Access.Company.SetManagerToSelf) || accessRights.Contains(Access.Company.SetManagerFromSelf) && managerId == _currentUserService.UserId)
        {
            result = result.Or(x => x.Id == _currentUserService.UserId);
        }

        return result;
    }

    private Expression<Func<ApplicationUser, bool>> GetExpression(string[] accessRights)
    {
        if (accessRights.Contains(Access.Company.SetManagerToAny))
        {
            return PredicateBuilder.True<ApplicationUser>();
        }

        var result = PredicateBuilder.False<ApplicationUser>();
        if (accessRights.Contains(Access.Company.SetManagerToSelf))
        {
            result = result.Or(x => x.Id == _currentUserService.UserId);
        }

        return result;
    }

    private async Task<GetCompanyInitDataResponse> BuildResponseAsync(IQueryable<ApplicationUser> query, bool includeEmptyManager, CancellationToken cancellationToken)
    {
        var managers = new List<ManagerDto>();
        if (includeEmptyManager)
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
