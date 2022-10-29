using CRM.Application;
using CRM.Domain.Entities;
using Microsoft.AspNetCore.Authorization;

namespace CRM.Infrastructure.Authorization.Handlers.Companies;

public class UserIsCompanyManagerHandler : AuthorizationHandler<UserIsCompanyManagerRequirement, Company>
{
    protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, UserIsCompanyManagerRequirement requirement, Company resource)
    {
        if (context.User.IsInRole(Constants.Roles.Administrator)
            || context.User.HasClaim(x => x.Value == Constants.Authorization.Claims.UpdateCompany))
        {
            context.Succeed(requirement);
        }

        return Task.CompletedTask;
    }
}
public class UserIsCompanyManagerRequirement : IAuthorizationRequirement { }
