using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.EntityFrameworkCore;
using ConsoleClient.Identity;
using ConsoleClient.Persistence;

var builder = Host.CreateApplicationBuilder(args);
var configuration = builder.Configuration
    .AddJsonFile("appsettings.json")
    .AddEnvironmentVariables()
    .Build();

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(configuration.GetConnectionString("DefaultConnection"), builder =>
        builder.MigrationsAssembly(typeof(ApplicationDbContext).Assembly.FullName)));

builder.Services.AddTransient<IdentityService>();
builder.Services.AddTransient<ApplicationDbContextInitialiser>();

using var host = builder.Build();

var environmentName = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
if (environmentName == "Development")
{
    var initialiser = host.Services.GetRequiredService<ApplicationDbContextInitialiser>();
    await initialiser.InitialiseAsync();
    await initialiser.SeedAsync();
}

await host.StartAsync();

var identityService = host.Services.GetRequiredService<IdentityService>();
await App.AuthenticateAsync(identityService);

class App
{
    public static async Task AuthenticateAsync(IdentityService identityService)
    {
        Console.WriteLine("User name:");
        var userName = Console.ReadLine();

        Console.WriteLine("Password:");
        var password = Console.ReadLine();

        var (ok, user) = await identityService.AuthenticateAsync(userName, password);
        if (!ok)
        {
            Console.WriteLine("Wrong credentials");
        }

        IdentityService.User = user!;

        Console.WriteLine("Press any key to continue");
        Console.ReadKey();
    }
}