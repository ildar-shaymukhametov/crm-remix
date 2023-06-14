using CRM.Application.Common.Interfaces;
using CRM.Domain.Entities;
using MediatR;

namespace CRM.Application.Companies.Queries.GetNewCompany;

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

    public Task<NewCompanyVm> Handle(GetNewCompanyQuery request, CancellationToken cancellationToken)
    {
        var result = new NewCompanyVm
        {
            Fields = new Dictionary<string, object?>
            {
                [nameof(Company.Name)] = null,
                [nameof(Company.Address)] = null,
                [nameof(Company.Ceo)] = null,
                [nameof(Company.Contacts)] = null,
                [nameof(Company.Email)] = null,
                [nameof(Company.Inn)] = null,
                [nameof(Company.Phone)] = null,
                [nameof(Company.Type)] = null,
                [nameof(Company.Manager)] = null
            }
        };

        return Task.FromResult(result);
    }
}
