using MediatR;
using Microsoft.Extensions.Logging;
using MyReliableSite.Application.Common.Event;
using MyReliableSite.Domain.Identity.Events;

namespace MyReliableSite.Application.Identity.EventHandlers;

public class AdminGroupDeletedEventHandler : INotificationHandler<EventNotification<AdminGroupDeletedEvent>>
{
    private readonly ILogger<AdminGroupDeletedEventHandler> _logger;

    public AdminGroupDeletedEventHandler(ILogger<AdminGroupDeletedEventHandler> logger)
    {
        _logger = logger;
    }

    public Task Handle(EventNotification<AdminGroupDeletedEvent> notification, CancellationToken cancellationToken)
    {
        _logger.LogInformation("{event} Triggered", notification.DomainEvent.GetType().Name);
        return Task.CompletedTask;
    }
}
