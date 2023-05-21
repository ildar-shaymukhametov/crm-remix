using CRM.Application.Companies.Queries.GetCompanyManagers;
using CRM.Application.IntegrationTests;
using static CRM.Application.Constants;

namespace Application.IntegrationTests.Companies.Queries;

public class GetCompanyManagersQueryTests : BaseTest
{
    public GetCompanyManagersQueryTests(BaseTestFixture fixture) : base(fixture) { }

    [Fact]
    public async Task User_is_admin___Returns_all_users()
    {
        var currentUser = await _fixture.RunAsAdministratorAsync("user", "1");

        var user3 = await _fixture.AddUserAsync("user", "3");
        var user2 = await _fixture.AddUserAsync("user", "2");

        var request = new GetCompanyInitDataQuery();
        var result = await _fixture.SendAsync(request);

        Assert.Collection(result.Managers,
            x => Assert.True(x.Id == null),
            x => Assert.True(x.Id == currentUser.Id && x.LastName == currentUser.ApplicationUser?.LastName),
            x => Assert.True(x.Id == user2.Id && x.LastName == user2.ApplicationUser?.LastName),
            x => Assert.True(x.Id == user3.Id && x.LastName == user3.ApplicationUser?.LastName)
        );
    }

    [Fact]
    public async Task User_can_set_manager_from_none_to_any_in_any_company___Returns_all_users()
    {
        var currentUser = await _fixture.RunAsDefaultUserAsync("user", "1", Claims.Company.Any.SetManagerFromNoneToAny);

        var user3 = await _fixture.AddUserAsync("user", "3");
        var user2 = await _fixture.AddUserAsync("user", "2");

        var query = new GetCompanyInitDataQuery();
        var result = await _fixture.SendAsync(query);

        Assert.Collection(result.Managers,
            x => Assert.True(x.Id == null),
            x => Assert.True(x.Id == currentUser.Id),
            x => Assert.True(x.Id == user2.Id),
            x => Assert.True(x.Id == user3.Id)
        );
    }

    [Fact]
    public async Task User_can_set_manager_from_none_to_self_in_any_company___Returns_self()
    {
        var currentUser = await _fixture.RunAsDefaultUserAsync(Claims.Company.Any.SetManagerFromNoneToSelf);
        var user2 = await _fixture.AddUserAsync("user", "2");

        var query = new GetCompanyInitDataQuery();
        var result = await _fixture.SendAsync(query);

        Assert.Collection(result.Managers,
            x => Assert.True(x.Id == currentUser.Id)
        );
    }

    [Fact]
    public async Task User_can_set_manager_to_none_in_new_company___Returns_empty_manager()
    {
        var currentUser = await _fixture.RunAsDefaultUserAsync(Claims.Company.New.SetManagerToNone);
        var user2 = await _fixture.AddUserAsync("user", "2");

        var query = new GetCompanyInitDataQuery();
        var result = await _fixture.SendAsync(query);

        Assert.Collection(result.Managers,
            x => Assert.True(x.Id == null)
        );
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
}
