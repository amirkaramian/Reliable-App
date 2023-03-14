using MyReliableSite.Domain.Common.Contracts;

namespace MyReliableSite.Domain.Billing.Events;

public class HooksCreatedEvent : DomainEvent
{
    public HooksCreatedEvent(Hooks hooks)
    {
        Hooks = hooks;
    }

    public Hooks Hooks { get; }
}