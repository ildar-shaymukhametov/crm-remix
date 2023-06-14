using CRM.Application.Common.Extensions;
using CRM.Application.Common.Interfaces;
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
    private readonly IIdentityService _identityService;

    public GetNewCompanyRequestHandler(IAccessService accessService, ICurrentUserService currentUserService, IIdentityService identityService)
    {
        _accessService = accessService;
        _currentUserService = currentUserService;
        _identityService = identityService;
    }

    public async Task<NewCompanyVm> Handle(GetNewCompanyQuery request, CancellationToken cancellationToken)
    {
        var accessRights = await _accessService.CheckAccessAsync(_currentUserService.UserId!);
        var result = new NewCompanyVm
        {
            Fields = new Dictionary<string, object?>
            {
                [nameof(Company.Name)] = default,
            }
        };

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
}
