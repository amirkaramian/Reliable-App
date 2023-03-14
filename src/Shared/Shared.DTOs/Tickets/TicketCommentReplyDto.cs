namespace MyReliableSite.Shared.DTOs.Tickets;

public class TicketCommentReplyDto : IDto
{
    public Guid Id { get; set; }
    public string CommentText { get; set; }
    public string UserId { get; set; }
    public Guid? TicketCommentParentReplyId { get; set; }
    public string UserFullName { get; set; }
    public Guid CreatedBy { get; set; }
    public DateTime CreatedOn { get; set; }
    public Guid LastModifiedBy { get; set; }
    public DateTime? LastModifiedOn { get; set; }
    public string UserImagePath { get; set; }

    // public ICollection<TicketCommentReplyDto> TicketCommentReplies { get; set; }
    // public TicketCommentDto TicketComment { get; set; }
}
