using System.Reflection;
using CRM.Application.Common.Behaviours.Authorization;
using CRM.Application.Common.Exceptions;
using CRM.Application.Common.Interfaces;
using CRM.Application.Common.Security;
using CRM.Domain.Interfaces;
using MediatR;

namespace CRM.Application.Common.Behaviours;

public class AuthorizationBehaviour<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse> where TRequest : notnull
{
    private readonly ICurrentUserService _currentUserService;
    private readonly IIdentityService _identityService;
    private readonly IRequestResourceProvider _requestResourceProvider;
    private readonly IAppAuthorizationService _authorizationService;

    public AuthorizationBehaviour(ICurrentUserService currentUserService, IIdentityService identityService, IRequestResourceProvider requestResourceProvider, IAppAuthorizationService authorizationService)
    {
        _currentUserService = currentUserService;
        _identityService = identityService;
        _requestResourceProvider = requestResourceProvider;
        _authorizationService = authorizationService;
    }

    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        var authorizeAttributes = request.GetType().GetCustomAttributes<AuthorizeAttribute>();
        if (!authorizeAttributes.Any())
        {
            return await next();
        }

        // Must be authenticated user
        if (_currentUserService.UserId == null)
        {
            throw new UnauthorizedAccessException();
        }

        // Role-based authorization
        var authorizeAttributesWithRoles = authorizeAttributes.Where(a => !string.IsNullOrWhiteSpace(a.Roles));

        if (authorizeAttributesWithRoles.Any())
        {
            var authorized = false;

            foreach (var roles in authorizeAttributesWithRoles.Select(a => a.Roles.Split(',')))
            {
                foreach (var role in roles)
                {
                    var isInRole = await _identityService.IsInRoleAsync(_currentUserService.UserId, role.Trim());
                    if (isInRole)
                    {
                        authorized = true;
                        break;
                    }
                }
            }

            // Must be a member of at least one role in roles
            if (!authorized)
            {
                throw new ForbiddenAccessException();
            }
        }

        // Policy-based authorization
        var authorizeAttributesWithPolicies = authorizeAttributes.Where(a => !string.IsNullOrWhiteSpace(a.Policy));
        if (authorizeAttributesWithPolicies.Any())
        {
            foreach (var policy in authorizeAttributesWithPolicies.Select(a => a.Policy))
            {
                var (resource, key) = await _requestResourceProvider.GetResourceAsync(request);
                if (key != null && resource is null)
                {
                    throw new NotFoundException($"Resource request: {typeof(TRequest)}", key);
                }

                var result = await _authorizationService.AuthorizeAsync(_currentUserService.UserId, resource, policy);
                if (!result.Succeeded)
                {
                    throw new ForbiddenAccessException(result.Errors.FirstOrDefault() ?? string.Empty);
                }
            }
        }

        // User is authorized / authorization not required
        return await next();
    }
}
