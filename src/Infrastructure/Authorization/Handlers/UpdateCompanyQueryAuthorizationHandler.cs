using CRM.Application.Common.Extensions;
using CRM.Application.Common.Interfaces;
using CRM.Domain.Entities;
using Duende.IdentityServer.Extensions;
using Microsoft.AspNetCore.Authorization;
using static CRM.Application.Constants;

namespace CRM.Infrastructure.Authorization.Handlers;

public class UpdateCompanyQueryRequirement : IAuthorizationRequirement { }

public class UpdateCompanyQueryAuthorizationHandler : BaseAuthorizationHandler<UpdateCompanyQueryRequirement>
{
    public UpdateCompanyQueryAuthorizationHandler(IAccessService accessService) : base(accessService)
    {
    }

    protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, UpdateCompanyQueryRequirement requirement)
    {
        var accessRights = _accessService.CheckAccess(context.User);
        if (accessRights.Contains(Access.Company.Any.Other.Update))
        {
            return Ok(context, requirement);
        }

        var userId = context.User.GetSubjectId();
        var company = GetResources(context);

        var canUpdate = new[] {
            company.ManagerId != userId && accessRights.ContainsAny(
                Access.Company.Any.Manager.SetFromAnyToAny,
                Access.Company.Any.Manager.SetFromAnyToNone,
                Access.Company.Any.Manager.SetFromAnyToSelf
            ),
            company.ManagerId == userId && accessRights.ContainsAny(
                Access.Company.Any.Manager.SetFromSelfToNone,
                Access.Company.Any.Manager.SetFromSelfToAny,
                Access.Company.Any.Manager.SetFromAnyToAny,
                Access.Company.Any.Manager.SetFromAnyToNone,
                Access.Company.Any.Manager.SetFromAnyToSelf,
                Access.Company.WhereUserIsManager.Other.Update,
                Access.Company.WhereUserIsManager.Manager.SetFromSelfToAny,
                Access.Company.WhereUserIsManager.Manager.SetFromSelfToNone
            ),
            company.ManagerId == null && accessRights.ContainsAny(
                Access.Company.Any.Manager.SetFromNoneToAny,
                Access.Company.Any.Manager.SetFromNoneToSelf
            )
        }.Any(x => x);

        if (!canUpdate)
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
