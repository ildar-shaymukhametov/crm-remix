using CRM.Application.Common.Extensions;
using CRM.Application.Common.Interfaces;
using CRM.Application.Companies.Commands.CreateCompany;
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
        if (!accessRights.ContainsAny(Access.Company.Create, Access.Company.New.SetOther))
        {
            return Fail(context, "Create company");
        }

        if (context.Resource is CreateCompanyCommand resource)
        {
            if (resource.ManagerId != null)
            {
                if (resource.ManagerId == context.User.GetSubjectId())
                {
                    if (!accessRights.ContainsAny(
                        Access.Company.Any.Manager.SetFromAnyToAny,
                        Access.Company.Any.Manager.SetFromAnyToSelf,
                        Access.Company.Any.Manager.SetFromNoneToSelf,
                        Access.Company.Any.Manager.SetFromNoneToAny
                    ))
                    {
                        return Fail(context, "Set manager to self");
                    }
                }
                else
                {
                    if (!accessRights.ContainsAny(
                        Access.Company.Any.Manager.SetFromNoneToAny,
                        Access.Company.Any.Manager.SetFromSelfToAny,
                        Access.Company.Any.Manager.SetFromAnyToAny
                    ))
                    {
                        return Fail(context, "Set manager to any");
                    }
                }
            }
        }

        return Ok(context, requirement);
    }
}

