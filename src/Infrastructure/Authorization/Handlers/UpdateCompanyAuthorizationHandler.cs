using CRM.Application.Common.Behaviours.Authorization.Resources;
using CRM.Application.Common.Interfaces;
using Duende.IdentityServer.Extensions;
using Microsoft.AspNetCore.Authorization;
using static CRM.Application.Constants;

namespace CRM.Infrastructure.Authorization.Handlers;

public class UpdateCompanyRequirement : IAuthorizationRequirement { }

public class UpdateCompanyAuthorizationHandler : BaseAuthorizationHandler<UpdateCompanyRequirement>
{
    public UpdateCompanyAuthorizationHandler(IAccessService accessService) : base(accessService)
    {
    }

    protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, UpdateCompanyRequirement requirement)
    {
        if (context.Resource is not UpdateCompanyResource resource)
        {
            throw new InvalidOperationException($"Resource has invalid type. Required type: {typeof(UpdateCompanyResource)}");
        }

        var accessRights = _accessService.CheckAccess(context.User, new[]
        {
            Access.UpdateAnyCompany,
            Access.UpdateOwnCompany,
            Access.Company.Any.Manager.None.Set.Self
        });

        if (!accessRights.Any())
        {
            return Fail(context, "Update company");
        }

        if (resource.Command != null)
        {
            var userId = context.User.GetSubjectId();
            var canUpdateOwnCompany = resource.Request.ManagerId == userId && accessRights.Contains(Access.UpdateOwnCompany);
            if (!canUpdateOwnCompany && !accessRights.Contains(Access.UpdateAnyCompany))
            {
                return Fail(context, "Update company");
            }

            if (resource.Command.ManagerId == null && resource.Request.ManagerId == userId && !accessRights.Contains(Access.Company.Any.Manager.None.Set.Self))
            {
                return Fail(context, "Set self as manager");
            }
        }

        return Ok(context, requirement);
    }
}

