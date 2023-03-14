using MyReliableSite.Domain.Common.Contracts;
using MyReliableSite.Domain.Contracts;
using MyReliableSite.Shared.DTOs.Notifications.Enums;

namespace MyReliableSite.Domain.Billing;

public class Notification : AuditableEntity, IMustHaveTenant
{
    public Guid? ToUserId { get; set; }
    public NotificationType Type { get; set; }
    public NotificationStatus Status { get; set; }
    public NotificationTargetUserTypes TargetUserTypes { get; set; }
    public DateTime SentAt { get; set; }
    public string Title { get; set; }
    public string Body { get; set; }
    public string Tenant { get; set; }
    public bool? IsRead { get; set; }
    public Guid? NotifyModelId { get; set; }

    public Notification()
    {
    }

    public Notification(Guid? toUserId, NotificationType type, NotificationStatus status, NotificationTargetUserTypes targetUserTypes, DateTime sentAt, string body, string title, bool? isRead, Guid? notifyModelId)
    {
        ToUserId = toUserId;
        Type = type;
        Status = status;
        TargetUserTypes = targetUserTypes;
        SentAt = sentAt;
        Body = body;
        Title = title;
        NotifyModelId = notifyModelId;
    }

    public Notification Update(Guid? toUserId, NotificationType type, NotificationStatus status, NotificationTargetUserTypes targetUserTypes, DateTime sentAt, string body, string title, bool? isRead)
    {
        if (toUserId != null && toUserId != Guid.Empty && !string.Equals(ToUserId?.ToString(), toUserId.ToString(), StringComparison.CurrentCultureIgnoreCase)) ToUserId = toUserId;
        if(type != Type) Type = type;
        if(status != Status) Status = status;
        if(sentAt != SentAt) SentAt = sentAt;
        if(targetUserTypes != TargetUserTypes) TargetUserTypes = targetUserTypes;
        if (!string.IsNullOrWhiteSpace(body) && !string.Equals(Body, body, StringComparison.InvariantCultureIgnoreCase)) Body = body;
        if (!string.IsNullOrWhiteSpace(title) && !string.Equals(Title, title, StringComparison.InvariantCultureIgnoreCase)) Title = title;
        if (isRead != IsRead) IsRead = isRead;
        return this;
    }

    public Notification Update(bool isRead)
    {
        IsRead = isRead;
        return this;
    }

    public Notification UpdateSent(DateTime sentAt)
    {
        SentAt = sentAt;
        return this;
    }
}