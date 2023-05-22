using System.Linq.Expressions;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using CRM.Application.Common.Interfaces;
using CRM.Application.Common.Security;
using CRM.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using static CRM.Application.Constants;

namespace CRM.Application.Companies.Queries.GetCompanies;

[Authorize]
public record GetCompaniesQuery : IRequest<CompanyDto[]>;

public class GetCompaniesRequestHandler : IRequestHandler<GetCompaniesQuery, CompanyDto[]>
{
    private readonly IApplicationDbContext _dbContext;
    private readonly IMapper _mapper;
    private readonly ICurrentUserService _currentUserService;
    private readonly IAccessService _accessService;
    private readonly IPermissionsVerifier _permissionsVerifier;

    public GetCompaniesRequestHandler(IApplicationDbContext dbContext, IMapper mapper, ICurrentUserService currentUserService, IAccessService accessService, IPermissionsVerifier permissionsVerifier)
    {
        _currentUserService = currentUserService;
        _accessService = accessService;
        _permissionsVerifier = permissionsVerifier;
        _dbContext = dbContext;
        _mapper = mapper;
    }

    public async Task<CompanyDto[]> Handle(GetCompaniesQuery request, CancellationToken cancellationToken)
    {
        var accessRights = await _accessService.CheckAccessAsync(_currentUserService.UserId!);
        if (!accessRights.Any())
        {
            return Array.Empty<CompanyDto>();
        }

        var expressions = GetExpressions(accessRights);
        if (!expressions.Any())
        {
            return Array.Empty<CompanyDto>();
        }

        var query = _dbContext.Companies.AsNoTracking();
        foreach (var expression in expressions)
        {
            query = query.Where(expression);
        }

        var result = await query
            .ProjectTo<CompanyDto>(_mapper.ConfigurationProvider)
            .ToArrayAsync(cancellationToken);

        var permissions = await _permissionsVerifier.VerifyCompanyPermissionsAsync(_currentUserService.UserId!, result.Select(x => x.Id).ToArray(), new[] { Permissions.Company.Update, Permissions.Company.Delete });
        foreach (var item in result)
        {
            if (!permissions.ContainsKey(item.Id))
            {
                continue;
            }

            item.CanBeEdited = permissions[item.Id].Contains(Permissions.Company.Update);
            item.CanBeDeleted = permissions[item.Id].Contains(Permissions.Company.Delete);
        }

        return result;
    }

    private List<Expression<Func<Company, bool>>> GetExpressions(string[] accessRights)
    {
        var result = new List<Expression<Func<Company, bool>>>();
        if (accessRights.Contains(Access.Company.Any.View))
        {
            result.Add(x => true);
            return result;
        }

        if (accessRights.Contains(Access.Company.WhereUserIsManager.View))
        {
            result.Add(x => x.ManagerId == _currentUserService.UserId);
        }

        return result;
    }
}