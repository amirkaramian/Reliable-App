using MediatR;
using Microsoft.Extensions.Logging;
using MyReliableSite.Application.Common.Event;
using MyReliableSite.Domain.Billing.Events.Notifications.Templates;

namespace MyReliableSite.Application.Common.EventHandlers;

public class NotificationTemplateDeletedEventHandler : INotificationHandler<EventNotification<NotificationTemplateUpdatedEvent>>
{
    private readonly ILogger<NotificationTemplateDeletedEventHandler> _logger;

    public NotificationTemplateDeletedEventHandler(ILogger<NotificationTemplateDeletedEventHandler> logger) => _logger = logger;

    public Task Handle(EventNotification<NotificationTemplateUpdatedEvent> notification, CancellationToken cancellationToken)
    {
        _logger.LogInformation("{event} Triggered", notification.DomainEvent.GetType().Name);
        return Task.CompletedTask;
    }
}
