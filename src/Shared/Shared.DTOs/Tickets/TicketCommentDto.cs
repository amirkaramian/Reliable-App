namespace MyReliableSite.Shared.DTOs.Tickets;

public class TicketCommentDto : IDto
{
    public Guid Id { get; set; }
    public string TicketId { get; set; }
    public string CommentText { get; set; }
    public string UserId { get; set; }
    public bool IsSticky { get; set; }
    public bool IsDraft { get; set; }
    public TicketCommentAction TicketCommentAction { get; set; }
    public TicketCommentType TicketCommentType { get; set; }
    public Guid CreatedBy { get; set; }
    public DateTime CreatedOn { get; set; }
    public Guid LastModifiedBy { get; set; }
    public DateTime? LastModifiedOn { get; set; }
    public string UserFullName { get; set; }
    public string UserImagePath { get; set; }
    public List<TicketCommentReplyDto> TicketCommentReplies { get; set; }
    public TicketDto Ticket { get; set; }
}

