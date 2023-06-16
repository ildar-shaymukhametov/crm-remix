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
        var command = CreateAllFields(company.Id, managerId: manager.Id);

        await AssertCompanyUpdatedAsync(user, company.Id, command);
    }

    [Fact]
    public async Task User_has_no_claim___Throws_forbidden_access()
    {
        var user = await _fixture.RunAsDefaultUserAsync();
        var company = await _fixture.AddCompanyAsync();
        var command = CreateAllFields(company.Id);

        await Assert.ThrowsAsync<ForbiddenAccessException>(() => _fixture.SendAsync(command));
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

        await AssertCompanyUpdatedAsync(user, company.Id, command);
    }

    [Theory]
    [InlineData(Claims.Company.Any.Manager.SetFromAnyToAny)]
    [InlineData(Claims.Company.Any.Manager.SetFromAnyToNone)]
    [InlineData(Claims.Company.Any.Manager.SetFromAnyToSelf)]
    [InlineData(Claims.Company.Any.Manager.SetFromNoneToAny)]
    [InlineData(Claims.Company.Any.Manager.SetFromNoneToSelf)]
    [InlineData(Claims.Company.Any.Manager.SetFromSelfToAny)]
    [InlineData(Claims.Company.Any.Manager.SetFromSelfToNone)]
    [InlineData(Claims.Company.Any.Name.Set)]
    [InlineData(Claims.Company.WhereUserIsManager.Other.Set)]
    public async Task No_manager_company___Forbidden_to_update_other_fields(string claim)
    {
        var user = await _fixture.RunAsDefaultUserAsync(new[] { claim });
        var company = await _fixture.AddCompanyAsync();
        var command = CreateCopyData(company);
        command.Address = Faker.RandomString.Next();
        command.Ceo = Faker.RandomString.Next();
        command.Email = Faker.RandomString.Next();
        command.Inn = Faker.RandomString.Next();
        command.Phone = Faker.RandomString.Next();
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

        await AssertCompanyUpdatedAsync(user, company.Id, command);
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
        var user = await _fixture.RunAsDefaultUserAsync(new[] { claim, Claims.Company.Any.Other.Set });
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
        var user = await _fixture.RunAsDefaultUserAsync(new[] { Claims.Company.WhereUserIsManager.Other.Set });
        var company = await _fixture.AddCompanyAsync(user.Id);
        var command = CreateOtherFields(company);
        command.ManagerId = user.Id;

        await AssertCompanyUpdatedAsync(user, company.Id, command);
    }

    [Theory]
    [InlineData(Claims.Company.WhereUserIsManager.Other.Set)]
    [InlineData(Claims.Company.WhereUserIsManager.Name.Set)]
    public async Task Own_company___Forbidden_to_update_manager(string claim)
    {
        var user = await _fixture.RunAsDefaultUserAsync(new[] { claim });
        var company = await _fixture.AddCompanyAsync(user.Id);
        var command = CreateOtherFields(company);

        await Assert.ThrowsAsync<ForbiddenAccessException>(() => _fixture.SendAsync(command));
    }

    [Theory]
    [InlineData(Claims.Company.WhereUserIsManager.Other.Set)]
    [InlineData(Claims.Company.WhereUserIsManager.Name.Set)]
    public async Task No_manager_company___Forbidden_to_update_manager(string claim)
    {
        var user = await _fixture.RunAsDefaultUserAsync(new[] { claim });
        var company = await _fixture.AddCompanyAsync();
        var command = CreateOtherFields(company);
        command.ManagerId = user.Id;

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

    [Fact]
    public async Task User_has_claim_to_update_name_in_any_company___Updates_name()
    {
        var user = await _fixture.RunAsDefaultUserAsync(new[] { Claims.Company.Any.Name.Set });
        var company = await _fixture.AddCompanyAsync();
        var command = CreateCopyData(company);
        command.Name = Faker.RandomString.Next();

        await AssertCompanyUpdatedAsync(user, company.Id, command);
    }

    [Fact]
    public async Task User_has_claim_to_update_name_in_own_company___Updates_name()
    {
        var user = await _fixture.RunAsDefaultUserAsync(new[] { Claims.Company.WhereUserIsManager.Name.Set });
        var company = await _fixture.AddCompanyAsync(user.Id);
        var command = CreateCopyData(company);
        command.Name = Faker.RandomString.Next();

        await AssertCompanyUpdatedAsync(user, company.Id, command);
    }

    [Theory]
    [InlineData(Claims.Company.Any.Other.Set)]
    [InlineData(Claims.Company.Any.Manager.SetFromAnyToAny)]
    [InlineData(Claims.Company.Any.Manager.SetFromAnyToNone)]
    [InlineData(Claims.Company.Any.Manager.SetFromAnyToSelf)]
    [InlineData(Claims.Company.Any.Manager.SetFromNoneToAny)]
    [InlineData(Claims.Company.Any.Manager.SetFromNoneToSelf)]
    [InlineData(Claims.Company.Any.Manager.SetFromSelfToAny)]
    [InlineData(Claims.Company.Any.Manager.SetFromSelfToNone)]
    [InlineData(Claims.Company.WhereUserIsManager.Name.Set)]
    public async Task No_manager_company___Forbidden_to_update_name(string claim)
    {
        await _fixture.RunAsDefaultUserAsync(new[] { claim });
        var company = await _fixture.AddCompanyAsync();
        var command = CreateCopyData(company);
        command.Name = Faker.RandomString.Next();

        await Assert.ThrowsAsync<ForbiddenAccessException>(() => _fixture.SendAsync(command));
    }

    [Theory]
    [InlineData(Claims.Company.Any.Other.Set)]
    [InlineData(Claims.Company.Any.Manager.SetFromAnyToAny)]
    [InlineData(Claims.Company.Any.Manager.SetFromAnyToNone)]
    [InlineData(Claims.Company.Any.Manager.SetFromAnyToSelf)]
    [InlineData(Claims.Company.Any.Manager.SetFromNoneToAny)]
    [InlineData(Claims.Company.Any.Manager.SetFromNoneToSelf)]
    [InlineData(Claims.Company.Any.Manager.SetFromSelfToAny)]
    [InlineData(Claims.Company.Any.Manager.SetFromSelfToNone)]
    [InlineData(Claims.Company.WhereUserIsManager.Other.Set)]
    [InlineData(Claims.Company.WhereUserIsManager.Manager.SetFromSelfToAny)]
    [InlineData(Claims.Company.WhereUserIsManager.Manager.SetFromSelfToNone)]
    public async Task Own_company___Forbidden_to_update_name(string claim)
    {
        var user = await _fixture.RunAsDefaultUserAsync(new[] { claim });
        var company = await _fixture.AddCompanyAsync(user.Id);
        var command = CreateCopyData(company);
        command.Name = Faker.RandomString.Next();

        await Assert.ThrowsAsync<ForbiddenAccessException>(() => _fixture.SendAsync(command));
    }

    private static UpdateCompanyCommand CreateAllFields(int id, string? managerId = null)
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

    private static UpdateCompanyCommand CreateOtherFields(Company company)
    {
        var data = Faker.Builders.Company();
        return new UpdateCompanyCommand
        {
            Id = company.Id,
            Address = data.Address,
            Ceo = data.Ceo,
            Contacts = data.Contacts,
            Email = data.Email,
            Inn = data.Inn,
            Name = company.Name,
            Phone = data.Phone,
            TypeId = data.TypeId
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

    private async Task AssertCompanyUpdatedAsync(AspNetUser user, int companyId, UpdateCompanyCommand expected)
    {
        await _fixture.SendAsync(expected);
        var actual = await _fixture.FindAsync<Company>(companyId);

        Assert.Equal(BaseTestFixture.UtcNow, actual?.LastModifiedAtUtc);
        Assert.Equal(user.Id, actual?.LastModifiedBy);
        expected.Should().BeEquivalentTo(actual, x => x.ExcludingNestedObjects().ExcludingMissingMembers());
    }
}