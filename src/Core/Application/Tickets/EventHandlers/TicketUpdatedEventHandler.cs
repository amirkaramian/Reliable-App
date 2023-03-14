using MediatR;
using Microsoft.Extensions.Logging;
using MyReliableSite.Application.Common.Event;
using MyReliableSite.Domain.Tickets.Events;

namespace MyReliableSite.Application.Tickets.EventHandlers;

public class TicketUpdatedEventHandler : INotificationHandler<EventNotification<TicketUpdatedEvent>>
{
    private readonly ILogger<TicketUpdatedEventHandler> _logger;

    public TicketUpdatedEventHandler(ILogger<TicketUpdatedEventHandler> logger)
    {
        _logger = logger;
    }

    public Task Handle(EventNotification<TicketUpdatedEvent> notification, CancellationToken cancellationToken)
    {
        _logger.LogInformation("{event} Triggered", notification.DomainEvent.GetType().Name);
        return Task.CompletedTask;
    }
}
