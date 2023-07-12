using ConsoleClient.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace ConsoleClient.Persistence;

public class ApplicationDbContextInitialiser
{
    private readonly ILogger<ApplicationDbContextInitialiser> _logger;
    private readonly ApplicationDbContext _dbContext;

    public ApplicationDbContextInitialiser(ILogger<ApplicationDbContextInitialiser> logger, ApplicationDbContext dbContext)
    {
        _logger = logger;
        _dbContext = dbContext;
    }

    public async Task InitialiseAsync()
    {
        try
        {
            if (_dbContext.Database.IsSqlServer())
            {
                await _dbContext.Database.MigrateAsync();
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
        if (!await _dbContext.Users.AnyAsync(x => x.UserName == "default@user"))
        {
            await _dbContext.Users.AddAsync(new IdentityUser
            {
                Id = Guid.NewGuid().ToString(),
                UserName = "default@user",
                Email = "default@user",
                FirstName = "Default",
                LastName = "User",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("default")
            });

            await _dbContext.SaveChangesAsync();
        }
    }
}
