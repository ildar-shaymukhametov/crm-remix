using CRM.Application.Common.Extensions;
using CRM.Application.Common.Interfaces;
using CRM.Domain.Entities;
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
        var accessRights = _accessService.CheckAccess(context.User);
        var canDelete = accessRights.ContainsAny(Access.Company.Any.Delete, Access.Company.WhereUserIsManager.Delete);
        if (!canDelete)
        {
            return Fail(context, "Delete company");
        }

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

    private static Company GetResources(AuthorizationHandlerContext context)
    {
        if (context.Resource == null)
        {
            throw new InvalidOperationException("Resource is missing");
        }

        return (Company)context.Resource;
    }
}

