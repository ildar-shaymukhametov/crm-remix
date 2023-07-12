using System.Security.Claims;

namespace CRM.Domain.Interfaces;

public interface IAccessService
{
    Task<string[]> CheckAccessAsync(string userId);
    string[] CheckAccess(ClaimsPrincipal user);
}
