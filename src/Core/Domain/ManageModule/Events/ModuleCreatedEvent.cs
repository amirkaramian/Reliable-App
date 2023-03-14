using MyReliableSite.Domain.Common.Contracts;

namespace MyReliableSite.Domain.ManageModule.Events;

public class ModuleCreatedEvent : DomainEvent
{
    public ModuleCreatedEvent(Module module)
    {
        Module = module;
    }

    public Module Module { get; }
}
