using CRM.Application.Common.Behaviours.Authorization.Resources;
using CRM.Application.Common.Extensions;
using CRM.Application.Common.Interfaces;
using Duende.IdentityServer.Extensions;
using Microsoft.AspNetCore.Authorization;
using static CRM.Application.Constants;

namespace CRM.Infrastructure.Authorization.Handlers.Commands;

public class CreateCompanyRequirement : IAuthorizationRequirement { }

public class CreateCompanyAuthorizationHandler : BaseAuthorizationHandler<CreateCompanyRequirement>
{
    public CreateCompanyAuthorizationHandler(IAccessService accessService) : base(accessService)
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
                    if (!accessRights.ContainsAny(
                        Access.Company.Any.Manager.SetFromAnyToAny,
                        Access.Company.Any.Manager.SetFromAnyToSelf,
                        Access.Company.Any.Manager.SetFromNoneToSelf,
                        Access.Company.Any.Manager.SetFromNoneToAny))
                    {
                        return Fail(context, "Set manager to self");
                    }
                }
                else
                {
                    if (!accessRights.ContainsAny(
                        Access.Company.Any.Manager.SetFromNoneToAny,
                        Access.Company.Any.Manager.SetFromSelfToAny,
                        Access.Company.Any.Manager.SetFromAnyToAny))
                    {
                        return Fail(context, "Set manager to any");
                    }
                }
            }
        }

        return Ok(context, requirement);
    }
}

