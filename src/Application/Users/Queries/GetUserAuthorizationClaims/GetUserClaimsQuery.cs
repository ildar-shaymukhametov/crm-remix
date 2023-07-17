using CRM.Application.Common.Interfaces;
using CRM.Application.Common.Security;
using CRM.Domain.Interfaces;
using MediatR;

namespace CRM.Application.Users.Queries.GetUserAuthorizationClaims;

[Authorize]
public record GetUserAuthorizationClaimsQuery : IRequest<string[]>;

public class GetUserAuthorizationClaimsQueryHandler : IRequestHandler<GetUserAuthorizationClaimsQuery, string[]>
{
    private readonly IAppIdentityService _identityService;
    private readonly ICurrentUserService _currentUserService;

    public GetUserAuthorizationClaimsQueryHandler(IAppIdentityService identityService, ICurrentUserService currentUserService)
    {
        _identityService = identityService;
        _currentUserService = currentUserService;
    }

    public async Task<string[]> Handle(GetUserAuthorizationClaimsQuery request, CancellationToken cancellationToken)
    {
        return await _identityService.GetUserAuthorizationClaimsAsync(_currentUserService.UserId!);
    }
}