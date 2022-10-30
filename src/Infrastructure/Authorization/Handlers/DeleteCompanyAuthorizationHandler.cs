using Microsoft.AspNetCore.Authorization;
using static CRM.Application.Constants;

namespace CRM.Infrastructure.Authorization.Handlers;

public class DeleteCompanyRequirement : IAuthorizationRequirement { }

public class DeleteCompanyAuthorizationHandler : BaseAuthorizationHandler<DeleteCompanyRequirement>
{
    protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, DeleteCompanyRequirement requirement)
    {
        if (IsAdmin(context) || HasClaim(context, Claims.DeleteCompany))
        {
            context.Succeed(requirement);
        }

        return Task.CompletedTask;
    }
}

