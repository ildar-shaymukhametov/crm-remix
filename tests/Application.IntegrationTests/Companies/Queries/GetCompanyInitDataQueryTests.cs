using CRM.Application.Companies.Queries.GetCompanyManagers;
using FluentAssertions;
using static CRM.Application.Constants;

namespace CRM.Application.IntegrationTests.Companies.Queries;

public class GetCompanyInitDataQueryTests : BaseTest
{
    public GetCompanyInitDataQueryTests(BaseTestFixture fixture) : base(fixture) { }

    [Fact]
    public async Task User_is_admin___Returns_all_users()
    {
        var currentUser = await _fixture.RunAsAdministratorAsync();
        var anyUser = await _fixture.AddUserAsync();

        var request = new GetCompanyInitDataQuery();
        var result = await _fixture.SendAsync(request);

        var expected = new[] { string.Empty, currentUser.Id, anyUser.Id };
        result.Managers.Select(x => x.Id).Should().BeEquivalentTo(expected);
    }

    [Fact]
    public async Task User_has_no_claim___Returns_empty_list()
    {
        await _fixture.RunAsDefaultUserAsync();
        await _fixture.AddUserAsync();

        var query = new GetCompanyInitDataQuery();
        var result = await _fixture.SendAsync(query);

        Assert.Empty(result.Managers);
    }

    [Fact]
    public async Task User_can_set_manager_from_none_to_self_in_any_company_with_self_as_manager___Returns_empty_list()
    {
        var currentUser = await _fixture.RunAsDefaultUserAsync(Claims.Company.Any.Manager.SetFromNoneToSelf);
        await _fixture.AddUserAsync();
        var company = await _fixture.AddCompanyAsync(currentUser.Id);

        var query = new GetCompanyInitDataQuery { Id = company.Id };
        var result = await _fixture.SendAsync(query);

        Assert.Empty(result.Managers);
    }

    [Fact]
    public async Task User_can_set_manager_from_none_to_self_in_any_company_with_no_manager___Returns_self_and_empty_manager()
    {
        var currentUser = await _fixture.RunAsDefaultUserAsync(Claims.Company.Any.Manager.SetFromNoneToSelf);
        await _fixture.AddUserAsync();
        var company = await _fixture.AddCompanyAsync();

        var query = new GetCompanyInitDataQuery { Id = company.Id };
        var result = await _fixture.SendAsync(query);

        var expected = new[] { string.Empty, currentUser.Id };
        result.Managers.Select(x => x.Id).Should().BeEquivalentTo(expected);
    }

    [Fact]
    public async Task User_can_set_manager_from_none_to_self_in_any_company_with_any_manager___Returns_empty_list()
    {
        await _fixture.RunAsDefaultUserAsync(Claims.Company.Any.Manager.SetFromNoneToSelf);
        var anyUser = await _fixture.AddUserAsync();
        var company = await _fixture.AddCompanyAsync(anyUser.Id);

        var query = new GetCompanyInitDataQuery { Id = company.Id };
        var result = await _fixture.SendAsync(query);

        Assert.Empty(result.Managers);
    }

    [Fact]
    public async Task User_can_set_manager_from_none_to_any_in_any_company_with_self_as_manager___Returns_empty_list()
    {
        var currentUser = await _fixture.RunAsDefaultUserAsync(Claims.Company.Any.Manager.SetFromNoneToAny);
        await _fixture.AddUserAsync();
        var company = await _fixture.AddCompanyAsync(currentUser.Id);

        var query = new GetCompanyInitDataQuery { Id = company.Id };
        var result = await _fixture.SendAsync(query);

        Assert.Empty(result.Managers);
    }

    [Fact]
    public async Task User_can_set_manager_from_none_to_any_in_any_company_with_no_manager___Returns_all_users()
    {
        var currentUser = await _fixture.RunAsDefaultUserAsync(Claims.Company.Any.Manager.SetFromNoneToAny);
        var anyUser = await _fixture.AddUserAsync();
        var company = await _fixture.AddCompanyAsync();

        var query = new GetCompanyInitDataQuery { Id = company.Id };
        var result = await _fixture.SendAsync(query);

        var expected = new[] { string.Empty, currentUser.Id, anyUser.Id };
        result.Managers.Select(x => x.Id).Should().BeEquivalentTo(expected);
    }

    [Fact]
    public async Task User_can_set_manager_from_none_to_any_in_any_company_with_any_manager___Returns_empty_list()
    {
        await _fixture.RunAsDefaultUserAsync(Claims.Company.Any.Manager.SetFromNoneToAny);
        var anyUser = await _fixture.AddUserAsync();
        var company = await _fixture.AddCompanyAsync(anyUser.Id);

        var query = new GetCompanyInitDataQuery { Id = company.Id };
        var result = await _fixture.SendAsync(query);

        Assert.Empty(result.Managers);
    }

    [Fact]
    public async Task User_can_set_manager_from_any_to_none_in_any_company_with_self_as_manager___Returns_self_and_empty_manager()
    {
        var currentUser = await _fixture.RunAsDefaultUserAsync(Claims.Company.Any.Manager.SetFromAnyToNone);
        await _fixture.AddUserAsync();
        var company = await _fixture.AddCompanyAsync(currentUser.Id);

        var query = new GetCompanyInitDataQuery { Id = company.Id };
        var result = await _fixture.SendAsync(query);

        var expected = new[] { string.Empty, currentUser.Id };
        result.Managers.Select(x => x.Id).Should().BeEquivalentTo(expected);
    }

    [Fact]
    public async Task User_can_set_manager_from_any_to_none_in_any_company_with_no_manager___Returns_empty_manager()
    {
        await _fixture.RunAsDefaultUserAsync(Claims.Company.Any.Manager.SetFromAnyToNone);
        await _fixture.AddUserAsync();
        var company = await _fixture.AddCompanyAsync();

        var query = new GetCompanyInitDataQuery { Id = company.Id };
        var result = await _fixture.SendAsync(query);

        var expected = new[] { string.Empty };
        result.Managers.Select(x => x.Id).Should().BeEquivalentTo(expected);
    }

    [Fact]
    public async Task User_can_set_manager_from_any_to_none_in_any_company_with_any_manager___Returns_current_manager_and_empty_manager()
    {
        await _fixture.RunAsDefaultUserAsync(Claims.Company.Any.Manager.SetFromAnyToNone);
        var anyUser = await _fixture.AddUserAsync();
        var company = await _fixture.AddCompanyAsync(anyUser.Id);

        var query = new GetCompanyInitDataQuery { Id = company.Id };
        var result = await _fixture.SendAsync(query);

        var expected = new[] { string.Empty, anyUser.Id };
        result.Managers.Select(x => x.Id).Should().BeEquivalentTo(expected);
    }

    [Fact]
    public async Task User_can_set_manager_from_any_to_self_in_any_company_with_self_as_manager___Returns_self()
    {
        var currentUser = await _fixture.RunAsDefaultUserAsync(Claims.Company.Any.Manager.SetFromAnyToSelf);
        await _fixture.AddUserAsync();
        var company = await _fixture.AddCompanyAsync(currentUser.Id);

        var query = new GetCompanyInitDataQuery { Id = company.Id };
        var result = await _fixture.SendAsync(query);

        var expected = new[] { currentUser.Id };
        result.Managers.Select(x => x.Id).Should().BeEquivalentTo(expected);
    }

    [Fact]
    public async Task User_can_set_manager_from_any_to_self_in_any_company_with_no_manager___Returns_empty_manager_and_self()
    {
        var currentUser = await _fixture.RunAsDefaultUserAsync(Claims.Company.Any.Manager.SetFromAnyToSelf);
        await _fixture.AddUserAsync();
        var company = await _fixture.AddCompanyAsync();

        var query = new GetCompanyInitDataQuery { Id = company.Id };
        var result = await _fixture.SendAsync(query);

        var expected = new[] { currentUser.Id, string.Empty };
        result.Managers.Select(x => x.Id).Should().BeEquivalentTo(expected);
    }

    [Fact]
    public async Task User_can_set_manager_from_any_to_self_in_any_company_with_any_manager___Returns_current_manager_and_self()
    {
        var currentUser = await _fixture.RunAsDefaultUserAsync(Claims.Company.Any.Manager.SetFromAnyToSelf);
        var anyUser = await _fixture.AddUserAsync();
        var company = await _fixture.AddCompanyAsync(anyUser.Id);

        var query = new GetCompanyInitDataQuery { Id = company.Id };
        var result = await _fixture.SendAsync(query);

        var expected = new[] { currentUser.Id, anyUser.Id };
        result.Managers.Select(x => x.Id).Should().BeEquivalentTo(expected);
    }

    [Fact]
    public async Task User_can_set_manager_from_self_to_none_in_any_company_with_self_as_manager___Returns_empty_manager_and_self()
    {
        var currentUser = await _fixture.RunAsDefaultUserAsync(Claims.Company.Any.Manager.SetFromSelfToNone);
        await _fixture.AddUserAsync();
        var company = await _fixture.AddCompanyAsync(currentUser.Id);

        var query = new GetCompanyInitDataQuery { Id = company.Id };
        var result = await _fixture.SendAsync(query);

        var expected = new[] { string.Empty, currentUser.Id };
        result.Managers.Select(x => x.Id).Should().BeEquivalentTo(expected);
    }

    [Fact]
    public async Task User_can_set_manager_from_self_to_none_in_any_company_with_no_manager___Returns_empty_list()
    {
        await _fixture.RunAsDefaultUserAsync(Claims.Company.Any.Manager.SetFromSelfToNone);
        await _fixture.AddUserAsync();
        var company = await _fixture.AddCompanyAsync();

        var query = new GetCompanyInitDataQuery { Id = company.Id };
        var result = await _fixture.SendAsync(query);

        Assert.Empty(result.Managers);
    }

    [Fact]
    public async Task User_can_set_manager_from_self_to_none_in_any_company_with_any_manager___Returns_empty_list()
    {
        await _fixture.RunAsDefaultUserAsync(Claims.Company.Any.Manager.SetFromSelfToNone);
        var anyUser = await _fixture.AddUserAsync();
        var company = await _fixture.AddCompanyAsync(anyUser.Id);

        var query = new GetCompanyInitDataQuery { Id = company.Id };
        var result = await _fixture.SendAsync(query);

        Assert.Empty(result.Managers);
    }

    [Fact]
    public async Task User_can_set_manager_from_self_to_any_in_any_company_with_self_as_manager___Returns_all_users()
    {
        var currentUser = await _fixture.RunAsDefaultUserAsync(Claims.Company.Any.Manager.SetFromSelfToAny);
        var anyUser = await _fixture.AddUserAsync();
        var company = await _fixture.AddCompanyAsync(currentUser.Id);

        var query = new GetCompanyInitDataQuery { Id = company.Id };
        var result = await _fixture.SendAsync(query);

        var expected = new[] { string.Empty, currentUser.Id, anyUser.Id };
        result.Managers.Select(x => x.Id).Should().BeEquivalentTo(expected);
    }

    [Fact]
    public async Task User_can_set_manager_from_self_to_any_in_any_company_with_no_manager___Returns_empty_list()
    {
        await _fixture.RunAsDefaultUserAsync(Claims.Company.Any.Manager.SetFromSelfToAny);
        await _fixture.AddUserAsync();
        var company = await _fixture.AddCompanyAsync();

        var query = new GetCompanyInitDataQuery { Id = company.Id };
        var result = await _fixture.SendAsync(query);

        Assert.Empty(result.Managers);
    }

    [Fact]
    public async Task User_can_set_manager_from_self_to_any_in_any_company_with_any_manager___Returns_empty_list()
    {
        await _fixture.RunAsDefaultUserAsync(Claims.Company.Any.Manager.SetFromSelfToAny);
        var anyUser = await _fixture.AddUserAsync();
        var company = await _fixture.AddCompanyAsync(anyUser.Id);

        var query = new GetCompanyInitDataQuery { Id = company.Id };
        var result = await _fixture.SendAsync(query);

        Assert.Empty(result.Managers);
    }

    [Fact]
    public async Task User_can_set_manager_from_any_to_any_in_any_company_with_any_manager___Returns_all_users()
    {
        var currentUser = await _fixture.RunAsDefaultUserAsync(Claims.Company.Any.Manager.SetFromAnyToAny);
        var anyUser = await _fixture.AddUserAsync();
        var company = await _fixture.AddCompanyAsync(anyUser.Id);

        var query = new GetCompanyInitDataQuery { Id = company.Id };
        var result = await _fixture.SendAsync(query);

        var expected = new[] { string.Empty, currentUser.Id, anyUser.Id };
        result.Managers.Select(x => x.Id).Should().BeEquivalentTo(expected);
    }

    [Fact]
    public async Task User_can_set_manager_from_any_to_any_in_any_company_with_no_manager___Returns_all_users()
    {
        var currentUser = await _fixture.RunAsDefaultUserAsync(Claims.Company.Any.Manager.SetFromAnyToAny);
        var anyUser = await _fixture.AddUserAsync();
        var company = await _fixture.AddCompanyAsync();

        var query = new GetCompanyInitDataQuery { Id = company.Id };
        var result = await _fixture.SendAsync(query);

        var expected = new[] { string.Empty, currentUser.Id, anyUser.Id };
        result.Managers.Select(x => x.Id).Should().BeEquivalentTo(expected);
    }

    [Fact]
    public async Task User_can_set_manager_from_any_to_any_in_any_company_with_self_as_manager___Returns_all_users()
    {
        var currentUser = await _fixture.RunAsDefaultUserAsync(Claims.Company.Any.Manager.SetFromAnyToAny);
        var anyUser = await _fixture.AddUserAsync();
        var company = await _fixture.AddCompanyAsync(currentUser.Id);

        var query = new GetCompanyInitDataQuery { Id = company.Id };
        var result = await _fixture.SendAsync(query);

        var expected = new[] { string.Empty, currentUser.Id, anyUser.Id };
        result.Managers.Select(x => x.Id).Should().BeEquivalentTo(expected);
    }
}
