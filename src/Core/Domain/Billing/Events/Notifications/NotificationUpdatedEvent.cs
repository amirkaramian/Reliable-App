using MyReliableSite.Domain.Common.Contracts;

namespace MyReliableSite.Domain.Billing.Events.Notifications;

public class NotificationUpdatedEvent : DomainEvent
{
    public NotificationUpdatedEvent(Notification notification)
    {
        Notification = notification;
    }

    public Notification Notification { get; }
}