using CRM.Infrastructure.Services;
using Microsoft.AspNetCore.Authorization;

namespace CRM.Infrastructure.Authorization.Handlers;

public class DeleteCompanyRequirement : IAuthorizationRequirement { }

public class DeleteCompanyAuthorizationHandler : BaseAuthorizationHandler<DeleteCompanyRequirement>
{
    public DeleteCompanyAuthorizationHandler(IUserAuthorizationService userAuthorizationService) : base(userAuthorizationService)
    {
    }

    protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, DeleteCompanyRequirement requirement)
    {
        if (AuthorizationService.CanDeleteCompany(context.User))
        {
            context.Succeed(requirement);
        }

        return Task.CompletedTask;
    }
}

