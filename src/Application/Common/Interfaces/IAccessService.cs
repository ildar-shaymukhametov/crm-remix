using System.Security.Claims;

namespace CRM.Application.Common.Interfaces;

public interface IAccessService
{
    Task<string[]> CheckAccessAsync(string userId, string[] permissions);
    string[] CheckAccess(ClaimsPrincipal user, string[] accessRights);
}
