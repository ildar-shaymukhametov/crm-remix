using CRM.Application.Common.Behaviours.Authorization.Resources;
using CRM.Application.Common.Exceptions;
using CRM.Application.Common.Extensions;
using CRM.Application.Common.Interfaces;
using CRM.Application.Common.Models;
using CRM.Application.Companies.Commands.UpdateCompany;
using CRM.Domain.Entities;
using Duende.IdentityServer.Extensions;
using FluentValidation.Results;
using Microsoft.AspNetCore.Authorization;
using static CRM.Application.Constants;

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
            Access.Company.Any.Other.Set,
            Access.Company.WhereUserIsManager.Other.Set,
            Access.Company.Any.Name.Set,
            Access.Company.WhereUserIsManager.Name.Set,
            Access.Company.Any.Manager.SetFromAnyToAny,
            Access.Company.Any.Manager.SetFromAnyToNone,
            Access.Company.Any.Manager.SetFromAnyToSelf,
            Access.Company.Any.Manager.SetFromNoneToAny,
            Access.Company.Any.Manager.SetFromNoneToSelf,
            Access.Company.Any.Manager.SetFromSelfToAny,
            Access.Company.Any.Manager.SetFromSelfToNone,
            Access.Company.WhereUserIsManager.Manager.SetFromSelfToAny,
            Access.Company.WhereUserIsManager.Manager.SetFromSelfToNone
        ))
        {
            return Fail(context, "Update company");
        }

        var (company, request) = GetResources(context);
        if (!request.Fields.Any())
        {
            throw new ValidationException(new[] { new ValidationFailure(nameof(UpdateCompanyCommand.Fields), "Must provide at least one value") });
        }

        var userId = context.User.GetSubjectId();

        if (new[] {
            nameof(Company.Address),
            nameof(Company.Ceo),
            nameof(Company.Contacts),
            nameof(Company.Email),
            nameof(Company.Inn),
            nameof(Company.Phone),
            nameof(Company.TypeId)
        }.Any(request.Fields.ContainsKey))
        {
            var changes = new List<bool>();
            if (request.Fields.TryGetValue(nameof(Company.Address), out object? address))
            {
                changes.Add(!Equals(company.Address, address));
            }
            if (request.Fields.TryGetValue(nameof(Company.Ceo), out object? ceo))
            {
                changes.Add(!Equals(company.Ceo, ceo));
            }
            if (request.Fields.TryGetValue(nameof(Company.Contacts), out object? contacts))
            {
                changes.Add(!Equals(company.Contacts, contacts));
            }
            if (request.Fields.TryGetValue(nameof(Company.Email), out object? email))
            {
                changes.Add(!Equals(company.Email, email));
            }
            if (request.Fields.TryGetValue(nameof(Company.Inn), out object? inn))
            {
                changes.Add(!Equals(company.Inn, inn));
            }
            if (request.Fields.TryGetValue(nameof(Company.Phone), out object? phone))
            {
                changes.Add(!Equals(company.Phone, phone));
            }
            if (request.Fields.TryGetValue(nameof(Company.TypeId), out object? typeId))
            {
                try
                {
                    changes.Add(!Equals(company.TypeId, Convert.ToInt32(typeId)));
                }
                catch
                {
                    throw new ValidationException(new[] { new ValidationFailure(nameof(Company.TypeId), "Value must be convertible to a number") });
                }
            }

            if (changes.Any(x => true))
            {
                if (company.ManagerId == userId && !accessRights.ContainsAny(
                    Access.Company.WhereUserIsManager.Other.Set,
                    Access.Company.Any.Other.Set
                ))
                {
                    return Fail(context, "Update other fields");
                }
                else if (company.ManagerId != userId && !accessRights.Contains(Access.Company.Any.Other.Set))
                {
                    return Fail(context, "Update other fields");
                }
            }
        }

        if (request.Fields.TryGetValue(nameof(Company.ManagerId), out object? managerId) && !Equals(company.ManagerId, managerId))
        {
            if (company.ManagerId == userId && !accessRights.ContainsAny(
                Access.Company.WhereUserIsManager.Manager.SetFromSelfToAny,
                Access.Company.WhereUserIsManager.Manager.SetFromSelfToNone,
                Access.Company.Any.Manager.SetFromSelfToAny,
                Access.Company.Any.Manager.SetFromSelfToNone,
                Access.Company.Any.Manager.SetFromAnyToAny,
                Access.Company.Any.Manager.SetFromAnyToNone
            ))
            {
                return Fail(context, "Update manager");
            }
            else if (company.ManagerId != userId && !accessRights.ContainsAny(
                Access.Company.Any.Manager.SetFromAnyToAny,
                Access.Company.Any.Manager.SetFromAnyToNone,
                Access.Company.Any.Manager.SetFromAnyToSelf,
                Access.Company.Any.Manager.SetFromNoneToAny,
                Access.Company.Any.Manager.SetFromNoneToSelf,
                Access.Company.Any.Manager.SetFromSelfToAny,
                Access.Company.Any.Manager.SetFromSelfToNone
            ))
            {
                return Fail(context, "Update manager");
            }

            var managerResult = CheckManager(company, managerId, userId, accessRights);
            if (!managerResult.Succeeded)
            {
                return Fail(context, managerResult.Errors.First());
            }
        }

        if (request.Fields.TryGetValue(nameof(Company.Name), out object? name) && !Equals(company.Name, name))
        {
            if (company.ManagerId == userId && !accessRights.ContainsAny(
                Access.Company.Any.Name.Set,
                Access.Company.WhereUserIsManager.Name.Set
            ))
            {
                return Fail(context, "Update name");
            }
            else if (company.ManagerId != userId && !accessRights.Contains(Access.Company.Any.Name.Set))
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

    private static Result CheckManager(Company company, object? newManagerId, string userId, string[] accessRights)
    {
        if (company.ManagerId == null)
        {
            if (newManagerId != null) // from none...
            {
                if (Equals(newManagerId, userId)) // ...to self
                {
                    if (!accessRights.ContainsAny(
                        Access.Company.Any.Manager.SetFromNoneToSelf,
                        Access.Company.Any.Manager.SetFromNoneToAny,
                        Access.Company.Any.Manager.SetFromAnyToAny,
                        Access.Company.Any.Manager.SetFromAnyToSelf
                    ))
                    {
                        return Result.Failure(new[] { "Set manager from none to self in any company" });
                    }
                }
                else // ...to any
                {
                    if (!accessRights.ContainsAny(
                        Access.Company.Any.Manager.SetFromNoneToSelf,
                        Access.Company.Any.Manager.SetFromNoneToAny,
                        Access.Company.Any.Manager.SetFromAnyToSelf,
                        Access.Company.Any.Manager.SetFromAnyToAny
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
                    Access.Company.Any.Manager.SetFromSelfToAny,
                    Access.Company.Any.Manager.SetFromSelfToNone,
                    Access.Company.Any.Manager.SetFromAnyToAny,
                    Access.Company.Any.Manager.SetFromAnyToNone
                ) && !accessRights.ContainsAny(
                    Access.Company.WhereUserIsManager.Manager.SetFromSelfToNone,
                    Access.Company.WhereUserIsManager.Manager.SetFromSelfToAny
                ))
                {
                    return Result.Failure(new[] { "Set manager from self to none" });
                }
            }
            else // ...to any
            {
                if (!accessRights.ContainsAny(
                    Access.Company.Any.Manager.SetFromSelfToAny,
                    Access.Company.Any.Manager.SetFromAnyToAny,
                    Access.Company.WhereUserIsManager.Manager.SetFromSelfToAny
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
                if (!accessRights.ContainsAny(Access.Company.Any.Manager.SetFromAnyToSelf, Access.Company.Any.Manager.SetFromAnyToAny))
                {
                    return Result.Failure(new[] { "Set manager from any to self in any company" });
                }
            }
            else if (newManagerId == null) // ...to none
            {
                if (!accessRights.ContainsAny(Access.Company.Any.Manager.SetFromAnyToNone, Access.Company.Any.Manager.SetFromAnyToAny))
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

