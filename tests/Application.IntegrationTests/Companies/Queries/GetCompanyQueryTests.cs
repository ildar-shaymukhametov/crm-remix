using CRM.Application.Common.Exceptions;
using CRM.Application.Companies.Queries;
using CRM.Application.Companies.Queries.GetCompany;
using CRM.Domain.Entities;
using CRM.Infrastructure.Authorization.Handlers;
using Microsoft.AspNetCore.Authorization;

namespace CRM.Application.IntegrationTests.Companies.Queries;

public class GetCompanyTests : BaseTest
{
    public GetCompanyTests(BaseTestFixture fixture) : base(fixture) { }

    [Fact]
    public async Task User_is_admin___Returns_all_fields()
    {
        var user = await _fixture.RunAsAdministratorAsync();
        var company = await _fixture.AddCompanyAsync(user.Id);

        var actual = await _fixture.SendAsync(new GetCompanyQuery(company.Id));

        AssertInitialFields(company, actual);
        AssertOtherFields(company, actual);
        AssertManagerField(company, actual);
        AssertNameField(company, actual);
    }

    [Fact]
    public async Task User_has_no_claim___Forbidden()
    {
        var user = await _fixture.RunAsDefaultUserAsync();
        var company = await _fixture.AddCompanyAsync();

        await Assert.ThrowsAsync<ForbiddenAccessException>(() => _fixture.SendAsync(new GetCompanyQuery(company.Id)));
    }

    [Fact]
    public async Task User_has_claim_to_view_other_fields_in_any_company___Returns_other_fields()
    {
        await _fixture.RunAsDefaultUserAsync(new[] { Constants.Claims.Company.Any.Other.Get });
        var company = await _fixture.AddCompanyAsync();

        var actual = await _fixture.SendAsync(new GetCompanyQuery(company.Id));

        AssertInitialFields(company, actual);
        AssertOtherFields(company, actual);
        AssertNoManagerField(actual);
        AssertNoNameField(actual);
    }

    [Fact]
    public async Task User_has_claim_to_view_manager_in_any_company___Returns_manager()
    {
        var user = await _fixture.RunAsDefaultUserAsync(new[] { Constants.Claims.Company.Any.Manager.Get });
        var company = await _fixture.AddCompanyAsync(user.Id);

        var actual = await _fixture.SendAsync(new GetCompanyQuery(company.Id));

        AssertInitialFields(company, actual);
        AssertManagerField(company, actual);
        AssertNoOtherFields(actual);
        AssertNoNameField(actual);
    }

    [Fact]
    public async Task User_has_claim_to_view_other_fields_in_own_company___Returns_other_fields()
    {
        var user = await _fixture.RunAsDefaultUserAsync(new[] { Constants.Claims.Company.WhereUserIsManager.Other.Get });
        var company = await _fixture.AddCompanyAsync(user.Id);

        var actual = await _fixture.SendAsync(new GetCompanyQuery(company.Id));

        AssertInitialFields(company, actual);
        AssertOtherFields(company, actual);
        AssertNoManagerField(actual);
        AssertNoNameField(actual);
    }

    [Fact]
    public async Task User_has_claim_to_view_manager_in_own_company___Returns_manager()
    {
        var user = await _fixture.RunAsDefaultUserAsync(new[] { Constants.Claims.Company.WhereUserIsManager.Manager.Get });
        var company = await _fixture.AddCompanyAsync(user.Id);

        var actual = await _fixture.SendAsync(new GetCompanyQuery(company.Id));

        AssertInitialFields(company, actual);
        AssertManagerField(company, actual);
        AssertNoOtherFields(actual);
        AssertNoNameField(actual);
    }

    [Theory]
    [InlineData(Constants.Claims.Company.WhereUserIsManager.Other.Get)]
    [InlineData(Constants.Claims.Company.WhereUserIsManager.Manager.Get)]
    public async Task User_has_claim_to_view_certain_fields_in_own_company_and_is_not_manager___Forbidden(string claim)
    {
        await _fixture.RunAsDefaultUserAsync(new[] { claim });
        var company = await _fixture.AddCompanyAsync();

        await Assert.ThrowsAsync<ForbiddenAccessException>(() => _fixture.SendAsync(new GetCompanyQuery(company.Id)));
    }

    [Fact]
    public async Task User_can_delete_company___Returns_id()
    {
        await _fixture.RunAsDefaultUserAsync();
        var company = await _fixture.AddCompanyAsync();
        _fixture.ReplaceService<IAuthorizationHandler, DeleteCompanyAuthorizationHandler>(new DeleteCompanyAuthorizationHandlerMock());

        var actual = await _fixture.SendAsync(new GetCompanyQuery(company.Id));

        AssertInitialFields(company, actual);
        Assert.True(actual?.CanBeDeleted);
        AssertNoManagerField(actual);
        AssertNoOtherFields(actual);
        AssertNoNameField(actual);
    }

    [Fact]
    public async Task User_has_claim_to_update_other_fields_in_any_company___Returns_id_and_other_fields()
    {
        await _fixture.RunAsDefaultUserAsync(new[] { Constants.Claims.Company.Any.Other.Set });
        var company = await _fixture.AddCompanyAsync();

        var actual = await _fixture.SendAsync(new GetCompanyQuery(company.Id));

        AssertInitialFields(company, actual);
        Assert.True(actual?.CanBeUpdated);
        AssertNoManagerField(actual);
        AssertOtherFields(company, actual);
        AssertNoNameField(actual);
    }

    [Fact]
    public async Task User_has_claim_to_update_other_fields_in_own_company___Returns_id_and_other_fields()
    {
        var user = await _fixture.RunAsDefaultUserAsync(new[] { Constants.Claims.Company.WhereUserIsManager.Other.Set });
        var company = await _fixture.AddCompanyAsync(user.Id);

        var actual = await _fixture.SendAsync(new GetCompanyQuery(company.Id));

        AssertInitialFields(company, actual);
        Assert.True(actual?.CanBeUpdated);
        AssertNoManagerField(actual);
        AssertOtherFields(company, actual);
        AssertNoNameField(actual);
    }

    [Fact]
    public async Task User_has_claim_to_update_other_fields_in_own_company_and_is_not_manager___Forbidden()
    {
        await _fixture.RunAsDefaultUserAsync(new[] { Constants.Claims.Company.WhereUserIsManager.Other.Set });
        var company = await _fixture.AddCompanyAsync();

        await Assert.ThrowsAsync<ForbiddenAccessException>(() => _fixture.SendAsync(new GetCompanyQuery(company.Id)));
    }

    [Theory]
    [InlineData(Constants.Claims.Company.Any.Manager.SetFromAnyToAny)]
    [InlineData(Constants.Claims.Company.Any.Manager.SetFromAnyToNone)]
    [InlineData(Constants.Claims.Company.Any.Manager.SetFromAnyToSelf)]
    public async Task User_has_claim_to_set_manager_from_any_in_any_company___Returns_id_and_manager(string claim)
    {
        await _fixture.RunAsDefaultUserAsync(claim);
        var company = await _fixture.AddCompanyAsync();

        var actual = await _fixture.SendAsync(new GetCompanyQuery(company.Id));

        AssertInitialFields(company, actual);
        Assert.True(actual?.CanBeUpdated);
        AssertManagerField(company, actual);
        AssertNoOtherFields(actual);
        AssertNoNameField(actual);
    }

    [Theory]
    [InlineData(Constants.Claims.Company.Any.Manager.SetFromAnyToAny)]
    [InlineData(Constants.Claims.Company.Any.Manager.SetFromAnyToNone)]
    [InlineData(Constants.Claims.Company.Any.Manager.SetFromAnyToSelf)]
    public async Task User_has_claim_to_set_manager_from_any_in_any_company_and_is_manager___Returns_id_and_manager(string claim)
    {
        var user = await _fixture.RunAsDefaultUserAsync(claim);
        var company = await _fixture.AddCompanyAsync(user.Id);

        var actual = await _fixture.SendAsync(new GetCompanyQuery(company.Id));

        AssertInitialFields(company, actual);
        Assert.True(actual?.CanBeUpdated);
        AssertManagerField(company, actual);
        AssertNoOtherFields(actual);
        AssertNoNameField(actual);
    }

    [Theory]
    [InlineData(Constants.Claims.Company.Any.Manager.SetFromSelfToAny)]
    [InlineData(Constants.Claims.Company.Any.Manager.SetFromSelfToNone)]
    [InlineData(Constants.Claims.Company.WhereUserIsManager.Manager.SetFromSelfToNone)]
    [InlineData(Constants.Claims.Company.WhereUserIsManager.Manager.SetFromSelfToAny)]
    public async Task User_has_claim_to_set_manager_from_self___Returns_id_and_manager(string claim)
    {
        var user = await _fixture.RunAsDefaultUserAsync(claim);
        var company = await _fixture.AddCompanyAsync(user.Id);

        var actual = await _fixture.SendAsync(new GetCompanyQuery(company.Id));

        AssertInitialFields(company, actual);
        Assert.True(actual?.CanBeUpdated);
        AssertManagerField(company, actual);
        AssertNoOtherFields(actual);
        AssertNoNameField(actual);
    }

    [Theory]
    [InlineData(Constants.Claims.Company.Any.Manager.SetFromSelfToAny)]
    [InlineData(Constants.Claims.Company.Any.Manager.SetFromSelfToNone)]
    [InlineData(Constants.Claims.Company.WhereUserIsManager.Manager.SetFromSelfToNone)]
    [InlineData(Constants.Claims.Company.WhereUserIsManager.Manager.SetFromSelfToAny)]
    public async Task User_has_claim_to_set_manager_from_self_and_is_not_manager___Forbidden(string claim)
    {
        await _fixture.RunAsDefaultUserAsync(claim);
        var company = await _fixture.AddCompanyAsync();

        await Assert.ThrowsAsync<ForbiddenAccessException>(() => _fixture.SendAsync(new GetCompanyQuery(company.Id)));
    }

    [Theory]
    [InlineData(Constants.Claims.Company.Any.Manager.SetFromNoneToAny)]
    [InlineData(Constants.Claims.Company.Any.Manager.SetFromNoneToSelf)]
    public async Task User_has_claim_to_set_manager_from_none_in_any_company___Returns_id_and_manager(string claim)
    {
        await _fixture.RunAsDefaultUserAsync(claim);
        var company = await _fixture.AddCompanyAsync();

        var actual = await _fixture.SendAsync(new GetCompanyQuery(company.Id));

        AssertInitialFields(company, actual);
        Assert.True(actual?.CanBeUpdated);
        AssertManagerField(company, actual);
        AssertNoOtherFields(actual);
        AssertNoNameField(actual);
    }

    [Theory]
    [InlineData(Constants.Claims.Company.Any.Manager.SetFromNoneToAny)]
    [InlineData(Constants.Claims.Company.Any.Manager.SetFromNoneToSelf)]
    public async Task User_has_claim_to_set_manager_from_none_in_any_company_and_company_has_manager___Forbidden(string claim)
    {
        var user = await _fixture.RunAsDefaultUserAsync(claim);
        var company = await _fixture.AddCompanyAsync(user.Id);

        await Assert.ThrowsAsync<ForbiddenAccessException>(() => _fixture.SendAsync(new GetCompanyQuery(company.Id)));
    }

    [Fact]
    public async Task User_has_claim_to_view_name_in_any_company___Returns_name_field()
    {
        await _fixture.RunAsDefaultUserAsync(new[] { Constants.Claims.Company.Any.Name.Get });
        var company = await _fixture.AddCompanyAsync();

        var actual = await _fixture.SendAsync(new GetCompanyQuery(company.Id));

        AssertInitialFields(company, actual);
        AssertNameField(company, actual);
        AssertNoOtherFields(actual);
        AssertNoManagerField(actual);
    }

    private static void AssertNoManagerField(CompanyVm? actual)
    {
        Assert.False(actual?.Fields.ContainsKey(nameof(Company.Manager)));
    }

    private static void AssertNoOtherFields(CompanyVm? actual)
    {
        Assert.False(actual?.Fields.ContainsKey(nameof(Company.Address)));
        Assert.False(actual?.Fields.ContainsKey(nameof(Company.Ceo)));
        Assert.False(actual?.Fields.ContainsKey(nameof(Company.Contacts)));
        Assert.False(actual?.Fields.ContainsKey(nameof(Company.Email)));
        Assert.False(actual?.Fields.ContainsKey(nameof(Company.Inn)));
        Assert.False(actual?.Fields.ContainsKey(nameof(Company.Phone)));
        Assert.False(actual?.Fields.ContainsKey(nameof(Company.Type)));
    }

    private static void AssertOtherFields(Company? expected, CompanyVm? actual)
    {
        Assert.Equal(expected?.TypeId, (actual?.Fields[nameof(Company.Type)] as CompanyTypeDto)?.Id);
        Assert.Equal(expected?.Address, actual?.Fields[nameof(Company.Address)]);
        Assert.Equal(expected?.Ceo, actual?.Fields[nameof(Company.Ceo)]);
        Assert.Equal(expected?.Contacts, actual?.Fields[nameof(Company.Contacts)]);
        Assert.Equal(expected?.Email, actual?.Fields[nameof(Company.Email)]);
        Assert.Equal(expected?.Inn, actual?.Fields[nameof(Company.Inn)]);
        Assert.Equal(expected?.Phone, actual?.Fields[nameof(Company.Phone)]);
    }

    private static void AssertManagerField(Company? expected, CompanyVm? actual)
    {
        Assert.Equal(expected?.ManagerId, (actual?.Fields[nameof(Company.Manager)] as ManagerDto)?.Id);
    }

    private static void AssertInitialFields(Company? expected, CompanyVm? actual)
    {
        Assert.Equal(expected?.Id, actual?.Id);
    }

    private static void AssertNameField(Company? expected, CompanyVm? actual)
    {
        Assert.Equal(expected?.Name, actual?.Fields[nameof(Company.Name)]);
    }

    private static void AssertNoNameField(CompanyVm? actual)
    {
        Assert.False(actual?.Fields.ContainsKey(nameof(Company.Name)));
    }
}

internal class DeleteCompanyAuthorizationHandlerMock : AuthorizationHandler<DeleteCompanyRequirement>
{
    protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, DeleteCompanyRequirement requirement)
    {
        context.Succeed(requirement);
        return Task.CompletedTask;
    }
}
