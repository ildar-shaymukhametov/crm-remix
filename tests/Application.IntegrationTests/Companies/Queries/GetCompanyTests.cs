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
    public async Task User_has_claim_and_is_manager___Returns_company(string claim)
    {
        var user = await _fixture.RunAsDefaultUserAsync(new[] { claim });
        var company = Faker.Builders.Company(managerId: user.Id);
        await _fixture.AddAsync(company);

        var request = new GetCompanyQuery { Id = company.Id };
        var result = await _fixture.SendAsync(request);

        Assert.Equal(company.Id, result.Id);
    }

    [Theory]
    [InlineData(Constants.Claims.ViewCompany)]
    [InlineData(Constants.Claims.DeleteCompany)]
    [InlineData(Constants.Claims.UpdateCompany)]
    public async Task User_has_claim_and_is_not_manager___Throws_forbidden_access(string claim)
    {
        await _fixture.RunAsDefaultUserAsync(new[] { claim });

        var company = Faker.Builders.Company();
        await _fixture.AddAsync(company);

        var request = new GetCompanyQuery { Id = company.Id };
        await Assert.ThrowsAsync<ForbiddenAccessException>(() => _fixture.SendAsync(request));
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

    [Theory]
    [InlineData(Constants.Claims.ViewCompany)]
    [InlineData(Constants.Claims.DeleteCompany)]
    [InlineData(Constants.Claims.UpdateCompany)]
    public async Task Not_found(string claim)
    {
        var user = await _fixture.RunAsDefaultUserAsync(new[] { claim });
        var request = new GetCompanyQuery { Id = 1 };
        await Assert.ThrowsAsync<NotFoundException>(() => _fixture.SendAsync(request));
    }
}