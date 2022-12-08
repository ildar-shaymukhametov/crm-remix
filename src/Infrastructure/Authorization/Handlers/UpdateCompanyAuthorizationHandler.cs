using CRM.Application.Common.Behaviours.Authorization.Resources;
using CRM.Application.Common.Interfaces;
using Duende.IdentityServer.Extensions;
using Microsoft.AspNetCore.Authorization;
using static CRM.Application.Constants;

namespace CRM.Infrastructure.Authorization.Handlers;

public class UpdateCompanyRequirement : IAuthorizationRequirement { }

public class UpdateCompanyAuthorizationHandler : BaseAuthorizationHandler<UpdateCompanyRequirement>
{
    public UpdateCompanyAuthorizationHandler(IPermissionsService permissionsService) : base(permissionsService)
    {
    }

    protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, UpdateCompanyRequirement requirement)
    {
        var accessRights = _permissionsService.CheckAccess(context.User, new[]
        {
            Access.UpdateAnyCompany,
            Access.UpdateOwnCompany
        });

        if (accessRights.Contains(Access.UpdateAnyCompany)
            || accessRights.Contains(Access.UpdateOwnCompany)
                && context.Resource is CompanyDto company && company.ManagerId == context.User.GetSubjectId())
        {
            context.Succeed(requirement);
        }

        return Task.CompletedTask;
    }
}

