namespace MyReliableSite.Shared.DTOs.Products;
public class UpdateProductLineItemRequest : IMustBeValid
{
    public Guid Id { get; set; }
    public decimal Price { get; set; }
    public bool IsDeleted { get; set; } = false;
    public string LineItem { get; set; }
    public PriceType PriceType { get; set; }
}

public class UpdateOrderTemplateLineItemRequest : IMustBeValid
{
    public Guid Id { get; set; }
    public decimal Price { get; set; }
    public bool IsDeleted { get; set; } = false;
    public string LineItem { get; set; }
    public PriceType PriceType { get; set; }
}
