using CRM.Application.Common.Behaviours.Authorization.Resources;
using CRM.Application.Common.Interfaces;
using Duende.IdentityServer.Extensions;
using Microsoft.AspNetCore.Authorization;
using static CRM.Application.Constants;

namespace CRM.Infrastructure.Authorization.Handlers;

public class GetCompanyRequirement : IAuthorizationRequirement { }

public class GetCompanyAuthorizationHandler : BaseAuthorizationHandler<GetCompanyRequirement>
{
    public GetCompanyAuthorizationHandler(IAccessService accessService) : base(accessService)
    {
    }

    protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, GetCompanyRequirement requirement)
    {
        var accessRights = _accessService.CheckAccess(context.User);
        if (accessRights.Contains(Access.Company.Any.View))
        {
            context.Succeed(requirement);
        }

        if (accessRights.Contains(Access.Company.WhereUserIsManager.View) && context.Resource is CompanyDto company && company.ManagerId == context.User.GetSubjectId())
        {
            context.Succeed(requirement);
        }

        return Task.CompletedTask;
    }
}

