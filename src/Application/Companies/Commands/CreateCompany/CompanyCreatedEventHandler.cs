using CRM.Domain.Events;
using MediatR;
using Microsoft.Extensions.Logging;

namespace CRM.Application.TodoItems.EventHandlers;

public class CompanyCreatedEventHandler : INotificationHandler<CompanyCreatedEvent>
{
    private readonly ILogger<CompanyCreatedEventHandler> _logger;

    public CompanyCreatedEventHandler(ILogger<CompanyCreatedEventHandler> logger)
    {
        _logger = logger;
    }

    public Task Handle(CompanyCreatedEvent notification, CancellationToken cancellationToken)
    {
        _logger.LogInformation("CRM.Api Domain Event: {DomainEvent}", notification.GetType().Name);

        return Task.CompletedTask;
    }
}
