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
        var command = new CreateCompanyCommand();
        await Assert.ThrowsAsync<ValidationException>(() => _fixture.SendAsync(command));
    }

    [Fact]
    public async Task User_is_admin___Creates_company()
    {
        var user = await _fixture.RunAsAdministratorAsync();

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
            Type = data.Type
        };

        var companyId = await _fixture.SendAsync(command);
        var company = await _fixture.FindAsync<Company>(companyId);

        Assert.NotNull(company);
        Assert.Equal(companyId, company!.Id);
        Assert.Equal(BaseTestFixture.UtcNow, company.CreatedAtUtc);
        Assert.Equal(user.Id, company.CreatedBy);
    }
}