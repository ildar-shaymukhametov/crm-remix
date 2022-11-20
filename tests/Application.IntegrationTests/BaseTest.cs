namespace CRM.Application.IntegrationTests;

[Collection("Database")]
public abstract class BaseTest : IAsyncLifetime
{
    protected readonly BaseTestFixture _fixture;

    public BaseTest(BaseTestFixture fixture)
    {
        _fixture = fixture;
    }

    public async Task DisposeAsync()
    {
        await _fixture.ResetStateAsync();
    }

    public async Task InitializeAsync()
    {
        await _fixture.InitStateAsync();
    }
}
