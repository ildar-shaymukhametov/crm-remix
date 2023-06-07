using CRM.Application.Common.Interfaces;
using CRM.Domain.Entities;
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
        if (context.Resource is not Company)
        {
            throw new InvalidOperationException("Resource is missing");
        }

        var result = await _userAuthorizationService.AuthorizeDeleteCompanyAsync(context.User.GetSubjectId(), (Company)context.Resource);
        if (!result.Succeeded)
        {
            _ = Fail(context, "Delete company");
        }

        _ = Ok(context, requirement);
    }
}

