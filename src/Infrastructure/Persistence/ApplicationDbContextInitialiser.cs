using CRM.Application.Common.Interfaces;
using CRM.Infrastructure.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace CRM.Infrastructure.Persistence;

public class ApplicationDbContextInitialiser
{
    private readonly ILogger<ApplicationDbContextInitialiser> _logger;
    private readonly ApplicationDbContext _context;
    private readonly UserManager<AspNetUser> _userManager;
    private readonly RoleManager<IdentityRole> _roleManager;
    private readonly IIdentityService _identityService;

    public ApplicationDbContextInitialiser(ILogger<ApplicationDbContextInitialiser> logger, ApplicationDbContext context, UserManager<AspNetUser> userManager, RoleManager<IdentityRole> roleManager, IIdentityService identityService)
    {
        _logger = logger;
        _context = context;
        _userManager = userManager;
        _roleManager = roleManager;
        _identityService = identityService;
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
        var userName = "administrator@localhost";
        if (_userManager.Users.All(u => u.UserName != userName))
        {
            var (result, userId) = await _identityService.CreateUserAsync(userName, "Administrator1!", "admin", "localhost");
            var user = await _userManager.FindByIdAsync(userId);
            await _userManager.AddToRolesAsync(user, new[] { administratorRole.Name, testerRole.Name });
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
        var userName = "tester@localhost";
        if (_userManager.Users.All(u => u.UserName != userName))
        {
            var (result, userId) = await _identityService.CreateUserAsync(userName, "Tester1!", "tester", "localhost");
            var user = await _userManager.FindByIdAsync(userId);
            await _userManager.AddToRolesAsync(user, new[] { testerRole.Name });
        }
    }
}
