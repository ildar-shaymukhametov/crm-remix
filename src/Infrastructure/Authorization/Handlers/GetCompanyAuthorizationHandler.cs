using CRM.Application.Common.Behaviours.Authorization.Resources;
using CRM.Application.Common.Interfaces;
using Duende.IdentityServer.Extensions;
using Microsoft.AspNetCore.Authorization;
using static CRM.Application.Constants;

namespace CRM.Infrastructure.Authorization.Handlers;

public class GetCompanyRequirement : IAuthorizationRequirement { }

public class GetCompanyAuthorizationHandler : BaseAuthorizationHandler<GetCompanyRequirement>
{
    public GetCompanyAuthorizationHandler(IPermissionsService permissionsService) : base(permissionsService)
    {
    }

    protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, GetCompanyRequirement requirement)
    {
        if (IsAdmin(context)
            || HasAnyClaim(context, Claims.ViewCompany, Claims.DeleteCompany, Claims.UpdateCompany)
                && context.Resource is CompanyDto company && company.ManagerId == context.User.GetSubjectId())
        {
            context.Succeed(requirement);
        }

        return Task.CompletedTask;
    }
}

