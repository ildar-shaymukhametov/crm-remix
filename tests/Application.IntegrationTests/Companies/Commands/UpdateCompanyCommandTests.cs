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
        var command = CreateCommand(company.Id, managerId: manager.Id);

        await AssertOtherFieldsUpdatedAsync(user, company.Id, command);
    }

    [Fact]
    public async Task User_can_update_any_field_in_any_company___Updates_company()
    {
        var user = await _fixture.RunAsDefaultUserAsync(new[] { Constants.Claims.Company.Any.Update });
        var company = await _fixture.AddCompanyAsync();
        var manager = await _fixture.CreateUserAsync();
        var command = CreateCommand(company.Id, managerId: manager.Id);

        await AssertOtherFieldsUpdatedAsync(user, company.Id, command);
    }

    [Fact]
    public async Task User_has_no_claim___Throws_forbidden_access()
    {
        var user = await _fixture.RunAsDefaultUserAsync();
        var company = await _fixture.AddCompanyAsync();
        var command = CreateCommand(company.Id);

        var ex = await Assert.ThrowsAsync<ForbiddenAccessException>(() => _fixture.SendAsync(command));
        Assert.Contains("Update company", ex.Message, StringComparison.CurrentCultureIgnoreCase);
    }

    [Fact]
    public async Task Not_found()
    {
        await _fixture.RunAsDefaultUserAsync(new[]
        {
            Constants.Claims.Company.Any.Other.Update
        });
        var command = CreateCommand(1);
        await Assert.ThrowsAsync<NotFoundException>(() => _fixture.SendAsync(command));
    }

    [Fact]
    public async Task User_can_update_other_fields_in_any_company___Updates_other_fields()
    {
        var user = await _fixture.RunAsDefaultUserAsync(new[] { Constants.Claims.Company.Any.Other.Update });
        var company = await _fixture.AddCompanyAsync();
        var command = CreateCommand(company.Id);

        await AssertOtherFieldsUpdatedAsync(user, company.Id, command);
    }

    [Fact]
    public async Task User_can_update_other_fields_in_any_company___Forbidden_to_update_manager()
    {
        var user = await _fixture.RunAsDefaultUserAsync(new[] { Constants.Claims.Company.Any.Other.Update });
        var company = await _fixture.AddCompanyAsync();
        var command = CreateCommand(company.Id, user.Id);

        var ex = await Assert.ThrowsAsync<ForbiddenAccessException>(() => _fixture.SendAsync(command));
        Assert.Contains("Update manager", ex.Message, StringComparison.CurrentCultureIgnoreCase);
    }

    [Fact]
    public async Task User_can_update_manager_in_any_company___Updates_manager()
    {
        var user = await _fixture.RunAsDefaultUserAsync(new[] { Constants.Claims.Company.Any.Manager.SetFromAnyToAny });
        var company = await _fixture.AddCompanyAsync();
        var command = CreateCommand(company, managerId: user.Id);

        await AssertManagerUpdatedAsync(user, company.Id, command);
    }

    [Theory]
    [InlineData(Constants.Claims.Company.Any.Manager.SetFromAnyToAny)]
    [InlineData(Constants.Claims.Company.Any.Manager.SetFromAnyToNone)]
    [InlineData(Constants.Claims.Company.Any.Manager.SetFromAnyToSelf)]
    [InlineData(Constants.Claims.Company.Any.Manager.SetFromNoneToAny)]
    [InlineData(Constants.Claims.Company.Any.Manager.SetFromNoneToSelf)]
    [InlineData(Constants.Claims.Company.Any.Manager.SetFromSelfToAny)]
    [InlineData(Constants.Claims.Company.Any.Manager.SetFromSelfToNone)]
    public async Task User_can_update_manager_in_any_company___Forbidden_to_update_address(string claim)
    {
        var user = await _fixture.RunAsDefaultUserAsync(new[] { claim });
        var company = await _fixture.AddCompanyAsync();
        var command = CreateCommand(company, address: string.Empty);

        var ex = await Assert.ThrowsAsync<ForbiddenAccessException>(() => _fixture.SendAsync(command));
        Assert.Contains("Update other fields", ex.Message, StringComparison.CurrentCultureIgnoreCase);
    }

    [Fact]
    public async Task User_can_update_manager_in_any_company___Forbidden_to_update_ceo()
    {
        var user = await _fixture.RunAsDefaultUserAsync(new[] { Constants.Claims.Company.Any.Manager.SetFromAnyToAny });
        var company = await _fixture.AddCompanyAsync();
        var command = CreateCommand(company, ceo: string.Empty);

        var ex = await Assert.ThrowsAsync<ForbiddenAccessException>(() => _fixture.SendAsync(command));
        Assert.Contains("Update other fields", ex.Message, StringComparison.CurrentCultureIgnoreCase);
    }

    [Fact]
    public async Task User_can_update_manager_in_any_company___Forbidden_to_update_contacts()
    {
        var user = await _fixture.RunAsDefaultUserAsync(new[] { Constants.Claims.Company.Any.Manager.SetFromAnyToAny });
        var company = await _fixture.AddCompanyAsync();
        var command = CreateCommand(company, contacts: string.Empty);

        var ex = await Assert.ThrowsAsync<ForbiddenAccessException>(() => _fixture.SendAsync(command));
        Assert.Contains("Update other fields", ex.Message, StringComparison.CurrentCultureIgnoreCase);
    }

    [Fact]
    public async Task User_can_update_manager_in_any_company___Forbidden_to_update_email()
    {
        var user = await _fixture.RunAsDefaultUserAsync(new[] { Constants.Claims.Company.Any.Manager.SetFromAnyToAny });
        var company = await _fixture.AddCompanyAsync();
        var command = CreateCommand(company, email: string.Empty);

        var ex = await Assert.ThrowsAsync<ForbiddenAccessException>(() => _fixture.SendAsync(command));
        Assert.Contains("Update other fields", ex.Message, StringComparison.CurrentCultureIgnoreCase);
    }

    [Fact]
    public async Task User_can_update_manager_in_any_company___Forbidden_to_update_inn()
    {
        var user = await _fixture.RunAsDefaultUserAsync(new[] { Constants.Claims.Company.Any.Manager.SetFromAnyToAny });
        var company = await _fixture.AddCompanyAsync();
        var command = CreateCommand(company, inn: string.Empty);

        var ex = await Assert.ThrowsAsync<ForbiddenAccessException>(() => _fixture.SendAsync(command));
        Assert.Contains("Update other fields", ex.Message, StringComparison.CurrentCultureIgnoreCase);
    }

    [Fact]
    public async Task User_can_update_manager_in_any_company___Forbidden_to_update_name()
    {
        var user = await _fixture.RunAsDefaultUserAsync(new[] { Constants.Claims.Company.Any.Manager.SetFromAnyToAny });
        var company = await _fixture.AddCompanyAsync();
        var command = CreateCommand(company, name: string.Empty);

        var ex = await Assert.ThrowsAsync<ForbiddenAccessException>(() => _fixture.SendAsync(command));
        Assert.Contains("Update other fields", ex.Message, StringComparison.CurrentCultureIgnoreCase);
    }

    [Fact]
    public async Task User_can_update_manager_in_any_company___Forbidden_to_update_phone()
    {
        var user = await _fixture.RunAsDefaultUserAsync(new[] { Constants.Claims.Company.Any.Manager.SetFromAnyToAny });
        var company = await _fixture.AddCompanyAsync();
        var command = CreateCommand(company, phone: string.Empty);

        var ex = await Assert.ThrowsAsync<ForbiddenAccessException>(() => _fixture.SendAsync(command));
        Assert.Contains("Update other fields", ex.Message, StringComparison.CurrentCultureIgnoreCase);
    }

    [Fact]
    public async Task User_can_update_manager_in_any_company___Forbidden_to_update_type()
    {
        var user = await _fixture.RunAsDefaultUserAsync(new[] { Constants.Claims.Company.Any.Manager.SetFromAnyToAny });
        var company = await _fixture.AddCompanyAsync();
        var command = CreateCommand(company, typeId: 0);

        var ex = await Assert.ThrowsAsync<ForbiddenAccessException>(() => _fixture.SendAsync(command));
        Assert.Contains("Update other fields", ex.Message, StringComparison.CurrentCultureIgnoreCase);
    }

    [Theory]
    [InlineData(Constants.Claims.Company.Any.Manager.SetFromNoneToSelf)]
    [InlineData(Constants.Claims.Company.Any.Manager.SetFromNoneToAny)]
    [InlineData(Constants.Claims.Company.Any.Manager.SetFromAnyToSelf)]
    [InlineData(Constants.Claims.Company.Any.Manager.SetFromAnyToAny)]
    public async Task User_has_claim_to_set_manager_from_none_to_self_in_any_company___Updates_manager(string claim)
    {
        var user = await _fixture.RunAsDefaultUserAsync(new[] { claim });
        var company = await _fixture.AddCompanyAsync();
        var command = CreateCommand(company, managerId: user.Id);

        await AssertManagerUpdatedAsync(user, company.Id, command);
    }

    [Theory]
    [InlineData(Constants.Claims.Company.Any.Manager.SetFromAnyToNone)]
    [InlineData(Constants.Claims.Company.Any.Manager.SetFromSelfToAny)]
    [InlineData(Constants.Claims.Company.Any.Manager.SetFromSelfToNone)]
    public async Task User_has_no_claim_to_set_manager_from_none_to_self_in_any_company___Throws_forbidden_access(string claim)
    {
        var user = await _fixture.RunAsDefaultUserAsync(new[] { claim });
        var company = await _fixture.AddCompanyAsync();
        var command = CreateCommand(company, managerId: user.Id);

        var ex = await Assert.ThrowsAsync<ForbiddenAccessException>(() => _fixture.SendAsync(command));
        Assert.Contains("Set manager from none to self in any company", ex.Message, StringComparison.CurrentCultureIgnoreCase);
    }

    [Theory]
    [InlineData(Constants.Claims.Company.Any.Manager.SetFromAnyToAny)]
    [InlineData(Constants.Claims.Company.Any.Manager.SetFromNoneToAny)]
    public async Task User_has_claim_to_set_manager_from_none_to_any_in_any_company___Updates_manager(string claim)
    {
        var user = await _fixture.RunAsDefaultUserAsync(new[] { claim });
        var anotherUser = await _fixture.AddUserAsync();
        var company = await _fixture.AddCompanyAsync();
        var command = CreateCommand(company, managerId: anotherUser.Id);

        await AssertManagerUpdatedAsync(user, company.Id, command);
    }

    [Theory]
    [InlineData(Constants.Claims.Company.Any.Manager.SetFromAnyToNone)]
    [InlineData(Constants.Claims.Company.Any.Manager.SetFromSelfToNone)]
    [InlineData(Constants.Claims.Company.Any.Manager.SetFromSelfToAny)]
    [InlineData(Constants.Claims.Company.Any.Manager.SetFromAnyToSelf)]
    [InlineData(Constants.Claims.Company.Any.Manager.SetFromNoneToSelf)]
    public async Task User_has_no_claim_to_set_manager_from_none_to_any_in_any_company___Throws_forbidden_access(string claim)
    {
        var user = await _fixture.RunAsDefaultUserAsync(new[] { claim });
        var anotherUser = await _fixture.AddUserAsync();
        var company = await _fixture.AddCompanyAsync();
        var command = CreateCommand(company, managerId: anotherUser.Id);

        var ex = await Assert.ThrowsAsync<ForbiddenAccessException>(() => _fixture.SendAsync(command));
        Assert.Contains("Set manager from none to any in any company", ex.Message, StringComparison.CurrentCultureIgnoreCase);
    }

    [Theory]
    [InlineData(Constants.Claims.Company.Any.Manager.SetFromAnyToAny)]
    [InlineData(Constants.Claims.Company.Any.Manager.SetFromSelfToAny)]
    [InlineData(Constants.Claims.Company.Any.Manager.SetFromSelfToNone)]
    [InlineData(Constants.Claims.Company.Any.Manager.SetFromAnyToNone)]
    public async Task User_has_claim_to_set_manager_from_self_to_none_in_any_company___Updates_manager(string claim)
    {
        var user = await _fixture.RunAsDefaultUserAsync(new[] { claim, Constants.Claims.Company.Any.Other.Update });
        var company = await _fixture.AddCompanyAsync(user.Id);
        var command = CreateCommand(company);

        await AssertManagerUpdatedAsync(user, company.Id, command);
    }

    [Theory]
    [InlineData(Constants.Claims.Company.Any.Manager.SetFromNoneToAny)]
    [InlineData(Constants.Claims.Company.Any.Manager.SetFromNoneToSelf)]
    [InlineData(Constants.Claims.Company.Any.Manager.SetFromAnyToSelf)]
    public async Task User_has_no_claim_to_set_manager_from_self_to_none_in_any_company___Throws_forbidden_access(string claim)
    {
        var user = await _fixture.RunAsDefaultUserAsync(new[] { claim });
        var someUser = await _fixture.AddUserAsync();
        var company = await _fixture.AddCompanyAsync(user.Id);
        var command = CreateCommand(company);

        var ex = await Assert.ThrowsAsync<ForbiddenAccessException>(() => _fixture.SendAsync(command));
        Assert.Contains("Set manager from self to none in any company", ex.Message, StringComparison.CurrentCultureIgnoreCase);
    }

    [Theory]
    [InlineData(Constants.Claims.Company.Any.Manager.SetFromAnyToNone)]
    [InlineData(Constants.Claims.Company.Any.Manager.SetFromAnyToAny)]
    public async Task User_has_claim_to_set_manager_from_any_to_none_in_any_company___Updates_manager(string claim)
    {
        var user = await _fixture.RunAsDefaultUserAsync(new[] { claim });
        var someUser = await _fixture.AddUserAsync();
        var company = await _fixture.AddCompanyAsync(someUser.Id);
        var command = CreateCommand(company);

        await AssertManagerUpdatedAsync(user, company.Id, command);
    }

    [Theory]
    [InlineData(Constants.Claims.Company.Any.Manager.SetFromAnyToSelf)]
    [InlineData(Constants.Claims.Company.Any.Manager.SetFromNoneToAny)]
    [InlineData(Constants.Claims.Company.Any.Manager.SetFromNoneToSelf)]
    [InlineData(Constants.Claims.Company.Any.Manager.SetFromSelfToAny)]
    [InlineData(Constants.Claims.Company.Any.Manager.SetFromSelfToNone)]
    public async Task User_has_no_claim_to_set_manager_from_any_to_none_in_any_company___Throws_forbidden_access(string claim)
    {
        var user = await _fixture.RunAsDefaultUserAsync(new[] { claim });
        var someUser = await _fixture.AddUserAsync();
        var company = await _fixture.AddCompanyAsync(someUser.Id);
        var command = CreateCommand(company);

        var ex = await Assert.ThrowsAsync<ForbiddenAccessException>(() => _fixture.SendAsync(command));
        Assert.Contains("Set manager from any to none in any company", ex.Message, StringComparison.CurrentCultureIgnoreCase);
    }

    [Theory]
    [InlineData(Constants.Claims.Company.Any.Manager.SetFromAnyToSelf)]
    [InlineData(Constants.Claims.Company.Any.Manager.SetFromAnyToAny)]
    public async Task User_has_claim_to_set_manager_from_any_to_self_in_any_company___Updates_manager(string claim)
    {
        var user = await _fixture.RunAsDefaultUserAsync(new[] { claim });
        var someUser = await _fixture.AddUserAsync();
        var company = await _fixture.AddCompanyAsync(someUser.Id);
        var command = CreateCommand(company, managerId: user.Id);

        await AssertManagerUpdatedAsync(user, company.Id, command);
    }

    [Theory]
    [InlineData(Constants.Claims.Company.Any.Manager.SetFromAnyToNone)]
    [InlineData(Constants.Claims.Company.Any.Manager.SetFromNoneToAny)]
    [InlineData(Constants.Claims.Company.Any.Manager.SetFromNoneToSelf)]
    [InlineData(Constants.Claims.Company.Any.Manager.SetFromSelfToAny)]
    [InlineData(Constants.Claims.Company.Any.Manager.SetFromSelfToNone)]
    public async Task User_has_no_claim_to_set_manager_from_any_to_self_in_any_company___Throws_forbidden_access(string claim)
    {
        var user = await _fixture.RunAsDefaultUserAsync(new[] { claim });
        var someUser = await _fixture.AddUserAsync();
        var company = await _fixture.AddCompanyAsync(someUser.Id);
        var command = CreateCommand(company, managerId: user.Id);

        var ex = await Assert.ThrowsAsync<ForbiddenAccessException>(() => _fixture.SendAsync(command));
        Assert.Contains("Set manager from any to self in any company", ex.Message, StringComparison.CurrentCultureIgnoreCase);

    }

    [Theory]
    [InlineData(Constants.Claims.Company.Any.Manager.SetFromSelfToAny)]
    [InlineData(Constants.Claims.Company.Any.Manager.SetFromAnyToAny)]
    public async Task User_has_claim_to_set_manager_from_self_to_any_in_any_company___Updates_manager(string claim)
    {
        var user = await _fixture.RunAsDefaultUserAsync(new[] { claim });
        var someUser = await _fixture.AddUserAsync();
        var company = await _fixture.AddCompanyAsync(user.Id);
        var command = CreateCommand(company, managerId: someUser.Id);

        await AssertManagerUpdatedAsync(user, company.Id, command);
    }

    [Theory]
    [InlineData(Constants.Claims.Company.Any.Manager.SetFromAnyToNone)]
    [InlineData(Constants.Claims.Company.Any.Manager.SetFromAnyToSelf)]
    [InlineData(Constants.Claims.Company.Any.Manager.SetFromNoneToAny)]
    [InlineData(Constants.Claims.Company.Any.Manager.SetFromNoneToSelf)]
    [InlineData(Constants.Claims.Company.Any.Manager.SetFromSelfToNone)]
    public async Task User_has_no_claim_to_set_manager_from_self_to_any_in_any_company___Throws_forbidden_access(string claim)
    {
        var user = await _fixture.RunAsDefaultUserAsync(new[] { claim });
        var someUser = await _fixture.AddUserAsync();
        var company = await _fixture.AddCompanyAsync(user.Id);
        var command = CreateCommand(company, managerId: someUser.Id);

        var ex = await Assert.ThrowsAsync<ForbiddenAccessException>(() => _fixture.SendAsync(command));
        Assert.Contains("Set manager from self to any in any company", ex.Message, StringComparison.CurrentCultureIgnoreCase);
    }

    // [Theory]
    // [InlineData(Constants.Claims.Company.Any.Manager.SetFromAnyToAny)]
    // public async Task User_has_claim_to_set_manager_from_any_to_any_in_any_company___Updates_manager(string claim)
    // {
    //     var user = await _fixture.RunAsDefaultUserAsync(new[] { claim, Constants.Claims.Company.Any.Other.Update });
    //     var someUser = await _fixture.AddUserAsync();
    //     var anotherUser = await _fixture.AddUserAsync();
    //     var company = await _fixture.AddCompanyAsync(someUser.Id);
    //     var command = CreateCommand(company.Id, managerId: anotherUser.Id);

    //     await AssertCompanyUpdatedAsync(user, company.Id, command);
    // }

    // [Fact]
    // public async Task User_has_no_claim_to_set_manager_from_any_to_any_in_any_company___Throws_forbidden_access()
    // {
    //     var user = await _fixture.RunAsDefaultUserAsync(new[] { Constants.Claims.Company.Any.Other.Update });
    //     var someUser = await _fixture.AddUserAsync();
    //     var anotherUser = await _fixture.AddUserAsync();
    //     var company = await _fixture.AddCompanyAsync(someUser.Id);
    //     var command = CreateCommand(company.Id, managerId: anotherUser.Id);

    //     var ex = await Assert.ThrowsAsync<ForbiddenAccessException>(() => _fixture.SendAsync(command));
    //     Assert.Contains("Set manager from any to any in any company", ex.Message, StringComparison.CurrentCultureIgnoreCase);
    // }

    // [Fact]
    // public async Task User_has_claim_to_update_own_company_and_he_is_manager___Updates_company()
    // {
    //     var user = await _fixture.RunAsDefaultUserAsync(new []
    //     {
    //         Constants.Claims.Company.WhereUserIsManager.Other.Update,
    //         Constants.Claims.Company.Any.Manager.SetFromAnyToAny
    //     });
    //     var company = await _fixture.AddCompanyAsync(user.Id);
    //     var command = CreateCommand(company.Id);

    //     await AssertCompanyUpdatedAsync(user, company.Id, command);
    // }

    // [Fact]
    // public async Task User_has_claim_to_update_own_company_and_he_is_not_manager___Throws_forbidden_access()
    // {
    //     await _fixture.RunAsDefaultUserAsync(new[]
    //     {
    //         Constants.Claims.Company.WhereUserIsManager.Other.Update,
    //         Constants.Claims.Company.Any.Manager.SetFromAnyToAny
    //     });
    //     var company = await _fixture.AddCompanyAsync();
    //     var command = CreateCommand(company.Id);

    //     var ex = await Assert.ThrowsAsync<ForbiddenAccessException>(() => _fixture.SendAsync(command));
    //     Assert.Contains("Update any company", ex.Message, StringComparison.CurrentCultureIgnoreCase);
    // }

    // [Fact]
    // public async Task User_has_no_claim_to_update_own_company_and_he_is_manager___Throws_forbidden_access()
    // {
    //     var user = await _fixture.RunAsDefaultUserAsync(new[]
    //     {
    //         Constants.Claims.Company.Any.Manager.SetFromAnyToAny
    //     });
    //     var company = await _fixture.AddCompanyAsync(user.Id);
    //     var command = CreateCommand(company.Id);

    //     var ex = await Assert.ThrowsAsync<ForbiddenAccessException>(() => _fixture.SendAsync(command));
    //     Assert.Contains("Update own company", ex.Message, StringComparison.CurrentCultureIgnoreCase);
    // }

    // [Fact]
    // public async Task User_has_no_claim_to_update_own_company_and_he_is_not_manager___Throws_forbidden_access()
    // {
    //     await _fixture.RunAsDefaultUserAsync(new[]
    //     {
    //         Constants.Claims.Company.Any.Manager.SetFromAnyToAny
    //     });
    //     var company = await _fixture.AddCompanyAsync();
    //     var command = CreateCommand(company.Id);

    //     var ex = await Assert.ThrowsAsync<ForbiddenAccessException>(() => _fixture.SendAsync(command));
    //     Assert.Contains("Update any company", ex.Message, StringComparison.CurrentCultureIgnoreCase);
    // }

    // [Fact]
    // public async Task From_none_to_none_without_claims___Updates_company()
    // {
    //     var user = await _fixture.RunAsDefaultUserAsync(new[] { Constants.Claims.Company.Any.Other.Update });
    //     var company = await _fixture.AddCompanyAsync();
    //     var command = CreateCommand(company.Id);

    //     await AssertCompanyUpdatedAsync(user, company.Id, command);
    // }

    // [Fact]
    // public async Task From_manager_to_manager_without_claims___Updates_company()
    // {
    //     var user = await _fixture.RunAsDefaultUserAsync(new[] { Constants.Claims.Company.Any.Other.Update });
    //     var company = await _fixture.AddCompanyAsync(user.Id);
    //     var command = CreateCommand(company.Id, user.Id);

    //     await AssertCompanyUpdatedAsync(user, company.Id, command);
    // }

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
            TypeId = data.TypeId,
            ManagerId = managerId
        };

        return command;
    }

    private static UpdateCompanyCommand CreateCommand(Company data, string? address = null, string? ceo = null, string? contacts = null, string? email = null, string? inn = null, string? name = null, string? phone = null, int? typeId = null, string? managerId = null)
    {
        var command = new UpdateCompanyCommand
        {
            Id = data.Id,
            Address = address ?? data.Address,
            Ceo = ceo ?? data.Ceo,
            Contacts = contacts ?? data.Contacts,
            Email = email ?? data.Email,
            Inn = inn ?? data.Inn,
            Name = name ?? data.Name,
            Phone = phone ?? data.Phone,
            TypeId = typeId ?? data.TypeId,
            ManagerId = managerId
        };

        return command;
    }

    private async Task AssertOtherFieldsUpdatedAsync(AspNetUser user, int companyId, UpdateCompanyCommand command)
    {
        await _fixture.SendAsync(command);
        var updatedCompany = await _fixture.FindAsync<Company>(companyId);

        Assert.Equal(BaseTestFixture.UtcNow, updatedCompany?.LastModifiedAtUtc);
        Assert.Equal(user.Id, updatedCompany?.LastModifiedBy);
        command.Should().BeEquivalentTo(updatedCompany, options =>
            options.ExcludingNestedObjects().ExcludingMissingMembers());
    }

    private async Task AssertManagerUpdatedAsync(AspNetUser user, int companyId, UpdateCompanyCommand command)
    {
        await _fixture.SendAsync(command);
        var updatedCompany = await _fixture.FindAsync<Company>(companyId);

        Assert.Equal(BaseTestFixture.UtcNow, updatedCompany?.LastModifiedAtUtc);
        Assert.Equal(user.Id, updatedCompany?.LastModifiedBy);
        Assert.Equal(command.ManagerId, updatedCompany?.ManagerId);
    }
}