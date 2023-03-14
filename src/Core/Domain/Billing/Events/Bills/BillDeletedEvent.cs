using MyReliableSite.Domain.Common.Contracts;

namespace MyReliableSite.Domain.Billing.Events.Bills;

public class BillDeletedEvent : DomainEvent
{
    public BillDeletedEvent(Bill bill)
    {
        Brand = bill;
    }

    public Bill Brand { get; }
}