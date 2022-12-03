using System.Text;
using CRM.Application.Common.Interfaces;
using CRM.Application.Common.Security;
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
    private readonly IPermissionsService _permissionsService;

    public GetUserPermissionsQueryHandler(ICurrentUserService currentUserService, IPermissionsService permissionsService)
    {
        _currentUserService = currentUserService;
        _permissionsService = permissionsService;
    }

    public async Task<GetUserPermissionsQueryResponse> Handle(GetUserPermissionsQuery request, CancellationToken cancellationToken)
    {
        return new GetUserPermissionsQueryResponse
        {
            Permissions = await _permissionsService.CheckUserPermissionsAsync(_currentUserService.UserId!, request.ResourceKey, request.RequestedPermissions)
        };
    }
}
