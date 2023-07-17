using System.Security.Claims;
using CRM.Domain.Common;
using CRM.Domain.Interfaces;

namespace CRM.Application.Common.Interfaces;

public interface IAppIdentityService : IIdentityService
{
    Task<Result> AuthorizeAsync(ClaimsPrincipal principal, string policyName);
    Task<Result> AuthorizeAsync(string userId, object? resource, string policyName);
    Task<Result> AuthorizeAsync(ClaimsPrincipal principal, object? resource, string policyName);
    Task<(Result Result, string UserId)> CreateUserAsync(string userName, string password);
    Task<(Result Result, string UserId)> CreateUserAsync(string userName, string password, string firstName, string lastName);
}
