using CRM.Application;
using Microsoft.AspNetCore.Authorization;

namespace CRM.Infrastructure.Authorization.Handlers;

public class UserIsAdminHandler : AuthorizationHandler<UserIsAdminRequirement>
{
    protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, UserIsAdminRequirement requirement)
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

public class UserIsAdminRequirement : IOrAuthorizationRequirement { }
