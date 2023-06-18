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
        var command = CreateCommandWithAllFields(user.Id);

        var id = await _fixture.SendAsync(command);

        var actual = await _fixture.FindAsync<Company>(id);
        AssertCompanyCreated(user, id, command, actual);
        AssertOtherFields(command, actual);
        AssertManagerField(command, actual);
    }

    [Fact]
    public async Task User_has_no_claim___Throws_forbidden_access()
    {
        var user = await _fixture.RunAsDefaultUserAsync();
        await Assert.ThrowsAsync<ForbiddenAccessException>(() => _fixture.SendAsync(CreateMinimumRequiredCommand()));
    }

    [Theory]
    [InlineData(Constants.Claims.Company.New.Other.Set)]
    [InlineData(Constants.Claims.Company.New.Manager.SetToAny)]
    [InlineData(Constants.Claims.Company.New.Manager.SetToNone)]
    [InlineData(Constants.Claims.Company.New.Manager.SetToSelf)]
    public async Task User_has_no_claim_to_create_company__Throws_forbidden_access(string claim)
    {
        var user = await _fixture.RunAsDefaultUserAsync(new[] { claim });
        await Assert.ThrowsAsync<ForbiddenAccessException>(() => _fixture.SendAsync(CreateMinimumRequiredCommand()));
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

    [Theory]
    [InlineData(Constants.Claims.Company.Create)]
    [InlineData(Constants.Claims.Company.New.Manager.SetToAny)]
    [InlineData(Constants.Claims.Company.New.Manager.SetToSelf)]
    public async Task Forbidden_to_set_address(string claim)
    {
        var user = await _fixture.RunAsDefaultUserAsync(new[] { claim });
        var command = CreateMinimumRequiredCommand();
        command.Address = Faker.RandomString.Next();

        await Assert.ThrowsAsync<ForbiddenAccessException>(() => _fixture.SendAsync(command));
    }

    [Theory]
    [InlineData(Constants.Claims.Company.Create)]
    [InlineData(Constants.Claims.Company.New.Manager.SetToAny)]
    [InlineData(Constants.Claims.Company.New.Manager.SetToSelf)]
    public async Task Forbidden_to_set_ceo(string claim)
    {
        var user = await _fixture.RunAsDefaultUserAsync(new[] { claim });
        var command = CreateMinimumRequiredCommand();
        command.Ceo = Faker.RandomString.Next();

        await Assert.ThrowsAsync<ForbiddenAccessException>(() => _fixture.SendAsync(command));
    }

    [Theory]
    [InlineData(Constants.Claims.Company.Create)]
    [InlineData(Constants.Claims.Company.New.Manager.SetToAny)]
    [InlineData(Constants.Claims.Company.New.Manager.SetToSelf)]
    public async Task Forbidden_to_set_contacts(string claim)
    {
        var user = await _fixture.RunAsDefaultUserAsync(new[] { claim });
        var command = CreateMinimumRequiredCommand();
        command.Contacts = Faker.RandomString.Next();

        await Assert.ThrowsAsync<ForbiddenAccessException>(() => _fixture.SendAsync(command));
    }

    [Theory]
    [InlineData(Constants.Claims.Company.Create)]
    [InlineData(Constants.Claims.Company.New.Manager.SetToAny)]
    [InlineData(Constants.Claims.Company.New.Manager.SetToSelf)]
    public async Task Forbidden_to_set_email(string claim)
    {
        var user = await _fixture.RunAsDefaultUserAsync(new[] { claim });
        var command = CreateMinimumRequiredCommand();
        command.Email = Faker.RandomString.Next();

        await Assert.ThrowsAsync<ForbiddenAccessException>(() => _fixture.SendAsync(command));
    }

    [Theory]
    [InlineData(Constants.Claims.Company.Create)]
    [InlineData(Constants.Claims.Company.New.Manager.SetToAny)]
    [InlineData(Constants.Claims.Company.New.Manager.SetToSelf)]
    public async Task Forbidden_to_set_inn(string claim)
    {
        var user = await _fixture.RunAsDefaultUserAsync(new[] { claim });
        var command = CreateMinimumRequiredCommand();
        command.Inn = Faker.RandomString.Next();

        await Assert.ThrowsAsync<ForbiddenAccessException>(() => _fixture.SendAsync(command));
    }

    [Theory]
    [InlineData(Constants.Claims.Company.Create)]
    [InlineData(Constants.Claims.Company.New.Manager.SetToAny)]
    [InlineData(Constants.Claims.Company.New.Manager.SetToSelf)]
    public async Task Forbidden_to_set_phone(string claim)
    {
        var user = await _fixture.RunAsDefaultUserAsync(new[] { claim });
        var command = CreateMinimumRequiredCommand();
        command.Phone = Faker.RandomString.Next();

        await Assert.ThrowsAsync<ForbiddenAccessException>(() => _fixture.SendAsync(command));
    }

    [Theory]
    [InlineData(Constants.Claims.Company.Create)]
    [InlineData(Constants.Claims.Company.New.Manager.SetToAny)]
    [InlineData(Constants.Claims.Company.New.Manager.SetToSelf)]
    public async Task Forbidden_to_set_type(string claim)
    {
        var user = await _fixture.RunAsDefaultUserAsync(new[] { claim });
        var command = CreateMinimumRequiredCommand();
        command.TypeId = 1;

        await Assert.ThrowsAsync<ForbiddenAccessException>(() => _fixture.SendAsync(command));
    }

    [Fact]
    public async Task User_has_claim_to_set_other_fields___Creates_company_with_other_fields()
    {
        var user = await _fixture.RunAsDefaultUserAsync(new[] { Constants.Claims.Company.Create, Constants.Claims.Company.New.Other.Set });
        var command = CreateCommandWithOtherFields();

        var id = await _fixture.SendAsync(command);

        var actual = await _fixture.FindAsync<Company>(id);
        AssertCompanyCreated(user, id, command, actual);
        AssertOtherFields(command, actual);
        AssertNoManagerField(actual);
    }

    [Theory]
    [InlineData(Constants.Claims.Company.Create)]
    [InlineData(Constants.Claims.Company.New.Other.Set)]
    public async Task Forbidden_to_set_manager(string claim)
    {
        var user = await _fixture.RunAsDefaultUserAsync(new[] { claim });
        var command = CreateMinimumRequiredCommand();
        command.ManagerId = user.Id;

        await Assert.ThrowsAsync<ForbiddenAccessException>(() => _fixture.SendAsync(command));
    }

    [Theory]
    [InlineData(Constants.Claims.Company.New.Manager.SetToAny)]
    [InlineData(Constants.Claims.Company.New.Manager.SetToSelf)]
    public async Task User_can_set_manager_to_self___Creates_company_with_self_as_manager(string claim)
    {
        var user = await _fixture.RunAsDefaultUserAsync(new[] {Constants.Claims.Company.Create, claim });
        var command = CreateMinimumRequiredCommand();
        command.ManagerId = user.Id;

        var id = await _fixture.SendAsync(command);

        var actual = await _fixture.FindAsync<Company>(id);
        AssertCompanyCreated(user, id, command, actual);
        AssertNoOtherFields(actual);
        AssertManagerField(command, actual);
    }

    [Fact]
    public async Task User_can_set_manager_to_self___Forbidden_to_set_someone_else_as_manager()
    {
        var user = await _fixture.RunAsDefaultUserAsync(new[] { Constants.Claims.Company.New.Manager.SetToSelf });
        var someUser = await _fixture.AddUserAsync();
        var command = CreateMinimumRequiredCommand();
        command.ManagerId = someUser.Id;

        await Assert.ThrowsAsync<ForbiddenAccessException>(() => _fixture.SendAsync(command));
    }

    [Fact]
    public async Task User_can_set_manager_to_any___Creates_company_with_some_user_as_manager()
    {
        var user = await _fixture.RunAsDefaultUserAsync(new[] { Constants.Claims.Company.Create, Constants.Claims.Company.New.Manager.SetToAny });
        var someUser = await _fixture.AddUserAsync();
        var command = CreateMinimumRequiredCommand();
        command.ManagerId = someUser.Id;

        var id = await _fixture.SendAsync(command);

        var actual = await _fixture.FindAsync<Company>(id);
        AssertCompanyCreated(user, id, command, actual);
        AssertNoOtherFields(actual);
        AssertManagerField(command, actual);
    }

    public static CreateCompanyCommand CreateCommandWithAllFields(string? managerId = null)
    {
        var command = CreateMinimumRequiredCommand();
        var otherFields = CreateCommandWithOtherFields();
        command.Address = otherFields.Address;
        command.Ceo = otherFields.Ceo;
        command.Contacts = otherFields.Contacts;
        command.Email = otherFields.Email;
        command.Inn = otherFields.Inn;
        command.Phone = otherFields.Phone;
        command.TypeId = otherFields.TypeId;
        command.ManagerId = managerId;

        return command;
    }

    public static CreateCompanyCommand CreateCommandWithOtherFields()
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
}