using MyReliableSite.Domain.Common.Contracts;

namespace MyReliableSite.Domain.ManageModule.Events;

public class ModuleUpdatedEvent : DomainEvent
{
    public ModuleUpdatedEvent(Module module)
    {
        Module = module;
    }

    public Module Module { get; }
}
