using CRM.Application.IntegrationTests;
using CRM.Application.Users.Commands.UpdateUserAuthorizationClaims;

namespace Application.IntegrationTests.Users.Commands;

public class UpdateUserAuthorizationClaimsTests : BaseTest
{
    public UpdateUserAuthorizationClaimsTests(BaseTestFixture fixture) : base(fixture) { }

    [Fact]
    public async Task Updates_authorization_claims()
    {
        var initialClaims = new []
        {
            Utils.CreateAuthorizationClaim(Faker.Name.First())
        };

        var user = await _fixture.RunAsDefaultUserAsync(initialClaims);
        var request = new UpdateUserAuthorizationClaimsCommand
        {
            Claims = new []
            {
                Faker.Name.First(),
                Faker.Name.First()
            }
        };

        await _fixture.SendAsync(request);
        var claims = await _fixture.GetAuthorizationClaimsAsync(user);

        Assert.Equal(request.Claims, claims.Select(x => x.Value));
    }
}
