using MediatR;
using Microsoft.Extensions.Logging;
using MyReliableSite.Application.Common.Event;
using MyReliableSite.Domain.WebHooksDomain.Events;

namespace MyReliableSite.Application.WebHooks.EventHandlers;

public class WebHookRecordDeletedEventHandler : INotificationHandler<EventNotification<WebHookRecordDeletedEvent>>
{
    private readonly ILogger<WebHookRecordDeletedEventHandler> _logger;

    public WebHookRecordDeletedEventHandler(ILogger<WebHookRecordDeletedEventHandler> logger)
    {
        _logger = logger;
    }

    public Task Handle(EventNotification<WebHookRecordDeletedEvent> notification, CancellationToken cancellationToken)
    {
        _logger.LogInformation("{event} Triggered", notification.DomainEvent.GetType().Name);
        return Task.CompletedTask;
    }
}