using CRM.Application.Common.Interfaces;
using CRM.Application.Common.Security;
using MediatR;

namespace CRM.Application.Users.Queries.GetUserPermissions;

[Authorize]
public record GetUserPermissionsQuery : IRequest<string[]>
{
    public string[] RequestedPermissions { get; set; } = Array.Empty<string>();
}

public class GetUserPermissionsQueryHandler : IRequestHandler<GetUserPermissionsQuery, string[]>
{
    private readonly ICurrentUserService _currentUserService;
    private readonly IPermissionsService _permissionsService;

    public GetUserPermissionsQueryHandler(ICurrentUserService currentUserService, IPermissionsService permissionsService)
    {
        _currentUserService = currentUserService;
        _permissionsService = permissionsService;
    }

    public async Task<string[]> Handle(GetUserPermissionsQuery request, CancellationToken cancellationToken)
    {
        return await _permissionsService.CheckUserPermissionsAsync(_currentUserService.UserId!, request.RequestedPermissions);
    }
}
