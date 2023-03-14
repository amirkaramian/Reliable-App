using MyReliableSite.Domain.Common.Contracts;

namespace MyReliableSite.Domain.Billing.Events.Notifications;

public class NotificationDeletedEvent : DomainEvent
{
    public NotificationDeletedEvent(Notification notification)
    {
        Notification = notification;
    }

    public Notification Notification { get; }
}