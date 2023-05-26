using CRM.Application.Common.Interfaces;
using Microsoft.Extensions.Configuration;
using Respawn;
using Respawn.Graph;

namespace CRM.Infrastructure.Services;

public class TestService : ITestService
{
    private readonly string _connectionString;
    private Respawner _respawner = null!;

    public TestService(IConfiguration configuration)
    {
        _connectionString = configuration.GetConnectionString("DefaultConnection");
    }

    public async Task CreateCheckpointAsync()
    {
        _respawner = await Respawner.CreateAsync(_connectionString, new RespawnerOptions
        {
            TablesToIgnore = new Table[]
            {
                "__EFMigrationsHistory",
                "AspNetRoles",
                "AspNetUserLogins",
                "AspNetUserTokens",
                "DeviceCodes",
                "Keys",
                "PersistedGrants",
                "UserClaimTypes"
            }
        });
    }

    public async Task ResetDbAsync()
    {
        await _respawner.ResetAsync(_connectionString);
    }
}
