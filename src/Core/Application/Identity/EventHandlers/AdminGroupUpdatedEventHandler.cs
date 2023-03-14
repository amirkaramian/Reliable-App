using MediatR;
using Microsoft.Extensions.Logging;
using MyReliableSite.Application.Common.Event;
using MyReliableSite.Domain.Identity.Events;

namespace MyReliableSite.Application.Identity.EventHandlers;

public class AdminGroupUpdatedEventHandler : INotificationHandler<EventNotification<AdminGroupUpdatedEvent>>
{
    private readonly ILogger<AdminGroupUpdatedEventHandler> _logger;

    public AdminGroupUpdatedEventHandler(ILogger<AdminGroupUpdatedEventHandler> logger)
    {
        _logger = logger;
    }

    public Task Handle(EventNotification<AdminGroupUpdatedEvent> notification, CancellationToken cancellationToken)
    {
        _logger.LogInformation("{event} Triggered", notification.DomainEvent.GetType().Name);
        return Task.CompletedTask;
    }
}
