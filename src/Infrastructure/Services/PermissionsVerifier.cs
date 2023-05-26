using CRM.Application.Common.Behaviours.Authorization;
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
    private readonly IAccessService _accessService;

    public PermissionsVerifier(UserManager<AspNetUser> userManager, IUserClaimsPrincipalFactory<AspNetUser> userClaimsPrincipalFactory, IIdentityService identityService, IResourceProvider resourceProvider, IAccessService accessService)
    {
        _userManager = userManager;
        _userClaimsPrincipalFactory = userClaimsPrincipalFactory;
        _identityService = identityService;
        _resourceProvider = resourceProvider;
        _accessService = accessService;
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
        var accessRights = _accessService.CheckAccess(principal);

        if (permissions.Contains(Permissions.Company.Create) && accessRights.Contains(Access.Company.Create))
        {
            result.Add(Permissions.Company.Create);
        }

        if (permissions.ContainsAny(Permissions.Company.Update, Permissions.Company.View, Permissions.Company.Delete) && int.TryParse(resourceKey, out var id))
        {
            var resource = await _resourceProvider.GetCompanyAsync(id) ?? throw new NotFoundException("Company", id);
            if (permissions.Contains(Permissions.Company.Update) && await _identityService.AuthorizeAsync(principal, resource, Policies.Company.Update))
            {
                result.Add(Permissions.Company.Update);
            }

            if (permissions.Contains(Permissions.Company.View) && await _identityService.AuthorizeAsync(principal, resource, Policies.Company.View))
            {
                result.Add(Permissions.Company.View);
            }

            if (permissions.Contains(Permissions.Company.Delete) && await _identityService.AuthorizeAsync(principal, resource, Policies.Company.Delete))
            {
                result.Add(Permissions.Company.Delete);
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

        if (!permissions.ContainsAny(Permissions.Company.Update, Permissions.Company.View, Permissions.Company.Delete))
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

            if (permissions.Contains(Permissions.Company.Update) && await _identityService.AuthorizeAsync(principal, resource, Policies.Company.Update))
            {
                list.Add(Permissions.Company.Update);
            }

            if (permissions.Contains(Permissions.Company.View) && await _identityService.AuthorizeAsync(principal, resource, Policies.Company.View))
            {
                list.Add(Permissions.Company.View);
            }

            if (permissions.Contains(Permissions.Company.Delete) && await _identityService.AuthorizeAsync(principal, resource, Policies.Company.Delete))
            {
                list.Add(Permissions.Company.Delete);
            }

            result.Add(id, list.ToArray());
        }

        return result;
    }
}
