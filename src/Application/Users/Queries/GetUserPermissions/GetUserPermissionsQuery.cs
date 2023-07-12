using System.Text;
using CRM.Application.Common.Interfaces;
using CRM.Application.Common.Security;
using CRM.Domain.Interfaces;
using MediatR;

namespace CRM.Application.Users.Queries.GetUserPermissions;

[Authorize]
public record GetUserPermissionsQuery : IRequest<GetUserPermissionsQueryResponse>
{
    public string[] RequestedPermissions { get; set; } = Array.Empty<string>();
    public string? ResourceKey { get; set; }

    protected virtual bool PrintMembers(StringBuilder stringBuilder)
    {
        stringBuilder.Append($"{nameof(ResourceKey)} = {ResourceKey}, {nameof(RequestedPermissions)} = [{string.Join(", ", RequestedPermissions)}]");
        return true;
    }
}

public class GetUserPermissionsQueryHandler : IRequestHandler<GetUserPermissionsQuery, GetUserPermissionsQueryResponse>
{
    private readonly ICurrentUserService _currentUserService;
    private readonly IPermissionsVerifier _permissionsVerifier;

    public GetUserPermissionsQueryHandler(ICurrentUserService currentUserService, IPermissionsVerifier permissionsVerifier)
    {
        _currentUserService = currentUserService;
        _permissionsVerifier = permissionsVerifier;
    }

    public async Task<GetUserPermissionsQueryResponse> Handle(GetUserPermissionsQuery request, CancellationToken cancellationToken)
    {
        return new GetUserPermissionsQueryResponse
        {
            Permissions = await _permissionsVerifier.VerifyAsync(_currentUserService.UserId!, request.ResourceKey, request.RequestedPermissions)
        };
    }
}
