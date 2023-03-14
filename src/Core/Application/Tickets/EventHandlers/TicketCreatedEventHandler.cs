using MediatR;
using Microsoft.Extensions.Logging;
using MyReliableSite.Application.Common.Event;
using MyReliableSite.Domain.Tickets.Events;

namespace MyReliableSite.Application.Tickets.EventHandlers;

public class TicketCreatedEventHandler : INotificationHandler<EventNotification<TicketCreatedEvent>>
{
    private readonly ILogger<TicketCreatedEventHandler> _logger;

    public TicketCreatedEventHandler(ILogger<TicketCreatedEventHandler> logger)
    {
        _logger = logger;
    }

    public Task Handle(EventNotification<TicketCreatedEvent> notification, CancellationToken cancellationToken)
    {
        _logger.LogInformation("{event} Triggered", notification.DomainEvent.GetType().Name);
        return Task.CompletedTask;
    }
}
