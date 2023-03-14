using MediatR;
using Microsoft.Extensions.Logging;
using MyReliableSite.Application.Common.Event;
using MyReliableSite.Domain.WebHooksDomain.Events;

namespace MyReliableSite.Application.WebHooks.EventHandlers;

public class WebHookUpdatedEventHandler : INotificationHandler<EventNotification<WebHookUpdatedEvent>>
{
    private readonly ILogger<WebHookUpdatedEventHandler> _logger;

    public WebHookUpdatedEventHandler(ILogger<WebHookUpdatedEventHandler> logger)
    {
        _logger = logger;
    }

    public Task Handle(EventNotification<WebHookUpdatedEvent> notification, CancellationToken cancellationToken)
    {
        _logger.LogInformation("{event} Triggered", notification.DomainEvent.GetType().Name);
        return Task.CompletedTask;
    }
}