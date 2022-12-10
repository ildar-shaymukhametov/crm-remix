using CRM.Application;
using CRM.Application.Common.Exceptions;
using CRM.Application.Companies.Queries.GetCompanyManagers;
using CRM.Application.IntegrationTests;
using FluentAssertions;

namespace Application.IntegrationTests.Companies.Queries;

public class GetCompanyManagersQueryTests : BaseTest
{
    public GetCompanyManagersQueryTests(BaseTestFixture fixture) : base(fixture) { }

    [Theory]
    [InlineData(Constants.Claims.SetManagerToSelfFromNone)]
    [InlineData(Constants.Claims.SetManagerToAnyFromNone)]
    public async Task User_can_set_self_from_none___Returns_self(string claim)
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

    [Fact]
    public async Task User_can_set_any_from_none___Returns_users()
    {
        var user = await _fixture.RunAsDefaultUserAsync(Constants.Claims.SetManagerToAnyFromNone);
        var someUser = await _fixture.AddUserAsync();
        var company = Faker.Builders.Company();
        await _fixture.AddAsync(company);

        var query = new GetCompanyManagersQuery(company.Id);
        var result = await _fixture.SendAsync(query);

        var expected = new[] { user.Id, someUser.Id };
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
