using CRM.Application.Common.Extensions;
using CRM.Domain.Entities;
using CRM.Domain.Interfaces;
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
        if (accessRights.ContainsAny(
            Domain.Constants.Access.Company.Any.Other.Get,
            Domain.Constants.Access.Company.Any.Manager.Get,
            Domain.Constants.Access.Company.Any.Name.Get
        ))
        {
            context.Succeed(requirement);
        }
        else if (context.Resource is Company company)
        {
            if (company.ManagerId == context.User.GetSubjectId() && accessRights.ContainsAny(
                Domain.Constants.Access.Company.WhereUserIsManager.Other.Get,
                Domain.Constants.Access.Company.WhereUserIsManager.Manager.Get,
                Domain.Constants.Access.Company.WhereUserIsManager.Name.Get
            ))
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
        if (context.HasSucceeded)
        {
            return;
        }

        using var scope = _serviceProvider.CreateScope();
        var handler = scope.ServiceProvider
            .GetServices<IAuthorizationHandler>()
            .OfType<AuthorizationHandler<DeleteCompanyRequirement>>()
            .Single();
        var newContext = new AuthorizationHandlerContext(new[] { new DeleteCompanyRequirement() }, context.User, context.Resource);
        await handler.HandleAsync(newContext);
        if (newContext.HasSucceeded)
        {
            context.Succeed(requirement);
        }
    }
}

public class GetCompanyByUpdateAuthorizationHandler : AuthorizationHandler<GetCompanyRequirement>
{
    private readonly IServiceProvider _serviceProvider;

    public GetCompanyByUpdateAuthorizationHandler(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, GetCompanyRequirement requirement)
    {
        if (context.HasSucceeded)
        {
            return;
        }

        using var scope = _serviceProvider.CreateScope();
        var handler = scope.ServiceProvider
            .GetServices<IAuthorizationHandler>()
            .OfType<AuthorizationHandler<Queries.UpdateCompanyRequirement>>()
            .Single();
        var newContext = new AuthorizationHandlerContext(new[] { new Queries.UpdateCompanyRequirement() }, context.User, context.Resource);
        await handler.HandleAsync(newContext);
        if (newContext.HasSucceeded)
        {
            context.Succeed(requirement);
        }
    }
}
