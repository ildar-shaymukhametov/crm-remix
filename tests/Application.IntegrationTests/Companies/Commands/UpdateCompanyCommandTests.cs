using CRM.Application.Common.Exceptions;
using CRM.Application.Companies.Commands.UpdateCompany;
using CRM.Domain.Entities;
using CRM.Infrastructure.Identity;
using FluentAssertions;

namespace CRM.Application.IntegrationTests.Companies;

public class UpdateCompanyTests : BaseTest
{
    public UpdateCompanyTests(BaseTestFixture fixture) : base(fixture) { }

    [Fact]
    public async Task Requires_minimum_fields()
    {
        await _fixture.RunAsAdministratorAsync();
        var company = Faker.Builders.Company();
        await _fixture.AddAsync(company);

        var command = new UpdateCompanyCommand { Id = company.Id };

        await Assert.ThrowsAsync<ValidationException>(() => _fixture.SendAsync(command));
    }

    [Fact]
    public async Task User_is_admin___Updates_company()
    {
        var user = await _fixture.RunAsAdministratorAsync();

        var company = Faker.Builders.Company();
        await _fixture.AddAsync(company);
        var manager = await _fixture.CreateUserAsync();
        var command = CreateCommand(company.Id, managerId: manager.Id);

        await AssertCompanyUpdatedAsync(user, company, command);
    }

    [Theory]
    [InlineData(Constants.Claims.UpdateCompany)]
    [InlineData(Constants.Claims.UpdateAnyCompany)]
    public async Task User_has_claim_and_is_manager___Updates_company(string claim)
    {
        var user = await _fixture.RunAsDefaultUserAsync(new[] { claim });

        var company = Faker.Builders.Company(managerId: user.Id);
        await _fixture.AddAsync(company);
        var manager = await _fixture.CreateUserAsync();
        var command = CreateCommand(company.Id, managerId: manager.Id);

        await AssertCompanyUpdatedAsync(user, company, command);
    }

    private async Task AssertCompanyUpdatedAsync(AspNetUser user, Company company, UpdateCompanyCommand command)
    {
        await _fixture.SendAsync(command);
        var updatedCompany = await _fixture.FindAsync<Company>(company.Id);

        Assert.Equal(BaseTestFixture.UtcNow, company.LastModifiedAtUtc);
        Assert.Equal(user.Id, company.LastModifiedBy);
        command.Should().BeEquivalentTo(updatedCompany, options =>
            options.ExcludingNestedObjects().ExcludingMissingMembers());
    }

    [Fact]
    public async Task User_has_claim_and_is_not_manager___Throws_forbidden_access()
    {
        await _fixture.RunAsDefaultUserAsync(new[]
        {
            Constants.Claims.UpdateCompany
        });

        var company = Faker.Builders.Company();
        await _fixture.AddAsync(company);

        var manager = await _fixture.CreateUserAsync();
        var command = CreateCommand(company.Id, managerId: manager.Id);

        await Assert.ThrowsAsync<ForbiddenAccessException>(() => _fixture.SendAsync(command));
    }

    [Fact]
    public async Task User_has_claim_and_is_not_manager___Updates_company()
    {
        var user = await _fixture.RunAsDefaultUserAsync(new[]
        {
            Constants.Claims.UpdateAnyCompany
        });

        var company = Faker.Builders.Company();
        await _fixture.AddAsync(company);
        var manager = await _fixture.CreateUserAsync();
        var command = CreateCommand(company.Id, managerId: manager.Id);

        await AssertCompanyUpdatedAsync(user, company, command);
    }

    [Fact]
    public async Task User_has_no_claim___Throws_forbidden_access()
    {
        var user = await _fixture.RunAsDefaultUserAsync();

        var company = Faker.Builders.Company();
        await _fixture.AddAsync(company);

        var command = CreateCommand(company.Id);

        await Assert.ThrowsAsync<ForbiddenAccessException>(() => _fixture.SendAsync(command));
    }

    [Fact]
    public async Task Not_found()
    {
        await _fixture.RunAsDefaultUserAsync(new[]
        {
            Constants.Claims.UpdateCompany
        });
        var command = CreateCommand(1);
        await Assert.ThrowsAsync<NotFoundException>(() => _fixture.SendAsync(command));
    }

    private static UpdateCompanyCommand CreateCommand(int id, string? managerId = null)
    {
        var data = Faker.Builders.Company();
        var command = new UpdateCompanyCommand
        {
            Id = id,
            Address = data.Address,
            Ceo = data.Ceo,
            Contacts = data.Contacts,
            Email = data.Email,
            Inn = data.Inn,
            Name = data.Name,
            Phone = data.Phone,
            Type = data.Type,
            ManagerId = managerId
        };

        return command;
    }
}