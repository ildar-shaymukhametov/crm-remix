using CRM.Application.Common.Interfaces;
using CRM.Application.Common.Models;
using CRM.Domain.Entities;
using static CRM.Application.Constants;

namespace CRM.Infrastructure.Authorization;

public class UserAuthorizationService : IUserAuthorizationService
{
    private readonly IAccessService _accessService;

    public UserAuthorizationService(IAccessService accessService)
    {
        _accessService = accessService;
    }

    public async Task<Result> AuthorizeDeleteCompanyAsync(string userId, Company company)
    {
        var accessRights = await _accessService.CheckAccessAsync(userId);
        if (!accessRights.Any())
        {
            return Result.Failure(new[] { "Delete company" });
        }

        if (accessRights.Contains(Access.Company.Any.Delete)) // todo: check if needed
        {
            return Result.Success();
        }

        if (company.ManagerId == userId && !accessRights.Contains(Access.Company.WhereUserIsManager.Delete))
        {
            return Result.Failure(new[] { "Delete own company" });
        }
        else if (company.ManagerId != userId && !accessRights.Contains(Access.Company.Any.Delete))
        {
            return Result.Failure(new[] { "Delete any company" });
        }

        return Result.Success();
    }
}
