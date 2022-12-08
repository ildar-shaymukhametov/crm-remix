using CRM.Application;
using CRM.Application.Common.Interfaces;
using CRM.Application.IntegrationTests;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;

namespace Application.IntegrationTests.Services;

public class PermissionsServiceTests : BaseTest
{
    public PermissionsServiceTests(BaseTestFixture fixture) : base(fixture) { }

    [Theory]
    [InlineData(Constants.Access.CreateCompany, Constants.Claims.CreateCompany)]
    [InlineData(Constants.Access.DeleteOwnCompany, Constants.Claims.DeleteCompany)]
    [InlineData(Constants.Access.DeleteOwnCompany, Constants.Claims.DeleteAnyCompany)]
    [InlineData(Constants.Access.ViewOwnCompany, Constants.Claims.ViewCompany)]
    [InlineData(Constants.Access.ViewOwnCompany, Constants.Claims.ViewAnyCompany)]
    [InlineData(Constants.Access.ViewOwnCompany, Constants.Claims.DeleteAnyCompany)]
    [InlineData(Constants.Access.ViewOwnCompany, Constants.Claims.DeleteCompany)]
    [InlineData(Constants.Access.ViewOwnCompany, Constants.Claims.UpdateAnyCompany)]
    [InlineData(Constants.Access.ViewOwnCompany, Constants.Claims.UpdateCompany)]
    [InlineData(Constants.Access.UpdateOwnCompany, Constants.Claims.UpdateCompany)]
    [InlineData(Constants.Access.UpdateOwnCompany, Constants.Claims.UpdateAnyCompany)]
    [InlineData(Constants.Access.ViewAnyCompany, Constants.Claims.ViewAnyCompany)]
    [InlineData(Constants.Access.ViewAnyCompany, Constants.Claims.DeleteAnyCompany)]
    [InlineData(Constants.Access.ViewAnyCompany, Constants.Claims.UpdateAnyCompany)]
    public async Task DefaultUser(string expected, string claim)
    {
        var user = await _fixture.RunAsDefaultUserAsync(new[] { claim });
        using var scope = _fixture.GetServiceScope();
        var sut = scope.ServiceProvider.GetRequiredService<IPermissionsService>();

        var actual = await sut.CheckAccessAsync(user.Id, expected);

        Assert.Equal(new[] { expected }, actual);
    }

    [Fact]
    public async Task Admin()
    {
        var user = await _fixture.RunAsAdministratorAsync();
        using var scope = _fixture.GetServiceScope();
        var sut = scope.ServiceProvider.GetRequiredService<IPermissionsService>();

        var expected = new[]
        {
            Constants.Access.CreateCompany,
            Constants.Access.DeleteOwnCompany,
            Constants.Access.ViewOwnCompany,
            Constants.Access.UpdateOwnCompany,
            Constants.Access.DeleteAnyCompany,
            Constants.Access.ViewAnyCompany,
            Constants.Access.UpdateAnyCompany
        };
        var actual = await sut.CheckAccessAsync(user.Id, expected);

        expected.Should().BeEquivalentTo(actual);
    }
}
