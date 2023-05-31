using CRM.Application.Common.Exceptions;
using CRM.Application.Companies.Commands.CreateCompany;
using CRM.Domain.Entities;
using CRM.Infrastructure.Identity;

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
    public async Task User_is_admin___Creates_company()
    {
        var user = await _fixture.RunAsAdministratorAsync();
        var command = CreateCommand();
        await AssertCompanyCreatedAsync(user, command);
    }

    [Fact]
    public async Task User_has_claim___Creates_company()
    {
        var user = await _fixture.RunAsDefaultUserAsync(new[]
        {
            Constants.Claims.Company.Create
        });

        var command = CreateCommand();
        await AssertCompanyCreatedAsync(user, command);
    }

    private async Task AssertCompanyCreatedAsync(AspNetUser user, CreateCompanyCommand expected)
    {
        var companyId = await _fixture.SendAsync(expected);
        var actual = await _fixture.FindAsync<Company>(companyId);

        Assert.NotNull(actual);
        Assert.Equal(companyId, actual!.Id);
        Assert.Equal(BaseTestFixture.UtcNow, actual.CreatedAtUtc);
        Assert.Equal(user.Id, actual.CreatedBy);
        Assert.Equal(expected.ManagerId, actual.ManagerId);
    }

    [Fact]
    public async Task User_has_no_claim___Throws_forbidden_access()
    {
        var user = await _fixture.RunAsDefaultUserAsync();
        var command = CreateCommand();

        await Assert.ThrowsAsync<ForbiddenAccessException>(() => _fixture.SendAsync(command));
    }

    [Theory]
    [InlineData(Constants.Claims.Company.Any.Manager.SetFromAnyToAny)]
    [InlineData(Constants.Claims.Company.Any.Manager.SetFromAnyToSelf)]
    [InlineData(Constants.Claims.Company.Any.Manager.SetFromNoneToSelf)]
    [InlineData(Constants.Claims.Company.Any.Manager.SetFromNoneToAny)]
    public async Task User_can_set_self_as_manager___Creates_company(string claim)
    {
        var user = await _fixture.RunAsDefaultUserAsync(new[]
        {
            Constants.Claims.Company.Create,
            claim
        });

        var command = CreateCommand(managerId: user.Id);
        await AssertCompanyCreatedAsync(user, command);
    }

    [Fact]
    public async Task User_cannot_set_self_as_manager___Throws_forbidden_access()
    {
        var user = await _fixture.RunAsDefaultUserAsync(new[]
        {
            Constants.Claims.Company.Create
        });

        var command = CreateCommand(managerId: user.Id);
        await Assert.ThrowsAsync<ForbiddenAccessException>(() => _fixture.SendAsync(command));
    }

    [Theory]
    [InlineData(Constants.Claims.Company.Any.Manager.SetFromAnyToAny)]
    [InlineData(Constants.Claims.Company.Any.Manager.SetFromNoneToAny)]
    public async Task User_can_set_anyone_as_manager___Creates_company(string claim)
    {
        var user = await _fixture.RunAsDefaultUserAsync(new[]
        {
            Constants.Claims.Company.Create,
            claim
        });
        var anotherUser = await _fixture.CreateUserAsync();

        var command = CreateCommand(managerId: anotherUser.Id);
        await AssertCompanyCreatedAsync(user, command);
    }

    [Fact]
    public async Task User_cannot_set_anyone_as_manager___Throws_forbidden_access()
    {
        var user = await _fixture.RunAsDefaultUserAsync(new[]
        {
            Constants.Claims.Company.Create
        });
        var anotherUser = await _fixture.CreateUserAsync();

        var command = CreateCommand(managerId: anotherUser.Id);
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
            TypeId = data.TypeId,
            ManagerId = managerId
        };

        return command;
    }
}