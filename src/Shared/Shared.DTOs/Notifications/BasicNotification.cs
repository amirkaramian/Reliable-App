using MyReliableSite.Shared.DTOs.Notifications.Enums;

namespace MyReliableSite.Shared.DTOs.Notifications;

public class BasicNotification : INotificationMessage
{
    public enum LabelType
    {
        Information,
        Success,
        Warning,
        Error
    }

    public string MessageType { get; set; } = typeof(BasicNotification).Name;
    public string Message { get; set; }
    public LabelType Label { get; set; }
    public NotificationType NotificationType { get; set; }
    public NotificationTargetUserTypes TargetUserTypes { get; set; }
    public string Title { get; set; }
    public object Data { get; set; }
    public Guid? NotifyModelId { get; set; }
}