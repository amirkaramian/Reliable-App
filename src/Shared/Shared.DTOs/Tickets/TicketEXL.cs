namespace MyReliableSite.Shared.DTOs.Tickets;

public class TicketEXL
{
    public Guid Id { get; set; }
    public string Description { get; set; }
    public string Tenant { get; set; }
    public Guid CreatedBy { get; set; }
    public Guid LastModifiedBy { get; set; }
    public Guid DeletedBy { get; set; }
    public Guid AdminAsClient { get; set; }
    public string CreatedOn { get; set; }
    public string AssignedTo { get; set; }
    public string LastModifiedOn { get; set; }
    public string DeletedOn { get; set; }
    public TicketPriority TicketPriority { get; set; }
    public TicketRelatedTo TicketRelatedTo { get; set; }
    public string TicketRelatedToId { get; set; }
    public TicketStatus TicketStatus { get; set; }
    public string DepartmentId { get; set; }
    public Guid DepartmentId1 { get; set; }
    public string TicketTitle { get; set; }
    public string AgentUser { get; set; }
    public string BrandId { get; set; }
    public Guid BrandId1 { get; set; }
    public string ClientEmail { get; set; }
    public string ClientFullName { get; set; }
    public string ClientId { get; set; }
    public string Duration { get; set; }
    public string FollowUpComment { get; set; }
    public string FollowUpOn { get; set; }
    public string Group { get; set; }
    public string IdleTime { get; set; }
    public string Notes { get; set; }
    public bool PinTicket { get; set; }
    public int PriorityFollowUp { get; set; }
    public int TicketNumber { get; set; }
    public string TransferComments { get; set; }
    public string TransferOn { get; set; }
    public bool IncomingFromClient { get; set; }

}