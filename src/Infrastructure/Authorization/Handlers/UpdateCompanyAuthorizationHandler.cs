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
        if (context.Resource is not CompanyDto company)
        {
            throw new InvalidOperationException($"Resource has invalid type. Required type: {typeof(CompanyDto)}");
        }

        var accessRights = _accessService.CheckAccess(context.User, new[]
        {
            Access.UpdateAnyCompany,
            Access.UpdateOwnCompany
        });

        if (!accessRights.Any())
        {
            return Fail(context, "Update company");
        }

        var canUpdateOwnCompany = company.ManagerId == context.User.GetSubjectId() && accessRights.Contains(Access.UpdateOwnCompany);
        if (!canUpdateOwnCompany && !accessRights.Contains(Access.UpdateAnyCompany))
        {
            return Fail(context, "Update company");
        }

        return Ok(context, requirement);
    }
}

