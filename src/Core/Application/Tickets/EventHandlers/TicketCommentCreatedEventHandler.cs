using MediatR;
using Microsoft.Extensions.Logging;
using MyReliableSite.Application.Common.Event;
using MyReliableSite.Domain.Tickets.Events;

namespace MyReliableSite.Application.Tickets.EventHandlers;

public class TicketCommentCreatedEventHandler : INotificationHandler<EventNotification<TicketCommentCreatedEvent>>
{
    private readonly ILogger<TicketCommentCreatedEventHandler> _logger;

    public TicketCommentCreatedEventHandler(ILogger<TicketCommentCreatedEventHandler> logger)
    {
        _logger = logger;
    }

    public Task Handle(EventNotification<TicketCommentCreatedEvent> notification, CancellationToken cancellationToken)
    {
        _logger.LogInformation("{event} Triggered", notification.DomainEvent.GetType().Name);
        return Task.CompletedTask;
    }
}
