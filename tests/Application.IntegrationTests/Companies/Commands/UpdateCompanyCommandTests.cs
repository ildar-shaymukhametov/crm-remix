using CRM.Application.Common.Exceptions;
using CRM.Application.Companies.Commands.UpdateCompany;
using CRM.Domain.Entities;
using CRM.Infrastructure.Identity;
using static CRM.Application.Constants;

namespace CRM.Application.IntegrationTests.Companies.Commands;

public class UpdateCompanyTests : BaseTest
{
    public UpdateCompanyTests(BaseTestFixture fixture) : base(fixture) { }

    [Fact]
    public async Task User_is_admin___Updates_company()
    {
        var user = await _fixture.RunAsAdministratorAsync();
        var company = await _fixture.AddCompanyAsync();
        var manager = await _fixture.CreateUserAsync();
        var command = CreateAllFields(company.Id, managerId: manager.Id);

        await AssertCompanyUpdatedAsync(user, company, command, true, true, true);
    }

    [Fact]
    public async Task No_claims_and_some_fields___Forbidden()
    {
        var user = await _fixture.RunAsDefaultUserAsync();
        var company = await _fixture.AddCompanyAsync();
        var command = CreateAllFields(company.Id);

        await Assert.ThrowsAsync<ForbiddenAccessException>(() => _fixture.SendAsync(command));
    }

    [Fact]
    public async Task No_claims_and_no_fields___Forbidden()
    {
        await _fixture.RunAsDefaultUserAsync();
        var company = await _fixture.AddCompanyAsync();

        await Assert.ThrowsAsync<ForbiddenAccessException>(() => _fixture.SendAsync(new UpdateCompanyCommand(company.Id)));
    }

    [Fact]
    public async Task User_authorized_but_provides_no_fields___Throws()
    {
        var user = await _fixture.RunAsDefaultUserAsync(new[] { Claims.Company.Any.Other.Set });
        var company = await _fixture.AddCompanyAsync();

        await Assert.ThrowsAsync<ValidationException>(() => _fixture.SendAsync(new UpdateCompanyCommand(company.Id)));
    }

    [Fact]
    public async Task User_authorized_but_provides_incomplete_fields___Updates_provided_fields()
    {
        var user = await _fixture.RunAsDefaultUserAsync(new[]
        {
            Claims.Company.Any.Other.Set,
            Claims.Company.Any.Name.Set,
            Claims.Company.Any.Manager.SetFromAnyToAny
        });
        var company = await _fixture.AddCompanyAsync();
        var command = new UpdateCompanyCommand(company.Id);
        command.Fields.Add(nameof(Company.Name), Faker.RandomString.Next());
        command.Fields.Add(nameof(Company.ManagerId), user.Id);

        await AssertCompanyUpdatedAsync(user, company, command, nameField: true, managerField: true);
    }

    [Fact]
    public async Task Not_found()
    {
        await _fixture.RunAsDefaultUserAsync(new[] { Claims.Company.Any.Other.Set });
        var command = CreateAllFields(1);
        await Assert.ThrowsAsync<NotFoundException>(() => _fixture.SendAsync(command));
    }

    [Fact]
    public async Task User_has_claim_to_update_other_fields_in_any_company___Updates_other_fields()
    {
        var user = await _fixture.RunAsDefaultUserAsync(new[] { Claims.Company.Any.Other.Set });
        var company = await _fixture.AddCompanyAsync();
        var command = CreateOtherFields(company);

        await AssertCompanyUpdatedAsync(user, company, command, otherFields: true);
    }

    [Fact]
    public async Task User_has_claim_to_update_other_fields_in_own_company_and_is_manager___Updates_other_fields()
    {
        var user = await _fixture.RunAsDefaultUserAsync(new[] { Claims.Company.WhereUserIsManager.Other.Set });
        var company = await _fixture.AddCompanyAsync(user.Id);
        var command = CreateOtherFields(company);

        await AssertCompanyUpdatedAsync(user, company, command, otherFields: true);
    }

    [Fact]
    public async Task No_manager_in_company___Forbidden_to_update_address()
    {
        await _fixture.RunAsDefaultUserAsync(new[] { Claims.Company.WhereUserIsManager.Other.Set });
        var company = await _fixture.AddCompanyAsync();
        var command = new UpdateCompanyCommand(company.Id);
        command.Fields.Add(nameof(Company.Address), Faker.RandomString.Next());

        await Assert.ThrowsAsync<ForbiddenAccessException>(() => _fixture.SendAsync(command));
    }

    [Fact]
    public async Task No_manager_in_company___Forbidden_to_update_ceo()
    {
        await _fixture.RunAsDefaultUserAsync(new[] { Claims.Company.WhereUserIsManager.Other.Set });
        var company = await _fixture.AddCompanyAsync();
        var command = new UpdateCompanyCommand(company.Id);
        command.Fields.Add(nameof(Company.Ceo), Faker.RandomString.Next());

        await Assert.ThrowsAsync<ForbiddenAccessException>(() => _fixture.SendAsync(command));
    }

    [Fact]
    public async Task No_manager_in_company___Forbidden_to_update_email()
    {
        await _fixture.RunAsDefaultUserAsync(new[] { Claims.Company.WhereUserIsManager.Other.Set });
        var company = await _fixture.AddCompanyAsync();
        var command = new UpdateCompanyCommand(company.Id);
        command.Fields.Add(nameof(Company.Email), Faker.RandomString.Next());

        await Assert.ThrowsAsync<ForbiddenAccessException>(() => _fixture.SendAsync(command));
    }

    [Fact]
    public async Task No_manager_in_company___Forbidden_to_update_inn()
    {
        await _fixture.RunAsDefaultUserAsync(new[] { Claims.Company.WhereUserIsManager.Other.Set });
        var company = await _fixture.AddCompanyAsync();
        var command = new UpdateCompanyCommand(company.Id);
        command.Fields.Add(nameof(Company.Inn), Faker.RandomString.Next());

        await Assert.ThrowsAsync<ForbiddenAccessException>(() => _fixture.SendAsync(command));
    }

    [Fact]
    public async Task No_manager_in_company___Forbidden_to_update_phone()
    {
        await _fixture.RunAsDefaultUserAsync(new[] { Claims.Company.WhereUserIsManager.Other.Set });
        var company = await _fixture.AddCompanyAsync();
        var command = new UpdateCompanyCommand(company.Id);
        command.Fields.Add(nameof(Company.Phone), Faker.RandomString.Next());

        await Assert.ThrowsAsync<ForbiddenAccessException>(() => _fixture.SendAsync(command));
    }

    [Fact]
    public async Task No_manager_in_company___Forbidden_to_update_type()
    {
        await _fixture.RunAsDefaultUserAsync(new[] { Claims.Company.WhereUserIsManager.Other.Set });
        var company = await _fixture.AddCompanyAsync();
        var command = new UpdateCompanyCommand(company.Id);
        command.Fields.Add(nameof(Company.Address), null);

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
        var command = new UpdateCompanyCommand(company.Id);
        command.Fields.Add(nameof(Company.ManagerId), user.Id);

        await AssertCompanyUpdatedAsync(user, company, command, managerField: true);
    }

    [Fact]
    public async Task User_has_no_claim_to_set_manager_from_none_to_self_in_any_company___Forbidden()
    {
        var user = await _fixture.RunAsDefaultUserAsync();
        var company = await _fixture.AddCompanyAsync();
        var command = new UpdateCompanyCommand(company.Id);
        command.Fields.Add(nameof(Company.ManagerId), user.Id);

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
        var command = new UpdateCompanyCommand(company.Id);
        command.Fields.Add(nameof(Company.ManagerId), anotherUser.Id);

        await AssertCompanyUpdatedAsync(user, company, command, managerField: true);
    }

    [Fact]
    public async Task User_has_no_claim_to_set_manager_from_none_to_any_in_any_company___Forbidden()
    {
        var user = await _fixture.RunAsDefaultUserAsync();
        var anotherUser = await _fixture.AddUserAsync();
        var company = await _fixture.AddCompanyAsync();
        var command = new UpdateCompanyCommand(company.Id);
        command.Fields.Add(nameof(Company.ManagerId), anotherUser.Id);

        await Assert.ThrowsAsync<ForbiddenAccessException>(() => _fixture.SendAsync(command));
    }

    [Theory]
    [InlineData(Claims.Company.Any.Manager.SetFromAnyToAny)]
    [InlineData(Claims.Company.Any.Manager.SetFromSelfToAny)]
    [InlineData(Claims.Company.Any.Manager.SetFromSelfToNone)]
    [InlineData(Claims.Company.Any.Manager.SetFromAnyToNone)]
    public async Task User_has_claim_to_set_manager_from_self_to_none_in_any_company___Updates_manager(string claim)
    {
        var user = await _fixture.RunAsDefaultUserAsync(new[] { claim });
        var company = await _fixture.AddCompanyAsync(user.Id);
        var command = new UpdateCompanyCommand(company.Id);
        command.Fields.Add(nameof(Company.ManagerId), null);

        await AssertCompanyUpdatedAsync(user, company, command, managerField: true);
    }

    [Fact]
    public async Task User_has_no_claim_to_set_manager_from_self_to_none_in_any_company___Forbidden()
    {
        var user = await _fixture.RunAsDefaultUserAsync();
        var company = await _fixture.AddCompanyAsync(user.Id);
        var command = new UpdateCompanyCommand(company.Id);
        command.Fields.Add(nameof(Company.ManagerId), null);

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
        var command = new UpdateCompanyCommand(company.Id);
        command.Fields.Add(nameof(Company.ManagerId), null);

        await AssertCompanyUpdatedAsync(user, company, command, managerField: true);
    }

    [Fact]
    public async Task User_has_no_claim_to_set_manager_from_any_to_none_in_any_company___Forbidden()
    {
        var user = await _fixture.RunAsDefaultUserAsync();
        var someUser = await _fixture.AddUserAsync();
        var company = await _fixture.AddCompanyAsync(someUser.Id);
        var command = new UpdateCompanyCommand(company.Id);
        command.Fields.Add(nameof(Company.ManagerId), null);

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
        var command = new UpdateCompanyCommand(company.Id);
        command.Fields.Add(nameof(Company.ManagerId), user.Id);

        await AssertCompanyUpdatedAsync(user, company, command, managerField: true);
    }

    [Fact]
    public async Task User_has_no_claim_to_set_manager_from_any_to_self_in_any_company___Forbidden()
    {
        var user = await _fixture.RunAsDefaultUserAsync();
        var someUser = await _fixture.AddUserAsync();
        var company = await _fixture.AddCompanyAsync(someUser.Id);
        var command = new UpdateCompanyCommand(company.Id);
        command.Fields.Add(nameof(Company.ManagerId), user.Id);

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
        var command = new UpdateCompanyCommand(company.Id);
        command.Fields.Add(nameof(Company.ManagerId), someUser.Id);

        await AssertCompanyUpdatedAsync(user, company, command, managerField: true);
    }

    [Fact]
    public async Task User_has_no_claim_to_set_manager_from_self_to_any_in_any_company___Forbidden()
    {
        var user = await _fixture.RunAsDefaultUserAsync();
        var someUser = await _fixture.AddUserAsync();
        var company = await _fixture.AddCompanyAsync(user.Id);
        var command = new UpdateCompanyCommand(company.Id);
        command.Fields.Add(nameof(Company.ManagerId), someUser.Id);

        await Assert.ThrowsAsync<ForbiddenAccessException>(() => _fixture.SendAsync(command));
    }

    [Fact]
    public async Task User_has_claim_to_set_manager_from_any_to_any_in_any_company___Updates_manager()
    {
        var user = await _fixture.RunAsDefaultUserAsync(new[] { Claims.Company.Any.Manager.SetFromAnyToAny });
        var someUser = await _fixture.AddUserAsync();
        var anotherUser = await _fixture.AddUserAsync();
        var company = await _fixture.AddCompanyAsync(someUser.Id);
        var command = new UpdateCompanyCommand(company.Id);
        command.Fields.Add(nameof(Company.ManagerId), anotherUser.Id);

        await AssertCompanyUpdatedAsync(user, company, command, managerField: true);
    }

    [Fact]
    public async Task User_has_no_claim_to_set_manager_from_any_to_any_in_any_company___Forbidden()
    {
        var user = await _fixture.RunAsDefaultUserAsync();
        var someUser = await _fixture.AddUserAsync();
        var anotherUser = await _fixture.AddUserAsync();
        var company = await _fixture.AddCompanyAsync(someUser.Id);
        var command = new UpdateCompanyCommand(company.Id);
        command.Fields.Add(nameof(Company.ManagerId), anotherUser.Id);

        await Assert.ThrowsAsync<ForbiddenAccessException>(() => _fixture.SendAsync(command));
    }

    [Theory]
    [InlineData(Claims.Company.Any.Manager.SetFromSelfToAny)]
    [InlineData(Claims.Company.Any.Manager.SetFromSelfToNone)]
    [InlineData(Claims.Company.WhereUserIsManager.Manager.SetFromSelfToNone)]
    [InlineData(Claims.Company.WhereUserIsManager.Manager.SetFromSelfToAny)]
    public async Task No_manager_in_company___Forbidden_to_update_manager(string claim)
    {
        var user = await _fixture.RunAsDefaultUserAsync(new[] { claim });
        var company = await _fixture.AddCompanyAsync();
        var command = new UpdateCompanyCommand(company.Id);
        command.Fields.Add(nameof(Company.ManagerId), user.Id);

        await Assert.ThrowsAsync<ForbiddenAccessException>(() => _fixture.SendAsync(command));
    }

    [Theory]
    [InlineData(Claims.Company.WhereUserIsManager.Manager.SetFromSelfToAny)]
    [InlineData(Claims.Company.WhereUserIsManager.Manager.SetFromSelfToNone)]
    public async Task User_has_claim_to_update_manager_in_own_company_and_is_manager___Updates_manager(string claim)
    {
        var user = await _fixture.RunAsDefaultUserAsync(new[] { claim });
        var company = await _fixture.AddCompanyAsync(user.Id);
        var command = new UpdateCompanyCommand(company.Id);
        command.Fields.Add(nameof(Company.ManagerId), null);

        await AssertCompanyUpdatedAsync(user, company, command, managerField: true);
    }

    [Theory]
    [InlineData(Claims.Company.WhereUserIsManager.Manager.SetFromSelfToAny)]
    [InlineData(Claims.Company.WhereUserIsManager.Manager.SetFromSelfToNone)]
    public async Task User_has_claim_to_update_manager_in_own_company_and_is_manager___Forbidden_to_update_other_fields(string claim)
    {
        var user = await _fixture.RunAsDefaultUserAsync(new[] { claim });
        var company = await _fixture.AddCompanyAsync(user.Id);
        var command = CreateOtherFields(company);

        await Assert.ThrowsAsync<ForbiddenAccessException>(() => _fixture.SendAsync(command));
    }

    [Theory]
    [InlineData(Claims.Company.WhereUserIsManager.Manager.SetFromSelfToNone)]
    [InlineData(Claims.Company.WhereUserIsManager.Manager.SetFromSelfToAny)]
    public async Task User_has_claim_to_set_manager_from_self_to_none_in_own_company_and_is_manager___Updates_manager(string claim)
    {
        var user = await _fixture.RunAsDefaultUserAsync(new[] { claim });
        var company = await _fixture.AddCompanyAsync(user.Id);
        var command = new UpdateCompanyCommand(company.Id);
        command.Fields.Add(nameof(Company.ManagerId), null);

        await AssertCompanyUpdatedAsync(user, company, command, managerField: true);
    }

    [Fact]
    public async Task User_has_no_claim_to_set_manager_from_self_to_none_in_own_company_and_is_manager___Forbidden()
    {
        var user = await _fixture.RunAsDefaultUserAsync();
        var company = await _fixture.AddCompanyAsync(user.Id);
        var command = new UpdateCompanyCommand(company.Id);
        command.Fields.Add(nameof(Company.ManagerId), null);

        await Assert.ThrowsAsync<ForbiddenAccessException>(() => _fixture.SendAsync(command));
    }

    [Fact]
    public async Task User_has_claim_to_update_name_in_any_company___Updates_name()
    {
        var user = await _fixture.RunAsDefaultUserAsync(new[] { Claims.Company.Any.Name.Set });
        var company = await _fixture.AddCompanyAsync();
        var command = new UpdateCompanyCommand(company.Id);
        command.Fields.Add(nameof(Company.Name), Faker.RandomString.Next());

        await AssertCompanyUpdatedAsync(user, company, command, nameField: true);
    }

    [Fact]
    public async Task User_has_claim_to_update_name_in_own_company___Updates_name()
    {
        var user = await _fixture.RunAsDefaultUserAsync(new[] { Claims.Company.WhereUserIsManager.Name.Set });
        var company = await _fixture.AddCompanyAsync(user.Id);
        var command = new UpdateCompanyCommand(company.Id);
        command.Fields.Add(nameof(Company.Name), Faker.RandomString.Next());

        await AssertCompanyUpdatedAsync(user, company, command, nameField: true);
    }

    [Fact]
    public async Task User_has_claim_to_update_name_in_own_company_and_is_not_manager___Forbidden()
    {
        await _fixture.RunAsDefaultUserAsync(new[] { Claims.Company.WhereUserIsManager.Name.Set });
        var company = await _fixture.AddCompanyAsync();
        var command = new UpdateCompanyCommand(company.Id);
        command.Fields.Add(nameof(Company.Name), Faker.RandomString.Next());

        await Assert.ThrowsAsync<ForbiddenAccessException>(() => _fixture.SendAsync(command));
    }

    private static UpdateCompanyCommand CreateAllFields(int id, string? managerId = null)
    {
        var data = Faker.Builders.Company();
        var result = new UpdateCompanyCommand(id);
        result.Fields.Add(nameof(Company.Address), data.Address);
        result.Fields.Add(nameof(Company.Ceo), data.Ceo);
        result.Fields.Add(nameof(Company.Contacts), data.Contacts);
        result.Fields.Add(nameof(Company.Email), data.Email);
        result.Fields.Add(nameof(Company.Inn), data.Inn);
        result.Fields.Add(nameof(Company.Name), data.Name);
        result.Fields.Add(nameof(Company.Phone), data.Phone);
        result.Fields.Add(nameof(Company.TypeId), data.TypeId);
        result.Fields.Add(nameof(Company.ManagerId), managerId);

        return result;
    }

    private static UpdateCompanyCommand CreateOtherFields(Company company)
    {
        var data = Faker.Builders.Company();
        var result = new UpdateCompanyCommand(company.Id);
        result.Fields.Add(nameof(Company.Address), data.Address);
        result.Fields.Add(nameof(Company.Ceo), data.Ceo);
        result.Fields.Add(nameof(Company.Contacts), data.Contacts);
        result.Fields.Add(nameof(Company.Email), data.Email);
        result.Fields.Add(nameof(Company.Inn), data.Inn);
        result.Fields.Add(nameof(Company.Phone), data.Phone);
        result.Fields.Add(nameof(Company.TypeId), data.TypeId);

        return result;
    }

    private async Task AssertCompanyUpdatedAsync(AspNetUser user, Company currentCompany, UpdateCompanyCommand expected, bool nameField = false, bool otherFields = false, bool managerField = false)
    {
        await _fixture.SendAsync(expected);
        var actual = await _fixture.FindAsync<Company>(currentCompany.Id);

        Assert.Equal(BaseTestFixture.UtcNow, actual?.LastModifiedAtUtc);
        Assert.Equal(user.Id, actual?.LastModifiedBy);

        if (nameField)
        {
            Assert.Equal(expected.Fields[nameof(Company.Name)], actual?.Name);
        }
        else
        {
            Assert.Equal(currentCompany.Name, actual?.Name);
        }

        if (otherFields)
        {
            Assert.Equal(expected.Fields[nameof(Company.TypeId)], actual?.TypeId);
            Assert.Equal(expected.Fields[nameof(Company.Address)], actual?.Address);
            Assert.Equal(expected.Fields[nameof(Company.Ceo)], actual?.Ceo);
            Assert.Equal(expected.Fields[nameof(Company.Contacts)], actual?.Contacts);
            Assert.Equal(expected.Fields[nameof(Company.Email)], actual?.Email);
            Assert.Equal(expected.Fields[nameof(Company.Inn)], actual?.Inn);
            Assert.Equal(expected.Fields[nameof(Company.Phone)], actual?.Phone);
        }
        else
        {
            Assert.Equal(currentCompany.TypeId, actual?.TypeId);
            Assert.Equal(currentCompany.Address, actual?.Address);
            Assert.Equal(currentCompany.Ceo, actual?.Ceo);
            Assert.Equal(currentCompany.Contacts, actual?.Contacts);
            Assert.Equal(currentCompany.Email, actual?.Email);
            Assert.Equal(currentCompany.Inn, actual?.Inn);
            Assert.Equal(currentCompany.Phone, actual?.Phone);
        }

        if (managerField)
        {
            Assert.Equal(expected.Fields[nameof(Company.ManagerId)], actual?.ManagerId);
        }
        else
        {
            Assert.Equal(currentCompany.ManagerId, actual?.ManagerId);
        }
    }
}