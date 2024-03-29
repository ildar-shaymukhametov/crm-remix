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

        if (IsAdmin(user) || HasAnyClaim(user, Claims.Company.Create))
        {
            result.Add(Access.Company.Create);
        }

        if (IsAdmin(user) || HasAnyClaim(user, Claims.Company.Any.Delete))
        {
            result.Add(Access.Company.Any.Delete);
        }

        if (IsAdmin(user) || HasAnyClaim(user, Claims.Company.Any.Other.Get))
        {
            result.Add(Access.Company.Any.Other.Get);
        }

        if (IsAdmin(user) || HasAnyClaim(user, Claims.Company.Any.Other.Set))
        {
            result.Add(Access.Company.Any.Other.Set);
        }

        if (IsAdmin(user) || HasAnyClaim(user, Claims.Company.Any.Manager.SetFromNoneToSelf))
        {
            result.Add(Access.Company.Any.Manager.SetFromNoneToSelf);
        }

        if (IsAdmin(user) || HasAnyClaim(user, Claims.Company.Any.Manager.SetFromNoneToAny))
        {
            result.Add(Access.Company.Any.Manager.SetFromNoneToAny);
        }

        if (IsAdmin(user) || HasAnyClaim(user, Claims.Company.Any.Manager.SetFromAnyToNone))
        {
            result.Add(Access.Company.Any.Manager.SetFromAnyToNone);
        }

        if (IsAdmin(user) || HasAnyClaim(user, Claims.Company.Any.Manager.SetFromSelfToNone))
        {
            result.Add(Access.Company.Any.Manager.SetFromSelfToNone);
        }

        if (IsAdmin(user) || HasAnyClaim(user, Claims.Company.Any.Manager.SetFromAnyToSelf))
        {
            result.Add(Access.Company.Any.Manager.SetFromAnyToSelf);
        }

        if (IsAdmin(user) || HasAnyClaim(user, Claims.Company.Any.Manager.SetFromSelfToAny))
        {
            result.Add(Access.Company.Any.Manager.SetFromSelfToAny);
        }

        if (IsAdmin(user) || HasAnyClaim(user, Claims.Company.WhereUserIsManager.Manager.SetFromSelfToAny))
        {
            result.Add(Access.Company.WhereUserIsManager.Manager.SetFromSelfToAny);
        }

        if (IsAdmin(user) || HasAnyClaim(user, Claims.Company.Any.Manager.SetFromAnyToAny))
        {
            result.Add(Access.Company.Any.Manager.SetFromAnyToAny);
        }

        if (IsAdmin(user) || HasAnyClaim(user, Claims.Company.WhereUserIsManager.Other.Set))
        {
            result.Add(Access.Company.WhereUserIsManager.Other.Set);
        }

        if (IsAdmin(user) || HasAnyClaim(user, Claims.Company.WhereUserIsManager.Delete))
        {
            result.Add(Access.Company.WhereUserIsManager.Delete);
        }

        if (IsAdmin(user) || HasAnyClaim(user, Claims.Company.WhereUserIsManager.Manager.SetFromSelfToNone))
        {
            result.Add(Access.Company.WhereUserIsManager.Manager.SetFromSelfToNone);
        }

        if (IsAdmin(user) || HasAnyClaim(user, Claims.Company.WhereUserIsManager.Manager.SetFromSelfToAny))
        {
            result.Add(Access.Company.WhereUserIsManager.Manager.SetFromSelfToAny);
        }

        if (IsAdmin(user) || HasAnyClaim(user, Claims.Company.Any.Manager.Get))
        {
            result.Add(Access.Company.Any.Manager.Get);
        }

        if (IsAdmin(user) || HasAnyClaim(user, Claims.Company.WhereUserIsManager.Other.Get))
        {
            result.Add(Access.Company.WhereUserIsManager.Other.Get);
        }

        if (IsAdmin(user) || HasAnyClaim(user, Claims.Company.WhereUserIsManager.Manager.Get))
        {
            result.Add(Access.Company.WhereUserIsManager.Manager.Get);
        }

        if (IsAdmin(user) || HasAnyClaim(user, Claims.Company.New.Other.Set))
        {
            result.Add(Access.Company.New.Other.Set);
        }

        if (IsAdmin(user) || HasAnyClaim(user, Claims.Company.New.Manager.SetToAny))
        {
            result.Add(Access.Company.New.Manager.SetToAny);
        }

        if (IsAdmin(user) || HasAnyClaim(user, Claims.Company.New.Manager.SetToSelf))
        {
            result.Add(Access.Company.New.Manager.SetToSelf);
        }

        if (IsAdmin(user) || HasAnyClaim(user, Claims.Company.New.Manager.SetToNone))
        {
            result.Add(Access.Company.New.Manager.SetToNone);
        }

        if (IsAdmin(user) || HasAnyClaim(user, Claims.Company.Any.Name.Get))
        {
            result.Add(Access.Company.Any.Name.Get);
        }

        if (IsAdmin(user) || HasAnyClaim(user, Claims.Company.WhereUserIsManager.Name.Get))
        {
            result.Add(Access.Company.WhereUserIsManager.Name.Get);
        }

        if (IsAdmin(user) || HasAnyClaim(user, Claims.Company.Any.Name.Set))
        {
            result.Add(Access.Company.Any.Name.Set);
        }

        if (IsAdmin(user) || HasAnyClaim(user, Claims.Company.WhereUserIsManager.Name.Set))
        {
            result.Add(Access.Company.WhereUserIsManager.Name.Set);
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
