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
            result.Add(Access.Company.Any.View);
        }

        if (IsAdmin(user) || HasAnyClaim(user, Claims.Company.Any.Other.View, Claims.Company.Any.Other.Update))
        {
            result.Add(Access.Company.Any.Other.View);
        }

        if (IsAdmin(user) || HasAnyClaim(user, new[]
            {
                Claims.Company.Any.Other.Update
            }))
        {
            result.Add(Access.Company.Any.Other.Update);
        }

        if (IsAdmin(user) || HasAnyClaim(user, new[]
            {
                Claims.Company.Any.Manager.SetFromAnyToAny,
                Claims.Company.Any.Manager.SetFromAnyToSelf,
                Claims.Company.Any.Manager.SetFromNoneToSelf,
                Claims.Company.Any.Manager.SetFromNoneToAny,
            }))
        {
            result.Add(Access.Company.SetNewCompanyManager);
        }

        if (IsAdmin(user) || HasAnyClaim(user, new[]
            {
                Claims.Company.Any.Manager.SetFromNoneToSelf,
                Claims.Company.Any.Manager.SetFromNoneToAny,
                Claims.Company.Any.Manager.SetFromAnyToSelf,
                Claims.Company.Any.Manager.SetFromAnyToAny,
                Claims.Company.Any.Manager.SetFromSelfToAny,
                Claims.Company.Any.Manager.SetFromSelfToNone,
                Claims.Company.Any.Manager.SetFromAnyToNone,
            }))
        {
            result.Add(Access.Company.SetExistingCompanyManager);
        }

        if (IsAdmin(user) || HasAnyClaim(user, new[]
            {
                Claims.Company.Any.Manager.SetFromNoneToAny,
                Claims.Company.Any.Manager.SetFromNoneToSelf,
                Claims.Company.Any.Manager.SetFromAnyToAny,
                Claims.Company.Any.Manager.SetFromAnyToNone,
                Claims.Company.Any.Manager.SetFromAnyToSelf
            }))
        {
            result.Add(Access.Company.SetManagerFromNone);
        }

        if (IsAdmin(user) || HasAnyClaim(user, new[]
            {
                Claims.Company.Any.Manager.SetFromNoneToAny,
                Claims.Company.Any.Manager.SetFromSelfToAny,
                Claims.Company.Any.Manager.SetFromAnyToAny,
            }))
        {
            result.Add(Access.Company.SetManagerToAny);
        }

        if (IsAdmin(user) || HasAnyClaim(user, new[]
            {
                Claims.Company.Any.Manager.SetFromAnyToNone,
                Claims.Company.Any.Manager.SetFromAnyToSelf,
                Claims.Company.Any.Manager.SetFromAnyToAny,
            }))
        {
            result.Add(Access.Company.SetManagerFromAny);
        }

        if (IsAdmin(user) || HasAnyClaim(user, new[]
            {
                Claims.Company.Any.Manager.SetFromNoneToSelf,
                Claims.Company.Any.Manager.SetFromNoneToAny,
                Claims.Company.Any.Manager.SetFromAnyToSelf,
                Claims.Company.Any.Manager.SetFromAnyToAny
            }))
        {
            result.Add(Access.Company.Any.Manager.SetFromNoneToSelf);
        }

        if (IsAdmin(user) || HasAnyClaim(user, new[]
            {
                Claims.Company.Any.Manager.SetFromNoneToAny,
                Claims.Company.Any.Manager.SetFromAnyToAny
            }))
        {
            result.Add(Access.Company.Any.Manager.SetFromNoneToAny);
        }

        if (IsAdmin(user) || HasAnyClaim(user, new[]
            {
                Claims.Company.Any.Manager.SetFromAnyToNone,
                Claims.Company.Any.Manager.SetFromAnyToAny
            }))
        {
            result.Add(Access.Company.Any.Manager.SetFromAnyToNone);
        }

        if (IsAdmin(user) || HasAnyClaim(user, new[]
            {
                Claims.Company.Any.Manager.SetFromSelfToAny,
                Claims.Company.Any.Manager.SetFromSelfToNone,
                Claims.Company.Any.Manager.SetFromAnyToAny,
                Claims.Company.Any.Manager.SetFromAnyToNone
            }))
        {
            result.Add(Access.Company.Any.Manager.SetFromSelfToNone);
            result.Add(Access.Company.WhereUserIsManager.Manager.SetFromSelfToNone);
        }

        if (IsAdmin(user) || HasAnyClaim(user, new[]
            {
                Claims.Company.Any.Manager.SetFromAnyToSelf,
                Claims.Company.Any.Manager.SetFromAnyToAny
            }))
        {
            result.Add(Access.Company.Any.Manager.SetFromAnyToSelf);
        }

        if (IsAdmin(user) || HasAnyClaim(user, new[]
            {
                Claims.Company.Any.Manager.SetFromSelfToAny,
                Claims.Company.Any.Manager.SetFromAnyToAny
            }))
        {
            result.Add(Access.Company.Any.Manager.SetFromSelfToAny);
            result.Add(Access.Company.WhereUserIsManager.Manager.SetFromSelfToAny);
        }

        if (IsAdmin(user) || HasAnyClaim(user, new[]
            {
                Claims.Company.Any.Manager.SetFromAnyToAny
            }))
        {
            result.Add(Access.Company.Any.Manager.SetFromAnyToAny);
        }

        if (IsAdmin(user) || HasAnyClaim(user, new[]
            {
                Claims.Company.Any.Manager.SetFromSelfToAny,
                Claims.Company.Any.Manager.SetFromAnyToAny,
                Claims.Company.Any.Manager.SetFromSelfToNone,
            }))
        {
            result.Add(Access.Company.SetManagerFromSelf);
        }

        if (IsAdmin(user) || HasAnyClaim(user, new[]
            {
                Claims.Company.Any.Manager.SetFromAnyToAny,
                Claims.Company.Any.Manager.SetFromAnyToSelf,
                Claims.Company.Any.Manager.SetFromNoneToSelf,
                Claims.Company.Any.Manager.SetFromNoneToAny,
            }))
        {
            result.Add(Access.Company.SetManagerToSelf);
        }

        if (IsAdmin(user) || HasAnyClaim(user, new[]
            {
                Claims.Company.Any.Other.Update,
                Claims.Company.WhereUserIsManager.Other.Update
            }))
        {
            result.Add(Access.Company.WhereUserIsManager.Other.Update);
        }

        if (IsAdmin(user) || HasAnyClaim(user, new[]
            {
                Claims.Company.WhereUserIsManager.Delete
            }))
        {
            result.Add(Access.Company.WhereUserIsManager.Delete);
        }

        if (IsAdmin(user) || HasAnyClaim(user, new[]
            {
                Claims.Company.Any.Update
            }))
        {
            result.Add(Access.Company.Any.Update);
            result.Add(Access.Company.Any.View);
        }

        if (IsAdmin(user) || HasAnyClaim(user, new[]
            {
                Claims.Company.WhereUserIsManager.Update
            }))
        {
            result.Add(Access.Company.WhereUserIsManager.Update);
        }

        if (IsAdmin(user) || HasAnyClaim(user, new[]
            {
                Claims.Company.WhereUserIsManager.Manager.SetFromSelfToNone
            }))
        {
            result.Add(Access.Company.WhereUserIsManager.Manager.SetFromSelfToNone);
        }

        if (IsAdmin(user) || HasAnyClaim(user, new[]
            {
                Claims.Company.WhereUserIsManager.Manager.SetFromSelfToAny
            }))
        {
            result.Add(Access.Company.WhereUserIsManager.Manager.SetFromSelfToNone);
        }

        if (IsAdmin(user) || HasAnyClaim(user, new[]
            {
                Claims.Company.Any.Manager.View
            }))
        {
            result.Add(Access.Company.Any.Manager.View);
        }

        if (IsAdmin(user) || HasAnyClaim(user, new[]
            {
                Claims.Company.WhereUserIsManager.Other.View
            }))
        {
            result.Add(Access.Company.WhereUserIsManager.Other.View);
        }

        if (IsAdmin(user) || HasAnyClaim(user, new[]
            {
                Claims.Company.WhereUserIsManager.Manager.View
            }))
        {
            result.Add(Access.Company.WhereUserIsManager.Manager.View);
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
