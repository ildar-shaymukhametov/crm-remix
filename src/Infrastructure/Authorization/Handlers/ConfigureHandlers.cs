using System.Reflection;
using Microsoft.AspNetCore.Authorization;

namespace Microsoft.Extensions.DependencyInjection;

public static class ConfigureHandlers
{
    public static IServiceCollection AddAuthorizationHandlers(this IServiceCollection services)
    {
        var handlers = Assembly
            .GetExecutingAssembly()
            .GetTypes()
            .Where(x => x.GetInterfaces().Contains(typeof(IAuthorizationHandler)));

        foreach (var handler in handlers)
        {
            services.AddScoped(typeof(IAuthorizationHandler), handler);
        }

        return services;
    }
}
