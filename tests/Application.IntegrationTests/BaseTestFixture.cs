using System.Security.Claims;
using CRM.Infrastructure.Identity;
using CRM.Infrastructure.Persistence;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Respawn;
using Respawn.Graph;

namespace CRM.Application.IntegrationTests;

[CollectionDefinition("Database")]
public class DatabaseCollection : ICollectionFixture<BaseTestFixture>
{
    // This class has no code, and is never created. Its purpose is simply
    // to be the place to apply [CollectionDefinition] and all the
    // ICollectionFixture<> interfaces.
}

public class BaseTestFixture
{
    private static WebApplicationFactory<Program> _factory = null!;
    private static WebApplicationFactory<Program>? _modifiedFactory;
    private static WebApplicationFactory<Program> Factory => _modifiedFactory ?? _factory;
    private readonly string _connectionString;
    private static IServiceScopeFactory ScopeFactory => Factory.Services.GetRequiredService<IServiceScopeFactory>();
    private static Respawner _respawner = null!;
    private static string? _currentUserId;
    public static DateTime UtcNow = Faker.Date.RandomDateTimeUtc();
    public static DateTime Now = Faker.Date.RandomDateTime();

    public BaseTestFixture()
    {
        _factory = new CustomWebApplicationFactory();
        var configuration = _factory.Services.GetRequiredService<IConfiguration>();
        _connectionString = configuration.GetConnectionString("DefaultConnection");
    }

    public async Task<TResponse> SendAsync<TResponse>(IRequest<TResponse> request)
    {
        using var scope = ScopeFactory.CreateScope();

        var mediator = scope.ServiceProvider.GetRequiredService<ISender>();

        return await mediator.Send(request);
    }

    public static string? GetCurrentUserId()
    {
        return _currentUserId;
    }

    public async Task<ApplicationUser> RunAsDefaultUserAsync()
    {
        return await RunAsDefaultUserAsync(Array.Empty<Claim>());
    }

    public async Task<ApplicationUser> RunAsDefaultUserAsync(Claim[] claims)
    {
        return await RunAsUserAsync("test@local", "Testing1234!", Array.Empty<string>(), claims);
    }

    public async Task<ApplicationUser> RunAsDefaultUserAsync(Claim[] claims, string[] roles)
    {
        return await RunAsUserAsync("test@local", "Testing1234!", roles, claims);
    }

    public async Task<ApplicationUser> RunAsAdministratorAsync()
    {
        return await RunAsUserAsync("administrator@local", "Administrator1234!", new[] { Constants.Roles.Administrator }, Array.Empty<Claim>());
    }

    public async Task<ApplicationUser> RunAsUserAsync(string userName, string password, string[] roles, Claim[] claims)
    {
        using var scope = ScopeFactory.CreateScope();

        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
        var user = new ApplicationUser { UserName = userName, Email = userName };
        var result = await userManager.CreateAsync(user, password);

        if (roles.Any())
        {
            var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();

            foreach (var role in roles)
            {
                await roleManager.CreateAsync(new IdentityRole(role));
            }

            await userManager.AddToRolesAsync(user, roles);
        }

        if (claims.Any())
        {
            await userManager.AddClaimsAsync(user, claims);
        }

        if (result.Succeeded)
        {
            _currentUserId = user.Id;

            return user;
        }

        var errors = string.Join(Environment.NewLine, result.ToApplicationResult().Errors);

        throw new Exception($"Unable to create {userName}.{Environment.NewLine}{errors}");
    }

    public async Task ResetStateAsync()
    {
        await _respawner.ResetAsync(_connectionString);

        _currentUserId = null;
        _modifiedFactory = null;
    }

    public async Task InitStateAsync()
    {
        _respawner ??= await Respawner.CreateAsync(_connectionString, new RespawnerOptions
        {
            TablesToIgnore = new Table[] { "__EFMigrationsHistory" }
        });
    }

    public async Task<TEntity?> FindAsync<TEntity>(params object[] keyValues) where TEntity : class
    {
        using var scope = ScopeFactory.CreateScope();

        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        return await context.FindAsync<TEntity>(keyValues);
    }

    public async Task<List<Claim>> GetAuthorizationClaimsAsync(ApplicationUser user)
    {
        using var scope = ScopeFactory.CreateScope();
        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
        var claims = await userManager.GetClaimsAsync(user);
        return claims.Where(x => x.Type == Constants.Claims.ClaimType).ToList();
    }

    public async Task AddAsync<TEntity>(TEntity entity) where TEntity : class
    {
        using var scope = ScopeFactory.CreateScope();

        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        context.Add(entity);

        await context.SaveChangesAsync();
    }

    public async Task<int> CountAsync<TEntity>() where TEntity : class
    {
        using var scope = ScopeFactory.CreateScope();

        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        return await context.Set<TEntity>().CountAsync();
    }

    public void ReplaceServices(params (Type, object)[] replacements)
    {
        _modifiedFactory = Factory.WithWebHostBuilder(builder => builder.ConfigureServices(services =>
        {
            foreach (var (type, replacement) in replacements)
            {
                var serviceDescriptor = services.FirstOrDefault(d => d.ServiceType == type);
                if (serviceDescriptor != null)
                {
                    services.Remove(serviceDescriptor);
                    services.AddTransient(type, x => replacement);
                }
            }
        }));
    }
}
