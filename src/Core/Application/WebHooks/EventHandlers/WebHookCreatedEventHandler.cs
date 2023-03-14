using MediatR;
using Microsoft.Extensions.Logging;
using MyReliableSite.Application.Common.Event;
using MyReliableSite.Domain.WebHooksDomain.Events;

namespace MyReliableSite.Application.WebHooks.EventHandlers;

public class WebHookCreatedEventHandler : INotificationHandler<EventNotification<WebHookCreatedEvent>>
{
    private readonly ILogger<WebHookCreatedEventHandler> _logger;

    public WebHookCreatedEventHandler(ILogger<WebHookCreatedEventHandler> logger)
    {
        _logger = logger;
    }

    public Task Handle(EventNotification<WebHookCreatedEvent> notification, CancellationToken cancellationToken)
    {
        _logger.LogInformation("{event} Triggered", notification.DomainEvent.GetType().Name);
        return Task.CompletedTask;
    }
}
