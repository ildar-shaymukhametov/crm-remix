using CRM.Application.Companies.Queries;
using CRM.Application.Companies.Queries.GetCompanies;
using CRM.Domain.Entities;

namespace CRM.Application.IntegrationTests.Companies.Queries;

public class GetCompaniesQueryTests : BaseTest
{
    public GetCompaniesQueryTests(BaseTestFixture fixture) : base(fixture) { }

    [Fact]
    public async Task User_is_admin___Returns_all_companies_with_all_fields()
    {
        var user = await _fixture.RunAsAdministratorAsync();
        var company = await _fixture.AddCompanyAsync(user.Id);

        var actual = await _fixture.SendAsync(new GetCompaniesQuery());

        Assert.Collection(actual, x =>
        {
            Assert.Equal(company.Id, x.Id);
            Assert.True(x.CanBeUpdated);
            Assert.True(x.CanBeDeleted);
            AssertOtherFieldsEqual(company, x);
            AssertManagerEqual(company, x);
        });
    }

    [Fact]
    public async Task User_has_no_claim___Returns_empty_list()
    {
        await _fixture.RunAsDefaultUserAsync();
        await _fixture.AddCompanyAsync();

        var actual = await _fixture.SendAsync(new GetCompaniesQuery());

        Assert.Empty(actual);
    }

    [Fact]
    public async Task User_has_claim_to_view_other_fields_in_any_company___Returns_companies_with_other_fields_only()
    {
        var user = await _fixture.RunAsDefaultUserAsync(new[] { Constants.Claims.Company.Any.Other.View });
        var company = await _fixture.AddCompanyAsync();

        var actual = await _fixture.SendAsync(new GetCompaniesQuery());

        Assert.Collection(actual, x =>
        {
            Assert.Equal(company.Id, x.Id);
            AssertOtherFieldsEqual(company, x);
            AssertNoManager(x);
        });
    }

    [Fact]
    public async Task User_has_claim_to_view_manager_in_any_company___Returns_companies_with_manager_only()
    {
        var user = await _fixture.RunAsDefaultUserAsync(new[] { Constants.Claims.Company.Any.Manager.View });
        var company = await _fixture.AddCompanyAsync(user.Id);

        var actual = await _fixture.SendAsync(new GetCompaniesQuery());

        Assert.Collection(actual, x =>
        {
            Assert.Equal(company.Id, x.Id);
            AssertNoOtherFields(x);
            AssertManagerEqual(company, x);
        });
    }

    // [Fact]
    // public async Task User_can_update_any_company___Returns_companies()
    // {
    //     var user = await _fixture.RunAsDefaultUserAsync(Constants.Claims.Company.Any.Other.Update);
    //     var company = await _fixture.AddCompanyAsync();

    //     var request = new GetCompaniesQuery();
    //     var actual = await _fixture.SendAsync(request);

    //     Assert.Collection(actual, x => Assert.True(x.Id == company.Id && x.CanBeUpdated && !x.CanBeDeleted));
    // }

    // [Fact]
    // public async Task User_can_delete_any_company___Returns_companies()
    // {
    //     var user = await _fixture.RunAsDefaultUserAsync(Constants.Claims.Company.Any.Delete);
    //     var company = await _fixture.AddCompanyAsync();

    //     var request = new GetCompaniesQuery();
    //     var actual = await _fixture.SendAsync(request);

    //     Assert.Collection(actual, x => Assert.True(x.Id == company.Id && !x.CanBeUpdated && x.CanBeDeleted));
    // }

    // [Fact]
    // public async Task User_can_view_own_company_and_is_manager___Returns_companies()
    // {
    //     var user = await _fixture.RunAsDefaultUserAsync(Constants.Claims.Company.WhereUserIsManager.Other.View);
    //     var company = await _fixture.AddCompanyAsync(user.Id);

    //     var request = new GetCompaniesQuery();
    //     var actual = await _fixture.SendAsync(request);

    //     Assert.Collection(actual, x => Assert.True(x.Id == company.Id && !x.CanBeUpdated && !x.CanBeDeleted));
    // }

    // [Theory]
    // [InlineData(Constants.Claims.Company.WhereUserIsManager.Other.View)]
    // [InlineData(Constants.Claims.Company.WhereUserIsManager.Other.Update)]
    // [InlineData(Constants.Claims.Company.WhereUserIsManager.Delete)]
    // public async Task User_can_view_update_delete_own_company_and_is_not_manager___Returns_empty_list(string claim)
    // {
    //     await _fixture.RunAsDefaultUserAsync(new[] { claim });
    //     await _fixture.AddCompanyAsync();

    //     var request = new GetCompaniesQuery();
    //     var actual = await _fixture.SendAsync(request);

    //     Assert.Empty(actual);
    // }

    // [Fact]
    // public async Task User_can_update_own_company___Returns_companies()
    // {
    //     var user = await _fixture.RunAsDefaultUserAsync(Constants.Claims.Company.WhereUserIsManager.Other.Update);
    //     var company = await _fixture.AddCompanyAsync(user.Id);

    //     var request = new GetCompaniesQuery();
    //     var actual = await _fixture.SendAsync(request);

    //     Assert.Collection(actual, x => Assert.True(x.Id == company.Id && x.CanBeUpdated && !x.CanBeDeleted));
    // }

    // [Theory]
    // [InlineData(Constants.Claims.Company.Any.Delete)]
    // [InlineData(Constants.Claims.Company.WhereUserIsManager.Delete)]
    // public async Task User_can_delete_own_company___Returns_companies(string claim)
    // {
    //     var user = await _fixture.RunAsDefaultUserAsync(claim);
    //     var company = await _fixture.AddCompanyAsync(user.Id);

    //     var request = new GetCompaniesQuery();
    //     var actual = await _fixture.SendAsync(request);

    //     Assert.Collection(actual, x => Assert.True(x.Id == company.Id && !x.CanBeUpdated && x.CanBeDeleted));
    // }

    private static void AssertOtherFieldsEqual(Company? expected, CompanyVm? actual)
    {
        Assert.Equal(expected?.TypeId, (actual?.Fields[nameof(Company.Type)] as CompanyTypeDto)?.Id);
        Assert.Equal(expected?.Address, actual?.Fields[nameof(Company.Address)]);
        Assert.Equal(expected?.Ceo, actual?.Fields[nameof(Company.Ceo)]);
        Assert.Equal(expected?.Contacts, actual?.Fields[nameof(Company.Contacts)]);
        Assert.Equal(expected?.Email, actual?.Fields[nameof(Company.Email)]);
        Assert.Equal(expected?.Inn, actual?.Fields[nameof(Company.Inn)]);
        Assert.Equal(expected?.Name, actual?.Fields[nameof(Company.Name)]);
        Assert.Equal(expected?.Phone, actual?.Fields[nameof(Company.Phone)]);
    }

    private static void AssertManagerEqual(Company? expected, CompanyVm? actual)
    {
        Assert.Equal(expected?.ManagerId, (actual?.Fields[nameof(Company.Manager)] as ManagerDto)?.Id);
    }

    private static void AssertNoManager(CompanyVm? actual)
    {
        Assert.False(actual?.Fields.ContainsKey(nameof(Company.Manager)));
    }

    private static void AssertNoOtherFields(CompanyVm? actual)
    {
        Assert.False(actual?.Fields.ContainsKey(nameof(Company.Address)));
        Assert.False(actual?.Fields.ContainsKey(nameof(Company.Ceo)));
        Assert.False(actual?.Fields.ContainsKey(nameof(Company.Contacts)));
        Assert.False(actual?.Fields.ContainsKey(nameof(Company.Email)));
        Assert.False(actual?.Fields.ContainsKey(nameof(Company.Inn)));
        Assert.False(actual?.Fields.ContainsKey(nameof(Company.Name)));
        Assert.False(actual?.Fields.ContainsKey(nameof(Company.Phone)));
        Assert.False(actual?.Fields.ContainsKey(nameof(Company.Type)));
    }
}