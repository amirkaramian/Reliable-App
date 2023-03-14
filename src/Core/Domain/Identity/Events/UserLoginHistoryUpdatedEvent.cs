using MyReliableSite.Domain.Common.Contracts;

namespace MyReliableSite.Domain.Identity.Events;

public class UserLoginHistoryUpdatedEvent : DomainEvent
{
    public UserLoginHistoryUpdatedEvent(UserLoginHistory userLoginHistory)
    {
        UserLoginHistory = userLoginHistory;
    }

    public UserLoginHistory UserLoginHistory { get; }
}