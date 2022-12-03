using CRM.Application.Common.Exceptions;
using CRM.Application.Companies.Commands.CreateCompany;
using CRM.Domain.Entities;

namespace CRM.Application.IntegrationTests.Companies;

public class CreateCompanyTests : BaseTest
{
    public CreateCompanyTests(BaseTestFixture fixture) : base(fixture) { }

    [Fact]
    public async Task Requires_minimum_fields()
    {
        await _fixture.RunAsAdministratorAsync();
        var command = new CreateCompanyCommand();
        await Assert.ThrowsAsync<ValidationException>(() => _fixture.SendAsync(command));
    }

    [Fact]
    public async Task User_has_claim___Creates_company()
    {
        var user = await _fixture.RunAsDefaultUserAsync(new []
        {
            Constants.Claims.CreateCompany
        });

        var manager = await _fixture.CreateUserAsync();
        var command = CreateCommand(managerId: manager.Id);

        var companyId = await _fixture.SendAsync(command);
        var company = await _fixture.FindAsync<Company>(companyId);

        Assert.NotNull(company);
        Assert.Equal(companyId, company!.Id);
        Assert.Equal(BaseTestFixture.UtcNow, company.CreatedAtUtc);
        Assert.Equal(user.Id, company.CreatedBy);
        Assert.Equal(manager.Id, company.ManagerId);
    }

    [Fact]
    public async Task User_has_no_claim___Throws_forbidden_access()
    {
        var user = await _fixture.RunAsDefaultUserAsync();
        var command = CreateCommand();

        await Assert.ThrowsAsync<ForbiddenAccessException>(() => _fixture.SendAsync(command));
    }

    public static CreateCompanyCommand CreateCommand(string? managerId = null)
    {
        var data = Faker.Builders.Company();
        var command = new CreateCompanyCommand
        {
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