using CRM.Application.Common.Interfaces;
using CRM.Infrastructure.Identity;
using Microsoft.AspNetCore.Identity;
using static CRM.Application.Constants;

namespace CRM.Infrastructure.Services;

public class PermissionsVerifier : IPermissionsVerifier
{
    private readonly UserManager<AspNetUser> _userManager;
    private readonly IUserClaimsPrincipalFactory<AspNetUser> _userClaimsPrincipalFactory;
    private readonly IAppIdentityService _identityService;

    public PermissionsVerifier(UserManager<AspNetUser> userManager, IUserClaimsPrincipalFactory<AspNetUser> userClaimsPrincipalFactory, IAppIdentityService identityService)
    {
        _userManager = userManager;
        _userClaimsPrincipalFactory = userClaimsPrincipalFactory;
        _identityService = identityService;
    }

    /// <summary>
    /// Checks whether user has specific permissions.
    /// </summary>
    /// <param name="userId"></param>
    /// <param name="resourceKey">Optional resource id.</param>
    /// <param name="permissions">Permissions to check.</param>
    /// <returns>Permissions that passed the check.</returns>
    public async Task<string[]> VerifyAsync(string userId, string? resourceKey, params string[] permissions)
    {
        var user = await _userManager.FindByIdAsync(userId);
        if (user == null)
        {
            return Array.Empty<string>();
        }

        var principal = await _userClaimsPrincipalFactory.CreateAsync(user);
        var result = new List<string>();

        if (permissions.Contains(Permissions.Company.Create) && await _identityService.AuthorizeAsync(principal, Policies.Company.Queries.Create))
        {
            result.Add(Permissions.Company.Create);
        }

        return result.ToArray();
    }
}
