using CRM.Application.Common.Behaviours.Authorization.Resources;
using CRM.Application.Common.Extensions;
using CRM.Application.Common.Interfaces;
using CRM.Application.Common.Models;
using CRM.Application.Companies.Commands.UpdateCompany;
using CRM.Domain.Entities;
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
        var (company, request) = GetResources(context);
        var accessRights = _accessService.CheckAccess(context.User);
        var userId = context.User.GetSubjectId();

        // GET
        var canUpdateOtherFields = accessRights.ContainsAny(
            Access.Company.Any.Other.Update,
            Access.Company.WhereUserIsManager.Other.Update
        );

        var canSetManagerFromAny = company.ManagerId != userId && accessRights.ContainsAny(
            Access.Company.Any.Manager.SetFromAnyToAny,
            Access.Company.Any.Manager.SetFromAnyToNone,
            Access.Company.Any.Manager.SetFromAnyToSelf
        );

        var canSetManagerFromSelf = company.ManagerId == userId && accessRights.ContainsAny(
            Access.Company.Any.Manager.SetFromSelfToNone,
            Access.Company.Any.Manager.SetFromSelfToAny,
            Access.Company.Any.Manager.SetFromAnyToAny,
            Access.Company.Any.Manager.SetFromAnyToNone,
            Access.Company.Any.Manager.SetFromAnyToSelf
        );

        var canUpdate = canUpdateOtherFields || canSetManagerFromAny || canSetManagerFromSelf;
        if (!canUpdate)
        {
            return Fail(context, "Update company");
        }

        if (request == null)
        {
            return Ok(context, requirement);
        }

        // POST
        var otherFieldChanged = company.Address != request.Address || company.Ceo != request.Ceo || company.Contacts != request.Contacts || company.Email != request.Email || company.Inn != request.Inn || company.Name != request.Name || company.Phone != request.Phone || company.TypeId != request.TypeId;
        if (otherFieldChanged)
        {
            if (company.ManagerId == userId && !accessRights.ContainsAny(Access.Company.WhereUserIsManager.Other.Update, Claims.Company.Any.Other.Update))
            {
                return Fail(context, "Update other fields");
            }
            else if (company.ManagerId != userId && !accessRights.Contains(Access.Company.Any.Other.Update))
            {
                return Fail(context, "Update other fields");
            }
        }

        if (company.ManagerId != request.ManagerId)
        {
            if (company.ManagerId == userId && !accessRights.ContainsAny(Access.Company.WhereUserIsManager.Manager.SetFromSelfToAny, Access.Company.WhereUserIsManager.Manager.SetFromSelfToNone))
            {
                return Fail(context, "Update manager");
            }
            else if (company.ManagerId != userId && !accessRights.ContainsAny(Access.Company.Any.Manager.SetFromAnyToAny, Access.Company.Any.Manager.SetFromAnyToNone, Access.Company.Any.Manager.SetFromAnyToSelf, Access.Company.Any.Manager.SetFromNoneToAny, Access.Company.Any.Manager.SetFromNoneToSelf, Access.Company.Any.Manager.SetFromSelfToAny, Access.Company.Any.Manager.SetFromSelfToNone))
            {
                return Fail(context, "Update manager");
            }

            var managerResult = CheckManager(company, request, userId, accessRights);
            if (!managerResult.Succeeded)
            {
                return Fail(context, managerResult.Errors.First());
            }
        }

        return Ok(context, requirement);
    }

    private static (Company, UpdateCompanyCommand?) GetResources(AuthorizationHandlerContext context)
    {
        if (context.Resource == null)
        {
            throw new InvalidOperationException("Resource is missing");
        }

        if (context.Resource is Company company)
        {
            return (company, null);
        }

        var resource = (UpdateCompanyResource)context.Resource;
        return (resource.Company, resource.Request);
    }

    private static Result CheckManager(Company company, UpdateCompanyCommand request, string userId, string[] accessRights)
    {
        if (company.ManagerId == null)
        {
            if (request.ManagerId != null) // from none...
            {
                if (request.ManagerId == userId) // ...to self
                {
                    if (!accessRights.Contains(Access.Company.Any.Manager.SetFromNoneToSelf))
                    {
                        return Result.Failure(new[] { "Set manager from none to self in any company" });
                    }
                }
                else // ...to any
                {
                    if (!accessRights.Contains(Access.Company.Any.Manager.SetFromNoneToAny))
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
                if (!accessRights.Contains(Access.Company.Any.Manager.SetFromSelfToNone) && !accessRights.Contains(Access.Company.WhereUserIsManager.Manager.SetFromSelfToNone))
                {
                    return Result.Failure(new[] { "Set manager from self to none" });
                }
            }
            else // ...to any
            {
                if (!accessRights.Contains(Access.Company.Any.Manager.SetFromSelfToAny) && !accessRights.Contains(Access.Company.WhereUserIsManager.Manager.SetFromSelfToAny))
                {
                    return Result.Failure(new[] { "Set manager from self to any" });
                }
            }
        }
        else // from any...
        {
            if (request.ManagerId == userId) // ...to self
            {
                if (!accessRights.Contains(Access.Company.Any.Manager.SetFromAnyToSelf))
                {
                    return Result.Failure(new[] { "Set manager from any to self in any company" });
                }
            }
            else if (request.ManagerId == null) // ...to none
            {
                if (!accessRights.Contains(Access.Company.Any.Manager.SetFromAnyToNone))
                {
                    return Result.Failure(new[] { "Set manager from any to none in any company" });
                }
            }
            else // ...to any
            {
                if (!accessRights.Contains(Access.Company.Any.Manager.SetFromAnyToAny))
                {
                    return Result.Failure(new[] { "Set manager from any to any in any company" });
                }
            }
        }

        return Result.Success();
    }
}

