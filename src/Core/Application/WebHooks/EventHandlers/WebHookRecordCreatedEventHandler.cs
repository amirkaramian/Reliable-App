using MediatR;
using Microsoft.Extensions.Logging;
using MyReliableSite.Application.Common.Event;
using MyReliableSite.Domain.WebHooksDomain.Events;

namespace MyReliableSite.Application.WebHooks.EventHandlers;

public class WebHookRecordCreatedEventHandler : INotificationHandler<EventNotification<WebHookRecordCreatedEvent>>
{
    private readonly ILogger<WebHookRecordCreatedEventHandler> _logger;

    public WebHookRecordCreatedEventHandler(ILogger<WebHookRecordCreatedEventHandler> logger)
    {
        _logger = logger;
    }

    public Task Handle(EventNotification<WebHookRecordCreatedEvent> notification, CancellationToken cancellationToken)
    {
        _logger.LogInformation("{event} Triggered", notification.DomainEvent.GetType().Name);
        return Task.CompletedTask;
    }
}
