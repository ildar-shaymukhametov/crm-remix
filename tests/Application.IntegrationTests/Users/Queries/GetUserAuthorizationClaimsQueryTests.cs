using CRM.Application.Users.Queries.GetUserAuthorizationClaims;

namespace CRM.Application.IntegrationTests.Users.Queries;

public class GetUserAuthorizationClaimsTests : BaseTest
{
    public GetUserAuthorizationClaimsTests(BaseTestFixture fixture) : base(fixture) { }

    [Fact]
    public async Task Returns_authorization_claims()
    {
        var initialClaims = new[]
        {
            Faker.Name.First()
        };

        await _fixture.RunAsDefaultUserAsync(initialClaims);
        var request = new GetUserAuthorizationClaimsQuery();

        var claims = await _fixture.SendAsync(request);

        Assert.Equal(initialClaims, claims);
    }
}
