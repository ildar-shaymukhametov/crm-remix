using CRM.Application.Companies.Queries;
using CRM.Application.Companies.Queries.GetCompanies;
using CRM.Domain.Entities;
using CRM.Infrastructure.Authorization.Handlers;
using Microsoft.AspNetCore.Authorization;

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
            AssertInitialFieldsEqual(company, x);
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
        var user = await _fixture.RunAsDefaultUserAsync(new[] { Constants.Claims.Company.Any.Other.Get });
        var company = await _fixture.AddCompanyAsync();

        var actual = await _fixture.SendAsync(new GetCompaniesQuery());

        Assert.Collection(actual, x =>
        {
            AssertInitialFieldsEqual(company, x);
            AssertOtherFieldsEqual(company, x);
            AssertNoManager(x);
        });
    }

    [Fact]
    public async Task User_has_claim_to_view_manager_in_any_company___Returns_companies_with_manager_only()
    {
        var user = await _fixture.RunAsDefaultUserAsync(new[] { Constants.Claims.Company.Any.Manager.Get });
        var company = await _fixture.AddCompanyAsync(user.Id);

        var actual = await _fixture.SendAsync(new GetCompaniesQuery());

        Assert.Collection(actual, x =>
        {
            AssertInitialFieldsEqual(company, x);
            AssertNoOtherFields(x);
            AssertManagerEqual(company, x);
        });
    }

    [Fact]
    public async Task User_has_claim_to_view_other_fields_in_own_company___Returns_companies_with_other_fields_only()
    {
        var user = await _fixture.RunAsDefaultUserAsync(new[] { Constants.Claims.Company.WhereUserIsManager.Other.Get });
        var company = await _fixture.AddCompanyAsync(user.Id);

        var actual = await _fixture.SendAsync(new GetCompaniesQuery());

        Assert.Collection(actual, x =>
        {
            AssertInitialFieldsEqual(company, x);
            AssertOtherFieldsEqual(company, x);
            AssertNoManager(x);
        });
    }

    [Fact]
    public async Task User_has_claim_to_view_manager_in_own_company___Returns_companies_with_manager_only()
    {
        var user = await _fixture.RunAsDefaultUserAsync(new[] { Constants.Claims.Company.WhereUserIsManager.Manager.Get });
        var company = await _fixture.AddCompanyAsync(user.Id);

        var actual = await _fixture.SendAsync(new GetCompaniesQuery());

        Assert.Collection(actual, x =>
        {
            AssertInitialFieldsEqual(company, x);
            AssertManagerEqual(company, x);
            AssertNoOtherFields(x);
        });
    }

    [Theory]
    [InlineData(Constants.Claims.Company.WhereUserIsManager.Other.Get)]
    [InlineData(Constants.Claims.Company.WhereUserIsManager.Manager.Get)]
    [InlineData(Constants.Claims.Company.WhereUserIsManager.Delete)]
    public async Task User_has_claim_to_view_certain_fields_in_own_company_and_is_not_manager___Returns_empty_list(string claim)
    {
        await _fixture.RunAsDefaultUserAsync(new[] { claim });
        await _fixture.AddCompanyAsync();

        var actual = await _fixture.SendAsync(new GetCompaniesQuery());

        Assert.Empty(actual);
    }

    [Fact]
    public async Task User_has_claim_to_delete_any_company___Returns_companies_with_id_only()
    {
        await _fixture.RunAsDefaultUserAsync(new[] { Constants.Access.Company.Any.Delete });
        var company = await _fixture.AddCompanyAsync();
        _fixture.ReplaceService<IAuthorizationHandler, DeleteCompanyAuthorizationHandler>(new DeleteCompanyAuthorizationHandlerMock());

        var actual = await _fixture.SendAsync(new GetCompaniesQuery());

        Assert.Collection(actual, x =>
        {
            AssertInitialFieldsEqual(company, x);
            Assert.True(x?.CanBeDeleted);
            AssertNoManager(x);
            AssertNoOtherFields(x);
        });
    }

    [Fact]
    public async Task User_has_claim_to_delete_own_company___Returns_companies_with_id_only()
    {
        var user = await _fixture.RunAsDefaultUserAsync(new[] { Constants.Access.Company.WhereUserIsManager.Delete });
        var company = await _fixture.AddCompanyAsync(user.Id);
        _fixture.ReplaceService<IAuthorizationHandler, DeleteCompanyAuthorizationHandler>(new DeleteCompanyAuthorizationHandlerMock());

        var actual = await _fixture.SendAsync(new GetCompaniesQuery());

        Assert.Collection(actual, x =>
        {
            AssertInitialFieldsEqual(company, x);
            Assert.True(x?.CanBeDeleted);
            AssertNoManager(x);
            AssertNoOtherFields(x);
        });
    }

    [Fact]
    public async Task User_has_claim_to_update_other_fields_in_any_company___Returns_companies_with_id_and_other_fields_only()
    {
        await _fixture.RunAsDefaultUserAsync(new[] { Constants.Claims.Company.Any.Other.Set });
        var company = await _fixture.AddCompanyAsync();

        var actual = await _fixture.SendAsync(new GetCompaniesQuery());

        Assert.Collection(actual, x =>
        {
            AssertInitialFieldsEqual(company, x);
            Assert.True(x?.CanBeUpdated);
            AssertNoManager(x);
            AssertOtherFieldsEqual(company, x);
        });
    }

    [Fact]
    public async Task User_has_claim_to_update_other_fields_in_own_company___Returns_companies_with_id_and_other_fields_only()
    {
        var user = await _fixture.RunAsDefaultUserAsync(new[] { Constants.Claims.Company.WhereUserIsManager.Other.Set });
        var company = await _fixture.AddCompanyAsync(user.Id);

        var actual = await _fixture.SendAsync(new GetCompaniesQuery());
        Assert.Collection(actual, x =>
        {
            AssertInitialFieldsEqual(company, x);
            Assert.True(x?.CanBeUpdated);
            AssertNoManager(x);
            AssertOtherFieldsEqual(company, x);
        });
    }

    [Fact]
    public async Task User_has_claim_to_update_other_fields_in_own_company_and_is_not_manager___Returns_empty_list()
    {
        await _fixture.RunAsDefaultUserAsync(new[] { Constants.Claims.Company.WhereUserIsManager.Other.Set });
        await _fixture.AddCompanyAsync();

        var actual = await _fixture.SendAsync(new GetCompaniesQuery());

        Assert.Empty(actual);
    }

    [Theory]
    [InlineData(Constants.Claims.Company.Any.Manager.SetFromAnyToAny)]
    [InlineData(Constants.Claims.Company.Any.Manager.SetFromAnyToNone)]
    [InlineData(Constants.Claims.Company.Any.Manager.SetFromAnyToSelf)]
    public async Task User_has_claim_to_set_manager_from_any_in_any_company___Returns_companies_with_id_and_manager_only(string claim)
    {
        await _fixture.RunAsDefaultUserAsync(claim);
        var company = await _fixture.AddCompanyAsync();

        var actual = await _fixture.SendAsync(new GetCompaniesQuery());

        Assert.Collection(actual, x =>
        {
            AssertInitialFieldsEqual(company, x);
            Assert.True(x?.CanBeUpdated);
            AssertManagerEqual(company, x);
            AssertNoOtherFields(x);
        });
    }

    [Theory]
    [InlineData(Constants.Claims.Company.Any.Manager.SetFromAnyToAny)]
    [InlineData(Constants.Claims.Company.Any.Manager.SetFromAnyToNone)]
    [InlineData(Constants.Claims.Company.Any.Manager.SetFromAnyToSelf)]
    public async Task User_has_claim_to_set_manager_from_any_in_any_company_and_is_manager___Returns_companies_with_id_and_manager_only(string claim)
    {
        var user = await _fixture.RunAsDefaultUserAsync(claim);
        var company = await _fixture.AddCompanyAsync(user.Id);

        var actual = await _fixture.SendAsync(new GetCompaniesQuery());

        Assert.Collection(actual, x =>
        {
            AssertInitialFieldsEqual(company, x);
            Assert.True(x?.CanBeUpdated);
            AssertManagerEqual(company, x);
            AssertNoOtherFields(x);
        });
    }

    [Theory]
    [InlineData(Constants.Claims.Company.Any.Manager.SetFromSelfToAny)]
    [InlineData(Constants.Claims.Company.Any.Manager.SetFromSelfToNone)]
    [InlineData(Constants.Claims.Company.WhereUserIsManager.Manager.SetFromSelfToNone)]
    [InlineData(Constants.Claims.Company.WhereUserIsManager.Manager.SetFromSelfToAny)]
    public async Task User_has_claim_to_set_manager_from_self___Returns_companies_with_id_and_manager_only(string claim)
    {
        var user = await _fixture.RunAsDefaultUserAsync(claim);
        var company = await _fixture.AddCompanyAsync(user.Id);

        var actual = await _fixture.SendAsync(new GetCompaniesQuery());

        Assert.Collection(actual, x =>
        {
            AssertInitialFieldsEqual(company, x);
            Assert.True(x?.CanBeUpdated);
            AssertManagerEqual(company, x);
            AssertNoOtherFields(x);
        });
    }

    [Theory]
    [InlineData(Constants.Claims.Company.Any.Manager.SetFromSelfToAny)]
    [InlineData(Constants.Claims.Company.Any.Manager.SetFromSelfToNone)]
    [InlineData(Constants.Claims.Company.WhereUserIsManager.Manager.SetFromSelfToNone)]
    [InlineData(Constants.Claims.Company.WhereUserIsManager.Manager.SetFromSelfToAny)]
    public async Task User_has_claim_to_set_manager_from_self_and_is_not_manager___Returns_empty_list(string claim)
    {
        await _fixture.RunAsDefaultUserAsync(claim);
        await _fixture.AddCompanyAsync();

        var actual = await _fixture.SendAsync(new GetCompaniesQuery());

        Assert.Empty(actual);
    }

    [Theory]
    [InlineData(Constants.Claims.Company.Any.Manager.SetFromNoneToAny)]
    [InlineData(Constants.Claims.Company.Any.Manager.SetFromNoneToSelf)]
    public async Task User_has_claim_to_set_manager_from_none_in_any_company___Returns_companies_with_id_and_manager_only(string claim)
    {
        await _fixture.RunAsDefaultUserAsync(claim);
        var company = await _fixture.AddCompanyAsync();

        var actual = await _fixture.SendAsync(new GetCompaniesQuery());

        Assert.Collection(actual, x =>
        {
            AssertInitialFieldsEqual(company, x);
            Assert.True(x?.CanBeUpdated);
            AssertManagerEqual(company, x);
            AssertNoOtherFields(x);
        });
    }

    [Theory]
    [InlineData(Constants.Claims.Company.Any.Manager.SetFromNoneToAny)]
    [InlineData(Constants.Claims.Company.Any.Manager.SetFromNoneToSelf)]
    public async Task User_has_claim_to_set_manager_from_none_in_any_company_and_company_has_manager___Returns_empty_list(string claim)
    {
        var user = await _fixture.RunAsDefaultUserAsync(claim);
        await _fixture.AddCompanyAsync(user.Id);

        var actual = await _fixture.SendAsync(new GetCompaniesQuery());

        Assert.Empty(actual);
    }

    private static void AssertOtherFieldsEqual(Company? expected, CompanyVm? actual)
    {
        Assert.Equal(expected?.TypeId, (actual?.Fields[nameof(Company.Type)] as CompanyTypeDto)?.Id);
        Assert.Equal(expected?.Address, actual?.Fields[nameof(Company.Address)]);
        Assert.Equal(expected?.Ceo, actual?.Fields[nameof(Company.Ceo)]);
        Assert.Equal(expected?.Contacts, actual?.Fields[nameof(Company.Contacts)]);
        Assert.Equal(expected?.Email, actual?.Fields[nameof(Company.Email)]);
        Assert.Equal(expected?.Inn, actual?.Fields[nameof(Company.Inn)]);
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
        Assert.False(actual?.Fields.ContainsKey(nameof(Company.Phone)));
        Assert.False(actual?.Fields.ContainsKey(nameof(Company.Type)));
    }

    private static void AssertInitialFieldsEqual(Company? expected, CompanyVm? actual)
    {
        Assert.Equal(expected?.Id, actual?.Id);
        Assert.Equal(expected?.Name, actual?.Fields[nameof(Company.Name)]);
    }
}