using System.Security.Claims;
using CRM.Application.Common.Models;

namespace CRM.Application.Common.Interfaces;

public interface IIdentityService
{
    Task<string?> GetUserNameAsync(string userId);
    Task<bool> IsInRoleAsync(string userId, string role);
    Task<Result> AuthorizeAsync(string userId, string policyName);
    Task<Result> AuthorizeAsync(ClaimsPrincipal principal, string policyName);
    Task<Result> AuthorizeAsync(string userId, object? resource, string policyName);
    Task<Result> AuthorizeAsync(ClaimsPrincipal principal, object? resource, string policyName);
    Task<(Result Result, string UserId)> CreateUserAsync(string userName, string password);
    Task<(Result Result, string UserId)> CreateUserAsync(string userName, string password, string firstName, string lastName);
    Task<(Result Result, string UserId)> CreateUserAsync(string userName, string password, string firstName, string lastName, string[] claims, string[] roles);
    Task<Result> DeleteUserAsync(string userId);
    Task<Result> UpdateAuthorizationClaimsAsync(string userId, string[] claims);
    Task<string[]> GetUserAuthorizationClaimsAsync(string userId);
    Task<bool> IsAdminAsync(string userId);
}
