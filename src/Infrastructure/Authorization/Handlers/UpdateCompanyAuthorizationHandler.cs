using CRM.Infrastructure.Services;
using Microsoft.AspNetCore.Authorization;

namespace CRM.Infrastructure.Authorization.Handlers;

public class UpdateCompanyRequirement : IAuthorizationRequirement { }

public class UpdateCompanyAuthorizationHandler : BaseAuthorizationHandler<UpdateCompanyRequirement>
{
    public UpdateCompanyAuthorizationHandler(IUserAuthorizationService userAuthorizationService) : base(userAuthorizationService)
    {
    }

    protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, UpdateCompanyRequirement requirement)
    {
        if (AuthorizationService.CanUpdateCompany(context.User))
        {
            context.Succeed(requirement);
        }

        return Task.CompletedTask;
    }
}

