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
        var expected = await _fixture.AddCompanyAsync();

        var actual = await _fixture.SendAsync(new GetUpdateCompanyQuery(expected.Id));

        AssertNameField(expected, actual);
        AssertOtherFields(expected, actual);
        AssertManagerField(expected, actual);
        await AssertCompanyTypesInitDataAsync(actual);
        AssertManagerInitData(actual, new[] { user, new AspNetUser { Id = string.Empty } });
    }

    private static void AssertNameField(Company expected, UpdateCompanyVm actual)
    {
        Assert.Equal(expected.Name, actual.Fields[nameof(Company.Name)]);
    }

    private static void AssertManagerField(Company expected, UpdateCompanyVm actual)
    {
        Assert.Equal(expected.ManagerId, actual.Fields[nameof(Company.ManagerId)]);
    }

    private static void AssertOtherFields(Company expected, UpdateCompanyVm actual)
    {
        Assert.Equal(expected.TypeId, actual.Fields[nameof(Company.TypeId)]);
        Assert.Equal(expected.Address, actual.Fields[nameof(Company.Address)]);
        Assert.Equal(expected.Ceo, actual.Fields[nameof(Company.Ceo)]);
        Assert.Equal(expected.Contacts, actual.Fields[nameof(Company.Contacts)]);
        Assert.Equal(expected.Email, actual.Fields[nameof(Company.Email)]);
        Assert.Equal(expected.Inn, actual.Fields[nameof(Company.Inn)]);
        Assert.Equal(expected.Phone, actual.Fields[nameof(Company.Phone)]);
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
