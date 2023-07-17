using ConsoleClient.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace ConsoleClient.Persistence;

public class ApplicationDbContextInitialiser
{
    private readonly ILogger<ApplicationDbContextInitialiser> _logger;
    private readonly IServiceScopeFactory _scopeFactory;

    public ApplicationDbContextInitialiser(ILogger<ApplicationDbContextInitialiser> logger, IServiceScopeFactory scopeFactory)
    {
        _logger = logger;
        _scopeFactory = scopeFactory;
    }

    public async Task InitialiseAsync()
    {
        using var scope = _scopeFactory.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        try
        {
            if (dbContext.Database.IsSqlServer())
            {
                await dbContext.Database.MigrateAsync();
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while initialising the database.");
            throw;
        }
    }

    public async Task SeedAsync()
    {
        try
        {
            await TrySeedAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while seeding the database.");
            throw;
        }
    }

    public async Task TrySeedAsync()
    {
        using var scope = _scopeFactory.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        if (!await dbContext.Users.AnyAsync(x => x.UserName == "default@user"))
        {
            await dbContext.Users.AddAsync(new IdentityUser
            {
                Id = Guid.NewGuid().ToString(),
                UserName = "default@user",
                Email = "default@user",
                FirstName = "Default",
                LastName = "User",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("default")
            });

            await dbContext.SaveChangesAsync();
        }
    }
}
