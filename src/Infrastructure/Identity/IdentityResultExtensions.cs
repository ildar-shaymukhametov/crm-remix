using CRM.Application.Common.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;

namespace CRM.Infrastructure.Identity;

public static class IdentityResultExtensions
{
    public static Result ToApplicationResult(this IdentityResult result)
    {
        return result.Succeeded
            ? Result.Success()
            : Result.Failure(result.Errors.Select(e => e.Description));
    }

    public static Result ToApplicationResult(this AuthorizationResult result)
    {
        return result.Succeeded
            ? Result.Success()
            : Result.Failure(result.Failure?.FailureReasons.Select(x => x.Message) ?? Array.Empty<string>());
    }
}
