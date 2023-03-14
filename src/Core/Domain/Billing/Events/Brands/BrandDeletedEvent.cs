using MyReliableSite.Domain.Common.Contracts;

namespace MyReliableSite.Domain.Billing.Events.Brands;

public class BrandDeletedEvent : DomainEvent
{
    public BrandDeletedEvent(Brand brand)
    {
        Brand = brand;
    }

    public Brand Brand { get; }
}