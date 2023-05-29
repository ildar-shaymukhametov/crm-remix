using CRM.Application.Common.Behaviours.Authorization.Resources;
using CRM.Application.Common.Interfaces;
using Duende.IdentityServer.Extensions;
using Microsoft.AspNetCore.Authorization;
using static CRM.Application.Constants;

namespace CRM.Infrastructure.Authorization.Handlers;

public class DeleteCompanyRequirement : IAuthorizationRequirement { }

public class DeleteCompanyAuthorizationHandler : BaseAuthorizationHandler<DeleteCompanyRequirement>
{
    public DeleteCompanyAuthorizationHandler(IAccessService accessService) : base(accessService)
    {
    }

    protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, DeleteCompanyRequirement requirement)
    {
        var accessRights = _accessService.CheckAccess(context.User); // todo: if no rights, return early
        if (accessRights.Contains(Access.Company.Any.Delete))
        {
            return Ok(context, requirement);
        }

        var company = GetResources(context);
        var userId = context.User.GetSubjectId();

        if (company.ManagerId == userId && !accessRights.Contains(Access.Company.WhereUserIsManager.Delete))
        {
            return Fail(context, "Delete own company");
        }
        else if (company.ManagerId != userId && !accessRights.Contains(Access.Company.Any.Delete))
        {
            return Fail(context, "Delete any company");
        }

        return Ok(context, requirement);
    }

    private static CompanyDto GetResources(AuthorizationHandlerContext context)
    {
        if (context.Resource == null)
        {
            throw new InvalidOperationException("Resource is missing");
        }

        return (CompanyDto)context.Resource;
    }
}

