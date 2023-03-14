using MediatR;
using Microsoft.Extensions.Logging;
using MyReliableSite.Application.Common.Event;
using MyReliableSite.Domain.Tickets.Events;

namespace MyReliableSite.Application.Tickets.EventHandlers;

public class TicketCommentReplyDeletedEventHandler : INotificationHandler<EventNotification<TicketCommentReplyDeletedEvent>>
{
    private readonly ILogger<TicketCommentReplyDeletedEventHandler> _logger;

    public TicketCommentReplyDeletedEventHandler(ILogger<TicketCommentReplyDeletedEventHandler> logger)
    {
        _logger = logger;
    }

    public Task Handle(EventNotification<TicketCommentReplyDeletedEvent> notification, CancellationToken cancellationToken)
    {
        _logger.LogInformation("{event} Triggered", notification.DomainEvent.GetType().Name);
        return Task.CompletedTask;
    }
}
