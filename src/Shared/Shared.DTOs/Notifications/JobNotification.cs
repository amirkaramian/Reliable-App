using MyReliableSite.Shared.DTOs.Notifications.Enums;

namespace MyReliableSite.Shared.DTOs.Notifications;

public class JobNotification : INotificationMessage
{
    public string MessageType { get; set; } = typeof(JobNotification).Name;
    public string Message { get; set; }
    public string JobId { get; set; }
    public decimal Progress { get; set; }
    public NotificationType NotificationType { get; set; }
    public NotificationTargetUserTypes TargetUserTypes { get; set; }
    public string Title { get; set; }
    public Guid? NotifyModelId { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
}