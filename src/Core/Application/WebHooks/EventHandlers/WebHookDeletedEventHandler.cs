using MediatR;
using Microsoft.Extensions.Logging;
using MyReliableSite.Application.Common.Event;
using MyReliableSite.Domain.WebHooksDomain.Events;

namespace MyReliableSite.Application.WebHooks.EventHandlers;

public class WebHookDeletedEventHandler : INotificationHandler<EventNotification<WebHookDeletedEvent>>
{
    private readonly ILogger<WebHookDeletedEventHandler> _logger;

    public WebHookDeletedEventHandler(ILogger<WebHookDeletedEventHandler> logger)
    {
        _logger = logger;
    }

    public Task Handle(EventNotification<WebHookDeletedEvent> notification, CancellationToken cancellationToken)
    {
        _logger.LogInformation("{event} Triggered", notification.DomainEvent.GetType().Name);
        return Task.CompletedTask;
    }
}