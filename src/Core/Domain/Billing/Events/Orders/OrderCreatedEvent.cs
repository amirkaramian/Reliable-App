using MyReliableSite.Domain.Common.Contracts;

namespace MyReliableSite.Domain.Billing.Events.Orders;

public class OrderCreatedEvent : DomainEvent
{
    public OrderCreatedEvent(Order order)
    {
        Brand = order;
    }

    public Order Brand { get; }
}