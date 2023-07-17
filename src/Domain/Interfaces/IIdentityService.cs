using System.Security.Claims;

namespace CRM.Domain.Interfaces;

public interface IIdentityService
{
    Task<string?> GetUserNameAsync(string userId);
    Task<bool> IsInRoleAsync(string userId, string role);
    Task<(Result Result, string UserId)> CreateUserAsync(string userName, string password);
    Task<(Result Result, string UserId)> CreateUserAsync(string userName, string password, string firstName, string lastName);
    Task<(Result Result, string UserId)> CreateUserAsync(string userName, string password, string firstName, string lastName, string[] claims, string[] roles);
    Task<Result> DeleteUserAsync(string userId);
    Task<Result> UpdateAuthorizationClaimsAsync(string userId, string[] claims);
    Task<string[]> GetUserAuthorizationClaimsAsync(string userId);
}
