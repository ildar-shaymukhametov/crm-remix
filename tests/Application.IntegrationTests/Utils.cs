using System.Security.Claims;
using CRM.Application;

namespace Application.IntegrationTests;

public static class Utils
{
    public static Claim CreateAuthorizationClaim(string claimValue) => new Claim(Constants.Claims.ClaimType, claimValue);
}
