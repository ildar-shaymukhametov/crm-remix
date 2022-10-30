using System.Security.Claims;
using CRM.Application;

namespace Application.IntegrationTests;

public static class Utils
{
    public static Claim CreateClaim(string value, string type = Constants.Claims.ClaimType) => new Claim(type, value);
}
