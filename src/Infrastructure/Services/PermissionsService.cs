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

        if (permissions.Contains(Permissions.CreateCompany) && _authorizationService.CanCreateCompany(principal))
        {
            result.Add(Permissions.CreateCompany);
        }
        if (permissions.Contains(Permissions.UpdateCompany) && _authorizationService.CanUpdateCompany(principal))
        {
            result.Add(Permissions.UpdateCompany);
        }
        if (permissions.Contains(Permissions.ViewCompany) && _authorizationService.CanViewCompany(principal))
        {
            result.Add(Permissions.ViewCompany);
        }
        if (permissions.Contains(Permissions.DeleteCompany) && _authorizationService.CanDeleteCompany(principal))
        {
            result.Add(Permissions.DeleteCompany);
        }

        return result.ToArray();
    }
}
