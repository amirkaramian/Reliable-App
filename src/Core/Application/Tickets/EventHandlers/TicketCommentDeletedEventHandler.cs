using MediatR;
using Microsoft.Extensions.Logging;
using MyReliableSite.Application.Common.Event;
using MyReliableSite.Domain.Tickets.Events;

namespace MyReliableSite.Application.Tickets.EventHandlers;

public class TicketCommentDeletedEventHandler : INotificationHandler<EventNotification<TicketCommentDeletedEvent>>
{
    private readonly ILogger<TicketCommentDeletedEventHandler> _logger;

    public TicketCommentDeletedEventHandler(ILogger<TicketCommentDeletedEventHandler> logger)
    {
        _logger = logger;
    }

    public Task Handle(EventNotification<TicketCommentDeletedEvent> notification, CancellationToken cancellationToken)
    {
        _logger.LogInformation("{event} Triggered", notification.DomainEvent.GetType().Name);
        return Task.CompletedTask;
    }
}
