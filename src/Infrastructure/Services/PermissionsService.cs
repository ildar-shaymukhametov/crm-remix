using CRM.Application.Common.Interfaces;
using CRM.Infrastructure.Identity;
using Microsoft.AspNetCore.Identity;
using static CRM.Application.Constants;

namespace CRM.Infrastructure.Services;

public class PermissionsService : IPermissionsService
{
    private readonly IUserAuthorizationService _authorizationService;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IUserClaimsPrincipalFactory<ApplicationUser> _userClaimsPrincipalFactory;

    public PermissionsService(IUserAuthorizationService authorizationService, UserManager<ApplicationUser> userManager, IUserClaimsPrincipalFactory<ApplicationUser> userClaimsPrincipalFactory)
    {
        _authorizationService = authorizationService;
        _userManager = userManager;
        _userClaimsPrincipalFactory = userClaimsPrincipalFactory;
    }

    public async Task<string[]> CheckUserPermissionsAsync(string userId, string[] permissions)
    {
        var user = await _userManager.FindByIdAsync(userId);
        if (user == null)
        {
            return Array.Empty<string>();
        }

        var principal = await _userClaimsPrincipalFactory.CreateAsync(user);
        var result = new List<string>();

        if (permissions.Contains(Policies.CreateCompany) && _authorizationService.CanCreateCompany(principal))
        {
            result.Add(Policies.CreateCompany);
        }
        if (permissions.Contains(Policies.UpdateCompany) && _authorizationService.CanUpdateCompany(principal))
        {
            result.Add(Policies.UpdateCompany);
        }
        if (permissions.Contains(Policies.ViewCompany) && _authorizationService.CanViewCompany(principal))
        {
            result.Add(Policies.ViewCompany);
        }

        return result.ToArray();
    }
}
