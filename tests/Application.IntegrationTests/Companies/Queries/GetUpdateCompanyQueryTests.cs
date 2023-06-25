using CRM.Application.Common.Exceptions;
using CRM.Application.Companies.Queries;
using CRM.Application.Companies.Queries.GetUpdateCompany;
using CRM.Domain.Entities;
using CRM.Infrastructure.Identity;
using FluentAssertions;

namespace CRM.Application.IntegrationTests.Companies.Queries;

public class GetUpdateCompanyQueryTests : BaseTest
{
    public GetUpdateCompanyQueryTests(BaseTestFixture fixture) : base(fixture)
    {
    }

    [Fact]
    public async Task User_is_admin___Returns_all_fields_and_init_data()
    {
        var user = await _fixture.RunAsAdministratorAsync();
        var expected = await _fixture.AddCompanyAsync(user.Id);

        var actual = await _fixture.SendAsync(new GetUpdateCompanyQuery(expected.Id));

        AssertFields(expected, actual, true, true, true);
        await AssertCompanyTypesInitDataAsync(actual);
        AssertManagerInitData(actual, new[] { user, new AspNetUser { Id = string.Empty } });
    }

    [Fact]
    public async Task User_has_no_claim___Forbidden()
    {
        var user = await _fixture.RunAsDefaultUserAsync();
        var company = await _fixture.AddCompanyAsync();
        await Assert.ThrowsAsync<ForbiddenAccessException>(() => _fixture.SendAsync(new GetUpdateCompanyQuery(company.Id)));
    }

    [Fact]
    public async Task User_has_claim_to_set_name_in_any_company___Includes_name()
    {
        await _fixture.RunAsDefaultUserAsync(new[] { Constants.Claims.Company.Any.Name.Set });
        var expected = await _fixture.AddCompanyAsync();

        var actual = await _fixture.SendAsync(new GetUpdateCompanyQuery(expected.Id));

        AssertFields(expected, actual, name: true);
        Assert.Empty(actual.InitData.CompanyTypes);
        Assert.Empty(actual.InitData.Managers);
    }

    [Theory]
    [InlineData(Constants.Claims.Company.Any.Name.Set)]
    [InlineData(Constants.Claims.Company.WhereUserIsManager.Name.Set)]
    public async Task User_has_claim_to_set_name_in_own_company___Includes_name(string claim)
    {
        var user = await _fixture.RunAsDefaultUserAsync(new[] { claim });
        var expected = await _fixture.AddCompanyAsync(user.Id);

        var actual = await _fixture.SendAsync(new GetUpdateCompanyQuery(expected.Id));

        AssertFields(expected, actual, name: true);
        Assert.Empty(actual.InitData.CompanyTypes);
        Assert.Empty(actual.InitData.Managers);
    }

    [Fact]
    public async Task User_has_claim_to_set_name_in_own_company_and_is_not_manager___Forbidden()
    {
        await _fixture.RunAsDefaultUserAsync(new[] { Constants.Claims.Company.WhereUserIsManager.Name.Set });
        var expected = await _fixture.AddCompanyAsync();

        var command = new GetUpdateCompanyQuery(expected.Id);
        await Assert.ThrowsAsync<ForbiddenAccessException>(() => _fixture.SendAsync(command));
    }

    [Fact]
    public async Task User_has_claim_to_set_other_fields_in_any_company___Includes_other_fields()
    {
        await _fixture.RunAsDefaultUserAsync(new[] { Constants.Claims.Company.Any.Other.Set });
        var expected = await _fixture.AddCompanyAsync();

        var actual = await _fixture.SendAsync(new GetUpdateCompanyQuery(expected.Id));

        AssertFields(expected, actual, other: true);
        await AssertCompanyTypesInitDataAsync(actual);
        Assert.Empty(actual.InitData.Managers);
    }

    [Theory]
    [InlineData(Constants.Claims.Company.Any.Other.Set)]
    [InlineData(Constants.Claims.Company.WhereUserIsManager.Other.Set)]
    public async Task User_has_claim_to_set_other_fields_in_own_company___Includes_other_fields(string claim)
    {
        var user = await _fixture.RunAsDefaultUserAsync(new[] { claim });
        var expected = await _fixture.AddCompanyAsync(user.Id);

        var actual = await _fixture.SendAsync(new GetUpdateCompanyQuery(expected.Id));

        AssertFields(expected, actual, other: true);
        await AssertCompanyTypesInitDataAsync(actual);
        Assert.Empty(actual.InitData.Managers);
    }

    [Fact]
    public async Task User_has_claim_to_set_other_fields_in_own_company_and_is_not_manager___Forbidden()
    {
        await _fixture.RunAsDefaultUserAsync(new[] { Constants.Claims.Company.WhereUserIsManager.Other.Set });
        var expected = await _fixture.AddCompanyAsync();

        var command = new GetUpdateCompanyQuery(expected.Id);
        await Assert.ThrowsAsync<ForbiddenAccessException>(() => _fixture.SendAsync(command));
    }

    private static void AssertFields(Company expected, UpdateCompanyVm actual, bool name = false, bool manager = false, bool other = false)
    {
        Assert.Equal(expected.Id, actual.Id);

        if (other)
        {
            Assert.Equal(expected.TypeId, actual.Fields[nameof(Company.TypeId)]);
            Assert.Equal(expected.Address, actual.Fields[nameof(Company.Address)]);
            Assert.Equal(expected.Ceo, actual.Fields[nameof(Company.Ceo)]);
            Assert.Equal(expected.Contacts, actual.Fields[nameof(Company.Contacts)]);
            Assert.Equal(expected.Email, actual.Fields[nameof(Company.Email)]);
            Assert.Equal(expected.Inn, actual.Fields[nameof(Company.Inn)]);
            Assert.Equal(expected.Phone, actual.Fields[nameof(Company.Phone)]);
        }
        else
        {
            Assert.False(actual.Fields.ContainsKey(nameof(Company.Address)));
            Assert.False(actual.Fields.ContainsKey(nameof(Company.Ceo)));
            Assert.False(actual.Fields.ContainsKey(nameof(Company.Contacts)));
            Assert.False(actual.Fields.ContainsKey(nameof(Company.Email)));
            Assert.False(actual.Fields.ContainsKey(nameof(Company.Inn)));
            Assert.False(actual.Fields.ContainsKey(nameof(Company.Phone)));
            Assert.False(actual.Fields.ContainsKey(nameof(Company.Type)));
        }

        if (manager)
        {
            Assert.Equal(expected.ManagerId, actual.Fields[nameof(Company.ManagerId)]);
        }
        else
        {
            Assert.False(actual.Fields.ContainsKey(nameof(Company.ManagerId)));
        }

        if (name)
        {
            Assert.Equal(expected.Name, actual.Fields[nameof(Company.Name)]);
        }
        else
        {
            Assert.False(actual.Fields.ContainsKey(nameof(Company.Name)));
        }
    }

    private static void AssertManagerInitData(UpdateCompanyVm actual, AspNetUser[] expected)
    {
        actual.InitData.Managers.Select(x => x.Id).Should().BeEquivalentTo(expected.Select(x => x.Id));
    }

    private async Task AssertCompanyTypesInitDataAsync(UpdateCompanyVm actual)
    {
        var expected = await _fixture.GetCompanyTypesAsync();
        actual.InitData.CompanyTypes.Select(x => x.Id).Should().BeEquivalentTo(expected.Select(x => x.Id));
    }
}
