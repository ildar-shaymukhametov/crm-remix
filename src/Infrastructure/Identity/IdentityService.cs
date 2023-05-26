using System.Security.Claims;
using CRM.Application;
using CRM.Application.Common.Interfaces;
using CRM.Application.Common.Models;
using CRM.Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace CRM.Infrastructure.Identity;

public class IdentityService : IIdentityService
{
    private readonly UserManager<AspNetUser> _userManager;
    private readonly IUserClaimsPrincipalFactory<AspNetUser> _userClaimsPrincipalFactory;
    private readonly IAuthorizationService _authorizationService;
    private readonly IApplicationDbContext _dbContext;
    private readonly ILogger<IdentityService> _logger;
    private readonly RoleManager<IdentityRole> _roleManager;

    public IdentityService(UserManager<AspNetUser> userManager, IUserClaimsPrincipalFactory<AspNetUser> userClaimsPrincipalFactory, IAuthorizationService authorizationService, IApplicationDbContext dbContext, ILogger<IdentityService> logger, RoleManager<IdentityRole> roleManager)
    {
        _userManager = userManager;
        _userClaimsPrincipalFactory = userClaimsPrincipalFactory;
        _authorizationService = authorizationService;
        _dbContext = dbContext;
        _logger = logger;
        _roleManager = roleManager;
    }

    public async Task<string?> GetUserNameAsync(string userId)
    {
        var user = await _userManager.Users.FirstOrDefaultAsync(u => u.Id == userId);

        return user?.UserName;
    }

    public async Task<(Result Result, string UserId)> CreateUserAsync(string userName, string password)
    {
        return await CreateUserAsync(userName, password, string.Empty, string.Empty);
    }

    public async Task<(Result Result, string UserId)> CreateUserAsync(string userName, string password, string firstName, string lastName)
    {
        return await CreateUserAsync(userName, password, firstName, lastName, Array.Empty<string>(), Array.Empty<string>());
    }

    public async Task<(Result Result, string UserId)> CreateUserAsync(string userName, string password, string firstName, string lastName, string[] claims, string[] roles)
    {
        var user = new AspNetUser
        {
            UserName = userName,
            Email = userName,
        };

        using var transaction = await _dbContext.Database.BeginTransactionAsync();
        var result = await _userManager.CreateAsync(user, password);
        if (!result.Succeeded)
        {
            return (result.ToApplicationResult(), user.Id);
        }

        if (claims.Any())
        {
            var principal = await _userClaimsPrincipalFactory.CreateAsync(user);
            var authorizationClaims = principal.Claims.Where(x => x.Type == Constants.Claims.ClaimType);
            var removeClaimsResult = await _userManager.RemoveClaimsAsync(user, authorizationClaims);
            var newAuthorizationClaims = claims.Select(x => new Claim(Constants.Claims.ClaimType, x));
            var addClaimsResult = await _userManager.AddClaimsAsync(user, newAuthorizationClaims);
            if (!addClaimsResult.Succeeded)
            {
                return (result.ToApplicationResult(), user.Id);
            }
        }

        if (roles.Any())
        {
            foreach (var roleName in roles)
            {
                var role = new IdentityRole(roleName);
                if (!await _roleManager.RoleExistsAsync(role.Name))
                {
                    await _roleManager.CreateAsync(role);
                }
            }

            var addToRolesResult = await _userManager.AddToRolesAsync(user, roles);
            if (!addToRolesResult.Succeeded)
            {
                return (result.ToApplicationResult(), user.Id);
            }
        }

        var appUser = new ApplicationUser
        {
            Id = user.Id,
            Email = user.Email,
            FirstName = firstName,
            LastName = lastName
        };

        try
        {
            _dbContext.ApplicationUsers.Add(appUser);
            await _dbContext.SaveChangesAsync();
            await transaction.CommitAsync();
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync();
            _logger.LogError(ex, "Failed to create an ApplicationUser. Email: {email}", appUser.Email);
            result = IdentityResult.Failed(new IdentityError
            {
                Code = "CreateApplicationUserFail",
                Description = "Failed to create an application user"
            });

            return (result.ToApplicationResult(), user.Id);
        }

        return (result.ToApplicationResult(), user.Id);
    }

    public async Task<bool> IsInRoleAsync(string userId, string role)
    {
        var user = await _userManager.FindByIdAsync(userId);

        return user != null && await _userManager.IsInRoleAsync(user, role);
    }

    public async Task<Result> AuthorizeAsync(string userId, string policyName)
    {
        return await AuthorizeAsync(userId, null, policyName);
    }

    public async Task<Result> AuthorizeAsync(ClaimsPrincipal principal, string policyName)
    {
        var result = await _authorizationService.AuthorizeAsync(principal, policyName);
        return result.ToApplicationResult();
    }

    public async Task<Result> AuthorizeAsync(string userId, object? resource, string policyName)
    {
        var user = await _userManager.FindByIdAsync(userId);

        if (user == null)
        {
            return Result.Failure(new[] { "User is null" });
        }

        var principal = await _userClaimsPrincipalFactory.CreateAsync(user);

        return await AuthorizeAsync(principal, resource, policyName);
    }

    public async Task<Result> AuthorizeAsync(ClaimsPrincipal principal, object? resource, string policyName)
    {
        var result = await _authorizationService.AuthorizeAsync(principal, resource, policyName);
        return result.ToApplicationResult();
    }

    public async Task<Result> DeleteUserAsync(string userId)
    {
        var user = await _userManager.FindByIdAsync(userId);

        return user != null ? await DeleteUserAsync(user) : Result.Success();
    }

    public async Task<Result> DeleteUserAsync(AspNetUser user)
    {
        var result = await _userManager.DeleteAsync(user);

        return result.ToApplicationResult();
    }

    public async Task<Result> UpdateAuthorizationClaimsAsync(string userId, string[] claims)
    {
        var user = await _userManager.FindByIdAsync(userId);
        var principal = await _userClaimsPrincipalFactory.CreateAsync(user);
        var authorizationClaims = principal.Claims.Where(x => x.Type == Constants.Claims.ClaimType);
        var removeClaimsResult = await _userManager.RemoveClaimsAsync(user, authorizationClaims);
        if (!claims.Any())
        {
            return removeClaimsResult.ToApplicationResult();
        }

        var newAuthorizationClaims = claims.Select(x => new Claim(Constants.Claims.ClaimType, x));
        var result = await _userManager.AddClaimsAsync(user, newAuthorizationClaims);
        return result.ToApplicationResult();
    }

    public async Task<string[]> GetUserAuthorizationClaimsAsync(string? userId)
    {
        var user = await _userManager.FindByIdAsync(userId);
        var principal = await _userClaimsPrincipalFactory.CreateAsync(user);
        return principal.Claims.Where(x => x.Type == Constants.Claims.ClaimType).Select(x => x.Value).ToArray();
    }

    public async Task<bool> IsAdminAsync(string userId)
    {
        return await IsInRoleAsync(userId, Constants.Roles.Administrator);
    }
}
