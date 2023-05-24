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
        var someUser = await _fixture.AddUserAsync();

        var request = new GetCompanyInitDataQuery();
        var result = await _fixture.SendAsync(request);

        var expected = new[] { string.Empty, currentUser.Id, someUser.Id };
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

    [Theory]
    [InlineData(Claims.Company.Old.Any.SetManagerFromAnyToAny)]
    [InlineData(Claims.Company.Old.Any.SetManagerFromNoneToAny)]
    [InlineData(Claims.Company.Old.Any.SetManagerFromSelfToAny)]
    public async Task User_can_set_manager_to_any_in_any_company___Returns_all_users(string claim)
    {
        var currentUser = await _fixture.RunAsDefaultUserAsync(claim);
        var someUser = await _fixture.AddUserAsync();

        var query = new GetCompanyInitDataQuery();
        var result = await _fixture.SendAsync(query);

        var expected = new[] { string.Empty, currentUser.Id, someUser.Id };
        result.Managers.Select(x => x.Id).Should().BeEquivalentTo(expected);
    }

    [Theory]
    [InlineData(Claims.Company.Old.Any.SetManagerFromNoneToSelf)]
    [InlineData(Claims.Company.Old.Any.SetManagerFromSelfToNone)]
    public async Task User_can_set_manager_from_none_to_self_or_vice_versa_in_any_company___Returns_self_and_empty_manager(string claim)
    {
        var currentUser = await _fixture.RunAsDefaultUserAsync(claim);
        var someUser = await _fixture.AddUserAsync();

        var query = new GetCompanyInitDataQuery();
        var result = await _fixture.SendAsync(query);

        var expected = new[] { string.Empty, currentUser.Id };
        result.Managers.Select(x => x.Id).Should().BeEquivalentTo(expected);
    }

    [Fact]
    public async Task User_can_set_manager_from_any_to_none_in_any_company___Returns_current_manager_and_empty_manager()
    {
        var currentUser = await _fixture.RunAsDefaultUserAsync(Claims.Company.Old.Any.SetManagerFromAnyToNone);
        var someUser = await _fixture.AddUserAsync();
        var company = await _fixture.AddCompanyAsync(someUser.Id);

        var query = new GetCompanyInitDataQuery { Id = company.Id };
        var result = await _fixture.SendAsync(query);

        var expected = new[] { string.Empty, someUser.Id };
        result.Managers.Select(x => x.Id).Should().BeEquivalentTo(expected);
    }

    [Fact]
    public async Task User_can_set_manager_from_any_to_self_in_any_company___Returns_current_manager_and_self()
    {
        var currentUser = await _fixture.RunAsDefaultUserAsync(Claims.Company.Old.Any.SetManagerFromAnyToSelf);
        var someUser = await _fixture.AddUserAsync();
        var company = await _fixture.AddCompanyAsync(someUser.Id);

        var query = new GetCompanyInitDataQuery { Id = company.Id };
        var result = await _fixture.SendAsync(query);

        var expected = new[] { string.Empty, currentUser.Id, someUser.Id };
        result.Managers.Select(x => x.Id).Should().BeEquivalentTo(expected);
    }
}
