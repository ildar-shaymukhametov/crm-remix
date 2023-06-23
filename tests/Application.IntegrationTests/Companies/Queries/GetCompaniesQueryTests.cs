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
        var expected = await _fixture.AddCompanyAsync(user.Id);

        var actual = await _fixture.SendAsync(new GetCompaniesQuery());

        Assert.Collection(actual, x => AssertFields(expected, x, true, true, true));
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
    public async Task User_has_claim_to_view_other_fields_in_any_company___Returns_companies_with_other_fields()
    {
        var user = await _fixture.RunAsDefaultUserAsync(new[] { Constants.Claims.Company.Any.Other.Get });
        var expected = await _fixture.AddCompanyAsync();

        var actual = await _fixture.SendAsync(new GetCompaniesQuery());

        Assert.Collection(actual, x => AssertFields(expected, x, other: true));
    }

    [Fact]
    public async Task User_has_claim_to_view_manager_in_any_company___Returns_companies_with_manager()
    {
        var user = await _fixture.RunAsDefaultUserAsync(new[] { Constants.Claims.Company.Any.Manager.Get });
        var expected = await _fixture.AddCompanyAsync(user.Id);

        var actual = await _fixture.SendAsync(new GetCompaniesQuery());

        Assert.Collection(actual, x => AssertFields(expected, x, manager: true));
    }

    [Fact]
    public async Task User_has_claim_to_view_other_fields_in_own_company___Returns_companies_with_other_fields()
    {
        var user = await _fixture.RunAsDefaultUserAsync(new[] { Constants.Claims.Company.WhereUserIsManager.Other.Get });
        var expected = await _fixture.AddCompanyAsync(user.Id);

        var actual = await _fixture.SendAsync(new GetCompaniesQuery());

        Assert.Collection(actual, x => AssertFields(expected, x, other: true));
    }

    [Fact]
    public async Task User_has_claim_to_view_manager_in_own_company___Returns_companies_with_manager()
    {
        var user = await _fixture.RunAsDefaultUserAsync(new[] { Constants.Claims.Company.WhereUserIsManager.Manager.Get });
        var expected = await _fixture.AddCompanyAsync(user.Id);

        var actual = await _fixture.SendAsync(new GetCompaniesQuery());

        Assert.Collection(actual, x => AssertFields(expected, x, manager: true));
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
    public async Task User_has_claim_to_delete_any_company___Returns_companies_with_id()
    {
        await _fixture.RunAsDefaultUserAsync(new[] { Constants.Access.Company.Any.Delete });
        var expected = await _fixture.AddCompanyAsync();
        _fixture.ReplaceService<IAuthorizationHandler, DeleteCompanyAuthorizationHandler>(new DeleteCompanyAuthorizationHandlerMock());

        var actual = await _fixture.SendAsync(new GetCompaniesQuery());

        Assert.Collection(actual, x =>
        {
            AssertFields(expected, x);
            Assert.True(x.CanBeDeleted);
        });
    }

    [Fact]
    public async Task User_has_claim_to_delete_own_company___Returns_companies_with_id()
    {
        var user = await _fixture.RunAsDefaultUserAsync(new[] { Constants.Access.Company.WhereUserIsManager.Delete });
        var expected = await _fixture.AddCompanyAsync(user.Id);
        _fixture.ReplaceService<IAuthorizationHandler, DeleteCompanyAuthorizationHandler>(new DeleteCompanyAuthorizationHandlerMock());

        var actual = await _fixture.SendAsync(new GetCompaniesQuery());

        Assert.Collection(actual, x =>
        {
            AssertFields(expected, x);
            Assert.True(x.CanBeDeleted);
        });
    }

    [Fact]
    public async Task User_has_claim_to_update_other_fields_in_any_company___Returns_companies_with_id_and_other_fields()
    {
        await _fixture.RunAsDefaultUserAsync(new[] { Constants.Claims.Company.Any.Other.Set });
        var expected = await _fixture.AddCompanyAsync();

        var actual = await _fixture.SendAsync(new GetCompaniesQuery());

        Assert.Collection(actual, x =>
        {
            AssertFields(expected, x, other: true);
            Assert.True(x.CanBeUpdated);
        });
    }

    [Fact]
    public async Task User_has_claim_to_update_other_fields_in_own_company___Returns_companies_with_id_and_other_fields()
    {
        var user = await _fixture.RunAsDefaultUserAsync(new[] { Constants.Claims.Company.WhereUserIsManager.Other.Set });
        var expected = await _fixture.AddCompanyAsync(user.Id);

        var actual = await _fixture.SendAsync(new GetCompaniesQuery());
        Assert.Collection(actual, x =>
        {
            AssertFields(expected, x, other: true);
            Assert.True(x.CanBeUpdated);
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
    public async Task User_has_claim_to_set_manager_from_any_in_any_company___Returns_companies_with_id_and_manager(string claim)
    {
        await _fixture.RunAsDefaultUserAsync(claim);
        var expected = await _fixture.AddCompanyAsync();

        var actual = await _fixture.SendAsync(new GetCompaniesQuery());

        Assert.Collection(actual, x =>
        {
            AssertFields(expected, x, manager: true);
            Assert.True(x.CanBeUpdated);
        });
    }

    [Theory]
    [InlineData(Constants.Claims.Company.Any.Manager.SetFromAnyToAny)]
    [InlineData(Constants.Claims.Company.Any.Manager.SetFromAnyToNone)]
    [InlineData(Constants.Claims.Company.Any.Manager.SetFromAnyToSelf)]
    public async Task User_has_claim_to_set_manager_from_any_in_any_company_and_is_manager___Returns_companies_with_id_and_manager(string claim)
    {
        var user = await _fixture.RunAsDefaultUserAsync(claim);
        var expected = await _fixture.AddCompanyAsync(user.Id);

        var actual = await _fixture.SendAsync(new GetCompaniesQuery());

        Assert.Collection(actual, x =>
        {
            AssertFields(expected, x, manager: true);
            Assert.True(x.CanBeUpdated);
        });
    }

    [Theory]
    [InlineData(Constants.Claims.Company.Any.Manager.SetFromSelfToAny)]
    [InlineData(Constants.Claims.Company.Any.Manager.SetFromSelfToNone)]
    [InlineData(Constants.Claims.Company.WhereUserIsManager.Manager.SetFromSelfToNone)]
    [InlineData(Constants.Claims.Company.WhereUserIsManager.Manager.SetFromSelfToAny)]
    public async Task User_has_claim_to_set_manager_from_self___Returns_companies_with_id_and_manager(string claim)
    {
        var user = await _fixture.RunAsDefaultUserAsync(claim);
        var expected = await _fixture.AddCompanyAsync(user.Id);

        var actual = await _fixture.SendAsync(new GetCompaniesQuery());

        Assert.Collection(actual, x =>
        {
            AssertFields(expected, x, manager: true);
            Assert.True(x.CanBeUpdated);
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
    public async Task User_has_claim_to_set_manager_from_none_in_any_company___Returns_companies_with_id_and_manager(string claim)
    {
        await _fixture.RunAsDefaultUserAsync(claim);
        var expected = await _fixture.AddCompanyAsync();

        var actual = await _fixture.SendAsync(new GetCompaniesQuery());

        Assert.Collection(actual, x =>
        {
            AssertFields(expected, x, manager: true);
            Assert.True(x.CanBeUpdated);
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

    [Fact]
    public async Task User_has_claim_to_view_name_in_any_company___Returns_companies_with_name_fields()
    {
        var user = await _fixture.RunAsDefaultUserAsync(new[] { Constants.Claims.Company.Any.Name.Get });
        var expected = await _fixture.AddCompanyAsync();

        var actual = await _fixture.SendAsync(new GetCompaniesQuery());

        Assert.Collection(actual, x => AssertFields(expected, x, name: true));
    }

    [Fact]
    public async Task User_has_claim_to_view_name_in_own_company___Returns_companies_with_name_fields()
    {
        var user = await _fixture.RunAsDefaultUserAsync(new[] { Constants.Claims.Company.WhereUserIsManager.Name.Get });
        var expected = await _fixture.AddCompanyAsync(user.Id);

        var actual = await _fixture.SendAsync(new GetCompaniesQuery());

        Assert.Collection(actual, x => AssertFields(expected, x, name: true));
    }

    [Fact]
    public async Task User_has_claim_to_update_name_in_any_company___Returns_companies_with_id_and_name_fields()
    {
        await _fixture.RunAsDefaultUserAsync(new[] { Constants.Claims.Company.Any.Name.Set });
        var expected = await _fixture.AddCompanyAsync();

        var actual = await _fixture.SendAsync(new GetCompaniesQuery());

        Assert.Collection(actual, x =>
        {
            AssertFields(expected, x, name: true);
            Assert.True(x.CanBeUpdated);
        });
    }

    private static void AssertFields(Company? expected, CompanyVm? actual, bool name = false, bool manager = false, bool other = false)
    {
        Assert.Equal(expected?.Id, actual?.Id);

        if (other)
        {
            Assert.Equal(expected?.TypeId, (actual?.Fields[nameof(Company.Type)] as CompanyTypeDto)?.Id);
            Assert.Equal(expected?.Address, actual?.Fields[nameof(Company.Address)]);
            Assert.Equal(expected?.Ceo, actual?.Fields[nameof(Company.Ceo)]);
            Assert.Equal(expected?.Contacts, actual?.Fields[nameof(Company.Contacts)]);
            Assert.Equal(expected?.Email, actual?.Fields[nameof(Company.Email)]);
            Assert.Equal(expected?.Inn, actual?.Fields[nameof(Company.Inn)]);
            Assert.Equal(expected?.Phone, actual?.Fields[nameof(Company.Phone)]);
        }
        else
        {
            Assert.False(actual?.Fields.ContainsKey(nameof(Company.Address)));
            Assert.False(actual?.Fields.ContainsKey(nameof(Company.Ceo)));
            Assert.False(actual?.Fields.ContainsKey(nameof(Company.Contacts)));
            Assert.False(actual?.Fields.ContainsKey(nameof(Company.Email)));
            Assert.False(actual?.Fields.ContainsKey(nameof(Company.Inn)));
            Assert.False(actual?.Fields.ContainsKey(nameof(Company.Phone)));
            Assert.False(actual?.Fields.ContainsKey(nameof(Company.Type)));
        }

        if (manager)
        {
            Assert.Equal(expected?.ManagerId, (actual?.Fields[nameof(Company.Manager)] as ManagerDto)?.Id);
        }
        else
        {
            Assert.False(actual?.Fields.ContainsKey(nameof(Company.Manager)));
        }

        if (name)
        {
            Assert.Equal(expected?.Name, actual?.Fields[nameof(Company.Name)]);
        }
        else
        {
            Assert.False(actual?.Fields.ContainsKey(nameof(Company.Name)));
        }
    }
}