namespace MyReliableSite.Shared.DTOs.Tickets;

public class CreateTicketCommentRequest : IMustBeValid
{
    public Guid TicketId { get; set; }
    public string CommentText { get; set; }
    public bool IsSticky { get; set; }
    public bool IsDraft { get; set; }
    public TicketCommentAction TicketCommentAction { get; set; }
    public TicketCommentType TicketCommentType { get; set; }
}

public enum TicketCommentType
{
    Communication,
    General,
    Client
}

public enum TicketCommentAction
{
    StatusChange,
    Transfer,
    FollowUp,
    PriorityChange

}