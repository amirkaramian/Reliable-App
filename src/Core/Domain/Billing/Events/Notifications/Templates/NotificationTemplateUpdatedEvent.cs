using MyReliableSite.Domain.Common.Contracts;

namespace MyReliableSite.Domain.Billing.Events.Notifications.Templates;

public class NotificationTemplateUpdatedEvent : DomainEvent
{
    public NotificationTemplateUpdatedEvent(NotificationTemplate notificationTemplate)
    {
        NotificationTemplate = notificationTemplate;
    }

    public NotificationTemplate NotificationTemplate { get; }
}