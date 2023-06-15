using CRM.Application.Common.Exceptions;
using CRM.Application.Companies.Commands.CreateCompany;
using CRM.Domain.Entities;
using CRM.Infrastructure.Identity;

namespace CRM.Application.IntegrationTests.Companies.Commands;

public class CreateCompanyTests : BaseTest
{
    public CreateCompanyTests(BaseTestFixture fixture) : base(fixture) { }

    [Fact]
    public async Task Requires_minimum_fields()
    {
        await _fixture.RunAsAdministratorAsync();
        var command = new CreateCompanyCommand(string.Empty);
        await Assert.ThrowsAsync<ValidationException>(() => _fixture.SendAsync(command));
    }

    [Fact]
    public async Task User_is_admin___Creates_company_with_all_fields()
    {
        var user = await _fixture.RunAsAdministratorAsync();
        var command = CreateCommand(user.Id);

        var id = await _fixture.SendAsync(command);

        var actual = await _fixture.FindAsync<Company>(id);
        AssertCompanyCreated(user, id, command, actual);
        AssertOtherFields(command, actual);
        AssertManagerField(command, actual);
    }

    [Fact]
    public async Task User_has_claim_to_create_company___Creates_company_with_initial_fields()
    {
        var user = await _fixture.RunAsDefaultUserAsync(new[] { Constants.Claims.Company.Create });
        var command = CreateMinimumRequiredCommand();

        var id = await _fixture.SendAsync(command);

        var actual = await _fixture.FindAsync<Company>(id);
        AssertCompanyCreated(user, id, command, actual);
        AssertNoOtherFields(actual);
        AssertNoManagerField(actual);
    }

    [Fact]
    public async Task User_has_claim_to_create_company___Forbidden_to_set_address()
    {
        var user = await _fixture.RunAsDefaultUserAsync(new[] { Constants.Claims.Company.Create });
        var command = CreateMinimumRequiredCommand();
        command.Address = Faker.RandomString.Next();

        await Assert.ThrowsAsync<ForbiddenAccessException>(() => _fixture.SendAsync(command));
    }

    [Fact]
    public async Task User_has_claim_to_create_company___Forbidden_to_set_ceo()
    {
        var user = await _fixture.RunAsDefaultUserAsync(new[] { Constants.Claims.Company.Create });
        var command = CreateMinimumRequiredCommand();
        command.Ceo = Faker.RandomString.Next();

        await Assert.ThrowsAsync<ForbiddenAccessException>(() => _fixture.SendAsync(command));
    }

    [Fact]
    public async Task User_has_claim_to_create_company___Forbidden_to_set_contacts()
    {
        var user = await _fixture.RunAsDefaultUserAsync(new[] { Constants.Claims.Company.Create });
        var command = CreateMinimumRequiredCommand();
        command.Contacts = Faker.RandomString.Next();

        await Assert.ThrowsAsync<ForbiddenAccessException>(() => _fixture.SendAsync(command));
    }

    [Fact]
    public async Task User_has_claim_to_create_company___Forbidden_to_set_email()
    {
        var user = await _fixture.RunAsDefaultUserAsync(new[] { Constants.Claims.Company.Create });
        var command = CreateMinimumRequiredCommand();
        command.Email = Faker.RandomString.Next();

        await Assert.ThrowsAsync<ForbiddenAccessException>(() => _fixture.SendAsync(command));
    }

    [Fact]
    public async Task User_has_claim_to_set_other_fields___Creates_company_with_other_fields()
    {
        var user = await _fixture.RunAsDefaultUserAsync(new[] { Constants.Claims.Company.New.Other.Set });
        var command = CreateCommand();

        var id = await _fixture.SendAsync(command);

        var actual = await _fixture.FindAsync<Company>(id);
        AssertCompanyCreated(user, id, command, actual);
        AssertOtherFields(command, actual);
        AssertNoManagerField(actual);
    }

    [Fact]
    public async Task User_has_no_claim___Throws_forbidden_access()
    {
        var user = await _fixture.RunAsDefaultUserAsync();
        await Assert.ThrowsAsync<ForbiddenAccessException>(() => _fixture.SendAsync(CreateCommand()));
    }

    // [Theory]
    // [InlineData(Constants.Claims.Company.Any.Manager.SetFromAnyToAny)]
    // [InlineData(Constants.Claims.Company.Any.Manager.SetFromAnyToSelf)]
    // [InlineData(Constants.Claims.Company.Any.Manager.SetFromNoneToSelf)]
    // [InlineData(Constants.Claims.Company.Any.Manager.SetFromNoneToAny)]
    // public async Task User_can_set_self_as_manager___Creates_company(string claim)
    // {
    //     var user = await _fixture.RunAsDefaultUserAsync(new[]
    //     {
    //         Constants.Claims.Company.Create,
    //         claim
    //     });

    //     var command = CreateCommand(managerId: user.Id);
    //     await AssertCompanyCreatedAsync(user, command);
    // }

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

    // [Theory]
    // [InlineData(Constants.Claims.Company.Any.Manager.SetFromAnyToAny)]
    // [InlineData(Constants.Claims.Company.Any.Manager.SetFromNoneToAny)]
    // public async Task User_can_set_anyone_as_manager___Creates_company(string claim)
    // {
    //     var user = await _fixture.RunAsDefaultUserAsync(new[]
    //     {
    //         Constants.Claims.Company.Create,
    //         claim
    //     });
    //     var anotherUser = await _fixture.CreateUserAsync();

    //     var command = CreateCommand(managerId: anotherUser.Id);
    //     await AssertCompanyCreatedAsync(user, command);
    // }

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
        var command = CreateMinimumRequiredCommand();
        command.Address = data.Address;
        command.Ceo = data.Ceo;
        command.Contacts = data.Contacts;
        command.Email = data.Email;
        command.Inn = data.Inn;
        command.Phone = data.Phone;
        command.TypeId = data.TypeId;
        command.ManagerId = managerId;

        return command;
    }

    public static CreateCompanyCommand CreateMinimumRequiredCommand()
    {
        return new CreateCompanyCommand(Faker.Company.Name());
    }

    private static void AssertCompanyCreated(AspNetUser user, int companyId, CreateCompanyCommand expected, Company? actual)
    {
        Assert.NotNull(actual);
        Assert.Equal(companyId, actual.Id);
        Assert.Equal(expected.Name, actual.Name);
        Assert.Equal(BaseTestFixture.UtcNow, actual.CreatedAtUtc);
        Assert.Equal(user.Id, actual.CreatedBy);
    }

    private static void AssertOtherFields(CreateCompanyCommand expected, Company? actual)
    {
        Assert.Equal(expected.TypeId, actual?.TypeId);
        Assert.Equal(expected.Address, actual?.Address);
        Assert.Equal(expected.Ceo, actual?.Ceo);
        Assert.Equal(expected.Contacts, actual?.Contacts);
        Assert.Equal(expected.Email, actual?.Email);
        Assert.Equal(expected.Inn, actual?.Inn);
        Assert.Equal(expected.Phone, actual?.Phone);
    }

    private static void AssertNoOtherFields(Company? actual)
    {
        Assert.Equal(default, actual?.TypeId);
        Assert.Equal(default, actual?.Address);
        Assert.Equal(default, actual?.Ceo);
        Assert.Equal(default, actual?.Contacts);
        Assert.Equal(default, actual?.Email);
        Assert.Equal(default, actual?.Inn);
        Assert.Equal(default, actual?.Phone);
    }

    private static void AssertManagerField(CreateCompanyCommand expected, Company? actual)
    {
        Assert.Equal(expected.ManagerId, actual?.ManagerId);
    }

    private static void AssertNoManagerField(Company? actual)
    {
        Assert.Equal(default, actual?.ManagerId);
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
}