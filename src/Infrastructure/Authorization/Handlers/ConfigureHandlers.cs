using CRM.Infrastructure.Authorization.Handlers;
using Microsoft.AspNetCore.Authorization;

namespace Microsoft.Extensions.DependencyInjection;

public static class ConfigureHandlers
{
    public static IServiceCollection AddAuthorizationHandlers(this IServiceCollection services)
    {
        return services
            .AddScoped<IAuthorizationHandler, UserIsAdminHandler>()
            .AddScoped<IAuthorizationHandler, UserHasCreateCompanyClaimHandler>();
    }
}
