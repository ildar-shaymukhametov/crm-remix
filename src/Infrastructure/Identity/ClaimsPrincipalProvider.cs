using System.Security.Claims;
using CRM.Application.Common.Exceptions;
using CRM.Domain.Interfaces;
using Microsoft.AspNetCore.Identity;

namespace CRM.Infrastructure.Identity;

public class ClaimsPrincipalProvider : IClaimsPrincipalProvider
{
    private readonly UserManager<AspNetUser> _userManager;
    private readonly IUserClaimsPrincipalFactory<AspNetUser> _userClaimsPrincipalFactory;

    public ClaimsPrincipalProvider(UserManager<AspNetUser> userManager, IUserClaimsPrincipalFactory<AspNetUser> userClaimsPrincipalFactory)
    {
        _userManager = userManager;
        _userClaimsPrincipalFactory = userClaimsPrincipalFactory;
    }
    public async Task<ClaimsPrincipal> CreateClaimsPrincipalAsync(string userId)
    {
        var user = await _userManager.FindByIdAsync(userId);
        if (user == null)
        {
            throw new NotFoundException(nameof(AspNetUser), nameof(AspNetUser.Id));
        }

        return await _userClaimsPrincipalFactory.CreateAsync(user);
    }
}
