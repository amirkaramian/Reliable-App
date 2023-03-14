using MyReliableSite.Domain.Common;
using MyReliableSite.Domain.Common.Contracts;
using MyReliableSite.Domain.Contracts;
using MyReliableSite.Shared.DTOs.Tickets;

namespace MyReliableSite.Domain.Tickets;

public class TicketComment : AuditableEntity
{
    public Ticket Ticket { get; set; }
    public Guid TicketId { get; set; }
    public string CommentText { get; set; }
    public string UserId { get; set; }
    public bool IsDraft { get; set; } = false;
    public bool IsSticky { get; set; } = false;
    public TicketCommentAction TicketCommentAction { get; set; }
    public TicketCommentType TicketCommentType { get; set; }
    public ICollection<TicketCommentReply> TicketCommentReplies { get; set; }
    public TicketComment(string commentText, string userId, bool isSticky, TicketCommentAction ticketCommentAction, TicketCommentType ticketCommentType, bool isDraft)
    {
        CommentText = commentText;
        UserId = userId;
        IsSticky = isSticky;
        TicketCommentAction = ticketCommentAction;
        TicketCommentType = ticketCommentType;
        IsDraft = isDraft;
    }

    public TicketComment Update(string commentText, bool isSticky, TicketCommentAction ticketCommentAction, TicketCommentType ticketCommentType, bool isDraft)
    {
        if (commentText != null && !CommentText.NullToString().Equals(commentText)) CommentText = commentText;
        if (IsSticky != isSticky) { IsSticky = isSticky; }
        if (TicketCommentAction != ticketCommentAction) { TicketCommentAction = ticketCommentAction; }
        if (TicketCommentType != ticketCommentType) { TicketCommentType = ticketCommentType; }
        if (IsDraft != isDraft) { IsDraft = isDraft; }
        return this;
    }
}

public class TicketCommentHistory : BaseEntity
{
    public Guid TicketId { get; set; }
    public string CommentText { get; set; }
    public string UserId { get; set; }
    public bool IsDraft { get; set; } = false;
    public bool IsSticky { get; set; } = false;
    public TicketCommentAction TicketCommentAction { get; set; }
    public TicketCommentType TicketCommentType { get; set; }

    public TicketCommentHistory(string commentText, string userId, bool isSticky, TicketCommentAction ticketCommentAction, TicketCommentType ticketCommentType, bool isDraft)
    {
        CommentText = commentText;
        UserId = userId;
        IsSticky = isSticky;
        TicketCommentAction = ticketCommentAction;
        TicketCommentType = ticketCommentType;
        IsDraft = isDraft;
    }
}