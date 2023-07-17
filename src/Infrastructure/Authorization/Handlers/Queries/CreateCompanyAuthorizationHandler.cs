using CRM.Domain.Services;
using Microsoft.AspNetCore.Authorization;

namespace CRM.Infrastructure.Authorization.Handlers.Queries;

public class CreateCompanyRequirement : IAuthorizationRequirement { }

public class CreateCompanyAuthorizationHandler : BaseAuthorizationHandler<CreateCompanyRequirement>
{
    public CreateCompanyAuthorizationHandler(IAccessService accessService) : base(accessService)
    {
    }

    protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, CreateCompanyRequirement requirement)
    {
        var accessRights = _accessService.CheckAccess(context.User);
        if (!accessRights.Contains(Domain.Constants.Access.Company.Create))
        {
            return Fail(context, "Create company");
        }

        return Ok(context, requirement);
    }
}

