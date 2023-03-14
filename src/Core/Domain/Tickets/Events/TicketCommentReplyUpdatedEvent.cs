using MyReliableSite.Domain.Common.Contracts;

namespace MyReliableSite.Domain.Tickets.Events;

public class TicketCommentReplyUpdatedEvent : DomainEvent
{
    public TicketCommentReplyUpdatedEvent(TicketCommentReply ticketCommentReply)
    {
        TicketCommentReply = ticketCommentReply;
    }

    public TicketCommentReply TicketCommentReply { get; }
}
