﻿using System.Security.Claims;
using CRM.Application;
using CRM.Application.Common.Interfaces;
using CRM.Application.Common.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace CRM.Infrastructure.Identity;

public class IdentityService : IIdentityService
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IUserClaimsPrincipalFactory<ApplicationUser> _userClaimsPrincipalFactory;
    private readonly IAuthorizationService _authorizationService;

    public IdentityService(UserManager<ApplicationUser> userManager, IUserClaimsPrincipalFactory<ApplicationUser> userClaimsPrincipalFactory, IAuthorizationService authorizationService)
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

    // todo: test
    public async Task<(Result Result, string UserId)> CreateUserAsync(string userName, string password)
    {
        var user = new ApplicationUser
        {
            UserName = userName,
            Email = userName,
        };

        var result = await _userManager.CreateAsync(user, password);

        return (result.ToApplicationResult(), user.Id);
    }

    public async Task<bool> IsInRoleAsync(string userId, string role)
    {
        var user = await _userManager.FindByIdAsync(userId);

        return user != null && await _userManager.IsInRoleAsync(user, role);
    }

    public async Task<bool> AuthorizeAsync(string userId, string policyName)
    {
        var user = await _userManager.FindByIdAsync(userId);

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
        var user = await _userManager.FindByIdAsync(userId);

        return user != null ? await DeleteUserAsync(user) : Result.Success();
    }

    public async Task<Result> DeleteUserAsync(ApplicationUser user)
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
}
