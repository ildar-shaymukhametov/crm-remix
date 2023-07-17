using CRM.Domain.Interfaces;
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
    private readonly IIdentityService _identityService;

    public CreateUserCommandHandler(IIdentityService identityService)
    {
        _identityService = identityService;
    }

    public async Task<string> Handle(CreateUserCommand request, CancellationToken cancellationToken)
    {
        var (_, userId) = await _identityService.CreateUserAsync(request.UserName, request.Password, request.FirstName!, request.LastName!, request.Claims, request.Roles);
        return userId;
    }
}