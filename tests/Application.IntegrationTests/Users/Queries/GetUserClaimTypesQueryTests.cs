using CRM.Application.Companies.Queries.GetUserClaimsTypes;
using FluentAssertions;

namespace CRM.Application.IntegrationTests.Users.Queries;

public class GetUserClaimTypesQueryTests : BaseTest
{
    public GetUserClaimTypesQueryTests(BaseTestFixture fixture) : base(fixture) { }

    [Fact]
    public async Task Returns_claims()
    {
        await _fixture.RunAsDefaultUserAsync();
        var request = new GetUserClaimTypesQuery();

        var actual = await _fixture.SendAsync(request);

        var expected = new[]
        {
            Constants.Claims.Company.Create,
            Constants.Claims.Company.New.Other.Set,
            Constants.Claims.Company.New.Manager.SetToAny,
            Constants.Claims.Company.New.Manager.SetToNone,
            Constants.Claims.Company.New.Manager.SetToSelf,
            Constants.Claims.Company.Any.Delete,
            Constants.Claims.Company.Any.Other.Get,
            Constants.Claims.Company.Any.Other.Set,
            Constants.Claims.Company.Any.Name.Get,
            Constants.Claims.Company.Any.Name.Set,
            Constants.Claims.Company.Any.Manager.Get,
            Constants.Claims.Company.Any.Manager.SetFromAnyToAny,
            Constants.Claims.Company.Any.Manager.SetFromAnyToSelf,
            Constants.Claims.Company.Any.Manager.SetFromNoneToSelf,
            Constants.Claims.Company.Any.Manager.SetFromNoneToAny,
            Constants.Claims.Company.Any.Manager.SetFromAnyToNone,
            Constants.Claims.Company.Any.Manager.SetFromSelfToAny,
            Constants.Claims.Company.Any.Manager.SetFromSelfToNone,
            Constants.Claims.Company.WhereUserIsManager.Delete,
            Constants.Claims.Company.WhereUserIsManager.Other.Get,
            Constants.Claims.Company.WhereUserIsManager.Other.Set,
            Constants.Claims.Company.WhereUserIsManager.Name.Get,
            Constants.Claims.Company.WhereUserIsManager.Name.Set,
            Constants.Claims.Company.WhereUserIsManager.Manager.Get,
            Constants.Claims.Company.WhereUserIsManager.Manager.SetFromSelfToAny,
            Constants.Claims.Company.WhereUserIsManager.Manager.SetFromSelfToNone,
        };

        actual.Select(x => x.Value).Should().BeEquivalentTo(expected);
    }
}
