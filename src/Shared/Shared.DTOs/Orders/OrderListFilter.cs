using MyReliableSite.Shared.DTOs.Filters;

namespace MyReliableSite.Shared.DTOs.Orders;
public class OrderListFilter : PaginationFilter
{
    public int? OrderNo { get; set; }
    public decimal? Amount { get; set; }
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public string ClientId { get; set; }
    public OrderStatus? OrderStatus { get; set; }
    public string AdminAssigned { get; set; }
}

public class OrderTemplateListFilter : PaginationFilter
{
}
