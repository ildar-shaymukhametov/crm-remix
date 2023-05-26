using CRM.Application;
using CRM.Application.IntegrationTests;
using CRM.Application.IntegrationTests.Companies;
using CRM.Application.Tests;
using CRM.Domain.Entities;

namespace Application.IntegrationTests.Tests;

public class ResetDbCommandTests : BaseTest
{
    public ResetDbCommandTests(BaseTestFixture fixture) : base(fixture) { }

    [Fact]
    public async Task Resets_db()
    {
        await _fixture.RunAsDefaultUserAsync(new[]
        {
            Constants.Claims.Company.Create
        }, new [] { Constants.Roles.Tester });

        var createCompanyCommand = CreateCompanyTests.CreateCommand();
        var companyId = await _fixture.SendAsync(createCompanyCommand);
        var newCompany = await _fixture.FindAsync<Company>(companyId);
        Assert.NotNull(newCompany);

        var command = new ResetDbCommand();
        await _fixture.SendAsync(command);

        var company = await _fixture.FindAsync<Company>(companyId);
        Assert.Null(company);
    }
}
