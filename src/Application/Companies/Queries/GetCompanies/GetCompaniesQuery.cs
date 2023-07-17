using System.Linq.Expressions;
using AutoMapper;
using CRM.Application.Common.Extensions;
using CRM.Application.Common.Interfaces;
using CRM.Application.Common.Security;
using CRM.Domain.Entities;
using CRM.Domain.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CRM.Application.Companies.Queries.GetCompanies;

[Authorize]
public record GetCompaniesQuery : IRequest<CompanyVm[]>;

public class GetCompaniesRequestHandler : IRequestHandler<GetCompaniesQuery, CompanyVm[]>
{
    private readonly IApplicationDbContext _dbContext;
    private readonly IMapper _mapper;
    private readonly ICurrentUserService _currentUserService;
    private readonly IAccessService _accessService;
    private readonly IIdentityService _identityService;

    public GetCompaniesRequestHandler(IApplicationDbContext dbContext, IMapper mapper, ICurrentUserService currentUserService, IAccessService accessService, IIdentityService identityService)
    {
        _currentUserService = currentUserService;
        _accessService = accessService;
        _identityService = identityService;
        _dbContext = dbContext;
        _mapper = mapper;
    }

    public async Task<CompanyVm[]> Handle(GetCompaniesQuery request, CancellationToken cancellationToken)
    {
        var accessRights = await _accessService.CheckAccessAsync(_currentUserService.UserId!);
        if (!accessRights.Any())
        {
            return Array.Empty<CompanyVm>();
        }

        var expressions = GetExpressions(accessRights);
        if (!expressions.Any())
        {
            return Array.Empty<CompanyVm>();
        }

        var query = _dbContext.Companies
            .AsNoTracking()
            .Include(x => x.Type)
            .Include(x => x.Manager)
            .AsQueryable();

        foreach (var expression in expressions)
        {
            query = query.Where(expression);
        }

        var result = new List<CompanyVm>();
        var entities = await query.ToArrayAsync(cancellationToken);
        foreach (var entity in entities)
        {
            var company = new CompanyVm
            {
                Id = entity.Id
            };

            if (accessRights.ContainsAny(
                Domain.Constants.Access.Company.Any.Manager.Get,
                Domain.Constants.Access.Company.WhereUserIsManager.Manager.Get,
                Domain.Constants.Access.Company.Any.Manager.SetFromAnyToAny,
                Domain.Constants.Access.Company.Any.Manager.SetFromAnyToNone,
                Domain.Constants.Access.Company.Any.Manager.SetFromAnyToSelf,
                Domain.Constants.Access.Company.Any.Manager.SetFromSelfToAny,
                Domain.Constants.Access.Company.Any.Manager.SetFromSelfToNone,
                Domain.Constants.Access.Company.WhereUserIsManager.Manager.SetFromSelfToNone,
                Domain.Constants.Access.Company.WhereUserIsManager.Manager.SetFromSelfToAny,
                Domain.Constants.Access.Company.Any.Manager.SetFromNoneToAny,
                Domain.Constants.Access.Company.Any.Manager.SetFromNoneToSelf
            ))
            {
                company.Fields.Add(nameof(Company.Manager), _mapper.Map<ManagerDto>(entity.Manager));
            }

            if (accessRights.ContainsAny(
                Domain.Constants.Access.Company.Any.Other.Get,
                Domain.Constants.Access.Company.Any.Other.Set,
                Domain.Constants.Access.Company.WhereUserIsManager.Other.Get,
                Domain.Constants.Access.Company.WhereUserIsManager.Other.Set
            ))
            {
                company.Fields.Add(nameof(Company.Address), entity.Address);
                company.Fields.Add(nameof(Company.Ceo), entity.Ceo);
                company.Fields.Add(nameof(Company.Contacts), entity.Contacts);
                company.Fields.Add(nameof(Company.Email), entity.Email);
                company.Fields.Add(nameof(Company.Inn), entity.Inn);
                company.Fields.Add(nameof(Company.Phone), entity.Phone);
                company.Fields.Add(nameof(Company.Type), _mapper.Map<CompanyTypeDto>(entity.Type));
            }

            if (accessRights.ContainsAny(
                Domain.Constants.Access.Company.Any.Name.Get,
                Domain.Constants.Access.Company.Any.Name.Set,
                Domain.Constants.Access.Company.WhereUserIsManager.Name.Get,
                Domain.Constants.Access.Company.WhereUserIsManager.Name.Set
            ))
            {
                company.Fields.Add(nameof(Company.Name), entity.Name);
            }

            company.CanBeUpdated = await _identityService.AuthorizeAsync(_currentUserService.UserId!, entity, Constants.Policies.Company.Queries.Update);
            company.CanBeDeleted = await _identityService.AuthorizeAsync(_currentUserService.UserId!, entity, Constants.Policies.Company.Queries.Delete);

            result.Add(company);
        }

        return result.ToArray();
    }

    private List<Expression<Func<Company, bool>>> GetExpressions(string[] accessRights)
    {
        var result = new List<Expression<Func<Company, bool>>>();
        if (accessRights.ContainsAny(
            Domain.Constants.Access.Company.Any.Delete,
            Domain.Constants.Access.Company.Any.Other.Get,
            Domain.Constants.Access.Company.Any.Other.Set,
            Domain.Constants.Access.Company.Any.Manager.Get,
            Domain.Constants.Access.Company.Any.Manager.SetFromAnyToAny,
            Domain.Constants.Access.Company.Any.Manager.SetFromAnyToNone,
            Domain.Constants.Access.Company.Any.Manager.SetFromAnyToSelf,
            Domain.Constants.Access.Company.Any.Name.Get,
            Domain.Constants.Access.Company.Any.Name.Set
        ))
        {
            result.Add(x => true);
            return result;
        }

        if (accessRights.ContainsAny(
            Domain.Constants.Access.Company.Any.Manager.SetFromSelfToAny,
            Domain.Constants.Access.Company.Any.Manager.SetFromSelfToNone,
            Domain.Constants.Access.Company.WhereUserIsManager.Delete,
            Domain.Constants.Access.Company.WhereUserIsManager.Other.Get,
            Domain.Constants.Access.Company.WhereUserIsManager.Other.Set,
            Domain.Constants.Access.Company.WhereUserIsManager.Manager.Get,
            Domain.Constants.Access.Company.WhereUserIsManager.Manager.SetFromSelfToNone,
            Domain.Constants.Access.Company.WhereUserIsManager.Manager.SetFromSelfToAny,
            Domain.Constants.Access.Company.WhereUserIsManager.Name.Get,
            Domain.Constants.Access.Company.WhereUserIsManager.Name.Set
        ))
        {
            result.Add(x => x.ManagerId == _currentUserService.UserId);
        }

        if (accessRights.ContainsAny(
            Domain.Constants.Access.Company.Any.Manager.SetFromNoneToAny,
            Domain.Constants.Access.Company.Any.Manager.SetFromNoneToSelf
        ))
        {
            result.Add(x => x.ManagerId == null);
        }

        return result;
    }
}