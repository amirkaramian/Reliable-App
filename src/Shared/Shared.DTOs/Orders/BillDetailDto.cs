using MyReliableSite.Shared.DTOs.Identity;

namespace MyReliableSite.Shared.DTOs.Orders;
public class BillDetailDto : IDto
{
    public Guid Id { get; set; }
    public string BillNo { get; set; }
    public Guid OrderId { get; set; }
    public string UserId { get; set; }
    public DateTime CreatedOn { get; set; }
    public DateTime? LastModifiedOn { get; set; }
    public OrderStatus Status { get; set; }
    public DateTime DueDate { get; set; }
    public decimal VAT { get; set; }
    public decimal SubTotal { get; set; }
    public decimal TotalPrice { get; set; }
    public List<OrderedProductDetailDto> Products { get; set; }
    public List<OrderProductLineItemsDto> OrderProductLineItems { get; set; }
    public string IssuedBy { get; set; } // get brand name if any brand assigned to the client and if no brand is assigned then i will get company name from settings.
    public string IssueByImage { get; set; }
    public string IssuedFor { get; set; } // get company name from user detail if no company name found i will get full name of client.
    public string IssueForImage { get; set; }
    public string Tenant { get; set; }
    public string Notes { get; set; }
    public UserDetailsDto User { get; set; }
}
