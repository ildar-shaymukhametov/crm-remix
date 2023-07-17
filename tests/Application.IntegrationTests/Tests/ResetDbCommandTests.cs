using CRM.Application.IntegrationTests.Companies.Commands;
using CRM.Application.Tests;
using CRM.Domain.Entities;

namespace CRM.Application.IntegrationTests.Tests;

public class ResetDbCommandTests : BaseTest
{
    public ResetDbCommandTests(BaseTestFixture fixture) : base(fixture) { }

    [Fact]
    public async Task Resets_db()
    {
        await _fixture.RunAsDefaultUserAsync(new[]
        {
            Domain.Constants.Claims.Company.Create
        });

        var createCompanyCommand = CreateCompanyTests.CreateMinimumRequiredCommand();
        var companyId = await _fixture.SendAsync(createCompanyCommand);
        var newCompany = await _fixture.FindAsync<Company>(companyId);
        Assert.NotNull(newCompany);

        var command = new ResetDbCommand();
        await _fixture.SendAsync(command);

        var company = await _fixture.FindAsync<Company>(companyId);
        Assert.Null(company);
    }
}
