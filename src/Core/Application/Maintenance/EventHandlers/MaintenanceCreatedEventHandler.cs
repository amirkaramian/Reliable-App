using MediatR;
using Microsoft.Extensions.Logging;
using MyReliableSite.Application.Common.Event;
using MyReliableSite.Domain.MaintenanceMode.Events;

namespace MyReliableSite.Application.Maintenance.EventHandlers;

public class MaintenanceCreatedEventHandler : INotificationHandler<EventNotification<MaintenanceCreatedEvent>>
{
    private readonly ILogger<MaintenanceCreatedEventHandler> _logger;

    public MaintenanceCreatedEventHandler(ILogger<MaintenanceCreatedEventHandler> logger)
    {
        _logger = logger;
    }

    public Task Handle(EventNotification<MaintenanceCreatedEvent> notification, CancellationToken cancellationToken)
    {
        _logger.LogInformation("{event} Triggered", notification.DomainEvent.GetType().Name);
        return Task.CompletedTask;
    }
}
