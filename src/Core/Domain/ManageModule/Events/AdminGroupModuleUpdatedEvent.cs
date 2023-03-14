using MyReliableSite.Domain.Common.Contracts;

namespace MyReliableSite.Domain.ManageModule.Events;

public class AdminGroupModuleUpdatedEvent : DomainEvent
{
    public AdminGroupModuleUpdatedEvent(AdminGroupModule module)
    {
        AdminGroupModule = module;
    }

    public AdminGroupModule AdminGroupModule { get; }
}
