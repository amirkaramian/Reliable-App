using MyReliableSite.Shared.DTOs.Products;

namespace MyReliableSite.Shared.DTOs.Orders;
public class OrderTemplateDto : IDto
{
    public Guid Id { get; set; }
    public string ProductName { get; set; }
    public string ProductDescription { get; set; }
    public bool IsActive { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public string Notes { get; set; }
    public string Thumbnail { get; set; }
    public BillingCycle BillingCycle { get; set; }
    public PaymentType PaymentType { get; set; }
    public List<OrderTemplateLineItemDto> OrderTemplateLineItems { get; set; } = new List<OrderTemplateLineItemDto>();

    public decimal TotalPriceOfLineItems { get; set; } = 0;
    public string UserId { get; set; }
    public string Base64Image { get; set; }
    public string AssignedToClientId { get; set; }
    public string AssignedClient { get; set; }
    public DateTime? NextDueDate { get; set; }
    public DateTime? DeletedOn { get; set; }
    public Guid? DeletedBy { get; set; }
}
