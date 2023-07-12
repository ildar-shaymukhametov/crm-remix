using CRM.Application.Users.Queries.GetUserPermissions;

namespace CRM.Application.IntegrationTests.Users.Queries;

public class GetUserPermissionsQueryTests : BaseTest
{
    public GetUserPermissionsQueryTests(BaseTestFixture fixture) : base(fixture) { }

    [Fact]
    public async Task Returns_company_permissions()
    {
        await _fixture.RunAsAdministratorAsync();
        var company = Faker.Builders.Company();
        await _fixture.AddAsync(company);

        var expected = new[]
        {
            "Company.Create",
        };

        var request = new GetUserPermissionsQuery
        {
            ResourceKey = company.Id.ToString(),
            RequestedPermissions = expected
        };

        var actual = await _fixture.SendAsync(request);

        Assert.Equal(expected, actual.Permissions);
    }

    [Fact]
    public async Task Does_not_return_company_permissions()
    {
        await _fixture.RunAsDefaultUserAsync();
        var company = Faker.Builders.Company();
        await _fixture.AddAsync(company);

        var expected = new[]
        {
            "Company.Create",
        };

        var request = new GetUserPermissionsQuery
        {
            ResourceKey = company.Id.ToString(),
            RequestedPermissions = expected
        };

        var actual = await _fixture.SendAsync(request);

        Assert.Empty(actual.Permissions);
    }
}
