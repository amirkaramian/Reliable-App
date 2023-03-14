using MyReliableSite.Domain.Common.Contracts;

namespace MyReliableSite.Domain.Identity.Events;

public class AdminGroupUpdatedEvent : DomainEvent
{
    public AdminGroupUpdatedEvent(AdminGroup adminGroup)
    {
        AdminGroup = adminGroup;
    }

    public AdminGroup AdminGroup { get; }
}