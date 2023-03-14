using MediatR;
using Microsoft.Extensions.Logging;
using MyReliableSite.Application.Common.Event;
using MyReliableSite.Domain.Billing.Events.Notifications.Templates;

namespace MyReliableSite.Application.Common.EventHandlers;

public class NotificationTemplateCreatedEventHandler : INotificationHandler<EventNotification<NotificationTemplateCreatedEvent>>
{
    private readonly ILogger<NotificationTemplateCreatedEventHandler> _logger;

    public NotificationTemplateCreatedEventHandler(ILogger<NotificationTemplateCreatedEventHandler> logger) => _logger = logger;

    public Task Handle(EventNotification<NotificationTemplateCreatedEvent> notification, CancellationToken cancellationToken)
    {
        _logger.LogInformation("{event} Triggered", notification.DomainEvent.GetType().Name);
        return Task.CompletedTask;
    }
}
