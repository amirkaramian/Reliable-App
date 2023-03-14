using MediatR;
using Microsoft.Extensions.Logging;
using MyReliableSite.Application.Common.Event;
using MyReliableSite.Domain.Tickets.Events;

namespace MyReliableSite.Application.Tickets.EventHandlers;

public class TicketCommentReplyUpdatedEventHandler : INotificationHandler<EventNotification<TicketCommentReplyUpdatedEvent>>
{
    private readonly ILogger<TicketCommentReplyUpdatedEventHandler> _logger;

    public TicketCommentReplyUpdatedEventHandler(ILogger<TicketCommentReplyUpdatedEventHandler> logger)
    {
        _logger = logger;
    }

    public Task Handle(EventNotification<TicketCommentReplyUpdatedEvent> notification, CancellationToken cancellationToken)
    {
        _logger.LogInformation("{event} Triggered", notification.DomainEvent.GetType().Name);
        return Task.CompletedTask;
    }
}
