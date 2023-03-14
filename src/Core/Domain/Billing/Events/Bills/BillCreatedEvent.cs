using MyReliableSite.Domain.Common.Contracts;

namespace MyReliableSite.Domain.Billing.Events.Bills;

public class BillCreatedEvent : DomainEvent
{
    public BillCreatedEvent(Bill bill)
    {
        Bill = bill;
    }

    public Bill Bill { get; }
}