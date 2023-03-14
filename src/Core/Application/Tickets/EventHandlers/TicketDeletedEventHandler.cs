using MediatR;
using Microsoft.Extensions.Logging;
using MyReliableSite.Application.Common.Event;
using MyReliableSite.Domain.Tickets.Events;

namespace MyReliableSite.Application.Tickets.EventHandlers;

public class TicketDeletedEventHandler : INotificationHandler<EventNotification<TicketDeletedEvent>>
{
    private readonly ILogger<TicketDeletedEventHandler> _logger;

    public TicketDeletedEventHandler(ILogger<TicketDeletedEventHandler> logger)
    {
        _logger = logger;
    }

    public Task Handle(EventNotification<TicketDeletedEvent> notification, CancellationToken cancellationToken)
    {
        _logger.LogInformation("{event} Triggered", notification.DomainEvent.GetType().Name);
        return Task.CompletedTask;
    }
}
