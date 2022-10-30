using Microsoft.AspNetCore.Authorization;
using static CRM.Application.Constants;

namespace CRM.Infrastructure.Authorization.Handlers;

public abstract class BaseAuthorizationHandler<TRequirement> : AuthorizationHandler<TRequirement> where TRequirement : IAuthorizationRequirement
{
    protected bool IsAdmin(AuthorizationHandlerContext context)
    {
        return context.User.IsInRole(Roles.Administrator);
    }

    protected bool HasClaim(AuthorizationHandlerContext context, string claimValue)
    {
        return context.User.HasClaim(x => x.Value == claimValue);
    }
}
