using CRM.Application;
using CRM.Domain.Entities;
using Microsoft.AspNetCore.Authorization;

namespace CRM.Infrastructure.Authorization.Handlers;

public class UserIsAdminHandler : AuthorizationHandler<UserIsAdminRequirement>
{
    protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, UserIsAdminRequirement requirement)
    {
        if (context.User.IsInRole(Constants.Roles.Administrator))
        {
            context.Succeed(requirement);
        }

        return Task.CompletedTask;
    }
}

public class UserIsAdminRequirement : IAuthorizationRequirement { }
