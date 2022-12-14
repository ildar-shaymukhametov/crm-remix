using System.Security.Claims;
using CRM.Application.Common.Interfaces;
using CRM.Infrastructure.Identity;
using Microsoft.AspNetCore.Identity;
using static CRM.Application.Constants;

namespace CRM.Infrastructure.Services;

public class AccessService : IAccessService
{
    private readonly UserManager<AspNetUser> _userManager;
    private readonly IUserClaimsPrincipalFactory<AspNetUser> _userClaimsPrincipalFactory;

    public AccessService(UserManager<AspNetUser> userManager, IUserClaimsPrincipalFactory<AspNetUser> userClaimsPrincipalFactory)
    {
        _userManager = userManager;
        _userClaimsPrincipalFactory = userClaimsPrincipalFactory;
    }

    public async Task<string[]> CheckAccessAsync(string userId)
    {
        var user = await _userManager.FindByIdAsync(userId);
        if (user == null)
        {
            return Array.Empty<string>();
        }

        var principal = await _userClaimsPrincipalFactory.CreateAsync(user);

        return CheckAccess(principal);
    }

    public string[] CheckAccess(ClaimsPrincipal user)
    {
        var result = new List<string>();

        if (IsAdmin(user) || HasAnyClaim(user, Claims.CreateCompany))
        {
            result.Add(Access.CreateCompany);
        }

        if (IsAdmin(user) || HasAnyClaim(user,
                Claims.ViewCompany,
                Claims.UpdateCompany,
                Claims.DeleteCompany,
                Claims.ViewAnyCompany,
                Claims.UpdateAnyCompany,
                Claims.DeleteAnyCompany)
            )
        {
            result.Add(Access.ViewOwnCompany);
        }

        if (IsAdmin(user) || HasAnyClaim(user, new[]
            {
                Claims.UpdateCompany,
                Claims.UpdateAnyCompany
            }))
        {
            result.Add(Access.UpdateOwnCompany);
        }

        if (IsAdmin(user) || HasAnyClaim(user, Claims.DeleteCompany, Claims.DeleteAnyCompany))
        {
            result.Add(Access.DeleteOwnCompany);
        }

        if (IsAdmin(user) || HasAnyClaim(user, Claims.DeleteAnyCompany))
        {
            result.Add(Access.DeleteAnyCompany);
        }

        if (IsAdmin(user) || HasAnyClaim(user, Claims.ViewAnyCompany, Claims.DeleteAnyCompany, Claims.UpdateAnyCompany))
        {
            result.Add(Access.ViewAnyCompany);
        }

        if (IsAdmin(user) || HasAnyClaim(user, new[]
            {
                Claims.UpdateAnyCompany,
                Claims.Company.Any.Manager.None.Set.Self,
                Claims.Company.Any.Manager.None.Set.Any,
                Claims.Company.Any.Manager.Any.Set.Any
            }))
        {
            result.Add(Access.UpdateAnyCompany);
        }

        if (IsAdmin(user) || HasAnyClaim(user, new[]
            {
                Claims.Company.Any.Manager.None.Set.Self,
                Claims.Company.Any.Manager.Any.Set.Self
            }))
        {
            result.Add(Access.Company.Any.Manager.None.Set.Self);
        }

        if (IsAdmin(user) || HasAnyClaim(user, new[]
            {
                Claims.Company.Any.Manager.Any.Set.Self
            }))
        {
            result.Add(Access.Company.Any.Manager.Any.Set.Self);
        }

        if (IsAdmin(user) || HasAnyClaim(user, new[]
            {
                Claims.Company.Any.Manager.Any.Set.Any
            }))
        {
            result.Add(Access.Company.Any.Manager.Any.Set.Any);
        }

        if (IsAdmin(user) || HasAnyClaim(user, new[]
            {
                Claims.Company.Any.Manager.None.Set.Any,
                Claims.Company.Any.Manager.Any.Set.Any,
                Claims.UpdateAnyCompany
            }))
        {
            result.Add(Access.Company.Any.Manager.None.Set.Any);
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
