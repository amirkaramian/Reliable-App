using MyReliableSite.Shared.DTOs.Filters;

namespace MyReliableSite.Shared.DTOs.Tickets;

public class TicketListFilter : PaginationFilter
{
    public string TicketTitle { get; set; }
    public string ClientEmail { get; set; }
    public TicketStatus? TicketStatus { get; set; }
    public TicketPriority? TicketPriority { get; set; }
    public TicketRelatedTo? TicketRelatedTo { get; set; }
    public string ClientId { get; set; }
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public int? TicketNumber { get; set; }
    public string AssignedTo { get; set; }
    public string DepartmentId { get; set; }
}
