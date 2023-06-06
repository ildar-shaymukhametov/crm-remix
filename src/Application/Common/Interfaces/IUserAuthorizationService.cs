using CRM.Application.Common.Behaviours.Authorization.Resources;
using CRM.Application.Common.Models;

namespace CRM.Application.Common.Interfaces;

public interface IUserAuthorizationService
{
    Task<Result> AuthorizeDeleteCompanyAsync(string userId, CompanyDto company);
}
