using System.Security.Claims;
using CRM.Application.Common.Interfaces;
using CRM.Domain.Common;
using CRM.Infrastructure.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;

namespace CRM.Infrastructure.Authorization;

public class AppAuthorizationService : IAppAuthorizationService
{
    private readonly UserManager<AspNetUser> _userManager;
    private readonly IUserClaimsPrincipalFactory<AspNetUser> _userClaimsPrincipalFactory;
    private readonly IAuthorizationService _authorizationService;

    public AppAuthorizationService(UserManager<AspNetUser> userManager, IUserClaimsPrincipalFactory<AspNetUser> userClaimsPrincipalFactory, IAuthorizationService authorizationService, RoleManager<IdentityRole> roleManager)
    {
        _userManager = userManager;
        _userClaimsPrincipalFactory = userClaimsPrincipalFactory;
        _authorizationService = authorizationService;
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
}
