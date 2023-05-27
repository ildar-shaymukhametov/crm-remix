using CRM.Application.Common.Exceptions;
using CRM.Application.Companies.Queries.GetCompany;
using CRM.Domain.Entities;

namespace CRM.Application.IntegrationTests.Companies.Queries;

public class GetCompanyTests : BaseTest
{
    public GetCompanyTests(BaseTestFixture fixture) : base(fixture) { }

    [Fact]
    public async Task User_is_admin___Returns_company()
    {
        await _fixture.RunAsAdministratorAsync();
        var newCompany = Faker.Builders.Company();
        await _fixture.AddAsync(newCompany);

        var request = new GetCompanyQuery { Id = newCompany.Id };
        var result = await _fixture.SendAsync(request);
        var company = await _fixture.FindAsync<Company>(result.Id, nameof(Company.Type));

        Assert.Equal(company?.Id, result?.Id);
        Assert.Equal(company?.Type?.Name, result?.Type?.Name);
    }

    [Theory]
    [InlineData(Constants.Claims.Company.Any.View)]
    [InlineData(Constants.Claims.Company.Any.Delete)]
    [InlineData(Constants.Claims.Company.Any.Update)]
    public async Task User_has_claim_and_is_not_manager___Returns_company(string claim)
    {
        await _fixture.RunAsDefaultUserAsync(new[] { claim });

        var company = Faker.Builders.Company();
        await _fixture.AddAsync(company);

        var request = new GetCompanyQuery { Id = company.Id };
        var result = await _fixture.SendAsync(request);

        Assert.Equal(company.Id, result.Id);
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
}