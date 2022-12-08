using CRM.Application.Common.Interfaces;
using Microsoft.AspNetCore.Authorization;
using static CRM.Application.Constants;

namespace CRM.Infrastructure.Authorization.Handlers;

public class CreateCompanyRequirement : IAuthorizationRequirement { }

public class CreateCompanyAthorizationHandler : BaseAuthorizationHandler<CreateCompanyRequirement>
{
    public CreateCompanyAthorizationHandler(IPermissionsService permissionsService) : base(permissionsService)
    {
    }

    protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, CreateCompanyRequirement requirement)
    {
        var accessRights = _permissionsService.CheckAccess(context.User, new[]
        {
            Access.CreateCompany,
        });

        if (accessRights.Contains(Access.CreateCompany))
        {
            context.Succeed(requirement);
        }

        return Task.CompletedTask;
    }
}

