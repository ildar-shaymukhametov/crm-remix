using CRM.Infrastructure.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace CRM.Infrastructure.Persistence;

public class ApplicationDbContextInitialiser
{
    private readonly ILogger<ApplicationDbContextInitialiser> _logger;
    private readonly ApplicationDbContext _context;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly RoleManager<IdentityRole> _roleManager;

    public ApplicationDbContextInitialiser(ILogger<ApplicationDbContextInitialiser> logger, ApplicationDbContext context, UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
    {
        _logger = logger;
        _context = context;
        _userManager = userManager;
        _roleManager = roleManager;
    }

    public async Task InitialiseAsync()
    {
        try
        {
            if (_context.Database.IsSqlServer())
            {
                await _context.Database.MigrateAsync();
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

    public async Task SeedTestUsersAsync()
    {
        try
        {
            await TrySeedTestUsersAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while seeding the database.");
            throw;
        }
    }

    public async Task TrySeedAsync()
    {
        // Default roles
        var administratorRole = new IdentityRole("Administrator");

        if (_roleManager.Roles.All(r => r.Name != administratorRole.Name))
        {
            await _roleManager.CreateAsync(administratorRole);
        }

        var testerRole = new IdentityRole("Tester");
        if (_roleManager.Roles.All(r => r.Name != testerRole.Name))
        {
            await _roleManager.CreateAsync(testerRole);
        }

        // Default users
        var administrator = new ApplicationUser { UserName = "administrator@localhost", Email = "administrator@localhost" };

        if (_userManager.Users.All(u => u.UserName != administrator.UserName))
        {
            await _userManager.CreateAsync(administrator, "Administrator1!");
            await _userManager.AddToRolesAsync(administrator, new[] { administratorRole.Name, testerRole.Name });
        }
    }

    public async Task TrySeedTestUsersAsync()
    {
        // Default roles
        var testerRole = new IdentityRole("Tester");

        if (_roleManager.Roles.All(r => r.Name != testerRole.Name))
        {
            await _roleManager.CreateAsync(testerRole);
        }

        // Default users
        var defaultUser = new ApplicationUser { UserName = "tester@localhost", Email = "tester@localhost" };

        if (_userManager.Users.All(u => u.UserName != defaultUser.UserName))
        {
            await _userManager.CreateAsync(defaultUser, "Tester1!");
            await _userManager.AddToRolesAsync(defaultUser, new[] { testerRole.Name });
        }
    }
}
