using MyReliableSite.Shared.DTOs.Products;

namespace MyReliableSite.Shared.DTOs.Orders;
public class OrderTemplateDetailsDto : IDto
{
    public string Id { get; set; }
    public string Tenant { get; set; }
    public decimal Total { get; set; }
    public string ProductName { get; set; }
    public string ProductDescription { get; set; }
    public string Notes { get; set; }
    public bool IsActive { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public string Thumbnail { get; set; }
    public IList<OrderTemplateLineItemDto> OrderTemplateLineItems { get; set; }
    public PaymentType PaymentType { get; set; }
    public string UserId { get; set; }
    public string Base64Image { get; set; }
    public BillingCycle BillingCycle { get; set; }
    public DateTime? DeletedOn { get; set; }
    public Guid? DeletedBy { get; set; }

}
