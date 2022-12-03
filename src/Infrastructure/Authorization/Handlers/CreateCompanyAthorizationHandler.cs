using CRM.Infrastructure.Services;
using Microsoft.AspNetCore.Authorization;
using static CRM.Application.Constants;

namespace CRM.Infrastructure.Authorization.Handlers;

public class CreateCompanyRequirement : IAuthorizationRequirement { }

public class CreateCompanyAthorizationHandler : BaseAuthorizationHandler<CreateCompanyRequirement>
{
    public CreateCompanyAthorizationHandler(IUserAuthorizationService userAuthorizationService) : base(userAuthorizationService) { }

    protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, CreateCompanyRequirement requirement)
    {
        if (IsAdmin(context) || HasAnyClaim(context, Claims.CreateCompany))
        {
            context.Succeed(requirement);
        }

        return Task.CompletedTask;
    }
}

