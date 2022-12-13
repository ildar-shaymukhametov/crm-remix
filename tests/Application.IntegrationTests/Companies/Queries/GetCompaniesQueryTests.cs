using CRM.Application.Companies.Queries.GetCompanies;

namespace CRM.Application.IntegrationTests.Companies.Queries;

public class GetCompaniesQueryTests : BaseTest
{
    public GetCompaniesQueryTests(BaseTestFixture fixture) : base(fixture) { }

    [Fact]
    public async Task User_is_admin___Returns_companies()
    {
        var user = await _fixture.RunAsAdministratorAsync();

        var company = Faker.Builders.Company();
        await _fixture.AddAsync(company);

        var request = new GetCompaniesQuery();
        var actual = await _fixture.SendAsync(request);

        Assert.Collection(actual, x => Assert.True(x.Id == company.Id
            && x.CanBeEdited
            && x.CanBeDeleted));
    }

    [Theory]
    [InlineData(Constants.Claims.ViewCompany)]
    [InlineData(Constants.Claims.DeleteCompany)]
    [InlineData(Constants.Claims.UpdateCompany)]
    [InlineData(Constants.Claims.ViewAnyCompany)]
    [InlineData(Constants.Claims.DeleteAnyCompany)]
    [InlineData(Constants.Claims.UpdateAnyCompany)]
    public async Task User_has_claim_and_is_manager___Returns_company(string claim)
    {
        var user = await _fixture.RunAsDefaultUserAsync(new[] { claim });

        var company = Faker.Builders.Company(managerId: user.Id);
        await _fixture.AddAsync(company);

        var request = new GetCompaniesQuery();
        var actual = await _fixture.SendAsync(request);

        Assert.Collection(actual, x => Assert.True(x.Id == company.Id
            && x.CanBeEdited == (claim == Constants.Claims.UpdateCompany || claim == Constants.Claims.UpdateAnyCompany)
            && x.CanBeDeleted == (claim == Constants.Claims.DeleteCompany || claim == Constants.Claims.DeleteAnyCompany)));
    }

    [Theory]
    [InlineData(Constants.Claims.ViewCompany)]
    [InlineData(Constants.Claims.DeleteCompany)]
    [InlineData(Constants.Claims.UpdateCompany)]
    public async Task User_has_claim_and_is_not_manager___Returns_empty_list(string claim)
    {
        await _fixture.RunAsDefaultUserAsync(new[] { claim });

        var company = Faker.Builders.Company();
        await _fixture.AddAsync(company);

        var request = new GetCompaniesQuery();
        var actual = await _fixture.SendAsync(request);

        Assert.Empty(actual);
    }

    [Fact]
    public async Task User_has_no_permissions___Returns_empty_list()
    {
        await _fixture.RunAsDefaultUserAsync();

        var company = Faker.Builders.Company();
        await _fixture.AddAsync(company);

        var request = new GetCompaniesQuery();
        var actual = await _fixture.SendAsync(request);

        Assert.Empty(actual);
    }
}