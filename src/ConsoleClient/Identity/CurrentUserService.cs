using CRM.Domain.Interfaces;

namespace ConsoleClient.Identity;

public class CurrentUserService : ICurrentUserService
{
    public static IdentityUser User { get; set; } = default!;

    public string? UserId => User.Id;
}
