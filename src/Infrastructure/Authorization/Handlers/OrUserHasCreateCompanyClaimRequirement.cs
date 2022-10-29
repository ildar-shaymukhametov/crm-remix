using CRM.Application;
using Microsoft.AspNetCore.Authorization;

namespace CRM.Infrastructure.Authorization.Handlers;

public class OrUserHasCreateCompanyClaimRequirement : IOrAuthorizationRequirement { }

public class OrUserHasCreateCompanyClaimHandler : AuthorizationHandler<OrUserHasCreateCompanyClaimRequirement>
{
    protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, OrUserHasCreateCompanyClaimRequirement requirement)
    {
        if (context.User.HasClaim(x => x.Value == Constants.Authorization.Claims.CreateCompany))
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

