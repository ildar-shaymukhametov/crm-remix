using System.Security.Claims;

namespace CRM.Application.Common.Interfaces;

public interface IAccessService
{
    Task<string[]> CheckAccessAsync(string userId);
    string[] CheckAccess(ClaimsPrincipal user);
}
