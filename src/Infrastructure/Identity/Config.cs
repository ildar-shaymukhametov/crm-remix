using Duende.IdentityServer.Models;
using Microsoft.IdentityModel.Tokens;

namespace CRM.Infrastructure.Identity;

public static class Config
{
    public static IEnumerable<ApiScope> ApiScopes => new ApiScope[]
    {
        new ApiScope("company.read"),
        new ApiScope("weather.read"),
    };

    public static IEnumerable<IdentityResource> IdentityResources => new IdentityResource[]
    {
        new IdentityResources.OpenId(),
        new IdentityResources.Profile()
    };

    public static IEnumerable<Client> Clients => new Client[]
    {
        new Client
        {
            ClientId = "remix",
            ClientSecrets = { new Secret("secret".Sha256()) },
            AllowedGrantTypes = { GrantType.AuthorizationCode, GrantType.ResourceOwnerPassword },
            AllowedScopes =
            {
                "openid",
                "profile",
                "company.read",
                "weather.read",
                "CRM.ApiAPI"
            },
            ClientName = "Remix App",
            RedirectUris = { "http://localhost:3000/authentication/login-callback" },
            RequirePkce = false,
            AlwaysIncludeUserClaimsInIdToken = true,
            PostLogoutRedirectUris = { "http://localhost:3000/authentication/logout-callback" },
            Properties = { { "Profile", "SPA" } }
        }
    };
}