using System.Linq.Expressions;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using CRM.Application.Common.Extensions;
using CRM.Application.Common.Interfaces;
using CRM.Application.Common.Mappings;
using CRM.Application.Common.Security;
using CRM.Application.Utils;
using CRM.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CRM.Application.Companies.Queries.GetUpdateCompany;

[Authorize(Constants.Policies.Company.Queries.Update)]
public record GetUpdateCompanyQuery(int Id) : IRequest<UpdateCompanyVm>
{
}

public class GetUpdateCompanyRequestHandler : IRequestHandler<GetUpdateCompanyQuery, UpdateCompanyVm>
{
    private readonly IMapper _mapper;
    private readonly IAccessService _accessService;
    private readonly ICurrentUserService _currentUserService;
    private readonly IApplicationDbContext _dbContext;
    private readonly Expression<Func<ApplicationUser, bool>> _emptyManager = x => x.Id == null;
    private readonly Expression<Func<ApplicationUser, bool>> _allManagers = x => true;

    public GetUpdateCompanyRequestHandler(IApplicationDbContext dbContext, IMapper mapper, IAccessService accessService, ICurrentUserService currentUserService)
    {
        _dbContext = dbContext;
        _mapper = mapper;
        _accessService = accessService;
        _currentUserService = currentUserService;
    }

    public async Task<UpdateCompanyVm> Handle(GetUpdateCompanyQuery request, CancellationToken cancellationToken)
    {
        var result = new UpdateCompanyVm(request.Id);
        var company = _dbContext.Companies.Single(x => x.Id == request.Id);
        var accessRights = await _accessService.CheckAccessAsync(_currentUserService.UserId!);
        if (accessRights.ContainsAny(
            Constants.Access.Company.Any.Name.Set,
            Constants.Access.Company.WhereUserIsManager.Name.Set
        ))
        {
            result.Fields.Add(nameof(Company.Name), company.Name);
        }

        if (accessRights.ContainsAny(
            Constants.Access.Company.Any.Other.Set,
            Constants.Access.Company.WhereUserIsManager.Other.Set
        ))
        {
            result.Fields.Add(nameof(Company.Address), company.Address);
            result.Fields.Add(nameof(Company.Ceo), company.Ceo);
            result.Fields.Add(nameof(Company.Contacts), company.Contacts);
            result.Fields.Add(nameof(Company.Email), company.Email);
            result.Fields.Add(nameof(Company.Inn), company.Inn);
            result.Fields.Add(nameof(Company.Phone), company.Phone);
            result.Fields.Add(nameof(Company.TypeId), company.TypeId);
            result.InitData.CompanyTypes = await _dbContext.CompanyTypes.ProjectToListAsync<CompanyTypeDto>(_mapper.ConfigurationProvider);
        }

        if (accessRights.Contains(Constants.Access.Company.Any.Manager.SetFromAnyToAny))
        {
            result.Fields.Add(nameof(Company.ManagerId), company.ManagerId);
            result.InitData.Managers = await GetManagersAsync(accessRights);
        }

        return result;
    }

    private async Task<List<ManagerDto>> GetManagersAsync(string[] accessRights)
    {
        var result = new List<ManagerDto>();

        var expressions = GetManagerExpressions(accessRights);
        if (!expressions.Any())
        {
            return result;
        }

        if (expressions.Contains(_emptyManager) || expressions.Contains(_allManagers))
        {
            expressions.Remove(_emptyManager);
            result.Add(new ManagerDto
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
            .ToListAsync();

        result.AddRange(users);

        return result;
    }

    private List<Expression<Func<ApplicationUser, bool>>> GetManagerExpressions(string[] accessRights)
    {
        var result = new List<Expression<Func<ApplicationUser, bool>>>();
        result.Add(_allManagers);
        return result;
    }
}