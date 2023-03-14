using MediatR;
using Microsoft.Extensions.Logging;
using MyReliableSite.Application.Common.Event;
using MyReliableSite.Domain.Billing.Events.Notifications;

namespace MyReliableSite.Application.Common.EventHandlers;

public class NotificationCreatedEventHandler : INotificationHandler<EventNotification<NotificationCreatedEvent>>
{
    private readonly ILogger<NotificationCreatedEventHandler> _logger;

    public NotificationCreatedEventHandler(ILogger<NotificationCreatedEventHandler> logger) => _logger = logger;

    public Task Handle(EventNotification<NotificationCreatedEvent> notification, CancellationToken cancellationToken)
    {
        _logger.LogInformation("{event} Triggered", notification.DomainEvent.GetType().Name);
        return Task.CompletedTask;
    }
}
