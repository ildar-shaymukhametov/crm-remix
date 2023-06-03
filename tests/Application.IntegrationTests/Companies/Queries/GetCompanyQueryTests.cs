using CRM.Application.Common.Exceptions;
using CRM.Application.Companies.Queries.GetCompany;
using CRM.Domain.Entities;

namespace CRM.Application.IntegrationTests.Companies.Queries;

public class GetCompanyTests : BaseTest
{
    public GetCompanyTests(BaseTestFixture fixture) : base(fixture) { }

    [Fact]
    public async Task User_is_admin___Returns_all_fields()
    {
        var user = await _fixture.RunAsAdministratorAsync();
        var newCompany = Faker.Builders.Company(managerId: user.Id);
        await _fixture.AddAsync(newCompany);

        var request = new GetCompanyQuery { Id = newCompany.Id };
        var result = await _fixture.SendAsync(request);
        var company = await _fixture.FindAsync<Company>(result.Id, nameof(Company.Type));

        Assert.Equal(company?.Id, result?.Id);
        AssertOtherFieldsEqual(company, result);
        AssertManagerEqual(company, result);
    }

    [Fact]
    public async Task User_has_claim_to_view_any_company___Returns_all_fields()
    {
        var user = await _fixture.RunAsDefaultUserAsync(new[] { Constants.Claims.Company.Any.View });

        var company = Faker.Builders.Company(managerId: user.Id);
        await _fixture.AddAsync(company);

        var request = new GetCompanyQuery { Id = company.Id };
        var result = await _fixture.SendAsync(request);

        Assert.Equal(company?.Id, result?.Id);
        AssertOtherFieldsEqual(company, result);
        AssertManagerEqual(company, result);
    }

    [Fact]
    public async Task User_has_no_claim___Throws_forbidden_access()
    {
        var user = await _fixture.RunAsDefaultUserAsync();

        var company = Faker.Builders.Company();
        await _fixture.AddAsync(company);

        var request = new GetCompanyQuery { Id = company.Id };
        await Assert.ThrowsAsync<ForbiddenAccessException>(() => _fixture.SendAsync(request));
    }

    [Fact]
    public async Task User_has_claim_to_view_other_fields_in_any_company___Returns_other_fields()
    {
        await _fixture.RunAsDefaultUserAsync(new[] { Constants.Claims.Company.Any.Other.View });

        var company = Faker.Builders.Company();
        await _fixture.AddAsync(company);

        var request = new GetCompanyQuery { Id = company.Id };
        var result = await _fixture.SendAsync(request);

        Assert.Equal(company?.Id, result?.Id);
        AssertOtherFieldsEqual(company, result);
    }

    [Fact]
    public async Task User_has_claim_to_view_other_fields_in_any_company___Does_not_return_manager()
    {
        await _fixture.RunAsDefaultUserAsync(new[] { Constants.Claims.Company.Any.Other.View });

        var company = Faker.Builders.Company();
        await _fixture.AddAsync(company);

        var request = new GetCompanyQuery { Id = company.Id };
        var result = await _fixture.SendAsync(request);

        AssertNoManager(result);
    }

    [Fact]
    public async Task User_has_claim_to_view_manager_in_any_company___Returns_manager()
    {
        var user = await _fixture.RunAsDefaultUserAsync(new[] { Constants.Claims.Company.Any.Manager.View });

        var company = Faker.Builders.Company(user.Id);
        await _fixture.AddAsync(company);

        var request = new GetCompanyQuery { Id = company.Id };
        var result = await _fixture.SendAsync(request);

        Assert.Equal(company?.Id, result?.Id);
        AssertManagerEqual(company, result);
    }

    [Fact]
    public async Task User_has_claim_to_view_manager_in_any_company___Does_not_return_other_fields()
    {
        await _fixture.RunAsDefaultUserAsync(new[] { Constants.Claims.Company.Any.Manager.View });

        var company = Faker.Builders.Company();
        await _fixture.AddAsync(company);

        var request = new GetCompanyQuery { Id = company.Id };
        var result = await _fixture.SendAsync(request);

        AssertNoOtherFields(result);
    }

    [Theory]
    [InlineData(Constants.Claims.Company.Any.Delete)]
    [InlineData(Constants.Claims.Company.Any.Update)]
    public async Task User_has_certain_claim___Returns_only_id(string claim)
    {
        var user = await _fixture.RunAsDefaultUserAsync(new[] { claim });

        var company = Faker.Builders.Company(managerId: user.Id);
        await _fixture.AddAsync(company);

        var request = new GetCompanyQuery { Id = company.Id };
        var result = await _fixture.SendAsync(request);

        Assert.Equal(company?.Id, result?.Id);
        AssertNoOtherFields(result);
        AssertNoManager(result);
    }

    private static void AssertNoManager(CompanyVm? result)
    {
        Assert.False(result?.Fields.ContainsKey(nameof(Company.Manager)));
    }

    private static void AssertNoOtherFields(CompanyVm? result)
    {
        Assert.False(result?.Fields.ContainsKey(nameof(Company.Address)));
        Assert.False(result?.Fields.ContainsKey(nameof(Company.Ceo)));
        Assert.False(result?.Fields.ContainsKey(nameof(Company.Contacts)));
        Assert.False(result?.Fields.ContainsKey(nameof(Company.Email)));
        Assert.False(result?.Fields.ContainsKey(nameof(Company.Inn)));
        Assert.False(result?.Fields.ContainsKey(nameof(Company.Name)));
        Assert.False(result?.Fields.ContainsKey(nameof(Company.Phone)));
        Assert.False(result?.Fields.ContainsKey(nameof(Company.Type)));
    }

    private static void AssertOtherFieldsEqual(Company? company, CompanyVm? result)
    {
        Assert.Equal(company?.TypeId, (result?.Fields[nameof(Company.Type)] as CompanyTypeDto)?.Id);
        Assert.Equal(company?.Address, result?.Fields[nameof(Company.Address)]);
        Assert.Equal(company?.Ceo, result?.Fields[nameof(Company.Ceo)]);
        Assert.Equal(company?.Contacts, result?.Fields[nameof(Company.Contacts)]);
        Assert.Equal(company?.Email, result?.Fields[nameof(Company.Email)]);
        Assert.Equal(company?.Inn, result?.Fields[nameof(Company.Inn)]);
        Assert.Equal(company?.Name, result?.Fields[nameof(Company.Name)]);
        Assert.Equal(company?.Phone, result?.Fields[nameof(Company.Phone)]);
    }

    private static void AssertManagerEqual(Company? company, CompanyVm? result)
    {
        Assert.Equal(company?.ManagerId, (result?.Fields[nameof(Company.Manager)] as ManagerDto)?.Id);
    }
}