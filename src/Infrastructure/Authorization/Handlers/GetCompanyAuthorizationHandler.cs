using CRM.Infrastructure.Services;
using Microsoft.AspNetCore.Authorization;

namespace CRM.Infrastructure.Authorization.Handlers;

public class GetCompanyRequirement : IAuthorizationRequirement { }

public class GetCompanyAuthorizationHandler : BaseAuthorizationHandler<GetCompanyRequirement>
{
    public GetCompanyAuthorizationHandler(IUserAuthorizationService userAuthorizationService) : base(userAuthorizationService)
    {
    }

    protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, GetCompanyRequirement requirement)
    {
        if (AuthorizationService.CanViewCompany(context.User))
        {
            context.Succeed(requirement);
        }

        return Task.CompletedTask;
    }
}

