using CRM.Application.Common.Exceptions;
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
        await _fixture.RunAsAdministratorAsync();

        var actual = await _fixture.SendAsync(new GetNewCompanyQuery());

        AssertInitialFields(actual);
        AssertOtherFields(actual);
        AssertManagerField(actual);
    }

    [Fact]
    public async Task User_has_no_claim___Forbidden()
    {
        var user = await _fixture.RunAsDefaultUserAsync();

        await Assert.ThrowsAsync<ForbiddenAccessException>(() => _fixture.SendAsync(new GetNewCompanyQuery()));
    }

    [Fact]
    public async Task User_has_claim_to_create_company___Includes_initial_fields()
    {
        await _fixture.RunAsDefaultUserAsync(new[] { Constants.Claims.Company.Create });

        var actual = await _fixture.SendAsync(new GetNewCompanyQuery());

        AssertInitialFields(actual);
        AssertNoOtherFields(actual);
        AssertNoManagerField(actual);
    }

    [Fact]
    public async Task User_has_claim_to_set_other_fields___Includes_other_fields()
    {
        await _fixture.RunAsDefaultUserAsync(new[] { Constants.Claims.Company.New.SetOther });

        var actual = await _fixture.SendAsync(new GetNewCompanyQuery());

        AssertInitialFields(actual);
        AssertOtherFields(actual);
        AssertNoManagerField(actual);
    }

    [Fact]
    public async Task User_has_claim_to_set_manager___Includes_manager_field()
    {
        await _fixture.RunAsDefaultUserAsync(new[] { Constants.Claims.Company.New.SetManager });

        var actual = await _fixture.SendAsync(new GetNewCompanyQuery());

        AssertInitialFields(actual);
        AssertNoOtherFields(actual);
        AssertManagerField(actual);
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

    private static void AssertNoOtherFields(NewCompanyVm? actual)
    {
        Assert.False(actual?.Fields.ContainsKey(nameof(Company.Type)));
        Assert.False(actual?.Fields.ContainsKey(nameof(Company.Address)));
        Assert.False(actual?.Fields.ContainsKey(nameof(Company.Ceo)));
        Assert.False(actual?.Fields.ContainsKey(nameof(Company.Contacts)));
        Assert.False(actual?.Fields.ContainsKey(nameof(Company.Email)));
        Assert.False(actual?.Fields.ContainsKey(nameof(Company.Inn)));
        Assert.False(actual?.Fields.ContainsKey(nameof(Company.Phone)));
    }

    private static void AssertManagerField(NewCompanyVm? actual)
    {
        Assert.Null((actual?.Fields[nameof(Company.Manager)] as ManagerDto)?.Id);
    }

    private static void AssertNoManagerField(NewCompanyVm? actual)
    {
        Assert.False(actual?.Fields.ContainsKey(nameof(Company.Manager)));
    }

    private static void AssertInitialFields(NewCompanyVm? actual)
    {
        Assert.Null(actual?.Fields[nameof(Company.Name)]);
    }
}
