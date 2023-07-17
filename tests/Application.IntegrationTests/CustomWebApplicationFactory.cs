using CRM.Application.Common.Interfaces;
using CRM.Domain.Interfaces;
using CRM.Infrastructure.Persistence;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NSubstitute;

namespace CRM.Application.IntegrationTests;

public class CustomWebApplicationFactory : WebApplicationFactory<Program>
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Testing");
        builder.ConfigureAppConfiguration(configurationBuilder =>
        {
            var integrationConfig = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json")
                .AddEnvironmentVariables()
                .Build();

            configurationBuilder.AddConfiguration(integrationConfig);
        });

        builder.ConfigureServices((builder, services) =>
        {
            services.Remove<ICurrentUserService>()
                .AddTransient(provider =>
                {
                    var service = Substitute.For<ICurrentUserService>();
                    service.UserId.Returns(BaseTestFixture.GetCurrentUserId());
                    return service;
                });

            services.Remove<DbContextOptions<ApplicationDbContext>>()
                .AddDbContext<ApplicationDbContext>((sp, options) =>
                    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"),
                        builder => builder.MigrationsAssembly(typeof(ApplicationDbContext).Assembly.FullName)));

            services.Remove<IDateTime>()
                .AddSingleton(provider =>
                {
                    var service = Substitute.For<IDateTime>();
                    service.Now.Returns(BaseTestFixture.Now);
                    service.UtcNow.Returns(BaseTestFixture.UtcNow);
                    return service;
                });
        });
    }
}
