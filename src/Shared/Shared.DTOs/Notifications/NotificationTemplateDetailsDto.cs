using MyReliableSite.Shared.DTOs.Notifications.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyReliableSite.Shared.DTOs.Notifications;

public class NotificationTemplateDetailsDto : IDto
{
    public int NotificationTemplateNo { get; set; }
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public string Title { get; set; }
    public string Body { get; set; }
    public string Tenant { get; set; }
    public Guid CreatedBy { get; set; }
    public NotificationTemplateStatus Status { get; set; }
    public NotificationTargetUserTypes TargetUserType { get; set; }
    public ConditionBasedOn Property { get; set; } // Like for Bills, Products, Tickets etc
    public string OperatorType { get; set; } // Like <=, >=, != etc
    public string Value { get; set; } // Like 100, ProductName etc
}
