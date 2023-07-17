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
            Domain.Constants.Claims.Company.Create,
            Domain.Constants.Claims.Company.New.Other.Set,
            Domain.Constants.Claims.Company.New.Manager.SetToAny,
            Domain.Constants.Claims.Company.New.Manager.SetToNone,
            Domain.Constants.Claims.Company.New.Manager.SetToSelf,
            Domain.Constants.Claims.Company.Any.Delete,
            Domain.Constants.Claims.Company.Any.Other.Get,
            Domain.Constants.Claims.Company.Any.Other.Set,
            Domain.Constants.Claims.Company.Any.Name.Get,
            Domain.Constants.Claims.Company.Any.Name.Set,
            Domain.Constants.Claims.Company.Any.Manager.Get,
            Domain.Constants.Claims.Company.Any.Manager.SetFromAnyToAny,
            Domain.Constants.Claims.Company.Any.Manager.SetFromAnyToSelf,
            Domain.Constants.Claims.Company.Any.Manager.SetFromNoneToSelf,
            Domain.Constants.Claims.Company.Any.Manager.SetFromNoneToAny,
            Domain.Constants.Claims.Company.Any.Manager.SetFromAnyToNone,
            Domain.Constants.Claims.Company.Any.Manager.SetFromSelfToAny,
            Domain.Constants.Claims.Company.Any.Manager.SetFromSelfToNone,
            Domain.Constants.Claims.Company.WhereUserIsManager.Delete,
            Domain.Constants.Claims.Company.WhereUserIsManager.Other.Get,
            Domain.Constants.Claims.Company.WhereUserIsManager.Other.Set,
            Domain.Constants.Claims.Company.WhereUserIsManager.Name.Get,
            Domain.Constants.Claims.Company.WhereUserIsManager.Name.Set,
            Domain.Constants.Claims.Company.WhereUserIsManager.Manager.Get,
            Domain.Constants.Claims.Company.WhereUserIsManager.Manager.SetFromSelfToAny,
            Domain.Constants.Claims.Company.WhereUserIsManager.Manager.SetFromSelfToNone,
        };

        actual.Select(x => x.Value).Should().BeEquivalentTo(expected);
    }
}
