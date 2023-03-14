using MediatR;
using Microsoft.Extensions.Logging;
using MyReliableSite.Application.Common.Event;
using MyReliableSite.Domain.ManageUserApiKey.Events;

namespace MyReliableSite.Application.ManageUserApiKey.EventHandlers;

public class APIKeyPairDeletedEventHandler : INotificationHandler<EventNotification<APIKeyPairDeletedEvent>>
{
    private readonly ILogger<APIKeyPairDeletedEventHandler> _logger;

    public APIKeyPairDeletedEventHandler(ILogger<APIKeyPairDeletedEventHandler> logger)
    {
        _logger = logger;
    }

    public Task Handle(EventNotification<APIKeyPairDeletedEvent> notification, CancellationToken cancellationToken)
    {
        _logger.LogInformation("{event} Triggered", notification.DomainEvent.GetType().Name);
        return Task.CompletedTask;
    }
}
