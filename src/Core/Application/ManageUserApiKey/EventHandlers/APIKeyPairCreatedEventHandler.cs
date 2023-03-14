using MediatR;
using Microsoft.Extensions.Logging;
using MyReliableSite.Application.Common.Event;
using MyReliableSite.Domain.ManageUserApiKey.Events;

namespace MyReliableSite.Application.ManageUserApiKey.EventHandlers;

public class APIKeyPairCreatedEventHandler : INotificationHandler<EventNotification<APIKeyPairCreatedEvent>>
{
    private readonly ILogger<APIKeyPairCreatedEventHandler> _logger;

    public APIKeyPairCreatedEventHandler(ILogger<APIKeyPairCreatedEventHandler> logger)
    {
        _logger = logger;
    }

    public Task Handle(EventNotification<APIKeyPairCreatedEvent> notification, CancellationToken cancellationToken)
    {
        _logger.LogInformation("{event} Triggered", notification.DomainEvent.GetType().Name);
        return Task.CompletedTask;
    }
}
