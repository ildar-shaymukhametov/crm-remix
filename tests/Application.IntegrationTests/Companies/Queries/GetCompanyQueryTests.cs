using CRM.Application.Common.Exceptions;
using CRM.Application.Companies.Queries.GetCompany;

namespace CRM.Application.IntegrationTests.Companies.Queries;

public class GetCompanyTests : BaseTest
{
    public GetCompanyTests(BaseTestFixture fixture) : base(fixture) { }

    [Fact]
    public async Task User_is_admin___Returns_company()
    {
        await _fixture.RunAsAdministratorAsync();
        var company = Faker.Builders.Company();
        await _fixture.AddAsync(company);

        var request = new GetCompanyQuery { Id = company.Id };
        var result = await _fixture.SendAsync(request);

        Assert.Equal(company.Id, result.Id);
    }

    [Theory]
    [InlineData(Constants.Claims.Company.Old.WhereUserIsManager.View)]
    [InlineData(Constants.Claims.Company.Old.WhereUserIsManager.Delete)]
    [InlineData(Constants.Claims.Company.Old.WhereUserIsManager.Update)]
    [InlineData(Constants.Claims.Company.Old.Any.View)]
    [InlineData(Constants.Claims.Company.Old.Any.Delete)]
    [InlineData(Constants.Claims.Company.Old.Any.Update)]
    public async Task User_has_claim_and_is_manager___Returns_company(string claim)
    {
        var user = await _fixture.RunAsDefaultUserAsync(new[] { claim });
        var company = Faker.Builders.Company(managerId: user.Id);
        await _fixture.AddAsync(company);

        var request = new GetCompanyQuery { Id = company.Id };
        var result = await _fixture.SendAsync(request);

        Assert.Equal(company.Id, result.Id);
    }

    [Theory]
    [InlineData(Constants.Claims.Company.Old.WhereUserIsManager.View)]
    [InlineData(Constants.Claims.Company.Old.WhereUserIsManager.Delete)]
    [InlineData(Constants.Claims.Company.Old.WhereUserIsManager.Update)]
    public async Task User_has_claim_and_is_not_manager___Throws_forbidden_access(string claim)
    {
        await _fixture.RunAsDefaultUserAsync(new[] { claim });

        var company = Faker.Builders.Company();
        await _fixture.AddAsync(company);

        var request = new GetCompanyQuery { Id = company.Id };
        await Assert.ThrowsAsync<ForbiddenAccessException>(() => _fixture.SendAsync(request));
    }

    [Theory]
    [InlineData(Constants.Claims.Company.Old.Any.View)]
    [InlineData(Constants.Claims.Company.Old.Any.Delete)]
    [InlineData(Constants.Claims.Company.Old.Any.Update)]
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

    [Theory]
    [InlineData(Constants.Claims.Company.Old.WhereUserIsManager.View)]
    [InlineData(Constants.Claims.Company.Old.WhereUserIsManager.Delete)]
    [InlineData(Constants.Claims.Company.Old.WhereUserIsManager.Update)]
    public async Task Not_found(string claim)
    {
        var user = await _fixture.RunAsDefaultUserAsync(new[] { claim });
        var request = new GetCompanyQuery { Id = 1 };
        await Assert.ThrowsAsync<NotFoundException>(() => _fixture.SendAsync(request));
    }
}