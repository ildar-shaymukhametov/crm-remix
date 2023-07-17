using CRM.Domain.Entities;
using CRM.Domain.Interfaces;
using Duende.IdentityServer.Extensions;
using Microsoft.AspNetCore.Authorization;

namespace CRM.Infrastructure.Authorization.Handlers;

public class DeleteCompanyRequirement : IAuthorizationRequirement { }

public class DeleteCompanyAuthorizationHandler : BaseAuthorizationHandler<DeleteCompanyRequirement>
{
    public DeleteCompanyAuthorizationHandler(IAccessService accessService) : base(accessService)
    {
    }

    protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, DeleteCompanyRequirement requirement)
    {
        var accessRights = _accessService.CheckAccess(context.User);
        if (accessRights.Contains(Domain.Constants.Access.Company.Any.Delete))
        {
            return Ok(context, requirement);
        }

        var company = GetResources(context);
        var userId = context.User.GetSubjectId();

        if (company.ManagerId == userId && !accessRights.Contains(Domain.Constants.Access.Company.WhereUserIsManager.Delete))
        {
            return Fail(context, "Delete own company");
        }
        else if (company.ManagerId != userId && !accessRights.Contains(Domain.Constants.Access.Company.Any.Delete))
        {
            return Fail(context, "Delete any company");
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

