using System.Security.Claims;

namespace CRM.Application.Common.Interfaces;

public interface IAccessService
{
    Task<string[]> CheckAccessAsync(string userId, params string[] permissions);
    string[] CheckAccess(ClaimsPrincipal user, params string[] accessRights);
}
