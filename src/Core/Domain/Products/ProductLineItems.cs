using MyReliableSite.Domain.Common;
using MyReliableSite.Domain.Common.Contracts;
using MyReliableSite.Domain.Contracts;
using MyReliableSite.Shared.DTOs.Products;

namespace MyReliableSite.Domain.Products;

public class ProductLineItems : AuditableEntity, IMustHaveTenant
{
    public string LineItem { get; set; }
    public decimal Price { get; set; } = 0;
    public PriceType PriceType { get; set; }
    public Guid ProductId { get; set; }
    public virtual Product Product { get; set; }
    public string Tenant { get; set; }

    public ProductLineItems()
    {
    }

    public ProductLineItems(string name, decimal price, PriceType priceType)
    {
        LineItem = name;
        Price = price;
        PriceType = priceType;
    }

    public ProductLineItems(string name, decimal price, Guid productId, PriceType priceType)
    {
        LineItem = name;
        Price = price;
        ProductId = productId;
        PriceType = priceType;
    }

    public ProductLineItems Update(string name, decimal price, PriceType priceType)
    {
        if (name != null && !LineItem.NullToString().Equals(name)) LineItem = name;
        if (Price != price) Price = price;
        if (PriceType != priceType) PriceType = priceType;
        return this;
    }
}
