using CRM.Application.Common.Interfaces;
using CRM.Infrastructure.Identity;
using Duende.IdentityServer;
using Duende.IdentityServer.AspNetIdentity;
using Duende.IdentityServer.Extensions;
using Duende.IdentityServer.Models;
using Microsoft.AspNetCore.Identity;

namespace CRM.Infrastructure.Services;

public class ProfileService : ProfileService<AspNetUser>
{
    private readonly IIdentityService _identityService;

    public ProfileService(UserManager<AspNetUser> userManager, IUserClaimsPrincipalFactory<AspNetUser> claimsFactory, IIdentityService identityService) : base(userManager, claimsFactory)
    {
        _identityService = identityService;
    }

    public override async Task GetProfileDataAsync(ProfileDataRequestContext context)
    {
        if (context.Caller != IdentityServerConstants.ProfileDataCallers.UserInfoEndpoint)
        {
            await base.GetProfileDataAsync(context);
            return;
        }

        var sub = context.Subject?.GetSubjectId();
        if (sub == null)
        {
            throw new Exception("No sub claim present");
        }

        var claims = await _identityService.GetUserClaimsAsync(sub);
        context.IssuedClaims.AddRange(claims);
    }
}