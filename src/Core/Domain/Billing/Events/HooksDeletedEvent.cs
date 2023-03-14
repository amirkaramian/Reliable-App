using MyReliableSite.Domain.Common.Contracts;

namespace MyReliableSite.Domain.Billing.Events;

public class HooksDeletedEvent : DomainEvent
{
    public HooksDeletedEvent(Hooks hooks)
    {
        Hooks = hooks;
    }

    public Hooks Hooks { get; }
}