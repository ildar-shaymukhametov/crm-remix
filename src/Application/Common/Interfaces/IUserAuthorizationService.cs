using CRM.Application.Common.Models;
using CRM.Domain.Entities;

namespace CRM.Application.Common.Interfaces;

public interface IUserAuthorizationService
{
    Task<Result> AuthorizeDeleteCompanyAsync(string userId, Company company);
}
