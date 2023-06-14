using CRM.Infrastructure.Authorization.Handlers;
using Microsoft.AspNetCore.Authorization;
using static CRM.Application.Constants;

namespace CRM.Infrastructure.Authorization;

public static class ConfigurePolicies
{
    public static void AddPolicies(this AuthorizationOptions options)
    {
        options.AddPolicy(Policies.Company.Commands.Create, policy =>
            policy.AddRequirements(new Handlers.Commands.CreateCompanyRequirement()));

        options.AddPolicy(Policies.Company.Queries.Create, policy =>
            policy.AddRequirements(new Handlers.Queries.CreateCompanyRequirement()));

        options.AddPolicy(Policies.Company.Queries.View, policy =>
            policy.AddRequirements(new GetCompanyRequirement()));

        options.AddPolicy(Policies.Company.Commands.Update, policy =>
            policy.AddRequirements(new UpdateCompanyCommandRequirement()));

        options.AddPolicy(Policies.Company.Commands.Delete, policy =>
            policy.AddRequirements(new DeleteCompanyRequirement()));

        options.AddPolicy(Policies.Company.Queries.Delete, policy =>
            policy.AddRequirements(new DeleteCompanyRequirement()));

        options.AddPolicy(Policies.Company.Queries.Update, policy =>
            policy.AddRequirements(new UpdateCompanyQueryRequirement()));
    }
}
