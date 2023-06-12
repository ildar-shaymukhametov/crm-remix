using CRM.Infrastructure.Authorization.Handlers;
using Microsoft.AspNetCore.Authorization;
using static CRM.Application.Constants;

namespace CRM.Infrastructure.Authorization;

public static class ConfigurePolicies
{
    public static void AddPolicies(this AuthorizationOptions options)
    {
        options.AddPolicy(Policies.Company.Create, policy =>
                policy.AddRequirements(new CreateCompanyRequirement()));

        options.AddPolicy(Policies.Company.View, policy =>
            policy.AddRequirements(new GetCompanyRequirement()));

        options.AddPolicy(Policies.Company.Update, policy =>
            policy.AddRequirements(new UpdateCompanyRequirement()));

        options.AddPolicy(Policies.Company.Delete, policy =>
            policy.AddRequirements(new DeleteCompanyRequirement()));

        options.AddPolicy(Policies.Company.QueryUpdate, policy =>
            policy.AddRequirements(new QueryUpdateCompanyRequirement()));
    }
}
