using CRM.Application.Common.Extensions;
using CRM.Application.Common.Interfaces;
using CRM.Domain.Entities;
using Duende.IdentityServer.Extensions;
using Microsoft.AspNetCore.Authorization;
using static CRM.Application.Constants;

namespace CRM.Infrastructure.Authorization.Handlers.Queries;

public class UpdateCompanyRequirement : IAuthorizationRequirement { }

public class UpdateCompanyAuthorizationHandler : BaseAuthorizationHandler<UpdateCompanyRequirement>
{
    public UpdateCompanyAuthorizationHandler(IAccessService accessService) : base(accessService)
    {
    }

    protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, UpdateCompanyRequirement requirement)
    {
        var accessRights = _accessService.CheckAccess(context.User);
        if (accessRights.ContainsAny(
            Access.Company.Any.Other.Set,
            Access.Company.Any.Manager.SetFromAnyToAny,
            Access.Company.Any.Manager.SetFromAnyToNone,
            Access.Company.Any.Manager.SetFromAnyToSelf,
            Access.Company.Any.Name.Set
        ))
        {
            return Ok(context, requirement);
        }

        var userId = context.User.GetSubjectId();
        var company = GetResources(context);

        if (company.ManagerId == userId)
        {
            if (!accessRights.ContainsAny(
                Access.Company.Any.Manager.SetFromSelfToNone,
                Access.Company.Any.Manager.SetFromSelfToAny,
                Access.Company.WhereUserIsManager.Other.Set,
                Access.Company.WhereUserIsManager.Manager.SetFromSelfToAny,
                Access.Company.WhereUserIsManager.Manager.SetFromSelfToNone,
                Access.Company.WhereUserIsManager.Name.Set
            ))
            {
                return Fail(context, "Update company");
            }
        }
        else if (company.ManagerId == null)
        {
            if (!accessRights.ContainsAny(
                Access.Company.Any.Manager.SetFromNoneToAny,
                Access.Company.Any.Manager.SetFromNoneToSelf
            ))
            {
                return Fail(context, "Update company");
            }
        }
        else
        {
            return Fail(context, "Update company");
        }

        return Ok(context, requirement);
    }

    private static Company GetResources(AuthorizationHandlerContext context)
    {
        if (context.Resource == null)
        {
            throw new InvalidOperationException("Resource is missing");
        }

        return (Company)context.Resource;
    }
}
