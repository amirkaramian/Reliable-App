using MyReliableSite.Domain.Common.Contracts;

namespace MyReliableSite.Domain.Billing.Events.Orders;

public class OrderUpdatedEvent : DomainEvent
{
    public OrderUpdatedEvent(Order order)
    {
        Brand = order;
    }

    public Order Brand { get; }
}