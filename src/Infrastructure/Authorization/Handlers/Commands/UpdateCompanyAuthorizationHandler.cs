using CRM.Application.Common.Behaviours.Authorization.Resources;
using CRM.Application.Common.Extensions;
using CRM.Application.Companies.Commands.UpdateCompany;
using CRM.Domain.Common;
using CRM.Domain.Entities;
using CRM.Domain.Services;
using Duende.IdentityServer.Extensions;
using Microsoft.AspNetCore.Authorization;

namespace CRM.Infrastructure.Authorization.Handlers.Commands;

public class UpdateCompanyRequirement : IAuthorizationRequirement { }

public class UpdateCompanyAuthorizationHandler : BaseAuthorizationHandler<UpdateCompanyRequirement>
{
    public UpdateCompanyAuthorizationHandler(IAccessService accessService) : base(accessService)
    {
    }

    protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, UpdateCompanyRequirement requirement)
    {
        var accessRights = _accessService.CheckAccess(context.User);
        if (!accessRights.ContainsAny(
            Domain.Constants.Access.Company.Any.Other.Set,
            Domain.Constants.Access.Company.WhereUserIsManager.Other.Set,
            Domain.Constants.Access.Company.Any.Name.Set,
            Domain.Constants.Access.Company.WhereUserIsManager.Name.Set,
            Domain.Constants.Access.Company.Any.Manager.SetFromAnyToAny,
            Domain.Constants.Access.Company.Any.Manager.SetFromAnyToNone,
            Domain.Constants.Access.Company.Any.Manager.SetFromAnyToSelf,
            Domain.Constants.Access.Company.Any.Manager.SetFromNoneToAny,
            Domain.Constants.Access.Company.Any.Manager.SetFromNoneToSelf,
            Domain.Constants.Access.Company.Any.Manager.SetFromSelfToAny,
            Domain.Constants.Access.Company.Any.Manager.SetFromSelfToNone,
            Domain.Constants.Access.Company.WhereUserIsManager.Manager.SetFromSelfToAny,
            Domain.Constants.Access.Company.WhereUserIsManager.Manager.SetFromSelfToNone
        ))
        {
            return Fail(context, "Update company");
        }

        var (company, request) = GetResources(context);
        var userId = context.User.GetSubjectId();

        if (accessRights.ContainsAny(
            Domain.Constants.Access.Company.WhereUserIsManager.Other.Set,
            Domain.Constants.Access.Company.Any.Other.Set
        ))
        {
            if (company.ManagerId == userId && !accessRights.ContainsAny(
                Domain.Constants.Access.Company.WhereUserIsManager.Other.Set,
                Domain.Constants.Access.Company.Any.Other.Set
            ))
            {
                return Fail(context, "Update other fields");
            }
            else if (company.ManagerId != userId && !accessRights.Contains(Domain.Constants.Access.Company.Any.Other.Set))
            {
                return Fail(context, "Update other fields");
            }
        }

        if (accessRights.ContainsAny(
            Domain.Constants.Access.Company.WhereUserIsManager.Manager.SetFromSelfToAny,
            Domain.Constants.Access.Company.WhereUserIsManager.Manager.SetFromSelfToNone,
            Domain.Constants.Access.Company.Any.Manager.SetFromAnyToAny,
            Domain.Constants.Access.Company.Any.Manager.SetFromAnyToNone,
            Domain.Constants.Access.Company.Any.Manager.SetFromAnyToSelf,
            Domain.Constants.Access.Company.Any.Manager.SetFromNoneToAny,
            Domain.Constants.Access.Company.Any.Manager.SetFromNoneToSelf,
            Domain.Constants.Access.Company.Any.Manager.SetFromSelfToAny,
            Domain.Constants.Access.Company.Any.Manager.SetFromSelfToNone
        ))
        {
            if (company.ManagerId == userId && !accessRights.ContainsAny(
                Domain.Constants.Access.Company.WhereUserIsManager.Manager.SetFromSelfToAny,
                Domain.Constants.Access.Company.WhereUserIsManager.Manager.SetFromSelfToNone,
                Domain.Constants.Access.Company.Any.Manager.SetFromSelfToAny,
                Domain.Constants.Access.Company.Any.Manager.SetFromSelfToNone,
                Domain.Constants.Access.Company.Any.Manager.SetFromAnyToAny,
                Domain.Constants.Access.Company.Any.Manager.SetFromAnyToNone
            ))
            {
                return Fail(context, "Update manager");
            }
            else if (company.ManagerId != userId && !accessRights.ContainsAny(
                Domain.Constants.Access.Company.Any.Manager.SetFromAnyToAny,
                Domain.Constants.Access.Company.Any.Manager.SetFromAnyToNone,
                Domain.Constants.Access.Company.Any.Manager.SetFromAnyToSelf,
                Domain.Constants.Access.Company.Any.Manager.SetFromNoneToAny,
                Domain.Constants.Access.Company.Any.Manager.SetFromNoneToSelf,
                Domain.Constants.Access.Company.Any.Manager.SetFromSelfToAny,
                Domain.Constants.Access.Company.Any.Manager.SetFromSelfToNone
            ))
            {
                return Fail(context, "Update manager");
            }

            var managerResult = CheckManager(company, request.ManagerId, userId, accessRights);
            if (!managerResult.Succeeded)
            {
                return Fail(context, managerResult.Errors.First());
            }
        }

        if (accessRights.ContainsAny(
            Domain.Constants.Access.Company.Any.Name.Set,
            Domain.Constants.Access.Company.WhereUserIsManager.Name.Set
        ))
        {
            if (company.ManagerId == userId && !accessRights.ContainsAny(
                Domain.Constants.Access.Company.Any.Name.Set,
                Domain.Constants.Access.Company.WhereUserIsManager.Name.Set
            ))
            {
                return Fail(context, "Update name");
            }
            else if (company.ManagerId != userId && !accessRights.Contains(Domain.Constants.Access.Company.Any.Name.Set))
            {
                return Fail(context, "Update name");
            }
        }

        return Ok(context, requirement);
    }

    private static (Company, UpdateCompanyCommand) GetResources(AuthorizationHandlerContext context)
    {
        if (context.Resource is not UpdateCompanyResource)
        {
            throw new InvalidOperationException("Resource is missing");
        }

        var resource = (UpdateCompanyResource)context.Resource;
        return (resource.Company, resource.Request);
    }

    private static Result CheckManager(Company company, string? newManagerId, string userId, string[] accessRights)
    {
        if (company.ManagerId == null)
        {
            if (newManagerId != null) // from none...
            {
                if (Equals(newManagerId, userId)) // ...to self
                {
                    if (!accessRights.ContainsAny(
                        Domain.Constants.Access.Company.Any.Manager.SetFromNoneToSelf,
                        Domain.Constants.Access.Company.Any.Manager.SetFromNoneToAny,
                        Domain.Constants.Access.Company.Any.Manager.SetFromAnyToAny,
                        Domain.Constants.Access.Company.Any.Manager.SetFromAnyToSelf
                    ))
                    {
                        return Result.Failure(new[] { "Set manager from none to self in any company" });
                    }
                }
                else // ...to any
                {
                    if (!accessRights.ContainsAny(
                        Domain.Constants.Access.Company.Any.Manager.SetFromNoneToSelf,
                        Domain.Constants.Access.Company.Any.Manager.SetFromNoneToAny,
                        Domain.Constants.Access.Company.Any.Manager.SetFromAnyToSelf,
                        Domain.Constants.Access.Company.Any.Manager.SetFromAnyToAny
                    ))
                    {
                        return Result.Failure(new[] { "Set manager from none to any in any company" });
                    }
                }
            }
        }
        else if (company.ManagerId == userId) // from self...
        {
            if (newManagerId == null) // ...to none
            {
                if (!accessRights.ContainsAny(
                    Domain.Constants.Access.Company.Any.Manager.SetFromSelfToAny,
                    Domain.Constants.Access.Company.Any.Manager.SetFromSelfToNone,
                    Domain.Constants.Access.Company.Any.Manager.SetFromAnyToAny,
                    Domain.Constants.Access.Company.Any.Manager.SetFromAnyToNone
                ) && !accessRights.ContainsAny(
                    Domain.Constants.Access.Company.WhereUserIsManager.Manager.SetFromSelfToNone,
                    Domain.Constants.Access.Company.WhereUserIsManager.Manager.SetFromSelfToAny
                ))
                {
                    return Result.Failure(new[] { "Set manager from self to none" });
                }
            }
            else // ...to any
            {
                if (!accessRights.ContainsAny(
                    Domain.Constants.Access.Company.Any.Manager.SetFromSelfToAny,
                    Domain.Constants.Access.Company.Any.Manager.SetFromAnyToAny,
                    Domain.Constants.Access.Company.WhereUserIsManager.Manager.SetFromSelfToAny
                ))
                {
                    return Result.Failure(new[] { "Set manager from self to any" });
                }
            }
        }
        else // from any...
        {
            if (Equals(newManagerId, userId)) // ...to self
            {
                if (!accessRights.ContainsAny(Domain.Constants.Access.Company.Any.Manager.SetFromAnyToSelf, Domain.Constants.Access.Company.Any.Manager.SetFromAnyToAny))
                {
                    return Result.Failure(new[] { "Set manager from any to self in any company" });
                }
            }
            else if (newManagerId == null) // ...to none
            {
                if (!accessRights.ContainsAny(Domain.Constants.Access.Company.Any.Manager.SetFromAnyToNone, Domain.Constants.Access.Company.Any.Manager.SetFromAnyToAny))
                {
                    return Result.Failure(new[] { "Set manager from any to none in any company" });
                }
            }
            else // ...to any
            {
                if (!accessRights.Contains(Domain.Constants.Access.Company.Any.Manager.SetFromAnyToAny))
                {
                    return Result.Failure(new[] { "Set manager from any to any in any company" });
                }
            }
        }

        return Result.Success();
    }
}

