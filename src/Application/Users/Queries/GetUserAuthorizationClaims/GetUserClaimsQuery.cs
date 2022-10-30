using CRM.Application.Common.Interfaces;
using CRM.Application.Common.Security;
using MediatR;

namespace CRM.Application.Users.Queries.GetUserAuthorizationClaims;

[Authorize]
public record GetUserAuthorizationClaimsQuery : IRequest<string[]>;

public class GetUserAuthorizationClaimsQueryHandler : IRequestHandler<GetUserAuthorizationClaimsQuery, string[]>
{
    private readonly IApplicationDbContext _dbContext;
    private readonly IIdentityService _identityService;
    private readonly ICurrentUserService _currentUserService;

    public GetUserAuthorizationClaimsQueryHandler(IApplicationDbContext dbContext, IIdentityService identityService, ICurrentUserService currentUserService)
    {
        _dbContext = dbContext;
        _identityService = identityService;
        _currentUserService = currentUserService;
    }

    public async Task<string[]> Handle(GetUserAuthorizationClaimsQuery request, CancellationToken cancellationToken)
    {
        return await _identityService.GetUserAuthorizationClaimsAsync(_currentUserService.UserId!);
    }
}