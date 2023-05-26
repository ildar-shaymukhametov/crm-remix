using CRM.Application.Common.Interfaces;
using MediatR;

namespace CRM.Application.Users.Commands.CreateUser;

public record CreateUserCommand : IRequest<string>
{
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public string Password { get; set; } = default!;
    public string UserName { get; set; } = default!;
    public string[] Claims { get; set; } = Array.Empty<string>();
    public string[] Roles { get; set; } = Array.Empty<string>();
}

public class CreateUserCommandHandler : IRequestHandler<CreateUserCommand, string>
{
    private readonly ICurrentUserService _currentUserService;
    private readonly IIdentityService _identityService;

    public CreateUserCommandHandler(ICurrentUserService currentUserService, IIdentityService identityService)
    {
        _currentUserService = currentUserService;
        _identityService = identityService;
    }

    public async Task<string> Handle(CreateUserCommand request, CancellationToken cancellationToken)
    {
        var (result, userId) = await _identityService.CreateUserAsync(request.UserName, request.Password, request.FirstName!, request.LastName!, request.Claims, request.Roles);
        return userId;
    }
}