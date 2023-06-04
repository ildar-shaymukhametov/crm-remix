using CRM.Application.Common.Behaviours.Authorization.Resources;
using CRM.Application.Common.Extensions;
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
        if (accessRights.ContainsAny(Access.Company.Any.Other.View, Access.Company.Any.Manager.View))
        {
            context.Succeed(requirement);
        }

        if (context.Resource is CompanyDto company)
        {
            if (company.ManagerId == context.User.GetSubjectId() && accessRights.ContainsAny(Access.Company.WhereUserIsManager.Other.View, Access.Company.WhereUserIsManager.Manager.View))
            {
                context.Succeed(requirement);
            }
        }

        return Task.CompletedTask;
    }
}

