using CRM.Application.Common.Behaviours.Authorization;
using CRM.Application.Common.Behaviours.Authorization.Resources;
using CRM.Application.Common.Exceptions;
using CRM.Application.Common.Extensions;
using CRM.Application.Common.Interfaces;
using CRM.Infrastructure.Identity;
using Microsoft.AspNetCore.Identity;
using static CRM.Application.Constants;

namespace CRM.Infrastructure.Services;

public class PermissionsVerifier : IPermissionsVerifier
{
    private readonly UserManager<AspNetUser> _userManager;
    private readonly IUserClaimsPrincipalFactory<AspNetUser> _userClaimsPrincipalFactory;
    private readonly IIdentityService _identityService;
    private readonly IResourceProvider _resourceProvider;

    public PermissionsVerifier(UserManager<AspNetUser> userManager, IUserClaimsPrincipalFactory<AspNetUser> userClaimsPrincipalFactory, IIdentityService identityService, IResourceProvider resourceProvider)
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
    public async Task<string[]> VerifyAsync(string userId, string? resourceKey, params string[] permissions)
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
            if (permissions.Contains(Permissions.UpdateCompany) && await _identityService.AuthorizeAsync(principal, new UpdateCompanyResource(resource), Policies.UpdateCompany))
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

    public async Task<Dictionary<int, string[]>> VerifyCompanyPermissionsAsync(string userId, int[] ids, params string[] permissions)
    {
        var user = await _userManager.FindByIdAsync(userId);
        if (user == null)
        {
            return new Dictionary<int, string[]>();
        }

        var principal = await _userClaimsPrincipalFactory.CreateAsync(user);
        var result = new Dictionary<int, string[]>();

        if (!permissions.ContainsAny(Permissions.UpdateCompany, Permissions.ViewCompany, Permissions.DeleteCompany))
        {
            return result;
        }

        var resources = await _resourceProvider.GetCompaniesAsync(ids);
        foreach (var id in ids)
        {
            var list = new List<string>();
            var resource = resources.Find(x => x.Id == id);
            if (resource == null)
            {
                result.Add(id, list.ToArray());
                continue;
            }

            if (permissions.Contains(Permissions.UpdateCompany) && await _identityService.AuthorizeAsync(principal, new UpdateCompanyResource(resource), Policies.UpdateCompany))
            {
                list.Add(Permissions.UpdateCompany);
            }
            if (permissions.Contains(Permissions.ViewCompany) && await _identityService.AuthorizeAsync(principal, resource, Policies.GetCompany))
            {
                list.Add(Permissions.ViewCompany);
            }
            if (permissions.Contains(Permissions.DeleteCompany) && await _identityService.AuthorizeAsync(principal, resource, Policies.DeleteCompany))
            {
                list.Add(Permissions.DeleteCompany);
            }

            result.Add(id, list.ToArray());
        }

        return result;
    }
}
