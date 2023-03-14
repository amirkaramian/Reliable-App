using MyReliableSite.Shared.DTOs.Brands;
using MyReliableSite.Shared.DTOs.Departments;

namespace MyReliableSite.Shared.DTOs.Tickets;

public class TicketDto : IDto
{
    public Guid Id { get; set; }
    public int TicketNumber { get; set; }
    public string AssignedTo { get; set; }
    public TicketStatus TicketStatus { get; set; }
    public TicketPriority TicketPriority { get; set; }
    public string TicketTitle { get; set; }
    public string Description { get; set; }
    public TicketRelatedTo TicketRelatedTo { get; set; }
    public string TicketRelatedToId { get; set; }
    public string DepartmentId { get; set; }
    public DepartmentDto Department { get; set; }
    public List<TicketCommentDto> TicketComments { get; set; }
    public Guid CreatedBy { get; set; }
    public DateTime CreatedOn { get; set; }
    public Guid LastModifiedBy { get; set; }
    public DateTime? LastModifiedOn { get; set; }
    public string AssignedToFullName { get; set; }
    public string CreatedByName { get; set; }

    public string FollowUpComment { get; set; }
    public string ClientEmail { get; set; }
    public string ClientFullName { get; set; }
    public string BrandId { get; set; }
    public BrandDto Brand { get; set; }
    public string Duration { get; set; }
    public string IdleTime { get; set; }
    public bool PinTicket { get; set; }
    public DateTime? FollowUpOn { get; set; }
    public string Group { get; set; }
    public string AgentUser { get; set; }
    public PriorityFollowUp? PriorityFollowUp { get; set; }
    public string Notes { get; set; }
    public string TransferComments { get; set; }
    public DateTime? TransferOn { get; set; }
    public bool IncomingFromClient { get; set; }
    public int TicketCommentsCount { get; set; }
}

public enum TicketPriority
{
    Low,
    Normal,
    High
}

public enum TicketRelatedTo
{
    KnowledgeBase,
    ClientProblem
}

public enum TicketStatus
{
    Active,
    Waiting,
    Closed,
    ClosedAndLocked,
    Disabled,
    FollowUp
}

public enum PriorityFollowUp
{
    Low,
    Normal,
    High
}

public class TicketHistoryDto : IDto
{
    public Guid Id { get; set; }
    public int TicketNumber { get; set; }
    public string AssignedTo { get; set; }
    public TicketStatus TicketStatus { get; set; }
    public TicketPriority TicketPriority { get; set; }
    public string TicketTitle { get; set; }
    public string Description { get; set; }
    public TicketRelatedTo TicketRelatedTo { get; set; }
    public string TicketRelatedToId { get; set; }
    public string DepartmentId { get; set; }
    public DepartmentDto Department { get; set; }
    public List<TicketCommentDto> TicketComments { get; set; }
    public DateTime CreatedOn { get; set; }
    public Guid ActionBy { get; set; }
    public Guid TicketId { get; set; }
    public string AssignedToFullName { get; set; }
    public string ActionByName { get; set; }

    public string FollowUpComment { get; set; }
    public string ClientEmail { get; set; }
    public string ClientFullName { get; set; }
    public string BrandId { get; set; }
    public string Duration { get; set; }
    public string IdleTime { get; set; }
    public bool PinTicket { get; set; }
    public DateTime? FollowUpOn { get; set; }
    public string Group { get; set; }
    public string AgentUser { get; set; }
    public PriorityFollowUp? PriorityFollowUp { get; set; }
    public string Notes { get; set; }
    public string TransferComments { get; set; }
    public DateTime? TransferOn { get; set; }
    public Guid? TicketCommentId { get; set; }
    public Guid? TicketCommentReplyId { get; set; }
    public TicketCommentReplyHistoryDto TicketCommentReplyHistory { get; set; }
    public TicketCommentHistoryDto TicketCommentHistory { get; set; }
    public Guid? TicketCommentHistoryId { get; set; }
    public Guid? TicketCommentReplyHistoryId { get; set; }
    public bool IncomingFromClient { get; set; }

}

public class TicketCommentHistoryDto : IDto
{
    public Guid TicketId { get; set; }
    public string CommentText { get; set; }
    public string UserId { get; set; }
    public bool IsDraft { get; set; } = false;
    public bool IsSticky { get; set; } = false;
    public TicketCommentAction TicketCommentAction { get; set; }
    public TicketCommentType TicketCommentType { get; set; }
}

public class TicketCommentReplyHistoryDto : IDto
{
    public string CommentText { get; set; }
    public string UserId { get; set; }
    public Guid? TicketCommentHistoryParentReplyId { get; set; }
    public Guid TicketCommentId { get; set; }
}