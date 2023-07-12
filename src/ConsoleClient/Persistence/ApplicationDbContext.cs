using System.Reflection;
using ConsoleClient.Identity;
using Microsoft.EntityFrameworkCore;

namespace ConsoleClient.Persistence;

public class ApplicationDbContext : DbContext
{
    public DbSet<IdentityUser> Users => Set<IdentityUser>();

    public ApplicationDbContext(DbContextOptions options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        builder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        base.OnModelCreating(builder);
    }
}
