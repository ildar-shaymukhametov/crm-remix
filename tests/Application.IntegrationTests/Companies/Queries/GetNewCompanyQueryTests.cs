using CRM.Application.Common.Exceptions;
using CRM.Application.Companies.Queries.GetNewCompany;
using CRM.Domain.Entities;
using CRM.Infrastructure.Identity;
using FluentAssertions;

namespace CRM.Application.IntegrationTests.Companies.Queries;

public class GetNewCompanyQueryTests : BaseTest
{
    public GetNewCompanyQueryTests(BaseTestFixture fixture) : base(fixture)
    {
    }

    [Fact]
    public async Task User_is_admin___Returns_all_fields_and_init_data()
    {
        var user = await _fixture.RunAsAdministratorAsync();

        var actual = await _fixture.SendAsync(new GetNewCompanyQuery());

        AssertRequiredFields(actual);
        AssertOtherFields(actual);
        AssertManagerField(actual);
        await AssertCompanyTypesInitDataAsync(actual);
        AssertManagerInitData(actual, new[] { user, new AspNetUser { Id = string.Empty } });
    }

    [Fact]
    public async Task User_has_no_claim___Forbidden()
    {
        var user = await _fixture.RunAsDefaultUserAsync();
        await Assert.ThrowsAsync<ForbiddenAccessException>(() => _fixture.SendAsync(new GetNewCompanyQuery()));
    }

    [Fact]
    public async Task User_has_claim_to_create_company___Includes_required_fields()
    {
        await _fixture.RunAsDefaultUserAsync(new[] { Constants.Claims.Company.Create });

        var actual = await _fixture.SendAsync(new GetNewCompanyQuery());

        AssertRequiredFields(actual);
        AssertNoOtherFields(actual);
        AssertNoManagerField(actual);
    }

    [Fact]
    public async Task User_has_claim_to_create_company___Does_not_return_any_init_data()
    {
        await _fixture.RunAsDefaultUserAsync(new[] { Constants.Claims.Company.Create });

        var actual = await _fixture.SendAsync(new GetNewCompanyQuery());

        Assert.Empty(actual.InitData.CompanyTypes);
        Assert.Empty(actual.InitData.Managers);
    }

    [Theory]
    [InlineData(Constants.Claims.Company.New.Other.Set)]
    [InlineData(Constants.Claims.Company.New.Manager.SetToAny)]
    [InlineData(Constants.Claims.Company.New.Manager.SetToNone)]
    [InlineData(Constants.Claims.Company.New.Manager.SetToSelf)]
    public async Task User_has_no_claim_to_create_company___Forbidden(string claim)
    {
        await _fixture.RunAsDefaultUserAsync(new[] { claim });
        await Assert.ThrowsAsync<ForbiddenAccessException>(() => _fixture.SendAsync(new GetNewCompanyQuery()));
    }

    [Fact]
    public async Task User_has_claim_to_set_other_fields___Includes_other_fields()
    {
        await _fixture.RunAsDefaultUserAsync(new[] {Constants.Claims.Company.Create, Constants.Claims.Company.New.Other.Set });

        var actual = await _fixture.SendAsync(new GetNewCompanyQuery());

        AssertRequiredFields(actual);
        AssertOtherFields(actual);
        AssertNoManagerField(actual);
    }

    [Fact]
    public async Task User_has_claim_to_set_other_fields___Returns_other_fields_init_data()
    {
        await _fixture.RunAsDefaultUserAsync(new[] {Constants.Claims.Company.Create, Constants.Claims.Company.New.Other.Set });
        var actual = await _fixture.SendAsync(new GetNewCompanyQuery());
        await AssertCompanyTypesInitDataAsync(actual);
    }

    [Fact]
    public async Task User_has_claim_to_set_other_fields___Does_not_return_unrelated_init_data()
    {
        await _fixture.RunAsDefaultUserAsync(new[] {Constants.Claims.Company.Create, Constants.Claims.Company.New.Other.Set });
        var actual = await _fixture.SendAsync(new GetNewCompanyQuery());
        Assert.Empty(actual.InitData.Managers);
    }

    [Theory]
    [InlineData(Constants.Claims.Company.New.Manager.SetToAny)]
    [InlineData(Constants.Claims.Company.New.Manager.SetToSelf)]
    public async Task User_has_claim_to_set_manager___Includes_manager_field(string claim)
    {
        await _fixture.RunAsDefaultUserAsync(new[] {Constants.Claims.Company.Create, claim });

        var actual = await _fixture.SendAsync(new GetNewCompanyQuery());

        AssertRequiredFields(actual);
        AssertNoOtherFields(actual);
        AssertManagerField(actual);
    }

    [Fact]
    public async Task User_has_claim_to_set_manager_to_self___Returns_self_as_init_data()
    {
        var user = await _fixture.RunAsDefaultUserAsync(new[] {Constants.Claims.Company.Create, Constants.Claims.Company.New.Manager.SetToSelf });
        var actual = await _fixture.SendAsync(new GetNewCompanyQuery());
        AssertManagerInitData(actual, new[] { user });
    }

    [Fact]
    public async Task User_has_claim_to_set_manager_to_self___Does_not_return_other_users()
    {
        var user = await _fixture.RunAsDefaultUserAsync(new[] {Constants.Claims.Company.Create, Constants.Claims.Company.New.Manager.SetToSelf });
        await _fixture.AddUserAsync();

        var actual = await _fixture.SendAsync(new GetNewCompanyQuery());

        AssertManagerInitData(actual, new[] { user });
    }

    [Fact]
    public async Task User_has_claim_to_set_manager_to_self___Does_not_return_unrelated_init_data()
    {
        await _fixture.RunAsDefaultUserAsync(new[] {Constants.Claims.Company.Create, Constants.Claims.Company.New.Manager.SetToSelf });
        var actual = await _fixture.SendAsync(new GetNewCompanyQuery());
        Assert.Empty(actual.InitData.CompanyTypes);
    }

    [Fact]
    public async Task User_has_claim_to_set_manager_to_any___Returns_all_users_as_init_data()
    {
        var user = await _fixture.RunAsDefaultUserAsync(new[] {Constants.Claims.Company.Create, Constants.Claims.Company.New.Manager.SetToAny });
        var anotherUser = await _fixture.AddUserAsync();

        var actual = await _fixture.SendAsync(new GetNewCompanyQuery());

        AssertManagerInitData(actual, new[] { user, anotherUser, new AspNetUser { Id = string.Empty } });
    }

    [Fact]
    public async Task User_has_claim_to_set_manager_to_any___Does_not_return_unrelated_init_data()
    {
        await _fixture.RunAsDefaultUserAsync(new[] { Constants.Claims.Company.Create, Constants.Claims.Company.New.Manager.SetToAny });
        var actual = await _fixture.SendAsync(new GetNewCompanyQuery());
        Assert.Empty(actual.InitData.CompanyTypes);
    }

    [Fact]
    public async Task User_has_claim_to_set_manager_to_none___Does_not_return_empty_manager_as_init_data()
    {
        await _fixture.RunAsDefaultUserAsync(new[]
        {
            Constants.Claims.Company.Create,
            Constants.Claims.Company.New.Manager.SetToNone
        });

        var actual = await _fixture.SendAsync(new GetNewCompanyQuery());
        AssertManagerInitData(actual, Array.Empty<AspNetUser>());
    }

    [Fact]
    public async Task User_has_claim_to_set_manager_to_none___Does_not_include_manager_field()
    {
        await _fixture.RunAsDefaultUserAsync(new[]
         {
            Constants.Claims.Company.Create,
            Constants.Claims.Company.New.Manager.SetToNone
        });

        var actual = await _fixture.SendAsync(new GetNewCompanyQuery());
        AssertNoManagerField(actual);
    }

    private static void AssertOtherFields(NewCompanyVm actual)
    {
        Assert.Null(actual.Fields[nameof(Company.TypeId)]);
        Assert.Null(actual.Fields[nameof(Company.Address)]);
        Assert.Null(actual.Fields[nameof(Company.Ceo)]);
        Assert.Null(actual.Fields[nameof(Company.Contacts)]);
        Assert.Null(actual.Fields[nameof(Company.Email)]);
        Assert.Null(actual.Fields[nameof(Company.Inn)]);
        Assert.Null(actual.Fields[nameof(Company.Phone)]);
    }

    private static void AssertNoOtherFields(NewCompanyVm actual)
    {
        Assert.False(actual.Fields.ContainsKey(nameof(Company.TypeId)));
        Assert.False(actual.Fields.ContainsKey(nameof(Company.Address)));
        Assert.False(actual.Fields.ContainsKey(nameof(Company.Ceo)));
        Assert.False(actual.Fields.ContainsKey(nameof(Company.Contacts)));
        Assert.False(actual.Fields.ContainsKey(nameof(Company.Email)));
        Assert.False(actual.Fields.ContainsKey(nameof(Company.Inn)));
        Assert.False(actual.Fields.ContainsKey(nameof(Company.Phone)));
    }

    private static void AssertManagerField(NewCompanyVm actual)
    {
        Assert.Null(actual.Fields[nameof(Company.ManagerId)]);
    }

    private static void AssertNoManagerField(NewCompanyVm actual)
    {
        Assert.False(actual.Fields.ContainsKey(nameof(Company.ManagerId)));
    }

    private static void AssertRequiredFields(NewCompanyVm actual)
    {
        Assert.Null(actual.Fields[nameof(Company.Name)]);
    }

    private static void AssertManagerInitData(NewCompanyVm actual, AspNetUser[] expected)
    {
        actual.InitData.Managers.Select(x => x.Id).Should().BeEquivalentTo(expected.Select(x => x.Id));
    }

    private async Task AssertCompanyTypesInitDataAsync(NewCompanyVm actual)
    {
        var expected = await _fixture.GetCompanyTypesAsync();
        actual.InitData.CompanyTypes.Select(x => x.Id).Should().BeEquivalentTo(expected.Select(x => x.Id));
    }
}
