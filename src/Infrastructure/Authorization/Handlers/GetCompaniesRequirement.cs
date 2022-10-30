using Microsoft.AspNetCore.Authorization;
using static CRM.Application.Constants;

namespace CRM.Infrastructure.Authorization.Handlers;

public class GetCompaniesRequirement : IAuthorizationRequirement { }

public class GetCompaniesRequirementHandler : BaseAuthorizationHandler<GetCompaniesRequirement>
{
    protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, GetCompaniesRequirement requirement)
    {
        if (IsAdmin(context) || HasClaim(context, Claims.ViewCompany))
        {
            context.Succeed(requirement);
        }

        return Task.CompletedTask;
    }
}

