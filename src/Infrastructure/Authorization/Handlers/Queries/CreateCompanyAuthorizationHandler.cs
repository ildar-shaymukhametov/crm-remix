using CRM.Application.Common.Extensions;
using CRM.Application.Common.Interfaces;
using Microsoft.AspNetCore.Authorization;
using static CRM.Application.Constants;

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
        if (!accessRights.ContainsAny(
            Access.Company.Create,
            Access.Company.New.SetOther,
            Access.Company.New.SetManager
        ))
        {
            return Fail(context, "Create company");
        }

        return Ok(context, requirement);
    }
}

