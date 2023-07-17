using System.Security.Claims;
using CRM.Domain.Common;

namespace CRM.Application.Common.Interfaces;

public interface IAppAuthorizationService
{
    Task<Result> AuthorizeAsync(ClaimsPrincipal principal, string policyName);
    Task<Result> AuthorizeAsync(string userId, object? resource, string policyName);
    Task<Result> AuthorizeAsync(ClaimsPrincipal principal, object? resource, string policyName);
}
