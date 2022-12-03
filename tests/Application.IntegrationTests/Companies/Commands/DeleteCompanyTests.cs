using CRM.Application.Common.Exceptions;
using CRM.Application.Companies.Commands.DeleteCompany;
using CRM.Domain.Entities;

namespace CRM.Application.IntegrationTests.Companies;

public class DeleteCompanyTests : BaseTest
{
    public DeleteCompanyTests(BaseTestFixture fixture) : base(fixture) { }

    [Fact]
    public async Task User_has_claim_and_is_manager___Deletes_company()
    {
        var user = await _fixture.RunAsDefaultUserAsync(new []
        {
            Constants.Claims.DeleteCompany
        });

        var company = Faker.Builders.Company(managerId: user.Id);
        await _fixture.AddAsync(company);

        var command = new DeleteCompanyCommand { Id = company.Id };
        await _fixture.SendAsync(command);
        var foundCompany = await _fixture.FindAsync<Company>(company.Id);

        Assert.Null(foundCompany);
    }

    [Fact]
    public async Task User_has_claim_and_is_not_manager___Throws_fobidden_access()
    {
        await _fixture.RunAsDefaultUserAsync(new[]
        {
            Constants.Claims.DeleteCompany
        });

        var company = Faker.Builders.Company();
        await _fixture.AddAsync(company);

        var command = new DeleteCompanyCommand { Id = company.Id };
        await Assert.ThrowsAsync<ForbiddenAccessException>(() => _fixture.SendAsync(command));
    }

    [Fact]
    public async Task User_has_no_claim___Throws_fobidden_access()
    {
        var user = await _fixture.RunAsDefaultUserAsync();

        var company = Faker.Builders.Company();
        await _fixture.AddAsync(company);

        var command = new DeleteCompanyCommand { Id = company.Id };
        await Assert.ThrowsAsync<ForbiddenAccessException>(() => _fixture.SendAsync(command));
    }
}