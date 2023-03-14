using MyReliableSite.Domain.Common.Contracts;

namespace MyReliableSite.Domain.ManageModule.Events;

public class ModuleDeletedEvent : DomainEvent
{
    public ModuleDeletedEvent(Module module)
    {
        Module = module;
    }

    public Module Module { get; }
}
