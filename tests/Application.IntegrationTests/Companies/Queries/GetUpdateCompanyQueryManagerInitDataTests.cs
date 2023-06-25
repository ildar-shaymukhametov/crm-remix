using CRM.Application.Companies.Queries.GetUpdateCompany;
using FluentAssertions;
using static CRM.Application.Constants;

namespace CRM.Application.IntegrationTests.Companies.Queries;

public class GetUpdateCompanyQueryManagerInitDataTests : BaseTest
{
    public GetUpdateCompanyQueryManagerInitDataTests(BaseTestFixture fixture) : base(fixture)
    {
    }

    [Fact]
    public async Task User_is_admin___Returns_all_users()
    {
        var user = await _fixture.RunAsAdministratorAsync();
        var anotherUser = await _fixture.AddUserAsync();
        var company = await _fixture.AddCompanyAsync();

        var result = await _fixture.SendAsync(new GetUpdateCompanyQuery(company.Id));

        var expected = new[] { string.Empty, user.Id, anotherUser.Id };
        result.InitData.Managers.Select(x => x.Id).Should().BeEquivalentTo(expected);
    }

    [Fact]
    public async Task User_can_set_manager_from_none_to_self_in_any_company_with_no_manager___Returns_self_and_empty_manager()
    {
        var user = await _fixture.RunAsDefaultUserAsync(Claims.Company.Any.Manager.SetFromNoneToSelf);
        await _fixture.AddUserAsync();
        var company = await _fixture.AddCompanyAsync();

        var result = await _fixture.SendAsync(new GetUpdateCompanyQuery(company.Id));

        var expected = new[] { string.Empty, user.Id };
        result.InitData.Managers.Select(x => x.Id).Should().BeEquivalentTo(expected);
    }

    [Fact]
    public async Task User_can_set_manager_from_none_to_self_in_any_company_with_any_manager___Returns_empty_list()
    {
        await _fixture.RunAsDefaultUserAsync(Claims.Company.Any.Manager.SetFromNoneToSelf);
        var anotherUser = await _fixture.AddUserAsync();
        var company = await _fixture.AddCompanyAsync(anotherUser.Id);

        var result = await _fixture.SendAsync(new GetUpdateCompanyQuery(company.Id));

        Assert.Empty(result.InitData.Managers);
    }

    [Fact]
    public async Task User_can_set_manager_from_none_to_any_in_any_company_with_no_manager___Returns_all_users()
    {
        var user = await _fixture.RunAsDefaultUserAsync(Claims.Company.Any.Manager.SetFromNoneToAny);
        var anotherUser = await _fixture.AddUserAsync();
        var company = await _fixture.AddCompanyAsync();

        var result = await _fixture.SendAsync(new GetUpdateCompanyQuery(company.Id));

        var expected = new[] { string.Empty, user.Id, anotherUser.Id };
        result.InitData.Managers.Select(x => x.Id).Should().BeEquivalentTo(expected);
    }

    [Fact]
    public async Task User_can_set_manager_from_none_to_any_in_any_company_with_any_manager___Returns_empty_list()
    {
        await _fixture.RunAsDefaultUserAsync(Claims.Company.Any.Manager.SetFromNoneToAny);
        var anotherUser = await _fixture.AddUserAsync();
        var company = await _fixture.AddCompanyAsync(anotherUser.Id);

        var result = await _fixture.SendAsync(new GetUpdateCompanyQuery(company.Id));

        Assert.Empty(result.InitData.Managers);
    }

    [Fact]
    public async Task User_can_set_manager_from_any_to_none_in_any_company_with_self_as_manager___Returns_self_and_empty_manager()
    {
        var user = await _fixture.RunAsDefaultUserAsync(Claims.Company.Any.Manager.SetFromAnyToNone);
        await _fixture.AddUserAsync();
        var company = await _fixture.AddCompanyAsync(user.Id);

        var result = await _fixture.SendAsync(new GetUpdateCompanyQuery(company.Id));

        var expected = new[] { string.Empty, user.Id };
        result.InitData.Managers.Select(x => x.Id).Should().BeEquivalentTo(expected);
    }

    [Fact]
    public async Task User_can_set_manager_from_any_to_none_in_any_company_with_no_manager___Returns_empty_manager()
    {
        await _fixture.RunAsDefaultUserAsync(Claims.Company.Any.Manager.SetFromAnyToNone);
        await _fixture.AddUserAsync();
        var company = await _fixture.AddCompanyAsync();

        var result = await _fixture.SendAsync(new GetUpdateCompanyQuery(company.Id));

        var expected = new[] { string.Empty };
        result.InitData.Managers.Select(x => x.Id).Should().BeEquivalentTo(expected);
    }

    [Fact]
    public async Task User_can_set_manager_from_any_to_none_in_any_company_with_any_manager___Returns_current_manager_and_empty_manager()
    {
        await _fixture.RunAsDefaultUserAsync(Claims.Company.Any.Manager.SetFromAnyToNone);
        var anotherUser = await _fixture.AddUserAsync();
        var company = await _fixture.AddCompanyAsync(anotherUser.Id);

        var result = await _fixture.SendAsync(new GetUpdateCompanyQuery(company.Id));

        var expected = new[] { string.Empty, anotherUser.Id };
        result.InitData.Managers.Select(x => x.Id).Should().BeEquivalentTo(expected);
    }

    [Fact]
    public async Task User_can_set_manager_from_any_to_self_in_any_company_with_self_as_manager___Returns_self()
    {
        var user = await _fixture.RunAsDefaultUserAsync(Claims.Company.Any.Manager.SetFromAnyToSelf);
        await _fixture.AddUserAsync();
        var company = await _fixture.AddCompanyAsync(user.Id);

        var result = await _fixture.SendAsync(new GetUpdateCompanyQuery(company.Id));

        var expected = new[] { user.Id };
        result.InitData.Managers.Select(x => x.Id).Should().BeEquivalentTo(expected);
    }

    [Fact]
    public async Task User_can_set_manager_from_any_to_self_in_any_company_with_no_manager___Returns_empty_manager_and_self()
    {
        var user = await _fixture.RunAsDefaultUserAsync(Claims.Company.Any.Manager.SetFromAnyToSelf);
        await _fixture.AddUserAsync();
        var company = await _fixture.AddCompanyAsync();

        var result = await _fixture.SendAsync(new GetUpdateCompanyQuery(company.Id));

        var expected = new[] { user.Id, string.Empty };
        result.InitData.Managers.Select(x => x.Id).Should().BeEquivalentTo(expected);
    }

    [Fact]
    public async Task User_can_set_manager_from_any_to_self_in_any_company_with_any_manager___Returns_current_manager_and_self()
    {
        var user = await _fixture.RunAsDefaultUserAsync(Claims.Company.Any.Manager.SetFromAnyToSelf);
        var anotherUser = await _fixture.AddUserAsync();
        var company = await _fixture.AddCompanyAsync(anotherUser.Id);

        var result = await _fixture.SendAsync(new GetUpdateCompanyQuery(company.Id));

        var expected = new[] { user.Id, anotherUser.Id };
        result.InitData.Managers.Select(x => x.Id).Should().BeEquivalentTo(expected);
    }

    [Fact]
    public async Task User_can_set_manager_from_self_to_none_in_any_company_with_self_as_manager___Returns_empty_manager_and_self()
    {
        var user = await _fixture.RunAsDefaultUserAsync(Claims.Company.Any.Manager.SetFromSelfToNone);
        await _fixture.AddUserAsync();
        var company = await _fixture.AddCompanyAsync(user.Id);

        var result = await _fixture.SendAsync(new GetUpdateCompanyQuery(company.Id));

        var expected = new[] { string.Empty, user.Id };
        result.InitData.Managers.Select(x => x.Id).Should().BeEquivalentTo(expected);
    }

    [Fact]
    public async Task User_can_set_manager_from_self_to_none_in_any_company_with_any_manager___Returns_empty_list()
    {
        await _fixture.RunAsDefaultUserAsync(Claims.Company.Any.Manager.SetFromSelfToNone);
        var anotherUser = await _fixture.AddUserAsync();
        var company = await _fixture.AddCompanyAsync(anotherUser.Id);

        var result = await _fixture.SendAsync(new GetUpdateCompanyQuery(company.Id));

        Assert.Empty(result.InitData.Managers);
    }

    [Fact]
    public async Task User_can_set_manager_from_self_to_any_in_any_company_with_self_as_manager___Returns_all_users()
    {
        var user = await _fixture.RunAsDefaultUserAsync(Claims.Company.Any.Manager.SetFromSelfToAny);
        var anotherUser = await _fixture.AddUserAsync();
        var company = await _fixture.AddCompanyAsync(user.Id);

        var result = await _fixture.SendAsync(new GetUpdateCompanyQuery(company.Id));

        var expected = new[] { string.Empty, user.Id, anotherUser.Id };
        result.InitData.Managers.Select(x => x.Id).Should().BeEquivalentTo(expected);
    }

    [Fact]
    public async Task User_can_set_manager_from_self_to_any_in_any_company_with_any_manager___Returns_empty_list()
    {
        await _fixture.RunAsDefaultUserAsync(Claims.Company.Any.Manager.SetFromSelfToAny);
        var anotherUser = await _fixture.AddUserAsync();
        var company = await _fixture.AddCompanyAsync(anotherUser.Id);

        var result = await _fixture.SendAsync(new GetUpdateCompanyQuery(company.Id));

        Assert.Empty(result.InitData.Managers);
    }

    [Fact]
    public async Task User_can_set_manager_from_any_to_any_in_any_company_with_any_manager___Returns_all_users()
    {
        var user = await _fixture.RunAsDefaultUserAsync(Claims.Company.Any.Manager.SetFromAnyToAny);
        var anotherUser = await _fixture.AddUserAsync();
        var company = await _fixture.AddCompanyAsync(anotherUser.Id);

        var result = await _fixture.SendAsync(new GetUpdateCompanyQuery(company.Id));

        var expected = new[] { string.Empty, user.Id, anotherUser.Id };
        result.InitData.Managers.Select(x => x.Id).Should().BeEquivalentTo(expected);
    }

    [Fact]
    public async Task User_can_set_manager_from_any_to_any_in_any_company_with_no_manager___Returns_all_users()
    {
        var user = await _fixture.RunAsDefaultUserAsync(Claims.Company.Any.Manager.SetFromAnyToAny);
        var anotherUser = await _fixture.AddUserAsync();
        var company = await _fixture.AddCompanyAsync();

        var result = await _fixture.SendAsync(new GetUpdateCompanyQuery(company.Id));

        var expected = new[] { string.Empty, user.Id, anotherUser.Id };
        result.InitData.Managers.Select(x => x.Id).Should().BeEquivalentTo(expected);
    }

    [Fact]
    public async Task User_can_set_manager_from_any_to_any_in_any_company_with_self_as_manager___Returns_all_users()
    {
        var user = await _fixture.RunAsDefaultUserAsync(Claims.Company.Any.Manager.SetFromAnyToAny);
        var anotherUser = await _fixture.AddUserAsync();
        var company = await _fixture.AddCompanyAsync(user.Id);

        var result = await _fixture.SendAsync(new GetUpdateCompanyQuery(company.Id));

        var expected = new[] { string.Empty, user.Id, anotherUser.Id };
        result.InitData.Managers.Select(x => x.Id).Should().BeEquivalentTo(expected);
    }
}
