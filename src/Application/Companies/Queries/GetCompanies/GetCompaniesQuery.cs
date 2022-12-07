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
    private readonly IPermissionsService _permissionsService;
    private readonly IPermissionsVerifier _permissionsVerifier;

    public GetCompaniesRequestHandler(IApplicationDbContext dbContext, IMapper mapper, ICurrentUserService currentUserService, IPermissionsService permissionsService, IPermissionsVerifier permissionsVerifier)
    {
        _currentUserService = currentUserService;
        _permissionsService = permissionsService;
        _permissionsVerifier = permissionsVerifier;
        _dbContext = dbContext;
        _mapper = mapper;
    }

    public async Task<CompanyDto[]> Handle(GetCompaniesQuery request, CancellationToken cancellationToken)
    {
        var accessRightsToCheck = new[]
        {
            Access.ViewAnyCompany,
            Access.ViewOwnCompany
        };

        var accessRights = await _permissionsService.CheckAccessAsync(_currentUserService.UserId!, accessRightsToCheck);
        if (!accessRights.Any())
        {
            return Array.Empty<CompanyDto>();
        }

        var expressions = new List<Expression<Func<Company, bool>>>();
        if (!accessRights.Contains(Access.ViewAnyCompany))
        {
            if (accessRights.Contains(Access.ViewOwnCompany))
            {
                expressions.Add(x => x.ManagerId == _currentUserService.UserId);
            }
        }

        var query = _dbContext.Companies.AsNoTracking();
        foreach (var expression in expressions)
        {
            query = query.Where(expression);
        }

        var result = await query
            .ProjectTo<CompanyDto>(_mapper.ConfigurationProvider)
            .ToArrayAsync(cancellationToken);

        var permissions = await _permissionsVerifier.VerifyCompanyPermissionsAsync(_currentUserService.UserId!, result.Select(x => x.Id).ToArray(), new [] { Permissions.UpdateCompany, Permissions.DeleteCompany });
        foreach (var item in result)
        {
            if (!permissions.ContainsKey(item.Id))
            {
                continue;
            }

            item.CanBeEdited = permissions[item.Id].Contains(Permissions.UpdateCompany);
            item.CanBeDeleted = permissions[item.Id].Contains(Permissions.DeleteCompany);
        }

        return result;
    }
}