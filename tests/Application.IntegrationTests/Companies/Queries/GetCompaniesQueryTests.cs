using CRM.Application.Companies.Queries.GetCompanies;
using FluentAssertions;

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

    [Fact]
    public async Task User_can_view_any_company___Returns_companies()
    {
        var user = await _fixture.RunAsDefaultUserAsync(Constants.Claims.Company.Any.View);

        var companyA = await _fixture.AddCompanyAsync();
        var companyB = await _fixture.AddCompanyAsync();

        var request = new GetCompaniesQuery();
        var actual = await _fixture.SendAsync(request);

        actual.Select(x => x.Id).Should().BeEquivalentTo(new [] { companyA.Id, companyB.Id });
    }

    [Fact]
    public async Task User_can_update_any_company___Returns_companies()
    {
        var user = await _fixture.RunAsDefaultUserAsync(Constants.Claims.Company.Any.Update);

        var companyA = await _fixture.AddCompanyAsync();
        var companyB = await _fixture.AddCompanyAsync();

        var request = new GetCompaniesQuery();
        var actual = await _fixture.SendAsync(request);

        Assert.Equal(2, actual.Length);
        Assert.Contains(actual, x => x.Id == companyA.Id && x.CanBeEdited == true && x.CanBeDeleted == false);
        Assert.Contains(actual, x => x.Id == companyB.Id && x.CanBeEdited == true && x.CanBeDeleted == false);
    }

    [Fact]
    public async Task User_can_delete_any_company___Returns_companies()
    {
        var user = await _fixture.RunAsDefaultUserAsync(Constants.Claims.Company.Any.Delete);

        var companyA = await _fixture.AddCompanyAsync();
        var companyB = await _fixture.AddCompanyAsync();

        var request = new GetCompaniesQuery();
        var actual = await _fixture.SendAsync(request);

        Assert.Equal(2, actual.Length);
        Assert.Contains(actual, x => x.Id == companyB.Id && x.CanBeEdited == false && x.CanBeDeleted == true);
        Assert.Contains(actual, x => x.Id == companyA.Id && x.CanBeEdited == false && x.CanBeDeleted == true);
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