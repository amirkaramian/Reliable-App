namespace MyReliableSite.Shared.DTOs.Tickets;

public class UpdateTicketCommentRequest : IMustBeValid
{
    public string CommentText { get; set; }
    public bool IsSticky { get; set; }
    public bool IsDraft { get; set; }
    public TicketCommentAction TicketCommentAction { get; set; }
    public TicketCommentType TicketCommentType { get; set; }
}
