using CRM.Application.Common.Behaviours.Authorization.Resources;
using CRM.Application.Common.Interfaces;
using Duende.IdentityServer.Extensions;
using Microsoft.AspNetCore.Authorization;
using static CRM.Application.Constants;

namespace CRM.Infrastructure.Authorization.Handlers;

public class CreateCompanyRequirement : IAuthorizationRequirement { }

public class CreateCompanyAthorizationHandler : BaseAuthorizationHandler<CreateCompanyRequirement>
{
    public CreateCompanyAthorizationHandler(IAccessService accessService) : base(accessService)
    {
    }

    protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, CreateCompanyRequirement requirement)
    {
        var accessRights = _accessService.CheckAccess(context.User);
        if (!accessRights.Contains(Access.Company.Create))
        {
            return Fail(context, "Create company");
        }

        if (context.Resource is CreateCompanyResource resource)
        {
            if (resource.Request.ManagerId != null)
            {
                if (resource.Request.ManagerId == context.User.GetSubjectId())
                {
                    var canSetManagerToSelf = new[]
                    {
                        Access.Company.New.SetManagerToAny,
                        Access.Company.New.SetManagerToSelf
                    }.Any(accessRights.Contains);

                    if (!canSetManagerToSelf)
                    {
                        return Fail(context, "Set manager to self");
                    }
                }
            }
        }

        return Ok(context, requirement);
    }
}

