using MediatR;
using Microsoft.Extensions.Logging;
using MyReliableSite.Application.Common.Event;
using MyReliableSite.Domain.Identity.Events;

namespace MyReliableSite.Application.Identity.EventHandlers;

public class AdminGroupCreatedEventHandler : INotificationHandler<EventNotification<AdminGroupCreatedEvent>>
{
    private readonly ILogger<AdminGroupCreatedEventHandler> _logger;

    public AdminGroupCreatedEventHandler(ILogger<AdminGroupCreatedEventHandler> logger)
    {
        _logger = logger;
    }

    public Task Handle(EventNotification<AdminGroupCreatedEvent> notification, CancellationToken cancellationToken)
    {
        _logger.LogInformation("{event} Triggered", notification.DomainEvent.GetType().Name);
        return Task.CompletedTask;
    }
}
