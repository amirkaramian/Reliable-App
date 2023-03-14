namespace MyReliableSite.Shared.DTOs.Tickets;

public class CreateTicketCommentReplyRequest : IMustBeValid
{
    public string CommentText { get; set; }
    public Guid? TicketCommentParentReplyId { get; set; }
    public Guid TicketCommentId { get; set; }
}