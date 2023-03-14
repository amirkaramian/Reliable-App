using MyReliableSite.Shared.DTOs.Filters;

namespace MyReliableSite.Shared.DTOs.Tickets;

public class TicketCommentListFilter : PaginationFilter
{
    public TicketCommentType TicketCommentType { get; set; }
}
