using CRM.Domain.Entities;
using CRM.Domain.Interfaces;
using CRM.Infrastructure.Identity;
using Microsoft.Extensions.DependencyInjection;

namespace CRM.Application.IntegrationTests.Services;

public class IdentityServiceTests : BaseTest
{
    public IdentityServiceTests(BaseTestFixture fixture) : base(fixture) { }

    [Fact]
    public async Task Creates_both_app_user_and_asp_net_user()
    {
        using var scope = _fixture.GetServiceScope();
        var sut = scope.ServiceProvider.GetRequiredService<IIdentityService>();

        var firstName = Faker.Name.First();
        var lastName = Faker.Name.Last();
        var (result, userId) = await sut.CreateUserAsync(Faker.Internet.UserName(), "Foobar1!", firstName, lastName);
        Assert.True(result.Succeeded);

        var appUser = await _fixture.FindAsync<ApplicationUser>(userId);
        Assert.NotNull(appUser);
        Assert.Equal(firstName, appUser?.FirstName);
        Assert.Equal(lastName, appUser?.LastName);

        var aspNetUser = await _fixture.FindAsync<AspNetUser>(userId);
        Assert.NotNull(aspNetUser);

        Assert.Equal(appUser?.Id, aspNetUser?.Id);
    }

    [Fact]
    public async Task Deleting_user_deletes_both_app_user_and_asp_net_user()
    {
        using var scope = _fixture.GetServiceScope();
        var sut = scope.ServiceProvider.GetRequiredService<IIdentityService>();

        var (result, userId) = await sut.CreateUserAsync(Faker.Internet.UserName(), "Foobar1!");
        Assert.True(result.Succeeded);

        var deleteResult = await sut.DeleteUserAsync(userId);
        Assert.True(deleteResult.Succeeded);

        var appUser = await _fixture.FindAsync<ApplicationUser>(userId);
        Assert.Null(appUser);

        var aspNetUser = await _fixture.FindAsync<AspNetUser>(userId);
        Assert.Null(aspNetUser);
    }
}
