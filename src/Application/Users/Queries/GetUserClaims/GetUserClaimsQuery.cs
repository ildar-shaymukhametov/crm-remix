using CRM.Application.Common.Interfaces;
using MediatR;

namespace CRM.Application.Users.Queries.GetUserClaims;

public record GetUserClaimsQuery : IRequest<string[]>;

public class GetUserClaimsQueryHandler : IRequestHandler<GetUserClaimsQuery, string[]>
{
    private readonly IApplicationDbContext _dbContext;
    private readonly IIdentityService _identityService;
    private readonly ICurrentUserService _currentUserService;

    public GetUserClaimsQueryHandler(IApplicationDbContext dbContext, IIdentityService identityService, ICurrentUserService currentUserService)
    {
        _dbContext = dbContext;
        _identityService = identityService;
        _currentUserService = currentUserService;
    }

    public async Task<string[]> Handle(GetUserClaimsQuery request, CancellationToken cancellationToken)
    {
        var claims = await _identityService.GetUserClaimsAsync(_currentUserService.UserId);
        return claims.Select(x => x.Value).ToArray();
    }
}