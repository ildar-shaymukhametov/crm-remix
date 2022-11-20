using CRM.Infrastructure.Services;
using Microsoft.AspNetCore.Authorization;

namespace CRM.Infrastructure.Authorization.Handlers;

public class CreateCompanyRequirement : IAuthorizationRequirement { }

public class CreateCompanyAthorizationHandler : BaseAuthorizationHandler<CreateCompanyRequirement>
{
    public CreateCompanyAthorizationHandler(IUserAuthorizationService userAuthorizationService) : base(userAuthorizationService)
    {
    }

    protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, CreateCompanyRequirement requirement)
    {
        if (AuthorizationService.CanCreateCompany(context.User))
        {
            context.Succeed(requirement);
        }

        return Task.CompletedTask;
    }
}

