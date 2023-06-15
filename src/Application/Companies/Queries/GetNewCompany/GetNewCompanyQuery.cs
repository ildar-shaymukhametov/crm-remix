using AutoMapper;
using CRM.Application.Common.Extensions;
using CRM.Application.Common.Interfaces;
using CRM.Application.Common.Mappings;
using CRM.Application.Common.Security;
using CRM.Domain.Entities;
using MediatR;

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
        result.InitData.CompanyTypes = await GetCompanyTypesAsync();
        result.InitData.Managers = await GetManagersAsync();

        if (accessRights.Contains(Constants.Access.Company.New.Other.Set))
        {
            result.Fields.Add(nameof(Company.Address), default);
            result.Fields.Add(nameof(Company.Ceo), default);
            result.Fields.Add(nameof(Company.Contacts), default);
            result.Fields.Add(nameof(Company.Email), default);
            result.Fields.Add(nameof(Company.Inn), default);
            result.Fields.Add(nameof(Company.Phone), default);
            result.Fields.Add(nameof(Company.Type), default);
        }

        if (accessRights.ContainsAny(
            Constants.Access.Company.New.Manager.SetToAny,
            Constants.Access.Company.New.Manager.SetToSelf
        ))
        {
            result.Fields.Add(nameof(Company.Manager), default);
        }

        return result;
    }

    private async Task<List<CompanyTypeDto>> GetCompanyTypesAsync()
    {
        return await _dbContext.CompanyTypes.ProjectToListAsync<CompanyTypeDto>(_mapper.ConfigurationProvider);
    }

    private async Task<List<ManagerDto>> GetManagersAsync()
    {
        return await _dbContext.ApplicationUsers.ProjectToListAsync<ManagerDto>(_mapper.ConfigurationProvider);
    }
}
