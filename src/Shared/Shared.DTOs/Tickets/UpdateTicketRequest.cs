namespace MyReliableSite.Shared.DTOs.Tickets;

public class UpdateTicketRequest : IMustBeValid
{
    public string AssignedTo { get; set; }
    public TicketStatus TicketStatus { get; set; }
    public TicketPriority TicketPriority { get; set; }
    public string TicketTitle { get; set; }
    public string Description { get; set; }
    public TicketRelatedTo TicketRelatedTo { get; set; }
    public string TicketRelatedToId { get; set; }
    public string DepartmentId { get; set; }
    public string BrandId { get; set; }
    public string Duration { get; set; }
    public string IdleTime { get; set; }
    public bool PinTicket { get; set; }
    public DateTime? FollowUpOn { get; set; }
    public string FollowUpComment { get; set; }
    public string Group { get; set; }
    public string AgentUser { get; set; }
    public PriorityFollowUp? PriorityFollowUp { get; set; }
    public string Notes { get; set; }
    public string TransferComments { get; set; }
    public DateTime? TransferOn { get; set; }
    public bool? IncomingFromClient { get; set; }
}