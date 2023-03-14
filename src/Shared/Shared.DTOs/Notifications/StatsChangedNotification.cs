using MyReliableSite.Shared.DTOs.Notifications.Enums;

namespace MyReliableSite.Shared.DTOs.Notifications;

public class StatsChangedNotification : INotificationMessage
{
    public string MessageType { get; set; } = typeof(StatsChangedNotification).Name;
    public string Message { get; set; }
    public NotificationType NotificationType { get; set; }
    public NotificationTargetUserTypes TargetUserTypes { get; set; }
    public string Title { get; set; }
    public Guid? NotifyModelId { get; set; }
}