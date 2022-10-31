using Application.IntegrationTests;
using CRM.Application.Common.Exceptions;
using CRM.Application.Companies.Queries.GetCompanies;

namespace CRM.Application.IntegrationTests.Companies.Queries;

public class GetCompaniesTests : BaseTest
{
    public GetCompaniesTests(BaseTestFixture fixture) : base(fixture) { }

    [Fact]
    public async Task User_is_admin___Returns_company()
    {
        var user = await _fixture.RunAsAdministratorAsync();

        var company = Faker.Builders.Company();
        await _fixture.AddAsync(company);

        var request = new GetCompaniesQuery();
        var result = await _fixture.SendAsync(request);

        Assert.Collection(result, x => Assert.Equal(company.Id, x.Id));
    }

    [Fact]
    public async Task User_has_claim___Returns_company()
    {
        var user = await _fixture.RunAsDefaultUserAsync(new[]
        {
            Constants.Claims.ViewCompany
        });

        var company = Faker.Builders.Company();
        await _fixture.AddAsync(company);

        var request = new GetCompaniesQuery();
        var result = await _fixture.SendAsync(request);

        Assert.Collection(result, x => Assert.Equal(company.Id, x.Id));
    }

    [Fact]
    public async Task User_has_no_claim___Throws_forbidden_access()
    {
        var user = await _fixture.RunAsDefaultUserAsync();

        var company = Faker.Builders.Company();
        await _fixture.AddAsync(company);

        var request = new GetCompaniesQuery();
        await Assert.ThrowsAsync<ForbiddenAccessException>(() => _fixture.SendAsync(request));
    }
}