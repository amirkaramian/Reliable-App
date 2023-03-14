using MyReliableSite.Domain.Common;
using MyReliableSite.Domain.Common.Contracts;
using MyReliableSite.Domain.Contracts;

namespace MyReliableSite.Domain.Billing;
public class OrderProductLineItem : AuditableEntity, IMustHaveTenant
{
    public string LineItem { get; set; }
    public decimal Price { get; set; }
    public Guid ProductLineItemId { get; set; }
    public Guid ProductId { get; set; }
    public Guid? OrderId { get; set; }
    public virtual Order Order { get; set; }
    public string Tenant { get; set; }

    public OrderProductLineItem()
    {

    }

    public OrderProductLineItem(string lineItem, decimal price, Guid orderId, Guid productId, Guid productLineItemId)
    {
        LineItem = lineItem;
        Price = price;
        OrderId = orderId;
        ProductId = productId;
        ProductLineItemId = productLineItemId;
    }

    public OrderProductLineItem(string lineItem, decimal price, Guid productId, Guid productLineItemId)
    {
        LineItem = lineItem;
        Price = price;
        ProductId = productId;
        ProductLineItemId = productLineItemId;
    }

    public OrderProductLineItem Update(string lineItem, decimal price)
    {
        if (lineItem != null && !LineItem.NullToString().Equals(lineItem)) LineItem = lineItem;
        if (price != Price) Price = price;
        return this;
    }
}
