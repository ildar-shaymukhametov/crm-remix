using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.EntityFrameworkCore;
using ConsoleClient.Identity;
using ConsoleClient.Persistence;
using CRM.Domain;
using CRM.Domain.Services;
using CRM.Domain.Interfaces;

var builder = Host.CreateApplicationBuilder(args);
var configuration = builder.Configuration
    .AddJsonFile("appsettings.json")
    .AddEnvironmentVariables()
    .Build();

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(configuration.GetConnectionString("DefaultConnection"), builder =>
        builder.MigrationsAssembly(typeof(ApplicationDbContext).Assembly.FullName)));

builder.Services.AddSingleton<IIdentityService, IdentityService>();
builder.Services.AddSingleton<IClaimsPrincipalProvider, ClaimsPrincipalProvider>();
builder.Services.AddSingleton<ApplicationDbContextInitialiser>();
builder.Services.AddSingleton<IAccessService, AccessService>();
builder.Services.AddSingleton<ICurrentUserService, CurrentUserService>();

using var host = builder.Build();

var environmentName = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
if (environmentName == "Development")
{
    var initialiser = host.Services.GetRequiredService<ApplicationDbContextInitialiser>();
    await initialiser.InitialiseAsync();
    await initialiser.SeedAsync();
}

await host.StartAsync();

var identityService = host.Services.GetRequiredService<IIdentityService>();
await App.AuthenticateAsync(host.Services.GetRequiredService<IServiceScopeFactory>());

Console.WriteLine($"Welcome, {CurrentUserService.User.ApplicationUser.FirstName}!");
Console.WriteLine("What do you want to do?");

var accessService = host.Services.GetRequiredService<IAccessService>();
var accessRights = await accessService.CheckAccessAsync(CurrentUserService.User.Id);
if (accessRights.Contains(Constants.Access.Company.Create))
{
    Console.WriteLine("1. Create company");
}

Console.WriteLine("Press any key to continue...");
Console.ReadKey();

class App
{
    public static async Task AuthenticateAsync(IServiceScopeFactory scopeFactory)
    {
        Console.WriteLine("User name:");
        var userName = Console.ReadLine();

        Console.WriteLine("Password:");
        var password = Console.ReadLine();

        var (ok, user) = await AuthenticateAsync(userName, password, scopeFactory);
        if (!ok)
        {
            Console.WriteLine("Wrong credentials");
            Console.ReadKey();
        }

        CurrentUserService.User = user!;
    }

    public static async Task<(bool, IdentityUser?)> AuthenticateAsync(string? userName, string? password, IServiceScopeFactory scopeFactory)
    {
        using var scope = scopeFactory.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        var user = await dbContext.IdentityUsers.Include(x => x.ApplicationUser).SingleOrDefaultAsync(x => x.UserName == userName);
        if (user is null)
        {
            return (false, null);
        }

        var ok = BCrypt.Net.BCrypt.Verify(password, user.PasswordHash);
        if (!ok)
        {
            return (false, null);
        }

        return (true, user);
    }
}