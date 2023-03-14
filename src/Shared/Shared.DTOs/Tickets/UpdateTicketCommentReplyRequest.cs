namespace MyReliableSite.Shared.DTOs.Tickets;

public class UpdateTicketCommentReplyRequest : IMustBeValid
{
    public string CommentText { get; set; }
    public Guid? TicketCommentParentReplyId { get; set; }
}