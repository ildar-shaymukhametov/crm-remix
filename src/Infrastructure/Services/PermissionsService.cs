using System.Security.Claims;
using CRM.Application.Common.Interfaces;
using CRM.Infrastructure.Identity;
using Microsoft.AspNetCore.Identity;
using static CRM.Application.Constants;

namespace CRM.Infrastructure.Services;

public class PermissionsService : IPermissionsService
{
    private readonly UserManager<AspNetUser> _userManager;
    private readonly IUserClaimsPrincipalFactory<AspNetUser> _userClaimsPrincipalFactory;

    public PermissionsService(UserManager<AspNetUser> userManager, IUserClaimsPrincipalFactory<AspNetUser> userClaimsPrincipalFactory)
    {
        _userManager = userManager;
        _userClaimsPrincipalFactory = userClaimsPrincipalFactory;
    }

    public async Task<string[]> CheckAccessAsync(string userId, params string[] accessRights)
    {
        var user = await _userManager.FindByIdAsync(userId);
        if (user == null)
        {
            return Array.Empty<string>();
        }

        var principal = await _userClaimsPrincipalFactory.CreateAsync(user);
        var result = new List<string>();

        if (accessRights.Contains(Access.CreateCompany)
            && (IsAdmin(principal) || HasAnyClaim(principal, Claims.CreateCompany)))
        {
            result.Add(Access.CreateCompany);
        }

        if (accessRights.Contains(Access.ViewOwnCompany)
            && (IsAdmin(principal) || HasAnyClaim(principal,
                Claims.ViewCompany,
                Claims.UpdateCompany,
                Claims.DeleteCompany,
                Claims.ViewAnyCompany,
                Claims.UpdateAnyCompany,
                Claims.DeleteAnyCompany)
            ))
        {
            result.Add(Access.ViewOwnCompany);
        }

        if (accessRights.Contains(Access.UpdateOwnCompany)
            && (IsAdmin(principal) || HasAnyClaim(principal, Claims.UpdateCompany, Claims.UpdateAnyCompany)))
        {
            result.Add(Access.UpdateOwnCompany);
        }

        if (accessRights.Contains(Access.DeleteOwnCompany)
            && (IsAdmin(principal) || HasAnyClaim(principal, Claims.DeleteCompany, Claims.DeleteAnyCompany)))
        {
            result.Add(Access.DeleteOwnCompany);
        }

        if (accessRights.Contains(Access.DeleteAnyCompany)
            && (IsAdmin(principal) || HasAnyClaim(principal, Claims.DeleteAnyCompany)))
        {
            result.Add(Access.DeleteAnyCompany);
        }

        if (accessRights.Contains(Access.ViewAnyCompany)
            && (IsAdmin(principal) || HasAnyClaim(principal, Claims.ViewAnyCompany)))
        {
            result.Add(Access.ViewAnyCompany);
        }

        if (accessRights.Contains(Access.UpdateAnyCompany)
            && (IsAdmin(principal) || HasAnyClaim(principal, Claims.UpdateAnyCompany)))
        {
            result.Add(Access.UpdateAnyCompany);
        }

        return result.ToArray();
    }

    protected static bool HasAnyClaim(ClaimsPrincipal user, params string[] claimValues)
    {
        return user.Claims.Any(x => claimValues.Contains(x.Value));
    }

    protected static bool IsAdmin(ClaimsPrincipal user)
    {
        return user.IsInRole(Roles.Administrator);
    }
}
