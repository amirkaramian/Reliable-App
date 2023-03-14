using MediatR;
using Microsoft.Extensions.Logging;
using MyReliableSite.Application.Common.Event;
using MyReliableSite.Domain.Billing.Events.Notifications.Templates;

namespace MyReliableSite.Application.Common.EventHandlers;

public class NotificationTemplateUpdatedEventHandler : INotificationHandler<EventNotification<NotificationTemplateDeletedEvent>>
{
    private readonly ILogger<NotificationTemplateUpdatedEventHandler> _logger;

    public NotificationTemplateUpdatedEventHandler(ILogger<NotificationTemplateUpdatedEventHandler> logger) => _logger = logger;

    public Task Handle(EventNotification<NotificationTemplateDeletedEvent> notification, CancellationToken cancellationToken)
    {
        _logger.LogInformation("{event} Triggered", notification.DomainEvent.GetType().Name);
        return Task.CompletedTask;
    }
}
