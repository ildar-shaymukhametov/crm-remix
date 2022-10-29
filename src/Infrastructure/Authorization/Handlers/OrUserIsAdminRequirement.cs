using CRM.Application;
using Microsoft.AspNetCore.Authorization;

namespace CRM.Infrastructure.Authorization.Handlers;

public class OrUserIsAdminRequirement : IOrAuthorizationRequirement { }

public class OrUserIsAdminHandler : AuthorizationHandler<OrUserIsAdminRequirement>
{
    protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, OrUserIsAdminRequirement requirement)
    {
        if (context.User.IsInRole(Constants.Roles.Administrator))
        {
            context.Succeed(requirement);
            if (requirement is IOrAuthorizationRequirement)
            {
                foreach (var orRequirement in context.PendingRequirements.OfType<IOrAuthorizationRequirement>())
                {
                    context.Succeed(orRequirement);
                }
            }
        }

        return Task.CompletedTask;
    }
}

