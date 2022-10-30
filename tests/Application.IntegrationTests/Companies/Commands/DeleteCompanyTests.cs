using Application.IntegrationTests;
using CRM.Application.Common.Exceptions;
using CRM.Application.Companies.Commands.DeleteCompany;
using CRM.Domain.Entities;

namespace CRM.Application.IntegrationTests.Companies;

public class DeleteCompanyTests : BaseTest
{
    public DeleteCompanyTests(BaseTestFixture fixture) : base(fixture) { }

    [Fact]
    public async Task User_is_admin___Deletes_company()
    {
        var user = await _fixture.RunAsAdministratorAsync();

        var company = Faker.Builders.Company();
        await _fixture.AddAsync(company);

        var command = new DeleteCompanyCommand { Id = company.Id };
        await _fixture.SendAsync(command);
        var foundCompany = await _fixture.FindAsync<Company>(company.Id);

        Assert.Null(foundCompany);
    }

    [Fact]
    public async Task User_has_claim___Deletes_company()
    {
        var user = await _fixture.RunAsDefaultUserAsync(new []
        {
            Utils.CreateClaim(Constants.Claims.DeleteCompany)
        });

        var company = Faker.Builders.Company();
        await _fixture.AddAsync(company);

        var command = new DeleteCompanyCommand { Id = company.Id };
        await _fixture.SendAsync(command);
        var foundCompany = await _fixture.FindAsync<Company>(company.Id);

        Assert.Null(foundCompany);
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