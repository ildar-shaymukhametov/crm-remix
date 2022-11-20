using CRM.Application.Common.Models;

namespace CRM.Application.Common.Interfaces;

public interface IIdentityService
{
    Task<string> GetUserNameAsync(string userId);
    Task<bool> IsInRoleAsync(string userId, string role);
    Task<bool> AuthorizeAsync(string userId, string policyName);
    Task<(Result Result, string UserId)> CreateUserAsync(string userName, string password);
    Task<Result> DeleteUserAsync(string userId);
    Task<Result> UpdateAuthorizationClaimsAsync(string userId, string[] claims);
    Task<string[]> GetUserAuthorizationClaimsAsync(string userId);
}
