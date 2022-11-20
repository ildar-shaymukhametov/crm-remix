using CRM.Application.IntegrationTests;
using CRM.Application.Users.Queries.GetUserAuthorizationClaims;

namespace Application.IntegrationTests.Users.Queries;

public class GetUserAuthorizationClaimsTests : BaseTest
{
    public GetUserAuthorizationClaimsTests(BaseTestFixture fixture) : base(fixture) { }

    [Fact]
    public async Task Returns_authorization_claims()
    {
        var initialClaims = new[]
        {
            Utils.CreateAuthorizationClaim(Faker.Name.First())
        };

        await _fixture.RunAsDefaultUserAsync(initialClaims);
        var request = new GetUserAuthorizationClaimsQuery();

        var claims = await _fixture.SendAsync(request);

        Assert.Equal(initialClaims.Select(x => x.Value), claims);
    }
}
