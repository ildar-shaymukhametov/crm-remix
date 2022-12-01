using System.Security.Claims;
using static CRM.Application.Constants;

namespace CRM.Infrastructure.Services;

public interface IUserAuthorizationService
{
    bool CanCreateCompany(ClaimsPrincipal principal);
    bool CanDeleteCompany(ClaimsPrincipal principal);
    bool CanUpdateCompany(ClaimsPrincipal principal);
    bool CanViewCompany(ClaimsPrincipal principal);
}

public class UserAuthorizationService : IUserAuthorizationService
{
    public bool CanViewCompany(ClaimsPrincipal principal)
    {
        return IsAdmin(principal) || HasAnyClaim(principal, Claims.ViewCompany, Claims.DeleteCompany, Claims.UpdateCompany);
    }

    public bool CanUpdateCompany(ClaimsPrincipal principal)
    {
        return IsAdmin(principal) || HasAnyClaim(principal, Claims.UpdateCompany);
    }

    public bool CanDeleteCompany(ClaimsPrincipal principal)
    {
        return IsAdmin(principal) || HasAnyClaim(principal, Claims.DeleteCompany);
    }

    public bool CanCreateCompany(ClaimsPrincipal principal)
    {
        return IsAdmin(principal) || HasAnyClaim(principal, Claims.CreateCompany);
    }

    private bool IsAdmin(ClaimsPrincipal user)
    {
        return user.IsInRole(Roles.Administrator);
    }

    private bool HasAnyClaim(ClaimsPrincipal user, params string[] claimValues)
    {
        return user.Claims.Any(x => claimValues.Contains(x.Value));
    }
}
