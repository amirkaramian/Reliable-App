namespace MyReliableSite.Shared.DTOs.Products;
public class ProductDto : IDto
{
    public Guid Id { get; set; }
    public int ProductNo { get; set; }
    public string OrderId { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public string Thumbnail { get; set; }
    public ProductStatus Status { get; set; }
    public BillingCycle BillingCycle { get; set; }
    public List<ProductLineItemDto> ProductLineItems { get; set; } = new List<ProductLineItemDto>();
    public decimal TotalPriceOfLineItems { get; set; } = 0;
    public string UserId { get; set; }
    public string Base64Image { get; set; }
    public string AssignedToClientId { get; set; }
    public string AssignedClient { get; set; }
    public DateTime? NextDueDate { get; set; }
    public DateTime? TerminationDate { get; set; }
    public DateTime? DeletedOn { get; set; }
    public Guid? DeletedBy { get; set; }
    public string DedicatedIP { get; set; }
    public string AssginedIPs { get; set; }
}
