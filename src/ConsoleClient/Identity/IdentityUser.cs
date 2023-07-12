using CRM.Domain.Entities;

namespace ConsoleClient.Identity;

public class IdentityUser : ApplicationUser
{
    public string? UserName { get; set; }
    public string? PasswordHash { get; set; }
}
