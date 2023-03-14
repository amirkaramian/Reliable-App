using MyReliableSite.Domain.Common.Contracts;

namespace MyReliableSite.Domain.Billing.Events.Notifications.Templates;

public class NotificationTemplateCreatedEvent : DomainEvent
{
    public NotificationTemplateCreatedEvent(NotificationTemplate notificationTemplate)
    {
        NotificationTemplate = notificationTemplate;
    }

    public NotificationTemplate NotificationTemplate { get; }
}