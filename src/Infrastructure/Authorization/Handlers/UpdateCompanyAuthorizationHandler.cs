using CRM.Application.Common.Behaviours.Authorization.Resources;
using CRM.Application.Common.Interfaces;
using CRM.Application.Common.Models;
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

        if (company.ManagerId == userId && !accessRights.Contains(Access.Company.WhereUserIsManager.Update))
        {
            return Fail(context, "Update own company");
        }
        else if (company.ManagerId != userId && !accessRights.Contains(Access.Company.Any.Update))
        {
            return Fail(context, "Update any company");
        }

        if (request != null)
        {
            var managerResult = CheckManager(company, request, userId, accessRights);
            if (!managerResult.Succeeded)
            {
                return Fail(context, managerResult.Errors.First());
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

    private static Result CheckManager(CompanyDto company, UpdateCompanyCommand request, string userId, string[] accessRights)
    {
        if (company.ManagerId == request.ManagerId)
        {
            return Result.Success();
        }

        if (company.ManagerId == null)
        {
            if (request.ManagerId != null) // from none...
            {
                if (request.ManagerId == userId) // ...to self
                {
                    if (!accessRights.Contains(Access.Company.Any.SetManagerFromNoneToSelf))
                    {
                        return Result.Failure(new[] { "Set manager from none to self in any company" });
                    }
                }
                else // ...to any
                {
                    if (!accessRights.Contains(Access.Company.Any.SetManagerFromNoneToAny))
                    {
                        return Result.Failure(new[] { "Set manager from none to any in any company" });
                    }
                }
            }
        }
        else if (company.ManagerId == userId) // from self...
        {
            if (request.ManagerId == null) // ..to none
            {
                if (!accessRights.Contains(Access.Company.Any.SetManagerFromSelfToNone))
                {
                    return Result.Failure(new[] { "Set manager from self to none in any company" });
                }
            }
            else // ...to any
            {
                if (!accessRights.Contains(Access.Company.Any.SetManagerFromSelfToAny))
                {
                    return Result.Failure(new[] { "Set manager from self to any in any company" });
                }
            }
        }
        else // from any...
        {
            if (request.ManagerId == userId) // ...to self
            {
                if (!accessRights.Contains(Access.Company.Any.SetManagerFromAnyToSelf))
                {
                    return Result.Failure(new[] { "Set manager from any to self in any company" });
                }
            }
            else if (request.ManagerId == null) // ...to none
            {
                if (!accessRights.Contains(Access.Company.Any.SetManagerFromAnyToNone))
                {
                    return Result.Failure(new[] { "Set manager from any to none in any company" });
                }
            }
            else // ...to any
            {
                if (!accessRights.Contains(Access.Company.Any.SetManagerFromAnyToAny))
                {
                    return Result.Failure(new[] { "Set manager from any to any in any company" });
                }
            }
        }

        return Result.Success();
    }
}

