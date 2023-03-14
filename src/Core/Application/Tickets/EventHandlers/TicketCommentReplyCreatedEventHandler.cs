using MediatR;
using Microsoft.Extensions.Logging;
using MyReliableSite.Application.Common.Event;
using MyReliableSite.Domain.Tickets.Events;

namespace MyReliableSite.Application.Tickets.EventHandlers;

public class TicketCommentReplyCreatedEventHandler : INotificationHandler<EventNotification<TicketCommentReplyCreatedEvent>>
{
    private readonly ILogger<TicketCommentReplyCreatedEventHandler> _logger;

    public TicketCommentReplyCreatedEventHandler(ILogger<TicketCommentReplyCreatedEventHandler> logger)
    {
        _logger = logger;
    }

    public Task Handle(EventNotification<TicketCommentReplyCreatedEvent> notification, CancellationToken cancellationToken)
    {
        _logger.LogInformation("{event} Triggered", notification.DomainEvent.GetType().Name);
        return Task.CompletedTask;
    }
}
