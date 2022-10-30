using CRM.Infrastructure.Authorization.Handlers;
using Microsoft.AspNetCore.Authorization;
using static CRM.Application.Constants;

namespace CRM.Infrastructure.Authorization;

public static class ConfigurePolicies
{
    public static void AddPolicies(this AuthorizationOptions options)
    {
        options.AddPolicy(Policies.CreateCompany, policy =>
                policy.AddRequirements(new CreateCompanyRequirement()));

        options.AddPolicy(Policies.GetCompany, policy =>
            policy.AddRequirements(new GetCompanyRequirement()));

        options.AddPolicy(Policies.GetCompanies, policy =>
            policy.AddRequirements(new GetCompaniesRequirement()));

        options.AddPolicy(Policies.UpdateCompany, policy =>
            policy.AddRequirements(new UpdateCompanyRequirement()));

        options.AddPolicy(Policies.DeleteCompany, policy =>
            policy.AddRequirements(new DeleteCompanyRequirement()));
    }
}
