using CRM.Application.Common.Exceptions;
using CRM.Application.Companies.Queries.GetCompanyManagers;
using CRM.Application.IntegrationTests;
using FluentAssertions;
using static CRM.Application.Constants;

namespace Application.IntegrationTests.Companies.Queries;

public class GetCompanyManagersQueryTests : BaseTest
{
    public GetCompanyManagersQueryTests(BaseTestFixture fixture) : base(fixture) { }

    [Fact]
    public async Task User_can_set_manager_from_any_to_any_in_any_company___Returns_all_users()
    {
        var currentUser = await _fixture.RunAsDefaultUserAsync(Claims.Company.Any.SetManagerFromAnyToAny);
        var anotherUser = await _fixture.AddUserAsync();

        var company = Faker.Builders.Company(managerId: anotherUser.Id);
        await _fixture.AddAsync(company);

        var query = new GetCompanyManagersQuery(company.Id);
        var result = await _fixture.SendAsync(query);

        var expected = new[] { currentUser.Id, anotherUser.Id, null };
        var actual = result.Managers.Select(x => x.Id);

        actual.Should().BeEquivalentTo(expected);
    }

    [Theory]
    [InlineData(Claims.Company.Any.SetManagerFromNoneToSelf)]
    [InlineData(Claims.Company.Any.SetManagerFromAnyToSelf)]
    public async Task User_can_set_manager_from_none_to_self_in_any_company_without_manager___Returns_self(string claim)
    {
        var currentUser = await _fixture.RunAsDefaultUserAsync(claim);

        var company = Faker.Builders.Company();
        await _fixture.AddAsync(company);

        var query = new GetCompanyManagersQuery(company.Id);
        var result = await _fixture.SendAsync(query);

        var expected = new[] { currentUser.Id };
        var actual = result.Managers.Select(x => x.Id);

        actual.Should().BeEquivalentTo(expected);
    }

    [Fact]
    public async Task User_can_set_manager_from_any_to_self_in_any_company_with_manager___Returns_self()
    {
        var currentUser = await _fixture.RunAsDefaultUserAsync(Claims.Company.Any.SetManagerFromAnyToSelf);
        var anotherUser = await _fixture.AddUserAsync();

        var company = Faker.Builders.Company(managerId: anotherUser.Id);
        await _fixture.AddAsync(company);

        var query = new GetCompanyManagersQuery(company.Id);
        var result = await _fixture.SendAsync(query);

        var expected = new[] { currentUser.Id };
        var actual = result.Managers.Select(x => x.Id);

        actual.Should().BeEquivalentTo(expected);
    }

    [Fact]
    public async Task User_has_no_claim___Returns_empty_list()
    {
        await _fixture.RunAsDefaultUserAsync();
        await _fixture.AddUserAsync();
        var company = Faker.Builders.Company();
        await _fixture.AddAsync(company);

        var query = new GetCompanyManagersQuery(company.Id);
        var result = await _fixture.SendAsync(query);

        Assert.Empty(result.Managers);
    }

    [Fact]
    public async Task Not_found()
    {
        var currentUser = await _fixture.RunAsDefaultUserAsync();
        var query = new GetCompanyManagersQuery(1);

        await Assert.ThrowsAsync<NotFoundException>(() => _fixture.SendAsync(query));
    }
}
