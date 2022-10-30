using CRM.Application;
using Microsoft.AspNetCore.Authorization;

namespace CRM.Infrastructure.Authorization.Handlers;

public class OrUserHasViewCompanyClaimRequirement : IOrAuthorizationRequirement { }

public class OrUserHasViewCompanyClaimHandler : AuthorizationHandler<OrUserHasViewCompanyClaimRequirement>
{
    protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, OrUserHasViewCompanyClaimRequirement requirement)
    {
        if (context.User.HasClaim(x => x.Value == Constants.Claims.ViewCompany))
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

