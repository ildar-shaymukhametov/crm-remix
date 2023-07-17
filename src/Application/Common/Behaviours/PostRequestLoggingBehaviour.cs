using CRM.Domain.Interfaces;
using MediatR.Pipeline;
using Microsoft.Extensions.Logging;

namespace CRM.Application.Common.Behaviours;

public class PostRequestLoggingBehaviour<TRequest, TResponse> : IRequestPostProcessor<TRequest, TResponse> where TRequest : notnull
{
    private readonly ILogger _logger;
    private readonly ICurrentUserService _currentUserService;
    private readonly IIdentityService _identityService;

    public PostRequestLoggingBehaviour(ILogger<TRequest> logger, ICurrentUserService currentUserService, IIdentityService identityService)
    {
        _logger = logger;
        _currentUserService = currentUserService;
        _identityService = identityService;
    }

    public async Task Process(TRequest request, TResponse response, CancellationToken cancellationToken)
    {
        var responseName = typeof(TResponse).Name;
        var userId = _currentUserService.UserId ?? string.Empty;
        string? userName = string.Empty;

        if (!string.IsNullOrEmpty(userId))
        {
            userName = await _identityService.GetUserNameAsync(userId);
        }

        _logger.LogInformation("CRM.Api Response: {Name} {@UserId} {@UserName} {@Request}", responseName, userId, userName, response);
    }
}
