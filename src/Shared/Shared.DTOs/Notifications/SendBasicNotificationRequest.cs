using MyReliableSite.Shared.DTOs.Notifications.Enums;

namespace MyReliableSite.Shared.DTOs.Notifications;

public class SendBasicNotificationRequest : IMustBeValid
{
    public NotificationType NotificationType { get; set; }
    public Guid NotificationTemplateId { get; set; }
    public NotificationTargetUserTypes TargetUserTypes { get; set; }
    public List<string> ToUserIds { get; set; } = new List<string>();
}
