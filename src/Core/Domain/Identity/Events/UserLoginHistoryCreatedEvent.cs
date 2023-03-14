using MyReliableSite.Domain.Common.Contracts;

namespace MyReliableSite.Domain.Identity.Events;

public class UserLoginHistoryCreatedEvent : DomainEvent
{
    public UserLoginHistoryCreatedEvent(UserLoginHistory userLoginHistory)
    {
        UserLoginHistory = userLoginHistory;
    }

    public UserLoginHistory UserLoginHistory { get; }
}