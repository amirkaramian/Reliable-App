using MyReliableSite.Domain.Common.Contracts;

namespace MyReliableSite.Domain.Tickets.Events;

public class TicketCommentReplyCreatedEvent : DomainEvent
{
    public TicketCommentReplyCreatedEvent(TicketCommentReply ticketCommentReply)
    {
        TicketCommentReply = ticketCommentReply;
    }

    public TicketCommentReply TicketCommentReply { get; }
}
