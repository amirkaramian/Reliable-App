using MyReliableSite.Domain.Common.Contracts;

namespace MyReliableSite.Domain.Products.Events;

public class ProductUpdatedEvent : DomainEvent
{
    public ProductUpdatedEvent(Product product)
    {
        Product = product;
    }

    public Product Product { get; }
}
