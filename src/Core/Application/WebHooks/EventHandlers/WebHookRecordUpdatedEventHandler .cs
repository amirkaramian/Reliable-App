using MediatR;
using Microsoft.Extensions.Logging;
using MyReliableSite.Application.Common.Event;
using MyReliableSite.Domain.WebHooksDomain.Events;

namespace MyReliableSite.Application.WebHooks.EventHandlers;

public class WebHookRecordUpdatedEventHandler : INotificationHandler<EventNotification<WebHookRecordUpdatedEvent>>
{
    private readonly ILogger<WebHookUpdatedEventHandler> _logger;

    public WebHookRecordUpdatedEventHandler(ILogger<WebHookUpdatedEventHandler> logger)
    {
        _logger = logger;
    }

    public Task Handle(EventNotification<WebHookRecordUpdatedEvent> notification, CancellationToken cancellationToken)
    {
        _logger.LogInformation("{event} Triggered", notification.DomainEvent.GetType().Name);
        return Task.CompletedTask;
    }
}