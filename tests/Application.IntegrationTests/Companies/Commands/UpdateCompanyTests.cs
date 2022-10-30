using Application.IntegrationTests;
using CRM.Application.Common.Exceptions;
using CRM.Application.Companies.Commands.CreateCompany;
using CRM.Application.Companies.Commands.UpdateCompany;
using CRM.Domain.Entities;
using FluentAssertions;

namespace CRM.Application.IntegrationTests.Companies;

public class UpdateCompanyTests : BaseTest
{
    public UpdateCompanyTests(BaseTestFixture fixture) : base(fixture) { }

    [Fact]
    public async Task Requires_minimum_fields()
    {
        await _fixture.RunAsAdministratorAsync();
        var command = new UpdateCompanyCommand();
        await Assert.ThrowsAsync<ValidationException>(() => _fixture.SendAsync(command));
    }

    [Fact]
    public async Task User_is_admin___Updates_company()
    {
        var user = await _fixture.RunAsAdministratorAsync();

        var company = Faker.Builders.Company();
        await _fixture.AddAsync(company);

        var command = CreateCommand(company.Id);
        await _fixture.SendAsync(command);
        var updatedCompany = await _fixture.FindAsync<Company>(company.Id);

        Assert.Equal(BaseTestFixture.UtcNow, company.LastModifiedAtUtc);
        Assert.Equal(user.Id, company.LastModifiedBy);
        command.Should().BeEquivalentTo(updatedCompany, options =>
            options.ExcludingNestedObjects().ExcludingMissingMembers());
    }

    [Fact]
    public async Task User_has_claim___Updates_company()
    {
        var user = await _fixture.RunAsDefaultUserAsync(new[]
        {
            Utils.CreateClaim(Constants.Claims.UpdateCompany)
        });

        var company = Faker.Builders.Company();
        await _fixture.AddAsync(company);

        var command = CreateCommand(company.Id);
        await _fixture.SendAsync(command);
        var updatedCompany = await _fixture.FindAsync<Company>(company.Id);

        Assert.Equal(BaseTestFixture.UtcNow, company.LastModifiedAtUtc);
        Assert.Equal(user.Id, company.LastModifiedBy);
        command.Should().BeEquivalentTo(updatedCompany, options =>
            options.ExcludingNestedObjects().ExcludingMissingMembers());
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

    private static UpdateCompanyCommand CreateCommand(int id)
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
            Type = data.Type
        };

        return command;
    }
}