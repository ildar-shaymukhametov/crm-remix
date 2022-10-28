using System.Security.Claims;
using CRM.Application.Common.Interfaces;
using CRM.Application.Common.Models;
using CRM.Domain.Entities;
using CRM.Infrastructure.Persistence;
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
    private readonly ApplicationDbContext _dbContext;
    private readonly ILogger<IdentityService> _logger;

    public IdentityService(
        UserManager<AspNetUser> userManager,
        IUserClaimsPrincipalFactory<AspNetUser> userClaimsPrincipalFactory,
        IAuthorizationService authorizationService,
        ApplicationDbContext dbContext,
        ILogger<IdentityService> logger)
    {
        _userManager = userManager;
        _userClaimsPrincipalFactory = userClaimsPrincipalFactory;
        _authorizationService = authorizationService;
        _dbContext = dbContext;
        _logger = logger;
    }

    public async Task<string> GetUserNameAsync(string userId)
    {
        var user = await _userManager.Users.FirstAsync(u => u.Id == userId);

        return user.UserName;
    }

    public async Task<(Result Result, string UserId)> CreateUserAsync(string userName, string password)
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

        var appUser = new ApplicationUser
        {
            Id = user.Id,
            Email = user.Email
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
        var user = _userManager.Users.SingleOrDefault(u => u.Id == userId);

        return user != null && await _userManager.IsInRoleAsync(user, role);
    }

    public async Task<bool> AuthorizeAsync(string userId, string policyName)
    {
        var user = _userManager.Users.SingleOrDefault(u => u.Id == userId);

        if (user == null)
        {
            return false;
        }

        var principal = await _userClaimsPrincipalFactory.CreateAsync(user);

        var result = await _authorizationService.AuthorizeAsync(principal, policyName);

        return result.Succeeded;
    }

    public async Task<Result> DeleteUserAsync(string userId)
    {
        var user = _userManager.Users.SingleOrDefault(u => u.Id == userId);

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
        var authorizationClaims = principal.Claims.Where(x => x.Type == "authorization");
        var removeResult = await _userManager.RemoveClaimsAsync(user, authorizationClaims);
        if (!claims.Any())
        {
            return removeResult.ToApplicationResult();
        }

        var newAuthorizationClaims = claims.Select(x => new Claim("authorization", x));
        var result = await _userManager.AddClaimsAsync(user, newAuthorizationClaims);
        return result.ToApplicationResult();
    }

    public async Task<string[]> GetUserAuthorizationClaimsAsync(string? userId)
    {
        var user = await _userManager.FindByIdAsync(userId);
        var principal = await _userClaimsPrincipalFactory.CreateAsync(user);
        return principal.Claims.Where(x => x.Type == "authorization").Select(x => x.Value).ToArray();
    }
}
