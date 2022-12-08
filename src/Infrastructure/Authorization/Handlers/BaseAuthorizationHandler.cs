using CRM.Application.Common.Interfaces;
using Microsoft.AspNetCore.Authorization;
using static CRM.Application.Constants;

namespace CRM.Infrastructure.Authorization.Handlers;

public abstract class BaseAuthorizationHandler<TRequirement> : AuthorizationHandler<TRequirement> where TRequirement : IAuthorizationRequirement
{
    protected readonly IPermissionsService _permissionsService;

    public BaseAuthorizationHandler(IPermissionsService permissionsService)
    {
        _permissionsService = permissionsService;
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
}

