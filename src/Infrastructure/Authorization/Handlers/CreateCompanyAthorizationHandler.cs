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
        var accessRights = _accessService.CheckAccess(context.User, new[]
        {
            Access.CreateCompany,
            Access.Company.Any.Manager.None.Set.Self,
            Access.Company.Any.Manager.Any.Set.Any
        });

        if (!accessRights.Contains(Access.CreateCompany))
        {
            return Fail(context, "Create company");
        }

        if (context.Resource is CreateCompanyResource resource)
        {
            if (resource.Request.ManagerId != null)
            {
                var canSetSelfAsManager = resource.Request.ManagerId == context.User.GetSubjectId() && accessRights.Contains(Access.Company.Any.Manager.None.Set.Self);
                if (!canSetSelfAsManager && !accessRights.Contains(Access.Company.Any.Manager.Any.Set.Any))
                {
                    return Fail(context, "Set self as manager");
                }
            }
        }

        return Ok(context, requirement);
    }
}

