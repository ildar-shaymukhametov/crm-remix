using CRM.Application.Common.Exceptions;
using CRM.Application.Companies.Commands.DeleteCompany;
using CRM.Domain.Entities;

namespace CRM.Application.IntegrationTests.Companies.Commands;

public class DeleteCompanyTests : BaseTest
{
    public DeleteCompanyTests(BaseTestFixture fixture) : base(fixture) { }

    [Fact]
    public async Task User_is_admin___Deletes_company()
    {
        await _fixture.RunAsAdministratorAsync();

        var company = Faker.Builders.Company();
        await _fixture.AddAsync(company);

        var command = new DeleteCompanyCommand { Id = company.Id };
        await _fixture.SendAsync(command);
        var foundCompany = await _fixture.FindAsync<Company>(company.Id);

        Assert.Null(foundCompany);
    }

    [Fact]
    public async Task User_has_claim_to_delete_any_company___Deletes_company()
    {
        await _fixture.RunAsDefaultUserAsync(new[] { Constants.Claims.Company.Any.Delete });

        var company = Faker.Builders.Company();
        await _fixture.AddAsync(company);

        var command = new DeleteCompanyCommand { Id = company.Id };
        await _fixture.SendAsync(command);
        var foundCompany = await _fixture.FindAsync<Company>(company.Id);

        Assert.Null(foundCompany);
    }

    [Fact]
    public async Task Not_found()
    {
        await _fixture.RunAsDefaultUserAsync();
        var command = new DeleteCompanyCommand { Id = 1 };
        await Assert.ThrowsAsync<NotFoundException>(() => _fixture.SendAsync(command));
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

    [Fact]
    public async Task User_has_claim_to_delete_own_company_and_is_manager___Deletes_company()
    {
        var user = await _fixture.RunAsDefaultUserAsync(new[] { Constants.Claims.Company.WhereUserIsManager.Delete });

        var company = Faker.Builders.Company(user.Id);
        await _fixture.AddAsync(company);

        var command = new DeleteCompanyCommand { Id = company.Id };
        await _fixture.SendAsync(command);
        var foundCompany = await _fixture.FindAsync<Company>(company.Id);

        Assert.Null(foundCompany);
    }

    [Fact]
    public async Task User_has_claim_to_delete_own_company_and_is_not_manager___Throws_fobidden_access()
    {
        var user = await _fixture.RunAsDefaultUserAsync(new[] { Constants.Claims.Company.WhereUserIsManager.Delete });

        var company = Faker.Builders.Company();
        await _fixture.AddAsync(company);
        var command = new DeleteCompanyCommand { Id = company.Id };

        await Assert.ThrowsAsync<ForbiddenAccessException>(() => _fixture.SendAsync(command));
    }

    [Fact]
    public async Task User_has_no_claim_to_delete_own_company_and_is_manager___Throws_fobidden_access()
    {
        var user = await _fixture.RunAsDefaultUserAsync();

        var company = Faker.Builders.Company(user.Id);
        await _fixture.AddAsync(company);
        var command = new DeleteCompanyCommand { Id = company.Id };

        await Assert.ThrowsAsync<ForbiddenAccessException>(() => _fixture.SendAsync(command));
    }
}