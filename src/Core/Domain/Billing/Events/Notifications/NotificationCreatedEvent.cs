using MyReliableSite.Domain.Common.Contracts;

namespace MyReliableSite.Domain.Billing.Events.Notifications;

public class NotificationCreatedEvent : DomainEvent
{
    public NotificationCreatedEvent(Notification notification)
    {
        Notification = notification;
    }

    public Notification Notification { get; }
}