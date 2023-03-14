using MediatR;
using Microsoft.Extensions.Logging;
using MyReliableSite.Application.Common.Event;
using MyReliableSite.Domain.Billing.Events.Notifications;

namespace MyReliableSite.Application.Common.EventHandlers;

public class NotificationUpdatedEventHandler : INotificationHandler<EventNotification<NotificationUpdatedEvent>>
{
    private readonly ILogger<NotificationUpdatedEventHandler> _logger;

    public NotificationUpdatedEventHandler(ILogger<NotificationUpdatedEventHandler> logger) => _logger = logger;

    public Task Handle(EventNotification<NotificationUpdatedEvent> notification, CancellationToken cancellationToken)
    {
        _logger.LogInformation("{event} Triggered", notification.DomainEvent.GetType().Name);
        return Task.CompletedTask;
    }
}
