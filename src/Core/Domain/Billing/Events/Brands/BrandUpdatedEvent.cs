using MyReliableSite.Domain.Common.Contracts;

namespace MyReliableSite.Domain.Billing.Events.Brands;

public class BrandUpdatedEvent : DomainEvent
{
    public BrandUpdatedEvent(Brand brand)
    {
        Brand = brand;
    }

    public Brand Brand { get; }
}