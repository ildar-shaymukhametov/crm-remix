using CRM.Application.Companies.Queries.GetCompanyManagers;
using CRM.Application.IntegrationTests;
using FluentAssertions;
using static CRM.Application.Constants;

namespace Application.IntegrationTests.Companies.Queries;

public class GetCompanyManagersQueryTests : BaseTest
{
    public GetCompanyManagersQueryTests(BaseTestFixture fixture) : base(fixture) { }

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

    // [Theory]
    // [InlineData(Claims.Company.Old.Any.SetManagerFromAnyToAny)]
    // [InlineData(Claims.Company.Old.Any.SetManagerFromNoneToAny)]
    // [InlineData(Claims.Company.Old.Any.SetManagerFromSelfToAny)]
    // public async Task User_can_set_manager_to_any_in_any_company___Returns_all_users(string claim)
    // {
    //     var currentUser = await _fixture.RunAsDefaultUserAsync(claim);
    //     var anyUser = await _fixture.AddUserAsync();
    //     var company = await _fixture.AddCompanyAsync(anyUser.Id);

    //     var query = new GetCompanyInitDataQuery { Id = company.Id };
    //     var result = await _fixture.SendAsync(query);

    //     var expected = new[] { string.Empty, currentUser.Id, anyUser.Id };
    //     result.Managers.Select(x => x.Id).Should().BeEquivalentTo(expected);
    // }

    [Theory]
    [InlineData(Claims.Company.Old.Any.SetManagerFromNoneToSelf)]
    [InlineData(Claims.Company.Old.Any.SetManagerFromAnyToSelf)]
    public async Task User_can_set_manager_from_none_to_self_in_any_company___Returns_self_and_empty_manager(string claim)
    {
        var currentUser = await _fixture.RunAsDefaultUserAsync(claim);
        var anyUser = await _fixture.AddUserAsync();
        var company = await _fixture.AddCompanyAsync();

        var query = new GetCompanyInitDataQuery { Id = company.Id };
        var result = await _fixture.SendAsync(query);

        var expected = new[] { string.Empty, currentUser.Id };
        result.Managers.Select(x => x.Id).Should().BeEquivalentTo(expected);
    }

    [Fact]
    public async Task User_can_set_manager_from_any_to_none_in_any_company___Returns_current_manager_and_empty_manager()
    {
        var currentUser = await _fixture.RunAsDefaultUserAsync(Claims.Company.Old.Any.SetManagerFromAnyToNone);
        var currentManager = await _fixture.AddUserAsync();
        var company = await _fixture.AddCompanyAsync(currentManager.Id);

        var query = new GetCompanyInitDataQuery { Id = company.Id };
        var result = await _fixture.SendAsync(query);

        var expected = new[] { string.Empty, currentManager.Id };
        result.Managers.Select(x => x.Id).Should().BeEquivalentTo(expected);
    }

    [Fact]
    public async Task User_can_set_manager_from_any_to_self_in_any_company___Returns_current_manager_and_self()
    {
        var currentUser = await _fixture.RunAsDefaultUserAsync(Claims.Company.Old.Any.SetManagerFromAnyToSelf);
        var currentManager = await _fixture.AddUserAsync();
        var company = await _fixture.AddCompanyAsync(currentManager.Id);

        var query = new GetCompanyInitDataQuery { Id = company.Id };
        var result = await _fixture.SendAsync(query);

        var expected = new[] { currentUser.Id, currentManager.Id };
        result.Managers.Select(x => x.Id).Should().BeEquivalentTo(expected);
    }

    [Theory]
    [InlineData(Claims.Company.Old.Any.SetManagerFromSelfToNone)]
    [InlineData(Claims.Company.Old.Any.SetManagerFromAnyToNone)]
    public async Task User_can_set_manager_from_self_to_none_in_any_company___Returns_empty_manager_and_self(string claim)
    {
        var currentUser = await _fixture.RunAsDefaultUserAsync(claim);
        await _fixture.AddUserAsync();
        var company = await _fixture.AddCompanyAsync(currentUser.Id);

        var query = new GetCompanyInitDataQuery { Id = company.Id };
        var result = await _fixture.SendAsync(query);

        var expected = new[] { string.Empty, currentUser.Id };
        result.Managers.Select(x => x.Id).Should().BeEquivalentTo(expected);
    }

    [Theory]
    [InlineData(Claims.Company.Old.Any.SetManagerFromAnyToAny)]
    [InlineData(Claims.Company.Old.Any.SetManagerFromSelfToAny)]
    public async Task User_can_set_manager_from_self_to_none_in_any_company___Contains_empty_manager_and_self(string claim)
    {
        var currentUser = await _fixture.RunAsDefaultUserAsync(claim);
        var anyUser = await _fixture.AddUserAsync();
        var company = await _fixture.AddCompanyAsync(currentUser.Id);

        var query = new GetCompanyInitDataQuery { Id = company.Id };
        var result = await _fixture.SendAsync(query);

        var expected = new[] { string.Empty, currentUser.Id, anyUser.Id };
        result.Managers.Select(x => x.Id).Should().BeEquivalentTo(expected);
    }

    [Theory]
    [InlineData(Claims.Company.Old.Any.SetManagerFromSelfToAny)]
    [InlineData(Claims.Company.Old.Any.SetManagerFromAnyToAny)]
    public async Task User_can_set_manager_from_self_to_any_in_any_company___Contains_any_manager_and_self(string claim)
    {
        var currentUser = await _fixture.RunAsDefaultUserAsync(claim);
        var anyUser = await _fixture.AddUserAsync();
        var company = await _fixture.AddCompanyAsync(currentUser.Id);

        var query = new GetCompanyInitDataQuery { Id = company.Id };
        var result = await _fixture.SendAsync(query);

        var expected = new[] { string.Empty, currentUser.Id, anyUser.Id };
        result.Managers.Select(x => x.Id).Should().BeEquivalentTo(expected);
    }

    [Theory]
    [InlineData(Claims.Company.Old.Any.SetManagerFromAnyToAny)]
    public async Task User_can_set_manager_from_any_to_any_in_any_company___Contains_both_current_and_new_manager(string claim)
    {
        var currentUser = await _fixture.RunAsDefaultUserAsync(claim);
        var currentManager = await _fixture.AddUserAsync();
        var anyUser = await _fixture.AddUserAsync();
        var company = await _fixture.AddCompanyAsync(currentManager.Id);

        var query = new GetCompanyInitDataQuery { Id = company.Id };
        var result = await _fixture.SendAsync(query);

        var expected = new[] { string.Empty, currentUser.Id, currentManager.Id, anyUser.Id };
        result.Managers.Select(x => x.Id).Should().BeEquivalentTo(expected);
    }
}
