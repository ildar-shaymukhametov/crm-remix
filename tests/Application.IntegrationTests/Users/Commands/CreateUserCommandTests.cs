using CRM.Application;
using CRM.Application.IntegrationTests;
using CRM.Application.Users.Commands.CreateUser;
using CRM.Infrastructure.Identity;

namespace Application.IntegrationTests.Users.Commands;

public class CreateUserCommandTests : BaseTest
{
    public CreateUserCommandTests(BaseTestFixture fixture) : base(fixture) { }

    [Fact]
    public async Task User_is_admin___Creates_user()
    {
        var command = CreateCommand(new[] { Constants.Claims.Company.Create }, new[] { Constants.Roles.Administrator });
        var userId = await _fixture.SendAsync(command);
        var user = await _fixture.FindAsync<AspNetUser>(userId, nameof(AspNetUser.ApplicationUser));
        var userClaims = await _fixture.GetAuthorizationClaimsAsync(user);
        var userRoles = await _fixture.GetUserRolesAsync(user);

        Assert.Equal(userId, user?.Id);
        Assert.Equal(command.FirstName, user?.ApplicationUser?.FirstName);
        Assert.Equal(command.LastName, user?.ApplicationUser?.LastName);
        Assert.Equal(command.UserName, user?.UserName);
        Assert.Equal(command.Claims, userClaims.Select(x => x.Value).ToArray());
        Assert.Equal(command.Roles, userRoles);
    }

    public static CreateUserCommand CreateCommand(string[] claims, string[] roles)
    {
        var command = new CreateUserCommand
        {
            FirstName = Faker.Name.First(),
            LastName = Faker.Name.Last(),
            Password = "Foobar1!",
            UserName = Faker.Internet.UserName(),
            Claims = claims,
            Roles = roles
        };

        return command;
    }
}
