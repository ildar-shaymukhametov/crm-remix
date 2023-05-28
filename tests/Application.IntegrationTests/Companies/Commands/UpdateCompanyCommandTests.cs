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

        await AssertCompanyUpdatedAsync(user, company, command);
    }

    private async Task AssertCompanyUpdatedAsync(AspNetUser user, Company company, UpdateCompanyCommand command)
    {
        await _fixture.SendAsync(command);
        var updatedCompany = await _fixture.FindAsync<Company>(company.Id);

        Assert.Equal(BaseTestFixture.UtcNow, company.LastModifiedAtUtc);
        Assert.Equal(user.Id, company.LastModifiedBy);
        command.Should().BeEquivalentTo(updatedCompany, options =>
            options.ExcludingNestedObjects().ExcludingMissingMembers());
    }

    [Fact]
    public async Task User_has_no_claim___Throws_forbidden_access()
    {
        var user = await _fixture.RunAsDefaultUserAsync();
        var company = await _fixture.AddCompanyAsync();
        var command = CreateCommand(company.Id);

        await Assert.ThrowsAsync<ForbiddenAccessException>(() => _fixture.SendAsync(command));
    }

    [Fact]
    public async Task Not_found()
    {
        await _fixture.RunAsDefaultUserAsync(new[]
        {
            Constants.Claims.Company.Any.Update
        });
        var command = CreateCommand(1);
        await Assert.ThrowsAsync<NotFoundException>(() => _fixture.SendAsync(command));
    }

    [Theory]
    [InlineData(Constants.Claims.Company.Any.SetManagerFromNoneToSelf)]
    [InlineData(Constants.Claims.Company.Any.SetManagerFromNoneToAny)]
    [InlineData(Constants.Claims.Company.Any.SetManagerFromAnyToSelf)]
    [InlineData(Constants.Claims.Company.Any.SetManagerFromAnyToAny)]
    public async Task User_has_claim_to_set_manager_from_none_to_self_in_any_company___Updates_manager(string claim)
    {
        var user = await _fixture.RunAsDefaultUserAsync(new[] { claim, Constants.Claims.Company.Any.Update });
        var company = await _fixture.AddCompanyAsync();
        var command = CreateCommand(company.Id, managerId: user.Id);

        await AssertCompanyUpdatedAsync(user, company, command);
    }

    [Fact]
    public async Task User_has_no_claim_to_set_manager_from_none_to_self_in_any_company___Throws_forbidden_access()
    {
        var user = await _fixture.RunAsDefaultUserAsync(new[] { Constants.Claims.Company.Any.Update });
        var company = await _fixture.AddCompanyAsync();
        var command = CreateCommand(company.Id, managerId: user.Id);

        var ex = await Assert.ThrowsAsync<ForbiddenAccessException>(() => _fixture.SendAsync(command));
        Assert.Contains("Set manager from none to self in any company", ex.Message, StringComparison.CurrentCultureIgnoreCase);

    }

    [Theory]
    [InlineData(Constants.Claims.Company.Any.SetManagerFromNoneToAny)]
    [InlineData(Constants.Claims.Company.Any.SetManagerFromAnyToAny)]
    public async Task User_has_claim_to_set_manager_from_none_to_any_in_any_company___Updates_manager(string claim)
    {
        var user = await _fixture.RunAsDefaultUserAsync(new[] { claim, Constants.Claims.Company.Any.Update });
        var anotherUser = await _fixture.AddUserAsync();
        var company = await _fixture.AddCompanyAsync();
        var command = CreateCommand(company.Id, managerId: anotherUser.Id);

        await AssertCompanyUpdatedAsync(user, company, command);
    }

    [Fact]
    public async Task User_has_no_claim_to_set_manager_from_none_to_any_in_any_company___Throws_forbidden_access()
    {
        var user = await _fixture.RunAsDefaultUserAsync(new[] { Constants.Claims.Company.Any.Update });
        var anotherUser = await _fixture.AddUserAsync();
        var company = await _fixture.AddCompanyAsync();
        var command = CreateCommand(company.Id, managerId: anotherUser.Id);

        var ex = await Assert.ThrowsAsync<ForbiddenAccessException>(() => _fixture.SendAsync(command));
        Assert.Contains("Set manager from none to any in any company", ex.Message, StringComparison.CurrentCultureIgnoreCase);

    }

    [Theory]
    [InlineData(Constants.Claims.Company.Any.SetManagerFromSelfToAny)]
    [InlineData(Constants.Claims.Company.Any.SetManagerFromSelfToNone)]
    [InlineData(Constants.Claims.Company.Any.SetManagerFromAnyToAny)]
    public async Task User_has_claim_to_set_manager_from_self_to_none_in_any_company___Updates_manager(string claim)
    {
        var user = await _fixture.RunAsDefaultUserAsync(new[] { claim, Constants.Claims.Company.Any.Update });
        var company = await _fixture.AddCompanyAsync(user.Id);
        var command = CreateCommand(company.Id);

        await AssertCompanyUpdatedAsync(user, company, command);
    }

    [Fact]
    public async Task User_has_no_claim_to_set_manager_from_self_to_none_in_any_company___Throws_forbidden_access()
    {
        var user = await _fixture.RunAsDefaultUserAsync(new[] { Constants.Claims.Company.Any.Update });
        var someUser = await _fixture.AddUserAsync();
        var company = await _fixture.AddCompanyAsync(user.Id);
        var command = CreateCommand(company.Id);

        var ex = await Assert.ThrowsAsync<ForbiddenAccessException>(() => _fixture.SendAsync(command));
        Assert.Contains("Set manager from self to none in any company", ex.Message, StringComparison.CurrentCultureIgnoreCase);
    }

    [Theory]
    [InlineData(Constants.Claims.Company.Any.SetManagerFromAnyToNone)]
    [InlineData(Constants.Claims.Company.Any.SetManagerFromAnyToAny)]
    public async Task User_has_claim_to_set_manager_from_any_to_none_in_any_company___Updates_manager(string claim)
    {
        var user = await _fixture.RunAsDefaultUserAsync(new[] { claim, Constants.Claims.Company.Any.Update });
        var someUser = await _fixture.AddUserAsync();
        var company = await _fixture.AddCompanyAsync(someUser.Id);
        var command = CreateCommand(company.Id);

        await AssertCompanyUpdatedAsync(user, company, command);
    }

    [Fact]
    public async Task User_has_no_claim_to_set_manager_from_any_to_none_in_any_company___Throws_forbidden_access()
    {
        var user = await _fixture.RunAsDefaultUserAsync(new[] { Constants.Claims.Company.Any.Update });
        var someUser = await _fixture.AddUserAsync();
        var company = await _fixture.AddCompanyAsync(someUser.Id);
        var command = CreateCommand(company.Id);

        var ex = await Assert.ThrowsAsync<ForbiddenAccessException>(() => _fixture.SendAsync(command));
        Assert.Contains("Set manager from any to none in any company", ex.Message, StringComparison.CurrentCultureIgnoreCase);
    }

    [Theory]
    [InlineData(Constants.Claims.Company.Any.SetManagerFromAnyToSelf)]
    [InlineData(Constants.Claims.Company.Any.SetManagerFromAnyToAny)]
    public async Task User_has_claim_to_set_manager_from_any_to_self_in_any_company___Updates_manager(string claim)
    {
        var user = await _fixture.RunAsDefaultUserAsync(new[] { claim, Constants.Claims.Company.Any.Update });
        var someUser = await _fixture.AddUserAsync();
        var company = await _fixture.AddCompanyAsync(someUser.Id);
        var command = CreateCommand(company.Id, managerId: user.Id);

        await AssertCompanyUpdatedAsync(user, company, command);
    }

    [Fact]
    public async Task User_has_no_claim_to_set_manager_from_any_to_self_in_any_company___Throws_forbidden_access()
    {
        var user = await _fixture.RunAsDefaultUserAsync(new[] { Constants.Claims.Company.Any.Update });
        var someUser = await _fixture.AddUserAsync();
        var company = await _fixture.AddCompanyAsync(someUser.Id);
        var command = CreateCommand(company.Id, managerId: user.Id);

        var ex = await Assert.ThrowsAsync<ForbiddenAccessException>(() => _fixture.SendAsync(command));
        Assert.Contains("Set manager from any to self in any company", ex.Message, StringComparison.CurrentCultureIgnoreCase);

    }

    [Theory]
    [InlineData(Constants.Claims.Company.Any.SetManagerFromSelfToAny)]
    [InlineData(Constants.Claims.Company.Any.SetManagerFromAnyToAny)]
    public async Task User_has_claim_to_set_manager_from_self_to_any_in_any_company___Updates_manager(string claim)
    {
        var user = await _fixture.RunAsDefaultUserAsync(new[] { claim, Constants.Claims.Company.Any.Update });
        var someUser = await _fixture.AddUserAsync();
        var company = await _fixture.AddCompanyAsync(user.Id);
        var command = CreateCommand(company.Id, managerId: someUser.Id);

        await AssertCompanyUpdatedAsync(user, company, command);
    }

    [Fact]
    public async Task User_has_no_claim_to_set_manager_from_self_to_any_in_any_company___Throws_forbidden_access()
    {
        var user = await _fixture.RunAsDefaultUserAsync(new[] { Constants.Claims.Company.Any.Update });
        var someUser = await _fixture.AddUserAsync();
        var company = await _fixture.AddCompanyAsync(user.Id);
        var command = CreateCommand(company.Id, managerId: someUser.Id);

        var ex = await Assert.ThrowsAsync<ForbiddenAccessException>(() => _fixture.SendAsync(command));
        Assert.Contains("Set manager from self to any in any company", ex.Message, StringComparison.CurrentCultureIgnoreCase);

    }

    [Theory]
    [InlineData(Constants.Claims.Company.Any.SetManagerFromAnyToAny)]
    public async Task User_has_claim_to_set_manager_from_any_to_any_in_any_company___Updates_manager(string claim)
    {
        var user = await _fixture.RunAsDefaultUserAsync(new[] { claim, Constants.Claims.Company.Any.Update });
        var someUser = await _fixture.AddUserAsync();
        var anotherUser = await _fixture.AddUserAsync();
        var company = await _fixture.AddCompanyAsync(someUser.Id);
        var command = CreateCommand(company.Id, managerId: anotherUser.Id);

        await AssertCompanyUpdatedAsync(user, company, command);
    }

    [Fact]
    public async Task User_has_no_claim_to_set_manager_from_any_to_any_in_any_company___Throws_forbidden_access()
    {
        var user = await _fixture.RunAsDefaultUserAsync(new[] { Constants.Claims.Company.Any.Update });
        var someUser = await _fixture.AddUserAsync();
        var anotherUser = await _fixture.AddUserAsync();
        var company = await _fixture.AddCompanyAsync(someUser.Id);
        var command = CreateCommand(company.Id, managerId: anotherUser.Id);

        var ex = await Assert.ThrowsAsync<ForbiddenAccessException>(() => _fixture.SendAsync(command));
        Assert.Contains("Set manager from any to any in any company", ex.Message, StringComparison.CurrentCultureIgnoreCase);
    }

    [Fact]
    public async Task User_has_claim_to_update_own_company_and_he_is_manager___Updates_company()
    {
        var user = await _fixture.RunAsDefaultUserAsync(new []
        {
            Constants.Claims.Company.WhereUserIsManager.Update,
            Constants.Claims.Company.Any.SetManagerFromAnyToAny
        });
        var company = await _fixture.AddCompanyAsync(user.Id);
        var command = CreateCommand(company.Id);

        await AssertCompanyUpdatedAsync(user, company, command);
    }

    [Fact]
    public async Task User_has_claim_to_update_own_company_and_he_is_not_manager___Throws_forbidden_access()
    {
        await _fixture.RunAsDefaultUserAsync(new[]
        {
            Constants.Claims.Company.WhereUserIsManager.Update,
            Constants.Claims.Company.Any.SetManagerFromAnyToAny
        });
        var company = await _fixture.AddCompanyAsync();
        var command = CreateCommand(company.Id);

        await Assert.ThrowsAsync<ForbiddenAccessException>(() => _fixture.SendAsync(command));
    }

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
}