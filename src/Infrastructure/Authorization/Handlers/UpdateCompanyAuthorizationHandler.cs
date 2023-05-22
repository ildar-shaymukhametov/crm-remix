using CRM.Application.Common.Behaviours.Authorization.Resources;
using CRM.Application.Common.Interfaces;
using CRM.Application.Companies.Commands.UpdateCompany;
using Duende.IdentityServer.Extensions;
using Microsoft.AspNetCore.Authorization;
using static CRM.Application.Constants;

namespace CRM.Infrastructure.Authorization.Handlers;

public class UpdateCompanyRequirement : IAuthorizationRequirement { }

public class UpdateCompanyAuthorizationHandler : BaseAuthorizationHandler<UpdateCompanyRequirement>
{
    public UpdateCompanyAuthorizationHandler(IAccessService accessService) : base(accessService)
    {
    }

    protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, UpdateCompanyRequirement requirement)
    {
        var accessRights = _accessService.CheckAccess(context.User);
        if (!accessRights.Any())
        {
            return Fail(context, "Update company");
        }

        var (company, request) = GetResources(context);
        var userId = context.User.GetSubjectId();
        var canUpdateOwnCompany = company.ManagerId == userId && accessRights.Contains(Access.Company.WhereUserIsManager.Update);
        if (!canUpdateOwnCompany && !accessRights.Contains(Access.Company.Any.Update))
        {
            return Fail(context, "Update company");
        }

        if (request != null)
        {
            if (company.ManagerId == null)
            {
                if (request.ManagerId != null)
                {
                    if (request.ManagerId == userId)
                    {
                        var canSetSelfAsManager = new[]
                        {
                            Access.Company.Any.SetManagerFromNoneToSelf,
                            Access.Company.Any.SetManagerFromNoneToAny,
                            Access.Company.Any.SetManagerFromAnyToSelf,
                            Access.Company.Any.SetManagerFromAnyToAny
                        }.Any(accessRights.Contains);

                        if (!canSetSelfAsManager)
                        {
                            return Fail(context, "Set manager from none to self in any company");
                        }
                    }
                }
            }
        }

        return Ok(context, requirement);
    }

    private static (CompanyDto, UpdateCompanyCommand?) GetResources(AuthorizationHandlerContext context)
    {
        if (context.Resource == null)
        {
            throw new InvalidOperationException("Resource is missing");
        }

        if (context.Resource is CompanyDto company)
        {
            return (company, null);
        }

        var resource = (UpdateCompanyResource)context.Resource;
        return (resource.Company, resource.Request);
    }
}

