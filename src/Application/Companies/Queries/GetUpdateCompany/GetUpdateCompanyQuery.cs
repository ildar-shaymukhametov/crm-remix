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

        if (accessRights.ContainsAny(
            Constants.Access.Company.Any.Manager.SetFromAnyToAny,
            Constants.Access.Company.Any.Manager.SetFromAnyToNone,
            Constants.Access.Company.Any.Manager.SetFromAnyToSelf,
            Constants.Access.Company.Any.Manager.SetFromNoneToAny,
            Constants.Access.Company.Any.Manager.SetFromNoneToSelf,
            Constants.Access.Company.Any.Manager.SetFromSelfToAny,
            Constants.Access.Company.Any.Manager.SetFromSelfToNone,
            Constants.Access.Company.WhereUserIsManager.Manager.SetFromSelfToAny,
            Constants.Access.Company.WhereUserIsManager.Manager.SetFromSelfToNone
        ))
        {
            result.Fields.Add(nameof(Company.ManagerId), company.ManagerId);
            result.InitData.Managers = await GetManagersAsync(request, accessRights);
        }

        return result;
    }

    private async Task<List<ManagerDto>> GetManagersAsync(GetUpdateCompanyQuery request, string[] accessRights)
    {
        var result = new List<ManagerDto>();

        var expressions = await GetManagerExpressionsAsync(request, accessRights);
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

    private async Task<List<Expression<Func<ApplicationUser, bool>>>> GetManagerExpressionsAsync(GetUpdateCompanyQuery request, string[] accessRights)
    {
        var result = new List<Expression<Func<ApplicationUser, bool>>>();
        if (accessRights.Contains(Constants.Access.Company.Any.Manager.SetFromAnyToAny))
        {
            result.Add(_allManagers);
            return result;
        }

        var managerId = await _dbContext.Companies
                .Where(x => x.Id == request.Id)
                .Select(x => x.ManagerId)
                .SingleAsync();

        if (accessRights.ContainsAny(
                Constants.Access.Company.Any.Manager.SetFromNoneToAny,
                Constants.Access.Company.Any.Manager.SetFromAnyToAny) && managerId == null
            || accessRights.ContainsAny(
                Constants.Access.Company.Any.Manager.SetFromSelfToAny,
                Constants.Access.Company.Any.Manager.SetFromAnyToAny) && managerId == _currentUserService.UserId)
        {
            result.Add(_allManagers);
            return result;
        }
        if (accessRights.ContainsAny(
                Constants.Access.Company.Any.Manager.SetFromAnyToSelf,
                Constants.Access.Company.Any.Manager.SetFromAnyToAny) && managerId != null
            || accessRights.ContainsAny(
                Constants.Access.Company.Any.Manager.SetFromNoneToSelf,
                Constants.Access.Company.Any.Manager.SetFromNoneToAny,
                Constants.Access.Company.Any.Manager.SetFromAnyToSelf,
                Constants.Access.Company.Any.Manager.SetFromAnyToAny) && managerId == null
            || accessRights.ContainsAny(
                Constants.Access.Company.Any.Manager.SetFromSelfToAny,
                Constants.Access.Company.Any.Manager.SetFromAnyToAny,
                Constants.Access.Company.Any.Manager.SetFromSelfToNone,
                Constants.Access.Company.Any.Manager.SetFromAnyToNone) && managerId == _currentUserService.UserId)
        {
            result.Add(x => x.Id == _currentUserService.UserId);
        }

        if (accessRights.ContainsAny(
            Constants.Access.Company.Any.Manager.SetFromAnyToNone,
            Constants.Access.Company.Any.Manager.SetFromAnyToSelf,
            Constants.Access.Company.Any.Manager.SetFromAnyToAny) && managerId != null)
        {
            result.Add(x => x.Id == managerId);
        }

        if (accessRights.ContainsAny(
                Constants.Access.Company.Any.Manager.SetFromSelfToAny,
                Constants.Access.Company.Any.Manager.SetFromAnyToAny,
                Constants.Access.Company.Any.Manager.SetFromSelfToNone,
                Constants.Access.Company.Any.Manager.SetFromAnyToNone) && managerId == _currentUserService.UserId
            || accessRights.ContainsAny(
                Constants.Access.Company.Any.Manager.SetFromNoneToAny,
                Constants.Access.Company.Any.Manager.SetFromNoneToSelf,
                Constants.Access.Company.Any.Manager.SetFromAnyToAny,
                Constants.Access.Company.Any.Manager.SetFromAnyToNone,
                Constants.Access.Company.Any.Manager.SetFromAnyToSelf) && managerId == null
            || accessRights.Contains(Constants.Access.Company.Any.Manager.SetFromAnyToNone) && managerId != null)
        {
            result.Add(_emptyManager);
        }

        return result;
    }
}