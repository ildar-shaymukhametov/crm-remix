using CRM.Application.Common.Exceptions;
using CRM.Application.Companies.Queries.GetCompany;

namespace CRM.Application.IntegrationTests.Companies.Queries;

public class GetCompanyTests : BaseTest
{
    public GetCompanyTests(BaseTestFixture fixture) : base(fixture) { }

    [Theory]
    [InlineData(Constants.Claims.ViewCompany)]
    [InlineData(Constants.Claims.DeleteCompany)]
    [InlineData(Constants.Claims.UpdateCompany)]
    public async Task User_has_claim_and_is_manager__Returns_company(string claim)
    {
        await _fixture.RunAsDefaultUserAsync(new[] { claim });

        var user = await _fixture.CreateUserAsync();
        var company = Faker.Builders.Company(managerId: user.Id);
        await _fixture.AddAsync(company);

        var request = new GetCompanyQuery { Id = company.Id };
        var result = await _fixture.SendAsync(request);

        Assert.Equal(company.Id, result.Id);
    }

    [Fact]
    public async Task User_has_no_claim___Throws_forbidden_access()
    {
        var user = await _fixture.RunAsDefaultUserAsync();

        var company = Faker.Builders.Company();
        await _fixture.AddAsync(company);

        var request = new GetCompanyQuery { Id = company.Id };
        await Assert.ThrowsAsync<ForbiddenAccessException>(() => _fixture.SendAsync(request));
    }
}