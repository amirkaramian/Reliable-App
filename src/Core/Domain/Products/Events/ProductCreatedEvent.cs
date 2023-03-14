using MyReliableSite.Domain.Common.Contracts;

namespace MyReliableSite.Domain.Products.Events;

public class ProductCreatedEvent : DomainEvent
{
    public ProductCreatedEvent(Product product, object data = null)
    {
        Product = product;
        Data = data;
    }

    public Product Product { get; }
    public object Data { get; }
}
