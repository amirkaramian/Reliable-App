using MyReliableSite.Domain.Common.Contracts;

namespace MyReliableSite.Domain.Identity.Events;

public class AdminGroupDeletedEvent : DomainEvent
{
    public AdminGroupDeletedEvent(AdminGroup adminGroup)
    {
        AdminGroup = adminGroup;
    }

    public AdminGroup AdminGroup { get; }
}