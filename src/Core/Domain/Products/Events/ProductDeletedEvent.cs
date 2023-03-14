using MyReliableSite.Domain.Common.Contracts;

namespace MyReliableSite.Domain.Products.Events;

public class ProductDeletedEvent : DomainEvent
{
    public ProductDeletedEvent(Product product)
    {
        Product = product;
    }

    public Product Product { get; }
}
