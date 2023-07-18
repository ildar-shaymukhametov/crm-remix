using CRM.Domain.Common;
using CRM.Domain.Interfaces;

namespace ConsoleClient.Identity;

internal class IdentityService : IIdentityService
{
    public Task<(Result Result, string UserId)> CreateUserAsync(string userName, string password)
    {
        throw new NotImplementedException();
    }

    public Task<(Result Result, string UserId)> CreateUserAsync(string userName, string password, string firstName, string lastName)
    {
        throw new NotImplementedException();
    }

    public Task<(Result Result, string UserId)> CreateUserAsync(string userName, string password, string firstName, string lastName, string[] claims, string[] roles)
    {
        throw new NotImplementedException();
    }

    public Task<Result> DeleteUserAsync(string userId)
    {
        throw new NotImplementedException();
    }

    public Task<string[]> GetUserAuthorizationClaimsAsync(string userId)
    {
        throw new NotImplementedException();
    }

    public Task<string?> GetUserNameAsync(string userId)
    {
        throw new NotImplementedException();
    }

    public Task<bool> IsInRoleAsync(string userId, string role)
    {
        throw new NotImplementedException();
    }

    public Task<Result> UpdateAuthorizationClaimsAsync(string userId, string[] claims)
    {
        throw new NotImplementedException();
    }
}
