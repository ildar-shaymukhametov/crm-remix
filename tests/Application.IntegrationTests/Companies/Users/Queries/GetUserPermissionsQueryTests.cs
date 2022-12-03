using CRM.Application.Common.Exceptions;
using CRM.Application.IntegrationTests;
using CRM.Application.Users.Queries.GetUserPermissions;

namespace Application.IntegrationTests.Companies.Users.Queries;

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
            "CreateCompany",
            "UpdateCompany",
            "ViewCompany",
            "DeleteCompany",
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
    public async Task Company_does_not_exist___Throws_not_found()
    {
        await _fixture.RunAsAdministratorAsync();

        var expected = new[]
        {
            "CreateCompany",
            "UpdateCompany",
            "ViewCompany",
            "DeleteCompany",
        };

        var request = new GetUserPermissionsQuery
        {
            ResourceKey = Faker.RandomNumber.Next().ToString(),
            RequestedPermissions = expected
        };

        await Assert.ThrowsAsync<NotFoundException>(() => _fixture.SendAsync(request));
    }

    [Fact]
    public async Task Does_not_return_company_permissions()
    {
        await _fixture.RunAsDefaultUserAsync();
        var company = Faker.Builders.Company();
        await _fixture.AddAsync(company);

        var expected = new[]
        {
            "CreateCompany",
            "UpdateCompany",
            "ViewCompany",
            "DeleteCompany",
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
