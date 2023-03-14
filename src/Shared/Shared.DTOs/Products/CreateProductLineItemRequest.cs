namespace MyReliableSite.Shared.DTOs.Products;
public class CreateProductLineItemRequest : IMustBeValid
{
    public string LineItem { get; set; }
    public decimal Price { get; set; }
    public PriceType PriceType { get; set; }
}

public enum PriceType
{
    OneTime,
    Recurring
}

public class CreateOrderTemplateLineItemRequest : IMustBeValid
{
    public string LineItem { get; set; }
    public decimal Price { get; set; }
    public PriceType PriceType { get; set; }
}
