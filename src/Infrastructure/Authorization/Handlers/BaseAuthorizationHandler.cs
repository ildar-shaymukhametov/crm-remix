using CRM.Domain.Interfaces;
using Microsoft.AspNetCore.Authorization;
using static CRM.Application.Constants;

namespace CRM.Infrastructure.Authorization.Handlers;

public abstract class BaseAuthorizationHandler<TRequirement> : AuthorizationHandler<TRequirement> where TRequirement : IAuthorizationRequirement
{
    protected readonly IAccessService _accessService;

    public BaseAuthorizationHandler(IAccessService accessService)
    {
        _accessService = accessService;
    }

    protected bool IsAdmin(AuthorizationHandlerContext context)
    {
        return context.User.IsInRole(Roles.Administrator);
    }

    protected bool HasClaim(AuthorizationHandlerContext context, string claimValue)
    {
        return context.User.HasClaim(x => x.Value == claimValue);
    }

    protected bool HasAnyClaim(AuthorizationHandlerContext context, params string[] claimValues)
    {
        return context.User.Claims.Any(x => claimValues.Contains(x.Value));
    }

    protected static Task Ok(AuthorizationHandlerContext context, IAuthorizationRequirement requirement)
    {
        context.Succeed(requirement);
        return Task.CompletedTask;
    }

    protected Task Fail(AuthorizationHandlerContext context, string operation)
    {
        context.Fail(new AuthorizationFailureReason(this, $"Not enough access rights to perform this operation: {operation}"));
        return Task.CompletedTask;
    }
}

