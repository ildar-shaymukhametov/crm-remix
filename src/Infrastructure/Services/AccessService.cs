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

        if (IsAdmin(user) || HasAnyClaim(user, Claims.Company.Any.View, Claims.Company.Any.Delete, Claims.Company.Any.Update))
        {
            result.Add(Access.Company.Any.View);
        }

        if (IsAdmin(user) || HasAnyClaim(user, new[]
            {
                Claims.Company.Any.Update,
                Claims.Company.Any.SetManagerFromNoneToSelf,
                Claims.Company.Any.SetManagerFromNoneToAny,
                Claims.Company.Any.SetManagerFromAnyToAny
            }))
        {
            result.Add(Access.Company.Any.Update);
        }

        if (IsAdmin(user) || HasAnyClaim(user, new[]
            {
                Claims.Company.Any.SetManagerFromAnyToAny,
                Claims.Company.Any.SetManagerFromAnyToSelf,
                Claims.Company.Any.SetManagerFromNoneToSelf,
                Claims.Company.Any.SetManagerFromNoneToAny,
            }))
        {
            result.Add(Access.Company.SetNewCompanyManager);
        }

        if (IsAdmin(user) || HasAnyClaim(user, new[]
            {
                Claims.Company.Any.SetManagerFromNoneToSelf,
                Claims.Company.Any.SetManagerFromNoneToAny,
                Claims.Company.Any.SetManagerFromAnyToSelf,
                Claims.Company.Any.SetManagerFromAnyToAny,
                Claims.Company.Any.SetManagerFromSelfToAny,
                Claims.Company.Any.SetManagerFromSelfToNone,
                Claims.Company.Any.SetManagerFromAnyToNone,
            }))
        {
            result.Add(Access.Company.SetExistingCompanyManager);
        }

        if (IsAdmin(user) || HasAnyClaim(user, new[]
            {
                Claims.Company.Any.SetManagerFromNoneToAny,
                Claims.Company.Any.SetManagerFromNoneToSelf,
                Claims.Company.Any.SetManagerFromAnyToAny,
                Claims.Company.Any.SetManagerFromAnyToNone,
                Claims.Company.Any.SetManagerFromAnyToSelf
            }))
        {
            result.Add(Access.Company.SetManagerFromNone);
        }

        if (IsAdmin(user) || HasAnyClaim(user, new[]
            {
                Claims.Company.Any.SetManagerFromNoneToAny,
                Claims.Company.Any.SetManagerFromSelfToAny,
                Claims.Company.Any.SetManagerFromAnyToAny,
            }))
        {
            result.Add(Access.Company.SetManagerToAny);
        }

        if (IsAdmin(user) || HasAnyClaim(user, new[]
            {
                Claims.Company.Any.SetManagerFromAnyToNone,
                Claims.Company.Any.SetManagerFromAnyToSelf,
                Claims.Company.Any.SetManagerFromAnyToAny,
            }))
        {
            result.Add(Access.Company.SetManagerFromAny);
        }

        if (IsAdmin(user) || HasAnyClaim(user, new[]
            {
                Claims.Company.Any.SetManagerFromNoneToSelf,
                Claims.Company.Any.SetManagerFromNoneToAny,
                Claims.Company.Any.SetManagerFromAnyToSelf,
                Claims.Company.Any.SetManagerFromAnyToAny
            }))
        {
            result.Add(Access.Company.Any.SetManagerFromNoneToSelf);
        }

        if (IsAdmin(user) || HasAnyClaim(user, new[]
            {
                Claims.Company.Any.SetManagerFromNoneToAny,
                Claims.Company.Any.SetManagerFromAnyToAny
            }))
        {
            result.Add(Access.Company.Any.SetManagerFromNoneToAny);
        }

        if (IsAdmin(user) || HasAnyClaim(user, new[]
            {
                Claims.Company.Any.SetManagerFromAnyToNone,
                Claims.Company.Any.SetManagerFromAnyToAny
            }))
        {
            result.Add(Access.Company.Any.SetManagerFromAnyToNone);
        }

        if (IsAdmin(user) || HasAnyClaim(user, new[]
            {
                Claims.Company.Any.SetManagerFromSelfToAny,
                Claims.Company.Any.SetManagerFromSelfToNone,
                Claims.Company.Any.SetManagerFromAnyToAny
            }))
        {
            result.Add(Access.Company.Any.SetManagerFromSelfToNone);
        }

        if (IsAdmin(user) || HasAnyClaim(user, new[]
            {
                Claims.Company.Any.SetManagerFromAnyToSelf,
                Claims.Company.Any.SetManagerFromAnyToAny
            }))
        {
            result.Add(Access.Company.Any.SetManagerFromAnyToSelf);
        }

        if (IsAdmin(user) || HasAnyClaim(user, new[]
            {
                Claims.Company.Any.SetManagerFromSelfToAny,
                Claims.Company.Any.SetManagerFromAnyToAny
            }))
        {
            result.Add(Access.Company.Any.SetManagerFromSelfToAny);
        }

        if (IsAdmin(user) || HasAnyClaim(user, new[]
            {
                Claims.Company.Any.SetManagerFromAnyToAny
            }))
        {
            result.Add(Access.Company.Any.SetManagerFromAnyToAny);
        }

        if (IsAdmin(user) || HasAnyClaim(user, new[]
            {
                Claims.Company.Any.SetManagerFromSelfToAny,
                Claims.Company.Any.SetManagerFromAnyToAny,
                Claims.Company.Any.SetManagerFromSelfToNone,
            }))
        {
            result.Add(Access.Company.SetManagerFromSelf);
        }

        if (IsAdmin(user) || HasAnyClaim(user, new[]
            {
                Claims.Company.Any.SetManagerFromAnyToAny,
                Claims.Company.Any.SetManagerFromAnyToSelf,
                Claims.Company.Any.SetManagerFromNoneToSelf,
                Claims.Company.Any.SetManagerFromNoneToAny,
            }))
        {
            result.Add(Access.Company.SetManagerToSelf);
        }

        if (IsAdmin(user) || HasAnyClaim(user, new[]
            {
                Claims.Company.WhereUserIsManager.View
            }))
        {
            result.Add(Access.Company.WhereUserIsManager.View);
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
