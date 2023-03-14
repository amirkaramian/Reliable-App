using MyReliableSite.Domain.Common.Contracts;

namespace MyReliableSite.Domain.Billing.Events.Notifications.Templates;

public class NotificationTemplateDeletedEvent : DomainEvent
{
    public NotificationTemplateDeletedEvent(NotificationTemplate notificationTemplate)
    {
        NotificationTemplate = notificationTemplate;
    }

    public NotificationTemplate NotificationTemplate { get; }
}