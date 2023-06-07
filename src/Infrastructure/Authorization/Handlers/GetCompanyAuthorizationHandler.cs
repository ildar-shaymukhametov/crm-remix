using CRM.Application.Common.Extensions;
using CRM.Application.Common.Interfaces;
using CRM.Domain.Entities;
using Duende.IdentityServer.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.DependencyInjection;
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
        else if (context.Resource is Company company)
        {
            if (company.ManagerId == context.User.GetSubjectId() && accessRights.ContainsAny(Access.Company.WhereUserIsManager.Other.View, Access.Company.WhereUserIsManager.Manager.View))
            {
                context.Succeed(requirement);
            }
        }
        
        return Task.CompletedTask;
    }
}

public class GetCompanyByDeleteAuthorizationHandler : AuthorizationHandler<GetCompanyRequirement>
{
    private readonly IServiceProvider _serviceProvider;

    public GetCompanyByDeleteAuthorizationHandler(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, GetCompanyRequirement requirement)
    {
        using var scope = _serviceProvider.CreateScope();
        var handler = scope.ServiceProvider.GetServices<IAuthorizationHandler>().Single(x => x.GetType().IsAssignableTo(typeof(AuthorizationHandler<DeleteCompanyRequirement>)));
        var newContext = new AuthorizationHandlerContext(new[] { new DeleteCompanyRequirement() }, context.User, context.Resource);
        await handler.HandleAsync(newContext);
        if (newContext.HasSucceeded)
        {
            context.Succeed(requirement);
        }
    }
}
