using MyReliableSite.Domain.Common.Contracts;

namespace MyReliableSite.Domain.ManageModule.Events;

public class AdminGroupModuleDeletedEvent : DomainEvent
{
    public AdminGroupModuleDeletedEvent(AdminGroupModule module)
    {
        AdminGroupModule = module;
    }

    public AdminGroupModule AdminGroupModule { get; }
}
