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

        if (IsAdmin(user) || HasAnyClaim(user,
                Claims.Company.Old.WhereUserIsManager.View,
                Claims.Company.Old.WhereUserIsManager.Update,
                Claims.Company.Old.WhereUserIsManager.Delete,
                Claims.Company.Old.Any.View,
                Claims.Company.Old.Any.Update,
                Claims.Company.Old.Any.Delete)
            )
        {
            result.Add(Access.Company.Old.WhereUserIsManager.View);
        }

        if (IsAdmin(user) || HasAnyClaim(user, new[]
            {
                Claims.Company.Old.WhereUserIsManager.Update,
                Claims.Company.Old.Any.Update
            }))
        {
            result.Add(Access.Company.Old.WhereUserIsManager.Update);
        }

        if (IsAdmin(user) || HasAnyClaim(user, Claims.Company.Old.WhereUserIsManager.Delete, Claims.Company.Old.Any.Delete))
        {
            result.Add(Access.Company.Old.WhereUserIsManager.Delete);
        }

        if (IsAdmin(user) || HasAnyClaim(user, Claims.Company.Old.Any.Delete))
        {
            result.Add(Access.Company.Old.Any.Delete);
        }

        if (IsAdmin(user) || HasAnyClaim(user, Claims.Company.Old.Any.View, Claims.Company.Old.Any.Delete, Claims.Company.Old.Any.Update))
        {
            result.Add(Access.Company.Old.Any.View);
        }

        if (IsAdmin(user) || HasAnyClaim(user, new[]
            {
                Claims.Company.Old.Any.Update,
                Claims.Company.Old.Any.SetManagerFromNoneToSelf,
                Claims.Company.Old.Any.SetManagerFromNoneToAny,
                Claims.Company.Old.Any.SetManagerFromAnyToAny
            }))
        {
            result.Add(Access.Company.Old.Any.Update);
        }

        if (IsAdmin(user) || HasAnyClaim(user, new[]
            {
                Claims.Company.New.SetManagerToSelf,
                Claims.Company.New.SetManagerToAny
            }))
        {
            result.Add(Access.Company.New.SetManager);
        }

        if (IsAdmin(user) || HasAnyClaim(user, new[]
            {
                Claims.Company.New.SetManagerToAny
            }))
        {
            result.Add(Access.Company.New.SetManagerToAny);
        }

        if (IsAdmin(user) || HasAnyClaim(user, new[]
            {
                Claims.Company.New.SetManagerToSelf
            }))
        {
            result.Add(Access.Company.New.SetManagerToSelf);
        }

        if (IsAdmin(user) || HasAnyClaim(user, new[]
            {
                Claims.Company.Old.Any.SetManagerFromNoneToSelf,
                Claims.Company.Old.Any.SetManagerFromNoneToAny,
                Claims.Company.Old.Any.SetManagerFromAnyToSelf,
                Claims.Company.Old.Any.SetManagerFromAnyToAny,
                Claims.Company.Old.Any.SetManagerFromSelfToAny,
                Claims.Company.Old.Any.SetManagerFromSelfToNone,
                Claims.Company.Old.Any.SetManagerFromAnyToNone,
            }))
        {
            result.Add(Access.Company.Old.SetManager);
        }

        if (IsAdmin(user) || HasAnyClaim(user, new[]
            {
                Claims.Company.Old.Any.SetManagerFromNoneToAny,
                Claims.Company.Old.Any.SetManagerFromNoneToSelf,
                Claims.Company.Old.Any.SetManagerFromAnyToSelf,
                Claims.Company.Old.Any.SetManagerFromAnyToAny,
                Claims.Company.Old.Any.SetManagerFromAnyToNone,
                Claims.Company.Old.Any.SetManagerFromSelfToAny,
                Claims.Company.Old.Any.SetManagerFromSelfToNone,
                Claims.Company.New.SetManagerToAny,
                Claims.Company.New.SetManagerToSelf
            }))
        {
            result.Add(Access.Company.SetManagerToOrFromNone);
        }

        if (IsAdmin(user) || HasAnyClaim(user, new[]
            {
                Claims.Company.Old.Any.SetManagerFromNoneToAny,
                Claims.Company.Old.Any.SetManagerFromSelfToAny,
                Claims.Company.Old.Any.SetManagerFromAnyToAny,
                Claims.Company.New.SetManagerToAny
            }))
        {
            result.Add(Access.Company.SetManagerToAny);
        }

        if (IsAdmin(user) || HasAnyClaim(user, new[]
            {
                Claims.Company.Old.Any.SetManagerFromNoneToSelf,
                Claims.Company.Old.Any.SetManagerFromAnyToSelf,
                Claims.Company.Old.Any.SetManagerFromSelfToNone,
                Claims.Company.New.SetManagerToSelf
            }))
        {
            result.Add(Access.Company.SetManagerToSelf);
        }

        if (IsAdmin(user) || HasAnyClaim(user, new[]
            {
                Claims.Company.Old.Any.SetManagerFromAnyToNone,
                Claims.Company.Old.Any.SetManagerFromAnyToSelf,
            }))
        {
            result.Add(Access.Company.Old.SetManagerFromAny);
        }

        if (IsAdmin(user) || HasAnyClaim(user, new[]
            {
                Claims.Company.Old.Any.SetManagerFromNoneToSelf,
                Claims.Company.Old.Any.SetManagerFromNoneToAny,
                Claims.Company.Old.Any.SetManagerFromAnyToSelf,
                Claims.Company.Old.Any.SetManagerFromAnyToAny
            }))
        {
            result.Add(Access.Company.Old.Any.SetManagerFromNoneToSelf);
        }

        if (IsAdmin(user) || HasAnyClaim(user, new[]
            {
                Claims.Company.Old.Any.SetManagerFromNoneToAny,
                Claims.Company.Old.Any.SetManagerFromAnyToAny
            }))
        {
            result.Add(Access.Company.Old.Any.SetManagerFromNoneToAny);
        }

        if (IsAdmin(user) || HasAnyClaim(user, new[]
            {
                Claims.Company.Old.Any.SetManagerFromAnyToNone,
                Claims.Company.Old.Any.SetManagerFromAnyToAny
            }))
        {
            result.Add(Access.Company.Old.Any.SetManagerFromAnyToNone);
        }

        if (IsAdmin(user) || HasAnyClaim(user, new[]
            {
                Claims.Company.Old.Any.SetManagerFromSelfToAny,
                Claims.Company.Old.Any.SetManagerFromSelfToNone,
                Claims.Company.Old.Any.SetManagerFromAnyToAny
            }))
        {
            result.Add(Access.Company.Old.Any.SetManagerFromSelfToNone);
        }

        if (IsAdmin(user) || HasAnyClaim(user, new[]
            {
                Claims.Company.Old.Any.SetManagerFromAnyToSelf,
                Claims.Company.Old.Any.SetManagerFromAnyToAny
            }))
        {
            result.Add(Access.Company.Old.Any.SetManagerFromAnyToSelf);
        }

        if (IsAdmin(user) || HasAnyClaim(user, new[]
            {
                Claims.Company.Old.Any.SetManagerFromSelfToAny,
                Claims.Company.Old.Any.SetManagerFromAnyToAny
            }))
        {
            result.Add(Access.Company.Old.Any.SetManagerFromSelfToAny);
        }

        if (IsAdmin(user) || HasAnyClaim(user, new[]
            {
                Claims.Company.Old.Any.SetManagerFromAnyToAny
            }))
        {
            result.Add(Access.Company.Old.Any.SetManagerFromAnyToAny);
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
