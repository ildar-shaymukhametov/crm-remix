using CRM.Application.Common.Extensions;
using CRM.Domain.Entities;
using CRM.Domain.Services;
using Duende.IdentityServer.Extensions;
using Microsoft.AspNetCore.Authorization;

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
            Domain.Constants.Access.Company.Any.Other.Set,
            Domain.Constants.Access.Company.Any.Manager.SetFromAnyToAny,
            Domain.Constants.Access.Company.Any.Manager.SetFromAnyToNone,
            Domain.Constants.Access.Company.Any.Manager.SetFromAnyToSelf,
            Domain.Constants.Access.Company.Any.Name.Set
        ))
        {
            return Ok(context, requirement);
        }

        var userId = context.User.GetSubjectId();
        var company = GetResources(context);

        if (company.ManagerId == userId)
        {
            if (!accessRights.ContainsAny(
                Domain.Constants.Access.Company.Any.Manager.SetFromSelfToNone,
                Domain.Constants.Access.Company.Any.Manager.SetFromSelfToAny,
                Domain.Constants.Access.Company.WhereUserIsManager.Other.Set,
                Domain.Constants.Access.Company.WhereUserIsManager.Manager.SetFromSelfToAny,
                Domain.Constants.Access.Company.WhereUserIsManager.Manager.SetFromSelfToNone,
                Domain.Constants.Access.Company.WhereUserIsManager.Name.Set
            ))
            {
                return Fail(context, "Update company");
            }
        }
        else if (company.ManagerId == null)
        {
            if (!accessRights.ContainsAny(
                Domain.Constants.Access.Company.Any.Manager.SetFromNoneToAny,
                Domain.Constants.Access.Company.Any.Manager.SetFromNoneToSelf
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
