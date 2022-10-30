using Microsoft.AspNetCore.Authorization;
using static CRM.Application.Constants;

namespace CRM.Infrastructure.Authorization.Handlers;

public class GetCompanyRequirement : IAuthorizationRequirement { }

public class GetCompanyAuthorizationHandler : BaseAuthorizationHandler<GetCompanyRequirement>
{
    protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, GetCompanyRequirement requirement)
    {
        if (IsAdmin(context) || HasClaim(context, Claims.ViewCompany))
        {
            context.Succeed(requirement);
        }

        return Task.CompletedTask;
    }
}

