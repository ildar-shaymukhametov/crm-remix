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
        if (context.Resource is not CreateCompanyResource resource)
        {
            throw new InvalidOperationException($"Resource has invalid type. Required type: {typeof(CreateCompanyResource)}");
        }

        var accessRights = _accessService.CheckAccess(context.User, new[]
        {
            Access.CreateCompany,
            Access.Company.Any.Manager.None.Set.Self
        });

        if (!accessRights.Contains(Access.CreateCompany))
        {
            return Fail(context, "Create company");
        }

        if (resource.Request.ManagerId == context.User.GetSubjectId())
        {
            if (!accessRights.Contains(Access.Company.Any.Manager.None.Set.Self))
            {
                return Fail(context, "Set self as manager");
            }
        }

        return Ok(context, requirement);
    }
}

