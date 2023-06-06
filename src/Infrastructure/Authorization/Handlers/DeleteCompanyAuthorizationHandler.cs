using CRM.Application.Common.Behaviours.Authorization.Resources;
using CRM.Application.Common.Interfaces;
using Duende.IdentityServer.Extensions;
using Microsoft.AspNetCore.Authorization;

namespace CRM.Infrastructure.Authorization.Handlers;

public class DeleteCompanyRequirement : IAuthorizationRequirement { }

public class DeleteCompanyAuthorizationHandler : BaseAuthorizationHandler<DeleteCompanyRequirement>
{
    private readonly IUserAuthorizationService _userAuthorizationService;

    public DeleteCompanyAuthorizationHandler(IAccessService accessService, IUserAuthorizationService userAuthorizationService) : base(accessService)
    {
        _userAuthorizationService = userAuthorizationService;
    }

    protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, DeleteCompanyRequirement requirement)
    {
        if (context.Resource is not CompanyDto)
        {
            throw new InvalidOperationException("Resource is missing");
        }

        var result = await _userAuthorizationService.AuthorizeDeleteCompanyAsync(context.User.GetSubjectId(), (CompanyDto)context.Resource);
        if (!result.Succeeded)
        {
            _ = Fail(context, "Delete company");
        }

        _ = Ok(context, requirement);
    }
}

