using MyReliableSite.Domain.Common.Contracts;

namespace MyReliableSite.Domain.ManageModule.Events;

public class AdminGroupModuleCreatedEvent : DomainEvent
{
    public AdminGroupModuleCreatedEvent(AdminGroupModule module)
    {
        AdminGroupModule = module;
    }

    public AdminGroupModule AdminGroupModule { get; }
}
