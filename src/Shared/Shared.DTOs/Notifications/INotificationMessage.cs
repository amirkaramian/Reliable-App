using MyReliableSite.Shared.DTOs.Notifications.Enums;

namespace MyReliableSite.Shared.DTOs.Notifications;

public interface INotificationMessage
{
    public string MessageType { get; set; }

    public string Message { get; set; }

    public string Title { get; set; }
    public NotificationType NotificationType { get; set; }
    public NotificationTargetUserTypes TargetUserTypes { get; set; }
    public Guid? NotifyModelId { get; set; }
}