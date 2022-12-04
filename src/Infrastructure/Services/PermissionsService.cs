using CRM.Application.Common.Behaviours.Authorization;
using CRM.Application.Common.Exceptions;
using CRM.Application.Common.Extensions;
using CRM.Application.Common.Interfaces;
using CRM.Infrastructure.Identity;
using Microsoft.AspNetCore.Identity;
using static CRM.Application.Constants;

namespace CRM.Infrastructure.Services;

public class PermissionsService : IPermissionsService
{
    private readonly UserManager<AspNetUser> _userManager;
    private readonly IUserClaimsPrincipalFactory<AspNetUser> _userClaimsPrincipalFactory;
    private readonly IIdentityService _identityService;
    private readonly IResourceProvider _resourceProvider;

    public PermissionsService(UserManager<AspNetUser> userManager, IUserClaimsPrincipalFactory<AspNetUser> userClaimsPrincipalFactory, IIdentityService identityService, IResourceProvider resourceProvider)
    {
        _userManager = userManager;
        _userClaimsPrincipalFactory = userClaimsPrincipalFactory;
        _identityService = identityService;
        _resourceProvider = resourceProvider;
    }

    /// <summary>
    /// Checks whether user has specific permissions.
    /// </summary>
    /// <param name="userId"></param>
    /// <param name="resourceKey">Optional resource id.</param>
    /// <param name="permissions">Permissions to check.</param>
    /// <returns>Permissions that passed the check.</returns>
    public async Task<string[]> CheckUserPermissionsAsync(string userId, string? resourceKey, params string[] permissions)
    {
        var user = await _userManager.FindByIdAsync(userId);
        if (user == null)
        {
            return Array.Empty<string>();
        }

        var principal = await _userClaimsPrincipalFactory.CreateAsync(user);
        var result = new List<string>();

        if (permissions.Contains(Permissions.CreateCompany) && await _identityService.AuthorizeAsync(principal, Policies.CreateCompany))
        {
            result.Add(Permissions.CreateCompany);
        }

        if (permissions.ContainsAny(Permissions.UpdateCompany, Permissions.ViewCompany, Permissions.DeleteCompany) && int.TryParse(resourceKey, out var id))
        {
            var resource = await _resourceProvider.GetCompanyAsync(id) ?? throw new NotFoundException("Company", id);
            if (permissions.Contains(Permissions.UpdateCompany) && await _identityService.AuthorizeAsync(principal, resource, Policies.UpdateCompany))
            {
                result.Add(Permissions.UpdateCompany);
            }
            if (permissions.Contains(Permissions.ViewCompany) && await _identityService.AuthorizeAsync(principal, resource, Policies.GetCompany))
            {
                result.Add(Permissions.ViewCompany);
            }
            if (permissions.Contains(Permissions.DeleteCompany) && await _identityService.AuthorizeAsync(principal, resource, Policies.DeleteCompany))
            {
                result.Add(Permissions.DeleteCompany);
            }
        }

        return result.ToArray();
    }
}
