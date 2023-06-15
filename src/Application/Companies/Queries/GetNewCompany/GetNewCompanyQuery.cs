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

namespace CRM.Application.Companies.Queries.GetNewCompany;

[Authorize(Constants.Policies.Company.Queries.Create)]
public record GetNewCompanyQuery : IRequest<NewCompanyVm>
{
}

public class GetNewCompanyRequestHandler : IRequestHandler<GetNewCompanyQuery, NewCompanyVm>
{
    private readonly IAccessService _accessService;
    private readonly ICurrentUserService _currentUserService;
    private readonly IApplicationDbContext _dbContext;
    private readonly IMapper _mapper;
    private readonly Expression<Func<ApplicationUser, bool>> _allManagers = x => true;

    public GetNewCompanyRequestHandler(IAccessService accessService, ICurrentUserService currentUserService, IApplicationDbContext dbContext, IMapper mapper)
    {
        _accessService = accessService;
        _currentUserService = currentUserService;
        _dbContext = dbContext;
        _mapper = mapper;
    }

    public async Task<NewCompanyVm> Handle(GetNewCompanyQuery request, CancellationToken cancellationToken)
    {
        var accessRights = await _accessService.CheckAccessAsync(_currentUserService.UserId!);
        var result = new NewCompanyVm();
        result.Fields.Add(nameof(Company.Name), default);


        if (accessRights.Contains(Constants.Access.Company.New.Other.Set))
        {
            result.Fields.Add(nameof(Company.Address), default);
            result.Fields.Add(nameof(Company.Ceo), default);
            result.Fields.Add(nameof(Company.Contacts), default);
            result.Fields.Add(nameof(Company.Email), default);
            result.Fields.Add(nameof(Company.Inn), default);
            result.Fields.Add(nameof(Company.Phone), default);
            result.Fields.Add(nameof(Company.Type), default);
            result.InitData.CompanyTypes = await _dbContext.CompanyTypes.ProjectToListAsync<CompanyTypeDto>(_mapper.ConfigurationProvider);
        }

        if (accessRights.ContainsAny(
            Constants.Access.Company.New.Manager.SetToAny,
            Constants.Access.Company.New.Manager.SetToSelf
        ))
        {
            result.Fields.Add(nameof(Company.Manager), default);
            result.InitData.Managers = await GetManagersAsync(accessRights);
        }

        return result;
    }

    private List<Expression<Func<ApplicationUser, bool>>> GetManagerExpressions(string[] accessRights)
    {
        var result = new List<Expression<Func<ApplicationUser, bool>>>();
        if (accessRights.Contains(Constants.Access.Company.New.Manager.SetToAny))
        {
            result.Add(_allManagers);
            return result;
        }

        if (accessRights.Contains(Constants.Access.Company.New.Manager.SetToSelf))
        {
            result.Add(x => x.Id == _currentUserService.UserId);
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
}
