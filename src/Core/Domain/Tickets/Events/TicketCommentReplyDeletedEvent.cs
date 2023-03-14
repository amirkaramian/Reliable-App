using MyReliableSite.Domain.Common.Contracts;

namespace MyReliableSite.Domain.Tickets.Events;

public class TicketCommentReplyDeletedEvent : DomainEvent
{
    public TicketCommentReplyDeletedEvent(TicketCommentReply ticketCommentReply)
    {
        TicketCommentReply = ticketCommentReply;
    }

    public TicketCommentReply TicketCommentReply { get; }
}
