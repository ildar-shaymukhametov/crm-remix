using System.Security.Claims;
using CRM.Application.Common.Interfaces;
using CRM.Domain.Entities;
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
    private static IConfiguration _configuration = null!;
    private readonly string _connectionString;
    private static IServiceScopeFactory _scopeFactory = null!;
    private static Respawner _respawner = null!;
    private static string? _currentUserId;
    public static DateTime UtcNow = Faker.Date.RandomDateTimeUtc();
    public static DateTime Now = Faker.Date.RandomDateTime();

    public BaseTestFixture()
    {
        _factory = new CustomWebApplicationFactory();
        _scopeFactory = _factory.Services.GetRequiredService<IServiceScopeFactory>();
        _configuration = _factory.Services.GetRequiredService<IConfiguration>();
        _connectionString = _configuration.GetConnectionString("DefaultConnection");
    }

    public async Task<TResponse> SendAsync<TResponse>(IRequest<TResponse> request)
    {
        using var scope = _scopeFactory.CreateScope();

        var mediator = scope.ServiceProvider.GetRequiredService<ISender>();

        return await mediator.Send(request);
    }

    public static string? GetCurrentUserId()
    {
        return _currentUserId;
    }

    public async Task<AspNetUser> RunAsDefaultUserAsync()
    {
        return await RunAsDefaultUserAsync(Array.Empty<string>());
    }

    public async Task<AspNetUser> RunAsDefaultUserAsync(params string[] claims)
    {
        return await RunAsDefaultUserAsync(string.Empty, string.Empty, claims);
    }

    public async Task<AspNetUser> RunAsDefaultUserAsync(string firstName, string lastName, params string[] claims)
    {
        return await RunAsUserAsync("test@local", "Testing1234!", Array.Empty<string>(), claims, firstName, lastName);
    }

    public async Task<AspNetUser> RunAsDefaultUserAsync(string[] claims, string[] roles)
    {
        return await RunAsUserAsync("test@local", "Testing1234!", roles, claims);
    }

    public async Task<AspNetUser> RunAsAdministratorAsync()
    {
        return await RunAsAdministratorAsync(string.Empty, string.Empty);
    }

    public async Task<AspNetUser> RunAsAdministratorAsync(string lastName)
    {
        return await RunAsAdministratorAsync(Faker.Internet.UserName(), lastName);
    }

    public async Task<AspNetUser> RunAsAdministratorAsync(string firstName, string lastName)
    {
        return await RunAsUserAsync("administrator@local", "Administrator1234!", new[] { Constants.Roles.Administrator }, Array.Empty<string>(), firstName, lastName);
    }

    public async Task<AspNetUser> RunAsUserAsync(string userName, string password, string[] roles, string[] claims)
    {
        return await RunAsUserAsync(userName, password, roles, claims, string.Empty, string.Empty);
    }

    public async Task<AspNetUser> RunAsUserAsync(string userName, string password, string[] roles, string[] claims, string firstName, string lastName)
    {
        using var scope = _scopeFactory.CreateScope();

        var identityService = scope.ServiceProvider.GetRequiredService<IIdentityService>();
        var (result, userId) = await identityService.CreateUserAsync(userName, password, firstName, lastName);
        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<AspNetUser>>();
        var user = await userManager.FindByIdAsync(userId);

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
            await userManager.AddClaimsAsync(user, claims.Select(x => new Claim(Constants.Claims.ClaimType, x)));
        }

        if (result.Succeeded)
        {
            _currentUserId = user.Id;

            return user;
        }

        var errors = string.Join(Environment.NewLine, result.Errors);

        throw new Exception($"Unable to create {userName}.{Environment.NewLine}{errors}");
    }

    public async Task<AspNetUser> AddUserAsync()
    {
        return await AddUserAsync(string.Empty, string.Empty);
    }

    public async Task<AspNetUser> AddUserAsync(string lastName)
    {
        return await AddUserAsync(Faker.Internet.UserName(), lastName);
    }

    public async Task<AspNetUser> AddUserAsync(string firstName, string lastName)
    {
        using var scope = _scopeFactory.CreateScope();

        var identityService = scope.ServiceProvider.GetRequiredService<IIdentityService>();
        var (result, userId) = await identityService.CreateUserAsync(Faker.Internet.UserName(), $"{Faker.Internet.UserName()}Z1!", firstName, lastName);
        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<AspNetUser>>();
        var user = await userManager.FindByIdAsync(userId);

        if (!result.Succeeded)
        {
            var errors = string.Join(Environment.NewLine, result.Errors);
            throw new Exception($"Unable to create a user.{Environment.NewLine}{errors}");
        }

        return user;
    }

    public async Task ResetStateAsync()
    {
        await _respawner.ResetAsync(_connectionString);

        _currentUserId = null;
    }

    public async Task InitStateAsync()
    {
        if (_respawner != null)
        {
            return;
        }

        _respawner = await Respawner.CreateAsync(_connectionString, new RespawnerOptions
        {
            TablesToIgnore = new Table[] { "__EFMigrationsHistory" }
        });

        await ResetStateAsync();
    }

    public async Task<TEntity?> FindAsync<TEntity>(params object[] keyValues) where TEntity : class
    {
        using var scope = _scopeFactory.CreateScope();

        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        return await context.FindAsync<TEntity>(keyValues);
    }

    public async Task<TEntity?> FindAsync<TEntity>(object key, params string[] inlcudePropertyNames) where TEntity : class
    {
        using var scope = _scopeFactory.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        var result = await context.FindAsync<TEntity>(key);
        if (result == null)
        {
            return null;
        }

        foreach (var propName in inlcudePropertyNames)
        {
            await context.Entry(result).Reference(propName).LoadAsync();
        }

        return result;
    }

    public async Task<List<Claim>> GetAuthorizationClaimsAsync(AspNetUser user)
    {
        using var scope = _scopeFactory.CreateScope();
        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<AspNetUser>>();
        var claims = await userManager.GetClaimsAsync(user);
        return claims.Where(x => x.Type == Constants.Claims.ClaimType).ToList();
    }

    public async Task<List<string>> GetUserRolesAsync(AspNetUser user)
    {
        using var scope = _scopeFactory.CreateScope();
        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<AspNetUser>>();
        var roles = await userManager.GetRolesAsync(user);
        return roles.ToList();
    }

    public async Task AddAsync<TEntity>(TEntity entity) where TEntity : class
    {
        using var scope = _scopeFactory.CreateScope();

        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        context.Add(entity);

        await context.SaveChangesAsync();
    }

    public async Task<Company> AddCompanyAsync(string? managerId = null)
    {
        var company = Faker.Builders.Company(managerId);
        await AddAsync(company);
        return company;
    }

    public async Task<int> CountAsync<TEntity>() where TEntity : class
    {
        using var scope = _scopeFactory.CreateScope();

        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        return await context.Set<TEntity>().CountAsync();
    }

    public IServiceScope GetServiceScope()
    {
        return _scopeFactory.CreateScope();
    }

    public async Task<ApplicationUser> CreateUserAsync()
    {
        using var scope = _scopeFactory.CreateScope();
        var service = scope.ServiceProvider.GetRequiredService<IIdentityService>();
        var (result, userId) = await service.CreateUserAsync(Faker.Internet.Email(), "Foobar1!");
        return await FindAsync<ApplicationUser>(userId) ?? throw new InvalidOperationException("Failed to create a user");
    }

    internal Task RunAsDefaultUserAsync(object setManagerFromAnyToNone)
    {
        throw new NotImplementedException();
    }
}
