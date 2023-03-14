using MyReliableSite.Domain.Common.Contracts;

namespace MyReliableSite.Domain.Tickets.Events;

public class TicketCommentUpdatedEvent : DomainEvent
{
    public TicketCommentUpdatedEvent(TicketComment ticketComment)
    {
        TicketComment = ticketComment;
    }

    public TicketComment TicketComment { get; }
}
