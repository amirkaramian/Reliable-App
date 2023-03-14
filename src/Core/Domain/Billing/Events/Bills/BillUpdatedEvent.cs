using MyReliableSite.Domain.Common.Contracts;

namespace MyReliableSite.Domain.Billing.Events.Bills;

public class BillUpdatedEvent : DomainEvent
{
    public BillUpdatedEvent(Bill bill)
    {
        Brand = bill;
    }

    public Bill Brand { get; }
}