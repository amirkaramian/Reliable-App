using MediatR;
using Microsoft.Extensions.Logging;
using MyReliableSite.Application.Common.Event;
using MyReliableSite.Domain.ManageUserApiKey.Events;

namespace MyReliableSite.Application.ManageUserApiKey.EventHandlers;

public class APIKeyPairUpdatedEventHandler : INotificationHandler<EventNotification<APIKeyPairUpdatedEvent>>
{
    private readonly ILogger<APIKeyPairUpdatedEventHandler> _logger;

    public APIKeyPairUpdatedEventHandler(ILogger<APIKeyPairUpdatedEventHandler> logger)
    {
        _logger = logger;
    }

    public Task Handle(EventNotification<APIKeyPairUpdatedEvent> notification, CancellationToken cancellationToken)
    {
        _logger.LogInformation("{event} Triggered", notification.DomainEvent.GetType().Name);
        return Task.CompletedTask;
    }
}
