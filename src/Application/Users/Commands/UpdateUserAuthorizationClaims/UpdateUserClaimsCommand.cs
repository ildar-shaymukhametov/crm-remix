using CRM.Application.Common.Exceptions;
using CRM.Application.Common.Interfaces;
using CRM.Application.Common.Security;
using MediatR;

namespace CRM.Application.Users.Commands.UpdateUserAuthorizationClaims;

[Authorize]
public record UpdateUserAuthorizationClaimsCommand : IRequest<Unit>
{
    public string[] Claims { get; set; } = Array.Empty<string>();
}

public class UpdateUserAuthorizationClaimsCommandHandler : IRequestHandler<UpdateUserAuthorizationClaimsCommand>
{
    private readonly ICurrentUserService _currentUserService;
    private readonly IIdentityService _identityService;

    public UpdateUserAuthorizationClaimsCommandHandler(ICurrentUserService currentUserService, IIdentityService identityService)
    {
        _currentUserService = currentUserService;
        _identityService = identityService;
    }

    public async Task<Unit> Handle(UpdateUserAuthorizationClaimsCommand request, CancellationToken cancellationToken)
    {
        var result = await _identityService.UpdateAuthorizationClaimsAsync(_currentUserService.UserId!, request.Claims);
        if (result.Errors.Any())
        {
            throw new InternalServerErrorException($"Failed to update claims for user {_currentUserService.UserId}");
        }

        return Unit.Value;
    }
}
