using MediatR;
using Microsoft.Extensions.Logging;
using MyReliableSite.Application.Common.Event;
using MyReliableSite.Domain.Tickets.Events;

namespace MyReliableSite.Application.Tickets.EventHandlers;

public class TicketCommentUpdatedEventHandler : INotificationHandler<EventNotification<TicketCommentUpdatedEvent>>
{
    private readonly ILogger<TicketCommentUpdatedEventHandler> _logger;

    public TicketCommentUpdatedEventHandler(ILogger<TicketCommentUpdatedEventHandler> logger)
    {
        _logger = logger;
    }

    public Task Handle(EventNotification<TicketCommentUpdatedEvent> notification, CancellationToken cancellationToken)
    {
        _logger.LogInformation("{event} Triggered", notification.DomainEvent.GetType().Name);
        return Task.CompletedTask;
    }
}
