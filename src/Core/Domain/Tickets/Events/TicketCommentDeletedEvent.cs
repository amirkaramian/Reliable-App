using MyReliableSite.Domain.Common.Contracts;

namespace MyReliableSite.Domain.Tickets.Events;

public class TicketCommentDeletedEvent : DomainEvent
{
    public TicketCommentDeletedEvent(TicketComment ticketComment)
    {
        TicketComment = ticketComment;
    }

    public TicketComment TicketComment { get; }
}
