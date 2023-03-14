using MyReliableSite.Domain.Common.Contracts;

namespace MyReliableSite.Domain.Identity.Events;

public class AdminGroupCreatedEvent : DomainEvent
{
    public AdminGroupCreatedEvent(AdminGroup adminGroup)
    {
        AdminGroup = adminGroup;
    }

    public AdminGroup AdminGroup { get; }
}