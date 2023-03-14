using MyReliableSite.Domain.Common.Contracts;

namespace MyReliableSite.Domain.Billing.Events.Orders;

public class OrderDeletedEvent : DomainEvent
{
    public OrderDeletedEvent(Order order)
    {
        Brand = order;
    }

    public Order Brand { get; }
}