using CRM.Application.Common.Extensions;
using CRM.Application.Companies.Commands.CreateCompany;
using CRM.Domain.Services;
using Duende.IdentityServer.Extensions;
using Microsoft.AspNetCore.Authorization;

namespace CRM.Infrastructure.Authorization.Handlers.Commands;

public class CreateCompanyRequirement : IAuthorizationRequirement { }

public class CreateCompanyAuthorizationHandler : BaseAuthorizationHandler<CreateCompanyRequirement>
{
    public CreateCompanyAuthorizationHandler(IAccessService accessService) : base(accessService)
    {
    }

    protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, CreateCompanyRequirement requirement)
    {
        var accessRights = _accessService.CheckAccess(context.User);
        if (!accessRights.Contains(Domain.Constants.Access.Company.Create))
        {
            return Fail(context, "Create company");
        }

        if (context.Resource is not CreateCompanyCommand company)
        {
            throw new InvalidOperationException("Resouce is missing");
        }

        var otherFieldChanged = company.Address != default || company.Ceo != default || company.Contacts != default || company.Email != default || company.Inn != default || company.Phone != default || company.TypeId != default;
        if (otherFieldChanged && !accessRights.Contains(Domain.Constants.Access.Company.New.Other.Set))
        {
            return Fail(context, "Set other fields");
        }

        if (company.ManagerId == context.User.GetSubjectId())
        {
            if (!accessRights.ContainsAny(
                Domain.Constants.Access.Company.New.Manager.SetToAny,
                Domain.Constants.Access.Company.New.Manager.SetToSelf
            ))
            {
                return Fail(context, "Set manager to self");
            }
        }
        else if (company.ManagerId != null)
        {
            if (!accessRights.Contains(Domain.Constants.Access.Company.New.Manager.SetToAny))
            {
                return Fail(context, "Set manager to any");
            }
        }

        return Ok(context, requirement);
    }
}

