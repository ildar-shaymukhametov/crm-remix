using CRM.Application.Companies.Queries;
using CRM.Application.Companies.Queries.GetNewCompany;
using CRM.Domain.Entities;

namespace CRM.Application.IntegrationTests.Companies.Queries;

public class GetNewCompanyQueryTests : BaseTest
{
    public GetNewCompanyQueryTests(BaseTestFixture fixture) : base(fixture)
    {
    }

    [Fact]
    public async Task User_is_admin___Returns_all_fields()
    {
        var user = await _fixture.RunAsAdministratorAsync();
        var company = await _fixture.AddCompanyAsync(user.Id);

        var actual = await _fixture.SendAsync(new GetNewCompanyQuery());

        AssertInitialFields(actual);
        AssertOtherFields(actual);
        AssertManager(actual);
    }

    private static void AssertOtherFields(NewCompanyVm? actual)
    {
        Assert.Null((actual?.Fields[nameof(Company.Type)] as CompanyTypeDto)?.Id);
        Assert.Null(actual?.Fields[nameof(Company.Address)]);
        Assert.Null(actual?.Fields[nameof(Company.Ceo)]);
        Assert.Null(actual?.Fields[nameof(Company.Contacts)]);
        Assert.Null(actual?.Fields[nameof(Company.Email)]);
        Assert.Null(actual?.Fields[nameof(Company.Inn)]);
        Assert.Null(actual?.Fields[nameof(Company.Phone)]);
    }

    private static void AssertManager(NewCompanyVm? actual)
    {
        Assert.Null((actual?.Fields[nameof(Company.Manager)] as ManagerDto)?.Id);
    }

    private static void AssertInitialFields(NewCompanyVm? actual)
    {
        Assert.Null(actual?.Fields[nameof(Company.Name)]);
    }
}
