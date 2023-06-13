using CRM.Application.Common.Exceptions;
using CRM.Application.Companies.Commands.UpdateCompany;
using CRM.Domain.Entities;
using CRM.Infrastructure.Identity;
using FluentAssertions;
using static CRM.Application.Constants;

namespace CRM.Application.IntegrationTests.Companies.Commands;

public class UpdateCompanyTests : BaseTest
{
    public UpdateCompanyTests(BaseTestFixture fixture) : base(fixture) { }

    [Fact]
    public async Task Requires_minimum_fields()
    {
        await _fixture.RunAsAdministratorAsync();
        var company = await _fixture.AddCompanyAsync();
        var command = new UpdateCompanyCommand { Id = company.Id };

        await Assert.ThrowsAsync<ValidationException>(() => _fixture.SendAsync(command));
    }

    [Fact]
    public async Task User_is_admin___Updates_company()
    {
        var user = await _fixture.RunAsAdministratorAsync();
        var company = await _fixture.AddCompanyAsync();
        var manager = await _fixture.CreateUserAsync();
        var command = CreateNewData(company.Id, managerId: manager.Id);

        await AssertCompanyUpdatedAsync(user, company.Id, command);
    }

    [Fact]
    public async Task User_has_no_claim___Throws_forbidden_access()
    {
        var user = await _fixture.RunAsDefaultUserAsync();
        var company = await _fixture.AddCompanyAsync();
        var command = CreateNewData(company.Id);

        await Assert.ThrowsAsync<ForbiddenAccessException>(() => _fixture.SendAsync(command));
    }

    [Fact]
    public async Task Not_found()
    {
        await _fixture.RunAsDefaultUserAsync(new[]
        {
            Claims.Company.Any.Other.Update
        });
        var command = CreateNewData(1);
        await Assert.ThrowsAsync<NotFoundException>(() => _fixture.SendAsync(command));
    }

    [Fact]
    public async Task User_has_claim_to_update_other_fields_in_any_company___Updates_other_fields()
    {
        var user = await _fixture.RunAsDefaultUserAsync(new[] { Claims.Company.Any.Other.Update });
        var company = await _fixture.AddCompanyAsync();
        var command = CreateNewData(company.Id);

        await _fixture.SendAsync(command);

        var actual = await _fixture.FindAsync<Company>(company.Id);
        AssertCompanyUpdated(user, actual);
        AssertOtherFieldsUpdated(command, actual);
        AssertManagerUnchanged(company, actual);
    }

    [Fact]
    public async Task User_has_claim_to_update_other_fields_in_any_company___Forbidden_to_update_manager()
    {
        var user = await _fixture.RunAsDefaultUserAsync(new[] { Claims.Company.Any.Other.Update });
        var company = await _fixture.AddCompanyAsync();
        var command = CreateNewData(company.Id, user.Id);

        await Assert.ThrowsAsync<ForbiddenAccessException>(() => _fixture.SendAsync(command));
    }

    [Theory]
    [InlineData(Claims.Company.Any.Manager.SetFromAnyToAny)]
    [InlineData(Claims.Company.Any.Manager.SetFromAnyToNone)]
    [InlineData(Claims.Company.Any.Manager.SetFromAnyToSelf)]
    [InlineData(Claims.Company.Any.Manager.SetFromNoneToAny)]
    [InlineData(Claims.Company.Any.Manager.SetFromNoneToSelf)]
    [InlineData(Claims.Company.Any.Manager.SetFromSelfToAny)]
    [InlineData(Claims.Company.Any.Manager.SetFromSelfToNone)]
    public async Task User_has_claim_to_update_manager_in_any_company___Forbidden_to_update_address(string claim)
    {
        var user = await _fixture.RunAsDefaultUserAsync(new[] { claim });
        var company = await _fixture.AddCompanyAsync();
        var command = CreateCopyData(company);
        command.Address = Faker.RandomString.Next();

        await Assert.ThrowsAsync<ForbiddenAccessException>(() => _fixture.SendAsync(command));
    }

    [Fact]
    public async Task User_has_claim_to_update_manager_in_any_company___Forbidden_to_update_ceo()
    {
        var user = await _fixture.RunAsDefaultUserAsync(new[] { Claims.Company.Any.Manager.SetFromAnyToAny });
        var company = await _fixture.AddCompanyAsync();
        var command = CreateCopyData(company);
        command.Ceo = Faker.RandomString.Next();

        await Assert.ThrowsAsync<ForbiddenAccessException>(() => _fixture.SendAsync(command));
    }

    [Fact]
    public async Task User_has_claim_to_update_manager_in_any_company___Forbidden_to_update_contacts()
    {
        var user = await _fixture.RunAsDefaultUserAsync(new[] { Claims.Company.Any.Manager.SetFromAnyToAny });
        var company = await _fixture.AddCompanyAsync();
        var command = CreateCopyData(company);
        command.Contacts = Faker.RandomString.Next();

        await Assert.ThrowsAsync<ForbiddenAccessException>(() => _fixture.SendAsync(command));
    }

    [Fact]
    public async Task User_has_claim_to_update_manager_in_any_company___Forbidden_to_update_email()
    {
        var user = await _fixture.RunAsDefaultUserAsync(new[] { Claims.Company.Any.Manager.SetFromAnyToAny });
        var company = await _fixture.AddCompanyAsync();
        var command = CreateCopyData(company);
        command.Email = Faker.RandomString.Next();

        await Assert.ThrowsAsync<ForbiddenAccessException>(() => _fixture.SendAsync(command));
    }

    [Fact]
    public async Task User_has_claim_to_update_manager_in_any_company___Forbidden_to_update_inn()
    {
        var user = await _fixture.RunAsDefaultUserAsync(new[] { Claims.Company.Any.Manager.SetFromAnyToAny });
        var company = await _fixture.AddCompanyAsync();
        var command = CreateCopyData(company);
        command.Inn = Faker.RandomString.Next();

        await Assert.ThrowsAsync<ForbiddenAccessException>(() => _fixture.SendAsync(command));
    }

    [Fact]
    public async Task User_has_claim_to_update_manager_in_any_company___Forbidden_to_update_name()
    {
        var user = await _fixture.RunAsDefaultUserAsync(new[] { Claims.Company.Any.Manager.SetFromAnyToAny });
        var company = await _fixture.AddCompanyAsync();
        var command = CreateCopyData(company);
        command.Name = Faker.RandomString.Next();

        await Assert.ThrowsAsync<ForbiddenAccessException>(() => _fixture.SendAsync(command));
    }

    [Fact]
    public async Task User_has_claim_to_update_manager_in_any_company___Forbidden_to_update_phone()
    {
        var user = await _fixture.RunAsDefaultUserAsync(new[] { Claims.Company.Any.Manager.SetFromAnyToAny });
        var company = await _fixture.AddCompanyAsync();
        var command = CreateCopyData(company);
        command.Phone = Faker.RandomString.Next();

        await Assert.ThrowsAsync<ForbiddenAccessException>(() => _fixture.SendAsync(command));
    }

    [Fact]
    public async Task User_has_claim_to_update_manager_in_any_company___Forbidden_to_update_type()
    {
        var user = await _fixture.RunAsDefaultUserAsync(new[] { Claims.Company.Any.Manager.SetFromAnyToAny });
        var company = await _fixture.AddCompanyAsync();
        var command = CreateCopyData(company);
        command.TypeId = null;

        await Assert.ThrowsAsync<ForbiddenAccessException>(() => _fixture.SendAsync(command));
    }

    [Theory]
    [InlineData(Claims.Company.Any.Manager.SetFromNoneToSelf)]
    [InlineData(Claims.Company.Any.Manager.SetFromNoneToAny)]
    [InlineData(Claims.Company.Any.Manager.SetFromAnyToSelf)]
    [InlineData(Claims.Company.Any.Manager.SetFromAnyToAny)]
    public async Task User_has_claim_to_set_manager_from_none_to_self_in_any_company___Updates_manager(string claim)
    {
        var user = await _fixture.RunAsDefaultUserAsync(new[] { claim });
        var company = await _fixture.AddCompanyAsync();
        var command = CreateCopyData(company);
        command.ManagerId = user.Id;

        await _fixture.SendAsync(command);

        var actual = await _fixture.FindAsync<Company>(company.Id);
        AssertCompanyUpdated(user, actual);
        AssertManagerUpdated(command, actual);
        AssertOtherFieldsUnchanged(company, actual);
    }

    [Fact]
    public async Task User_has_no_claim_to_set_manager_from_none_to_self_in_any_company___Throws_forbidden_access()
    {
        var user = await _fixture.RunAsDefaultUserAsync();
        var company = await _fixture.AddCompanyAsync();
        var command = CreateCopyData(company);
        command.ManagerId = user.Id;

        await Assert.ThrowsAsync<ForbiddenAccessException>(() => _fixture.SendAsync(command));
    }

    [Theory]
    [InlineData(Claims.Company.Any.Manager.SetFromAnyToAny)]
    [InlineData(Claims.Company.Any.Manager.SetFromNoneToAny)]
    public async Task User_has_claim_to_set_manager_from_none_to_any_in_any_company___Updates_manager(string claim)
    {
        var user = await _fixture.RunAsDefaultUserAsync(new[] { claim });
        var anotherUser = await _fixture.AddUserAsync();
        var company = await _fixture.AddCompanyAsync();
        var command = CreateCopyData(company);
        command.ManagerId = anotherUser.Id;

        await AssertCompanyUpdatedAsync(user, company.Id, command);
    }

    [Fact]
    public async Task User_has_no_claim_to_set_manager_from_none_to_any_in_any_company___Throws_forbidden_access()
    {
        var user = await _fixture.RunAsDefaultUserAsync();
        var anotherUser = await _fixture.AddUserAsync();
        var company = await _fixture.AddCompanyAsync();
        var command = CreateCopyData(company);
        command.ManagerId = anotherUser.Id;

        await Assert.ThrowsAsync<ForbiddenAccessException>(() => _fixture.SendAsync(command));
    }

    [Theory]
    [InlineData(Claims.Company.Any.Manager.SetFromAnyToAny)]
    [InlineData(Claims.Company.Any.Manager.SetFromSelfToAny)]
    [InlineData(Claims.Company.Any.Manager.SetFromSelfToNone)]
    [InlineData(Claims.Company.Any.Manager.SetFromAnyToNone)]
    public async Task User_has_claim_to_set_manager_from_self_to_none_in_any_company___Updates_manager(string claim)
    {
        var user = await _fixture.RunAsDefaultUserAsync(new[] { claim, Claims.Company.Any.Other.Update });
        var company = await _fixture.AddCompanyAsync(user.Id);
        var command = CreateCopyData(company);
        command.ManagerId = null;

        await AssertCompanyUpdatedAsync(user, company.Id, command);
    }

    [Fact]
    public async Task User_has_no_claim_to_set_manager_from_self_to_none_in_any_company___Throws_forbidden_access()
    {
        var user = await _fixture.RunAsDefaultUserAsync();
        var company = await _fixture.AddCompanyAsync(user.Id);
        var command = CreateCopyData(company);
        command.ManagerId = null;

        await Assert.ThrowsAsync<ForbiddenAccessException>(() => _fixture.SendAsync(command));
    }

    [Theory]
    [InlineData(Claims.Company.Any.Manager.SetFromAnyToNone)]
    [InlineData(Claims.Company.Any.Manager.SetFromAnyToAny)]
    public async Task User_has_claim_to_set_manager_from_any_to_none_in_any_company___Updates_manager(string claim)
    {
        var user = await _fixture.RunAsDefaultUserAsync(new[] { claim });
        var someUser = await _fixture.AddUserAsync();
        var company = await _fixture.AddCompanyAsync(someUser.Id);
        var command = CreateCopyData(company);
        command.ManagerId = null;

        await AssertCompanyUpdatedAsync(user, company.Id, command);
    }

    [Fact]
    public async Task User_has_no_claim_to_set_manager_from_any_to_none_in_any_company___Throws_forbidden_access()
    {
        var user = await _fixture.RunAsDefaultUserAsync();
        var someUser = await _fixture.AddUserAsync();
        var company = await _fixture.AddCompanyAsync(someUser.Id);
        var command = CreateCopyData(company);
        command.ManagerId = null;

        await Assert.ThrowsAsync<ForbiddenAccessException>(() => _fixture.SendAsync(command));
    }

    [Theory]
    [InlineData(Claims.Company.Any.Manager.SetFromAnyToSelf)]
    [InlineData(Claims.Company.Any.Manager.SetFromAnyToAny)]
    public async Task User_has_claim_to_set_manager_from_any_to_self_in_any_company___Updates_manager(string claim)
    {
        var user = await _fixture.RunAsDefaultUserAsync(new[] { claim });
        var someUser = await _fixture.AddUserAsync();
        var company = await _fixture.AddCompanyAsync(someUser.Id);
        var command = CreateCopyData(company);
        command.ManagerId = user.Id;

        await AssertCompanyUpdatedAsync(user, company.Id, command);
    }

    [Fact]
    public async Task User_has_no_claim_to_set_manager_from_any_to_self_in_any_company___Throws_forbidden_access()
    {
        var user = await _fixture.RunAsDefaultUserAsync();
        var someUser = await _fixture.AddUserAsync();
        var company = await _fixture.AddCompanyAsync(someUser.Id);
        var command = CreateCopyData(company);
        command.ManagerId = user.Id;

        await Assert.ThrowsAsync<ForbiddenAccessException>(() => _fixture.SendAsync(command));

    }

    [Theory]
    [InlineData(Claims.Company.Any.Manager.SetFromSelfToAny)]
    [InlineData(Claims.Company.Any.Manager.SetFromAnyToAny)]
    public async Task User_has_claim_to_set_manager_from_self_to_any_in_any_company___Updates_manager(string claim)
    {
        var user = await _fixture.RunAsDefaultUserAsync(new[] { claim });
        var someUser = await _fixture.AddUserAsync();
        var company = await _fixture.AddCompanyAsync(user.Id);
        var command = CreateCopyData(company);
        command.ManagerId = someUser.Id;

        await AssertCompanyUpdatedAsync(user, company.Id, command);
    }

    [Fact]
    public async Task User_has_no_claim_to_set_manager_from_self_to_any_in_any_company___Throws_forbidden_access()
    {
        var user = await _fixture.RunAsDefaultUserAsync();
        var someUser = await _fixture.AddUserAsync();
        var company = await _fixture.AddCompanyAsync(user.Id);
        var command = CreateCopyData(company);
        command.ManagerId = someUser.Id;

        await Assert.ThrowsAsync<ForbiddenAccessException>(() => _fixture.SendAsync(command));
    }

    [Fact]
    public async Task User_has_claim_to_set_manager_from_any_to_any_in_any_company___Updates_manager()
    {
        var user = await _fixture.RunAsDefaultUserAsync(new[] { Claims.Company.Any.Manager.SetFromAnyToAny });
        var someUser = await _fixture.AddUserAsync();
        var anotherUser = await _fixture.AddUserAsync();
        var company = await _fixture.AddCompanyAsync(someUser.Id);
        var command = CreateCopyData(company);
        command.ManagerId = anotherUser.Id;

        await AssertCompanyUpdatedAsync(user, company.Id, command);
    }

    [Fact]
    public async Task User_has_no_claim_to_set_manager_from_any_to_any_in_any_company___Throws_forbidden_access()
    {
        var user = await _fixture.RunAsDefaultUserAsync();
        var someUser = await _fixture.AddUserAsync();
        var anotherUser = await _fixture.AddUserAsync();
        var company = await _fixture.AddCompanyAsync(someUser.Id);
        var command = CreateCopyData(company);
        command.ManagerId = anotherUser.Id;

        await Assert.ThrowsAsync<ForbiddenAccessException>(() => _fixture.SendAsync(command));
    }

    [Fact]
    public async Task User_has_claim_to_update_other_fields_in_own_company_and_is_manager___Updates_other_fields()
    {
        var user = await _fixture.RunAsDefaultUserAsync(new[] { Claims.Company.WhereUserIsManager.Other.Update });
        var company = await _fixture.AddCompanyAsync(user.Id);
        var command = CreateNewData(company.Id, user.Id);

        await AssertCompanyUpdatedAsync(user, company.Id, command);
    }

    [Fact]
    public async Task User_has_claim_to_update_other_fields_in_own_company_and_is_manager___Forbidden_to_update_manager()
    {
        var user = await _fixture.RunAsDefaultUserAsync(new[] { Claims.Company.WhereUserIsManager.Other.Update });
        var company = await _fixture.AddCompanyAsync(user.Id);
        var command = CreateNewData(company.Id);

        await Assert.ThrowsAsync<ForbiddenAccessException>(() => _fixture.SendAsync(command));
    }

    [Theory]
    [InlineData(Claims.Company.WhereUserIsManager.Manager.SetFromSelfToAny)]
    [InlineData(Claims.Company.WhereUserIsManager.Manager.SetFromSelfToNone)]
    public async Task User_has_claim_to_update_manager_in_own_company_and_is_manager___Updates_manager(string claim)
    {
        var user = await _fixture.RunAsDefaultUserAsync(new[] { claim });
        var company = await _fixture.AddCompanyAsync(user.Id);
        var command = CreateCopyData(company);
        command.ManagerId = null;

        await AssertCompanyUpdatedAsync(user, company.Id, command);
    }

    [Theory]
    [InlineData(Claims.Company.WhereUserIsManager.Manager.SetFromSelfToAny)]
    [InlineData(Claims.Company.WhereUserIsManager.Manager.SetFromSelfToNone)]
    public async Task User_has_claim_to_update_manager_in_own_company_and_is_manager___Forbiden_to_update_other_fields(string claim)
    {
        var user = await _fixture.RunAsDefaultUserAsync(new[] { claim });
        var company = await _fixture.AddCompanyAsync(user.Id);
        var command = CreateNewData(company.Id);

        await Assert.ThrowsAsync<ForbiddenAccessException>(() => _fixture.SendAsync(command));
    }

    [Theory]
    [InlineData(Claims.Company.WhereUserIsManager.Manager.SetFromSelfToNone)]
    [InlineData(Claims.Company.WhereUserIsManager.Manager.SetFromSelfToAny)]
    public async Task User_has_claim_to_set_manager_from_self_to_none_in_own_company_and_is_manager___Updates_manager(string claim)
    {
        var user = await _fixture.RunAsDefaultUserAsync(new[] { claim });
        var company = await _fixture.AddCompanyAsync(user.Id);
        var command = CreateCopyData(company);
        command.ManagerId = null;

        await AssertCompanyUpdatedAsync(user, company.Id, command);
    }

    [Fact]
    public async Task User_has_no_claim_to_set_manager_from_self_to_none_in_own_company_and_is_manager___Throws_forbidden_access()
    {
        var user = await _fixture.RunAsDefaultUserAsync();
        var company = await _fixture.AddCompanyAsync(user.Id);
        var command = CreateCopyData(company);
        command.ManagerId = null;

        await Assert.ThrowsAsync<ForbiddenAccessException>(() => _fixture.SendAsync(command));
    }

    [Fact]
    public async Task From_none_to_none_without_claims___Updates_company()
    {
        var user = await _fixture.RunAsDefaultUserAsync();
        var company = await _fixture.AddCompanyAsync();
        var command = CreateCopyData(company);

        await AssertCompanyUpdatedAsync(user, company.Id, command);
    }

    [Fact]
    public async Task From_manager_to_manager_without_claims___Updates_company()
    {
        var user = await _fixture.RunAsDefaultUserAsync();
        var company = await _fixture.AddCompanyAsync(user.Id);
        var command = CreateCopyData(company);
        command.ManagerId = user.Id;

        await AssertCompanyUpdatedAsync(user, company.Id, command);
    }

    private static UpdateCompanyCommand CreateNewData(int id, string? managerId = null)
    {
        var data = Faker.Builders.Company();
        return new UpdateCompanyCommand
        {
            Id = id,
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
    }

    private static UpdateCompanyCommand CreateCopyData(Company data)
    {
        return new UpdateCompanyCommand
        {
            Id = data.Id,
            Address = data.Address,
            Ceo = data.Ceo,
            Contacts = data.Contacts,
            Email = data.Email,
            Inn = data.Inn,
            Name = data.Name,
            Phone = data.Phone,
            TypeId = data.TypeId,
            ManagerId = data.ManagerId
        };
    }

    private async Task AssertCompanyUpdatedAsync(AspNetUser user, int companyId, UpdateCompanyCommand command)
    {
        await _fixture.SendAsync(command);
        var updatedCompany = await _fixture.FindAsync<Company>(companyId);

        Assert.Equal(BaseTestFixture.UtcNow, updatedCompany?.LastModifiedAtUtc);
        Assert.Equal(user.Id, updatedCompany?.LastModifiedBy);
        command.Should().BeEquivalentTo(updatedCompany, options =>
            options.ExcludingNestedObjects().ExcludingMissingMembers());
    }

    private static void AssertCompanyUpdated(AspNetUser user, Company? actual)
    {
        Assert.Equal(BaseTestFixture.UtcNow, actual?.LastModifiedAtUtc);
        Assert.Equal(user.Id, actual?.LastModifiedBy);
    }

    private static void AssertOtherFieldsUpdated(UpdateCompanyCommand? expected, Company? actual)
    {
        Assert.Equal(expected?.TypeId, actual?.TypeId);
        Assert.Equal(expected?.Address, actual?.Address);
        Assert.Equal(expected?.Ceo, actual?.Ceo);
        Assert.Equal(expected?.Contacts, actual?.Contacts);
        Assert.Equal(expected?.Email, actual?.Email);
        Assert.Equal(expected?.Inn, actual?.Inn);
        Assert.Equal(expected?.Name, actual?.Name);
        Assert.Equal(expected?.Phone, actual?.Phone);
    }

    private static void AssertOtherFieldsUnchanged(Company? expected, Company? actual)
    {
        Assert.Equal(expected?.TypeId, actual?.TypeId);
        Assert.Equal(expected?.Address, actual?.Address);
        Assert.Equal(expected?.Ceo, actual?.Ceo);
        Assert.Equal(expected?.Contacts, actual?.Contacts);
        Assert.Equal(expected?.Email, actual?.Email);
        Assert.Equal(expected?.Inn, actual?.Inn);
        Assert.Equal(expected?.Name, actual?.Name);
        Assert.Equal(expected?.Phone, actual?.Phone);
    }

    private static void AssertManagerUpdated(UpdateCompanyCommand? expected, Company? actual)
    {
        Assert.Equal(expected?.ManagerId, actual?.ManagerId);
    }

    private static void AssertManagerUnchanged(Company? expected, Company? actual)
    {
        Assert.Equal(expected?.ManagerId, actual?.ManagerId);
    }
}