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
        Assert.Equal(company?.TypeId, (result?.Fields["Type"] as CompanyTypeDto)?.Id);
        Assert.Equal(company?.Address, result?.Fields["Address"]);
        Assert.Equal(company?.Ceo, result?.Fields["Ceo"]);
        Assert.Equal(company?.Contacts, result?.Fields["Contacts"]);
        Assert.Equal(company?.Email, result?.Fields["Email"]);
        Assert.Equal(company?.Inn, result?.Fields["Inn"]);
        Assert.Equal(company?.ManagerId, (result?.Fields["Manager"] as ManagerDto)?.Id);
        Assert.Equal(company?.Name, result?.Fields["Name"]);
        Assert.Equal(company?.Phone, result?.Fields["Phone"]);
    }

    // [Theory]
    // [InlineData(Constants.Claims.Company.Any.View)]
    // public async Task User_has_claim_to_view_any_company___Returns_all_fields(string claim)
    // {
    //     await _fixture.RunAsDefaultUserAsync(new[] { claim });

    //     var company = Faker.Builders.Company();
    //     await _fixture.AddAsync(company);

    //     var request = new GetCompanyQuery { Id = company.Id };
    //     var result = await _fixture.SendAsync(request);

    //     Assert.Equal(company.Id, result.Id);
    // }

    // [Fact]
    // public async Task User_has_no_claim___Throws_forbidden_access()
    // {
    //     var user = await _fixture.RunAsDefaultUserAsync();

    //     var company = Faker.Builders.Company();
    //     await _fixture.AddAsync(company);

    //     var request = new GetCompanyQuery { Id = company.Id };
    //     await Assert.ThrowsAsync<ForbiddenAccessException>(() => _fixture.SendAsync(request));
    // }

    // [Theory]
    // [InlineData(Constants.Claims.Company.WhereUserIsManager.Other.View)]
    // [InlineData(Constants.Claims.Company.WhereUserIsManager.Delete)]
    // [InlineData(Constants.Claims.Company.WhereUserIsManager.Other.Update)]
    // public async Task User_has_claim_and_is_manager___Returns_company(string claim)
    // {
    //     var user = await _fixture.RunAsDefaultUserAsync(new[] { claim });

    //     var company = Faker.Builders.Company(user.Id);
    //     await _fixture.AddAsync(company);

    //     var request = new GetCompanyQuery { Id = company.Id };
    //     var result = await _fixture.SendAsync(request);

    //     Assert.Equal(company.Id, result.Id);
    // }

    // [Theory]
    // [InlineData(Constants.Claims.Company.WhereUserIsManager.Other.View)]
    // [InlineData(Constants.Claims.Company.WhereUserIsManager.Delete)]
    // [InlineData(Constants.Claims.Company.WhereUserIsManager.Other.Update)]
    // public async Task User_has_claim_and_is_not_manager___Throws_forbidden_access(string claim)
    // {
    //     await _fixture.RunAsDefaultUserAsync(claim);

    //     var company = Faker.Builders.Company();
    //     await _fixture.AddAsync(company);

    //     var request = new GetCompanyQuery { Id = company.Id };
    //     await Assert.ThrowsAsync<ForbiddenAccessException>(() => _fixture.SendAsync(request));
    // }
}