using MyReliableSite.Domain.Common.Contracts;

namespace MyReliableSite.Domain.Identity.Events;

public class UserLoginHistoryDeletedEvent : DomainEvent
{
    public UserLoginHistoryDeletedEvent(UserLoginHistory userLoginHistory)
    {
        UserLoginHistory = userLoginHistory;
    }

    public UserLoginHistory UserLoginHistory { get; }
}