using CRM.Domain.Entities;
using Microsoft.AspNetCore.Identity;

namespace CRM.Infrastructure.Identity;

public class AspNetUser : IdentityUser
{
    public ApplicationUser? ApplicationUser { get; set; }
}
