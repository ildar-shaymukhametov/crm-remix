using CRM.Application.IntegrationTests;
using CRM.Application.Users.Commands.UpdateUserAuthorizationClaims;

namespace Application.IntegrationTests.Users.Commands;

public class UpdateUserAuthorizationClaimsTests : BaseTest
{
    public UpdateUserAuthorizationClaimsTests(BaseTestFixture fixture) : base(fixture) { }

    [Fact]
    public async Task Updates_authorization_claims()
    {
        var initialClaims = Faker.Lorem.Words(1).ToArray();
        var user = await _fixture.RunAsDefaultUserAsync(initialClaims);
        var request = new UpdateUserAuthorizationClaimsCommand
        {
            Claims = Faker.Lorem.Words(2).ToArray()
        };

        await _fixture.SendAsync(request);
        var claims = await _fixture.GetAuthorizationClaimsAsync(user);

        Assert.Equal(request.Claims, claims.Select(x => x.Value));
    }
}
