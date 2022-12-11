using CRM.Application.Common.Exceptions;
using CRM.Application.Companies.Queries.GetCompanyManagers;
using CRM.Application.IntegrationTests;
using FluentAssertions;
using static CRM.Application.Constants;

namespace Application.IntegrationTests.Companies.Queries;

public class GetCompanyManagersQueryTests : BaseTest
{
    public GetCompanyManagersQueryTests(BaseTestFixture fixture) : base(fixture) { }

    // [Fact]
    // public async Task User_can_set_manager_from_any_to_any_in_any_company___Returns_managers()
    // {
    //     var user = await _fixture.RunAsDefaultUserAsync(Claims.Company.Any.Manager.Any.Set.Any);
    //     var someUser = await _fixture.AddUserAsync();

    //     var company = Faker.Builders.Company(managerId: someUser.Id);
    //     await _fixture.AddAsync(company);

    //     var query = new GetCompanyManagersQuery(company.Id);
    //     var result = await _fixture.SendAsync(query);

    //     var expected = new[] { user.Id, someUser.Id };
    //     var actual = result.Managers.Select(x => x.Id);

    //     actual.Should().BeEquivalentTo(expected);
    // }

    [Theory]
    [InlineData(Claims.Company.Any.Manager.None.Set.Self)]
    [InlineData(Claims.Company.Any.Manager.Any.Set.Any)]
    [InlineData(Claims.Company.Any.Manager.Any.Set.Self)]
    public async Task User_can_set_manager_from_none_to_self_in_any_company___Returns_self(string claim)
    {
        var user = await _fixture.RunAsDefaultUserAsync(claim);

        var company = Faker.Builders.Company();
        await _fixture.AddAsync(company);

        var query = new GetCompanyManagersQuery(company.Id);
        var result = await _fixture.SendAsync(query);

        var expected = new[] { user.Id };
        var actual = result.Managers.Select(x => x.Id);

        actual.Should().BeEquivalentTo(expected);
    }

    [Theory]
    [InlineData(Claims.Company.Any.Manager.Any.Set.Self)]
    [InlineData(Claims.Company.Any.Manager.Any.Set.Any)]
    public async Task User_can_set_manager_from_any_to_self_in_any_company___Returns_self(string claim)
    {
        var user = await _fixture.RunAsDefaultUserAsync(claim);
        var someUser = await _fixture.AddUserAsync();

        var company = Faker.Builders.Company(managerId: someUser.Id);
        await _fixture.AddAsync(company);

        var query = new GetCompanyManagersQuery(company.Id);
        var result = await _fixture.SendAsync(query);

        var expected = new[] { user.Id };
        var actual = result.Managers.Select(x => x.Id);

        actual.Should().BeEquivalentTo(expected);
    }

    [Fact]
    public async Task User_has_no_claim___Returns_empty_list()
    {
        var user = await _fixture.RunAsDefaultUserAsync();
        var someUser = await _fixture.AddUserAsync();
        var company = Faker.Builders.Company();
        await _fixture.AddAsync(company);

        var query = new GetCompanyManagersQuery(company.Id);
        var result = await _fixture.SendAsync(query);
        var actual = result.Managers.Select(x => x.Id);

        Assert.Empty(actual);
    }

    [Fact]
    public async Task Not_found()
    {
        var user = await _fixture.RunAsDefaultUserAsync();
        var query = new GetCompanyManagersQuery(1);

        await Assert.ThrowsAsync<NotFoundException>(() => _fixture.SendAsync(query));
    }
}
