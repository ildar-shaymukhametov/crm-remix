using CRM.Application.IntegrationTests;
using CRM.Application.Users.Queries.GetUserPermissions;
using static CRM.Application.Constants;

namespace Application.IntegrationTests.Companies.Users.Queries;

public class GetUserPermissionsQueryTests : BaseTest
{
    public GetUserPermissionsQueryTests(BaseTestFixture fixture) : base(fixture) { }

    [Fact]
    public async Task Returns_permissions()
    {
        await _fixture.RunAsAdministratorAsync();

        var expected = new[]
        {
            Policies.CreateCompany,
            Policies.UpdateCompany,
        };

        var request = new GetUserPermissionsQuery
        {
            RequestedPermissions = expected
        };

        var actual = await _fixture.SendAsync(request);

        Assert.Equal(expected, actual);
    }

    [Fact]
    public async Task Does_not_return_permissions()
    {
        await _fixture.RunAsDefaultUserAsync();

        var expected = new[]
        {
            Policies.CreateCompany,
            Policies.UpdateCompany
        };

        var request = new GetUserPermissionsQuery
        {
            RequestedPermissions = expected
        };

        var actual = await _fixture.SendAsync(request);

        Assert.Empty(actual);
    }
}
