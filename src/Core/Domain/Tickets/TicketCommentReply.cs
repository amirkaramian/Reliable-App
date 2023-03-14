using MyReliableSite.Domain.Common;
using MyReliableSite.Domain.Common.Contracts;

namespace MyReliableSite.Domain.Tickets;

public class TicketCommentReply : AuditableEntity
{
    public string CommentText { get; set; }
    public string UserId { get; set; }
    public Guid? TicketCommentParentReplyId { get; set; }
    public TicketCommentReply TicketCommentParentReply { get; set; }
    public ICollection<TicketCommentReply> TicketCommentChildReplies { get; set; }
    public TicketComment TicketComment { get; set; }
    public Guid TicketCommentId { get; set; }

    public TicketCommentReply(string commentText, string userId, Guid? ticketCommentParentReplyId)
    {
        CommentText = commentText;
        UserId = userId;
        TicketCommentParentReplyId = ticketCommentParentReplyId;
    }

    public TicketCommentReply Update(string commentText, Guid? ticketCommentParentReplyId)
    {
        if (commentText != null && !CommentText.NullToString().Equals(commentText)) CommentText = commentText;
        if (ticketCommentParentReplyId != TicketCommentParentReplyId) TicketCommentParentReplyId = ticketCommentParentReplyId;
        return this;
    }
}

public class TicketCommentReplyHistory : BaseEntity
{
    public string CommentText { get; set; }
    public string UserId { get; set; }
    public Guid? TicketCommentHistoryParentReplyId { get; set; }
    public Guid TicketCommentId { get; set; }
    public TicketCommentReplyHistory(string commentText, string userId, Guid? ticketCommentHistoryParentReplyId)
    {
        CommentText = commentText;
        UserId = userId;
        TicketCommentHistoryParentReplyId = ticketCommentHistoryParentReplyId;
    }
}