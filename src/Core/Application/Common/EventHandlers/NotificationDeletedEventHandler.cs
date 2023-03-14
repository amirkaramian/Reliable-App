using MediatR;
using Microsoft.Extensions.Logging;
using MyReliableSite.Application.Common.Event;
using MyReliableSite.Domain.Billing.Events.Notifications;

namespace MyReliableSite.Application.Common.EventHandlers;

public class NotificationDeletedEventHandler : INotificationHandler<EventNotification<NotificationDeletedEvent>>
{
    private readonly ILogger<NotificationDeletedEventHandler> _logger;

    public NotificationDeletedEventHandler(ILogger<NotificationDeletedEventHandler> logger) => _logger = logger;

    public Task Handle(EventNotification<NotificationDeletedEvent> notification, CancellationToken cancellationToken)
    {
        _logger.LogInformation("{event} Triggered", notification.DomainEvent.GetType().Name);
        return Task.CompletedTask;
    }
}
