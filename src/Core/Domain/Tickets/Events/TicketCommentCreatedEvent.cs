using MyReliableSite.Domain.Common.Contracts;

namespace MyReliableSite.Domain.Tickets.Events;

public class TicketCommentCreatedEvent : DomainEvent
{
    public TicketCommentCreatedEvent(TicketComment ticketComment)
    {
        TicketComment = ticketComment;
    }

    public TicketComment TicketComment { get; }
}
