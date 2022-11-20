using System.Reflection;
using CRM.Infrastructure.Authorization.Handlers;
using Microsoft.AspNetCore.Authorization;

namespace Microsoft.Extensions.DependencyInjection;

public static class ConfigureHandlers
{
    public static IServiceCollection AddAuthorizationHandlers(this IServiceCollection services)
    {
        var handlers = Assembly
            .GetExecutingAssembly()
            .GetTypes()
            .Where(x => x.GetInterfaces().Contains(typeof(IAuthorizationHandler)))
            .Except(new [] { typeof(BaseAuthorizationHandler<>) });

        foreach (var handler in handlers)
        {
            services.AddScoped(typeof(IAuthorizationHandler), handler);
        }

        return services;
    }
}
