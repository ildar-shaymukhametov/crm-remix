using System.Security.Claims;
using CRM.Application.Common.Interfaces;
using CRM.Application.Common.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace CRM.Infrastructure.Identity;

public class IdentityService : IIdentityService
{
    private readonly UserManager<AspNetUser> _userManager;
    private readonly IUserClaimsPrincipalFactory<AspNetUser> _userClaimsPrincipalFactory;
    private readonly IAuthorizationService _authorizationService;

    public IdentityService(
        UserManager<AspNetUser> userManager,
        IUserClaimsPrincipalFactory<AspNetUser> userClaimsPrincipalFactory,
        IAuthorizationService authorizationService)
    {
        _userManager = userManager;
        _userClaimsPrincipalFactory = userClaimsPrincipalFactory;
        _authorizationService = authorizationService;
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

        var result = await _userManager.CreateAsync(user, password);

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

    public async Task<Result> UpdateClaimsAsync(string userId, string[] claims)
    {
        var user = await _userManager.FindByIdAsync(userId);
        var allClaims = await _userManager.GetClaimsAsync(user);
        var removeResult = await _userManager.RemoveClaimsAsync(user, allClaims.Where(x => x.Type == "auth"));
        if (!claims.Any())
        {
            return removeResult.ToApplicationResult();
        }

        var items = claims.Select(x => new Claim("auth", x));
        var result = await _userManager.AddClaimsAsync(user, items);
        return result.ToApplicationResult();
    }

    public async Task<string[]> GetUserClaimsAsync(string? userId)
    {
        var user = await _userManager.FindByIdAsync(userId);
        var claims = await _userManager.GetClaimsAsync(user);
        return claims.Where(x => x.Type == "auth").Select(x => x.Value).ToArray();
    }
}
