using CRM.Application.Common.Interfaces;
using CRM.Infrastructure.Identity;
using CRM.Infrastructure.Persistence;
using CRM.Infrastructure.Persistence.Interceptors;
using CRM.Infrastructure.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace Microsoft.Extensions.DependencyInjection;

public static class ConfigureServices
{
    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddScoped<AuditableEntitySaveChangesInterceptor>();

        services.AddDbContext<ApplicationDbContext>(options =>
            options.UseSqlServer(configuration.GetConnectionString("DefaultConnection"), builder =>
                builder.MigrationsAssembly(typeof(ApplicationDbContext).Assembly.FullName)));

        services.AddScoped<IApplicationDbContext>(provider => provider.GetRequiredService<ApplicationDbContext>());
        services.AddScoped<ApplicationDbContextInitialiser>();

        services.AddDefaultIdentity<AspNetUser>()
            .AddRoles<IdentityRole>()
            .AddEntityFrameworkStores<ApplicationDbContext>();

        services.AddIdentityServer(options =>
            {
                // options.UserInteraction.LogoutUrl = "/Account/Logout";
                options.Events.RaiseSuccessEvents = true;
                options.Events.RaiseFailureEvents = true;
                options.Events.RaiseErrorEvents = true;
                options.Events.RaiseInformationEvents = true;
            })
            .AddApiAuthorization<AspNetUser, ApplicationDbContext>(options =>
            {
                options.ApiScopes.AddRange(Config.ApiScopes.ToArray());
                options.Clients.AddRange(Config.Clients.ToArray());
                // var client = options.Clients.AddSPA("remix", options =>
                // {
                //     options.WithRedirectUri("http://localhost:3000/authentication/login-callback");
                //     options.WithLogoutRedirectUri("http://localhost:3000/authentication/logout-callback");
                //     options.WithScopes("openid profile foo");
                // });
                //
                // client.RequirePkce = false;
            });

        services.AddTransient<IDateTime, DateTimeService>();
        services.AddTransient<IIdentityService, IdentityService>();

        services.AddAuthentication()
            .AddIdentityServerJwt();

        return services;
    }
}
