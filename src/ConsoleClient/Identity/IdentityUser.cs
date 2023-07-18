using CRM.Domain.Entities;

namespace ConsoleClient.Identity;

public class IdentityUser
{
    public string Id { get; set; } = default!;
    public string UserName { get; set; } = default!;
    public string PasswordHash { get; set; } = default!;
    public virtual ICollection<IdentityClaim> Claims { get; set; } = default!;
    public virtual ApplicationUser ApplicationUser { get; set; } = default!;
}
