using System.Text;
using CRM.Application.Common.Exceptions;
using CRM.Application.Common.Interfaces;
using CRM.Application.Common.Security;
using CRM.Domain.Interfaces;
using MediatR;

namespace CRM.Application.Users.Commands.UpdateUserAuthorizationClaims;

[Authorize]
public record UpdateUserAuthorizationClaimsCommand : IRequest
{
    public string[] Claims { get; set; } = Array.Empty<string>();

    protected virtual bool PrintMembers(StringBuilder stringBuilder)
    {
        stringBuilder.Append($"{nameof(Claims)} = [{string.Join(", ", Claims)}]");
        return true;
    }
}

public class UpdateUserAuthorizationClaimsCommandHandler : IRequestHandler<UpdateUserAuthorizationClaimsCommand>
{
    private readonly ICurrentUserService _currentUserService;
    private readonly IAppIdentityService _identityService;

    public UpdateUserAuthorizationClaimsCommandHandler(ICurrentUserService currentUserService, IAppIdentityService identityService)
    {
        _currentUserService = currentUserService;
        _identityService = identityService;
    }

    public async Task Handle(UpdateUserAuthorizationClaimsCommand request, CancellationToken cancellationToken)
    {
        var result = await _identityService.UpdateAuthorizationClaimsAsync(_currentUserService.UserId!, request.Claims);
        if (result.Errors.Any())
        {
            throw new InternalServerErrorException($"Failed to update claims for user {_currentUserService.UserId}");
        }

        return;
    }
}
