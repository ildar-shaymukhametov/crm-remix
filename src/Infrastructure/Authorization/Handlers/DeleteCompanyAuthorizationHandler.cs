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
        var accessRights = _accessService.CheckAccess(context.User, new[]
        {
            Access.DeleteAnyCompany,
            Access.DeleteOwnCompany
        });

        if (accessRights.Contains(Access.DeleteAnyCompany)
            || accessRights.Contains(Access.DeleteOwnCompany)
                && context.Resource is CompanyDto company && company.ManagerId == context.User.GetSubjectId())
        {
            context.Succeed(requirement);
        }

        return Task.CompletedTask;
    }
}

