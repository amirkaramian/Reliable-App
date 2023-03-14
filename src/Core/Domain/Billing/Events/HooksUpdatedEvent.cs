using MyReliableSite.Domain.Common.Contracts;

namespace MyReliableSite.Domain.Billing.Events;

public class HooksUpdatedEvent : DomainEvent
{
    public HooksUpdatedEvent(Hooks hooks)
    {
        Hooks = hooks;
    }

    public Hooks Hooks { get; }
}