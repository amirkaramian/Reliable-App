using MyReliableSite.Shared.DTOs.Notifications.Enums;

namespace MyReliableSite.Shared.DTOs.Notifications.Templates;

public class NotificationTemplateDto : IDto
{
    public Guid Id { get; set; }
    public int NotificationTemplateNo { get; set; }
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public string Title { get; set; }
    public string Body { get; set; }
    public string Tenant { get; set; }
    public Guid CreatedBy { get; set; }
    public int TotalUsers { get; set; } // Total Admins or Clients based on Property and conditions
    public NotificationTemplateStatus Status { get; set; }
    public NotificationTargetUserTypes TargetUserType { get; set; }
    public ConditionBasedOn Property { get; set; } // Like for Bills, Products, Tickets etc
    public string OperatorType { get; set; } // Like <=, >=, != etc
    public string Value { get; set; } // Like 100, ProductName etc
}
