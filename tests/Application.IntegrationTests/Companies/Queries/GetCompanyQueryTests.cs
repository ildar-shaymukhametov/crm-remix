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
        Assert.Equal(company?.TypeId, (result?.Fields[nameof(Company.Type)] as CompanyTypeDto)?.Id);
        Assert.Equal(company?.Address, result?.Fields[nameof(Company.Address)]);
        Assert.Equal(company?.Ceo, result?.Fields[nameof(Company.Ceo)]);
        Assert.Equal(company?.Contacts, result?.Fields[nameof(Company.Contacts)]);
        Assert.Equal(company?.Email, result?.Fields[nameof(Company.Email)]);
        Assert.Equal(company?.Inn, result?.Fields[nameof(Company.Inn)]);
        Assert.Equal(company?.Name, result?.Fields[nameof(Company.Name)]);
        Assert.Equal(company?.Phone, result?.Fields[nameof(Company.Phone)]);
        Assert.Equal(company?.ManagerId, (result?.Fields[nameof(Company.Manager)] as ManagerDto)?.Id);
    }

    [Theory]
    [InlineData(Constants.Claims.Company.Any.View)]
    public async Task User_has_claim_to_view_any_company___Returns_all_fields(string claim)
    {
        var user = await _fixture.RunAsDefaultUserAsync(new[] { claim });

        var company = Faker.Builders.Company(managerId: user.Id);
        await _fixture.AddAsync(company);

        var request = new GetCompanyQuery { Id = company.Id };
        var result = await _fixture.SendAsync(request);

        Assert.Equal(company?.Id, result?.Id);
        Assert.Equal(company?.TypeId, (result?.Fields[nameof(Company.Type)] as CompanyTypeDto)?.Id);
        Assert.Equal(company?.Address, result?.Fields[nameof(Company.Address)]);
        Assert.Equal(company?.Ceo, result?.Fields[nameof(Company.Ceo)]);
        Assert.Equal(company?.Contacts, result?.Fields[nameof(Company.Contacts)]);
        Assert.Equal(company?.Email, result?.Fields[nameof(Company.Email)]);
        Assert.Equal(company?.Inn, result?.Fields[nameof(Company.Inn)]);
        Assert.Equal(company?.Name, result?.Fields[nameof(Company.Name)]);
        Assert.Equal(company?.Phone, result?.Fields[nameof(Company.Phone)]);
        Assert.Equal(company?.ManagerId, (result?.Fields[nameof(Company.Manager)] as ManagerDto)?.Id);
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

    [Theory]
    [InlineData(Constants.Claims.Company.Any.Other.View)]
    public async Task User_has_claim_to_view_other_fields_in_any_company___Returns_other_fields(string claim)
    {
        await _fixture.RunAsDefaultUserAsync(new[] { claim });

        var company = Faker.Builders.Company();
        await _fixture.AddAsync(company);

        var request = new GetCompanyQuery { Id = company.Id };
        var result = await _fixture.SendAsync(request);

        Assert.Equal(company?.Id, result?.Id);
        Assert.Equal(company?.TypeId, (result?.Fields[nameof(Company.Type)] as CompanyTypeDto)?.Id);
        Assert.Equal(company?.Address, result?.Fields[nameof(Company.Address)]);
        Assert.Equal(company?.Ceo, result?.Fields[nameof(Company.Ceo)]);
        Assert.Equal(company?.Contacts, result?.Fields[nameof(Company.Contacts)]);
        Assert.Equal(company?.Email, result?.Fields[nameof(Company.Email)]);
        Assert.Equal(company?.Inn, result?.Fields[nameof(Company.Inn)]);
        Assert.Equal(company?.Name, result?.Fields[nameof(Company.Name)]);
        Assert.Equal(company?.Phone, result?.Fields[nameof(Company.Phone)]);
    }

    [Fact]
    public async Task User_has_claim_to_view_other_fields_in_any_company___Does_not_return_manager()
    {
        await _fixture.RunAsDefaultUserAsync(new[] { Constants.Claims.Company.Any.Other.View });

        var company = Faker.Builders.Company();
        await _fixture.AddAsync(company);

        var request = new GetCompanyQuery { Id = company.Id };
        var result = await _fixture.SendAsync(request);

        Assert.False(result.Fields.ContainsKey(nameof(Company.Manager)));
    }
}