using MyReliableSite.Domain.Common.Contracts;
using MyReliableSite.Domain.Contracts;
using MyReliableSite.Shared.DTOs.Notifications.Enums;
using System.ComponentModel.DataAnnotations.Schema;

namespace MyReliableSite.Domain.Billing;

public class NotificationTemplate : AuditableEntity, IMustHaveTenant
{
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int NotificationTemplateNo { get; set; }
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public string Title { get; set; }
    public string Body { get; set; }
    public string Tenant { get; set; }
    public NotificationTemplateStatus Status { get; set; }
    public NotificationTargetUserTypes TargetUserType { get; set; }
    public ConditionBasedOn Property { get; set; } // Like for Bills, Products, Tickets etc
    public string OperatorType { get; set; } // Like <=, >=, != etc
    public string Value { get; set; } // Like 100, ProductName etc

    public NotificationTemplate()
    {
    }

    public NotificationTemplate(string title, string body, DateTime? startDate, DateTime? endDate, NotificationTemplateStatus status, NotificationTargetUserTypes targetUserType, ConditionBasedOn property, string operatorType, string value)
    {
        Title = title;
        Body = body;
        StartDate = startDate;
        EndDate = endDate;
        Status = status;
        TargetUserType = targetUserType;
        Property = property;
        OperatorType = operatorType;
        Value = value;
    }

    public NotificationTemplate Update(string title, string body, DateTime? startDate, DateTime? endDate, NotificationTemplateStatus status, NotificationTargetUserTypes targetUserType, ConditionBasedOn property, string operatorType, string value)
    {
        if (!string.IsNullOrWhiteSpace(title) && !string.Equals(Title, title, StringComparison.InvariantCultureIgnoreCase)) Title = title;
        if (!string.IsNullOrWhiteSpace(body) && !string.Equals(Body, body, StringComparison.InvariantCultureIgnoreCase)) Body = body;
        if(startDate != StartDate) StartDate = startDate;
        if(endDate != EndDate) EndDate = endDate;
        if(status != Status) Status = status;
        if(targetUserType != TargetUserType) TargetUserType = targetUserType;
        if(property != Property) Property = property;
        if (!string.IsNullOrWhiteSpace(operatorType) && !string.Equals(OperatorType, operatorType, StringComparison.InvariantCultureIgnoreCase)) OperatorType = operatorType;
        if (!string.IsNullOrWhiteSpace(value) && !string.Equals(Value, value, StringComparison.InvariantCultureIgnoreCase)) Value = value;
        return this;
    }
}