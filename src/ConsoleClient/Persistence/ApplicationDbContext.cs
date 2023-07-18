using System.Reflection;
using ConsoleClient.Identity;
using CRM.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace ConsoleClient.Persistence;

public class ApplicationDbContext : DbContext
{
    public DbSet<IdentityUser> IdentityUsers => Set<IdentityUser>();
    public DbSet<Company> Companies => Set<Company>();
    public DbSet<UserClaimType> UserClaimTypes => Set<UserClaimType>();
    public DbSet<CompanyType> CompanyTypes => Set<CompanyType>();
    public DbSet<ApplicationUser> ApplicationUsers => Set<ApplicationUser>();
    public DbSet<IdentityClaim> IdentityClaims => Set<IdentityClaim>();

    public ApplicationDbContext(DbContextOptions options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        builder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        builder.Seed();

        base.OnModelCreating(builder);
    }
}
