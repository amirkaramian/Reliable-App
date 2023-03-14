using MyReliableSite.Domain.Common.Contracts;

namespace MyReliableSite.Domain.Billing.Events.Brands;

public class BrandCreatedEvent : DomainEvent
{
    public BrandCreatedEvent(Brand brand)
    {
        Brand = brand;
    }

    public Brand Brand { get; }
}