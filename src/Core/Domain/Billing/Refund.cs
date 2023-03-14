using MyReliableSite.Domain.Common;
using MyReliableSite.Domain.Common.Contracts;
using MyReliableSite.Domain.Contracts;
using System.ComponentModel.DataAnnotations.Schema;

namespace MyReliableSite.Domain.Billing;

public class Refund : AuditableEntity, IMustHaveTenant
{
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int RefundNo { get; set; }
    public Guid RequestById { get; set; }
    public Guid? ActionTakenById { get; set; }
    public string Notes { get; set; }
    public decimal Total { get; set; } = 0;
    public decimal TotalAfterRetainPercentage { get; set; } = 0;
    public Guid OrderId { get; set; }
    public Order Order { get; set; }
    public RefundStatus RefundStatus { get; set; }
    public string Tenant { get; set; }
    public Refund(string notes, decimal total, decimal totalAfterRetainPercentage, RefundStatus refundStatus, Guid orderId, Guid requestById, Guid? actionTakenById)
    {
        OrderId = orderId;
        Notes = notes;
        Total = total;
        TotalAfterRetainPercentage = totalAfterRetainPercentage;
        RefundStatus = refundStatus;
        RequestById = requestById;
        ActionTakenById = actionTakenById;
    }

    public Refund Update(string notes, decimal total, decimal totalAfterRetainPercentage, RefundStatus refundStatus, Guid? actionTakenById)
    {
        if (Total != total) Total = total;
        if (TotalAfterRetainPercentage != totalAfterRetainPercentage) TotalAfterRetainPercentage = totalAfterRetainPercentage;
        if (notes != null && !Notes.NullToString().Equals(notes)) Notes = notes;
        if (RefundStatus != refundStatus) RefundStatus = refundStatus;
        if (ActionTakenById != actionTakenById) ActionTakenById = actionTakenById;
        return this;

    }
}

public enum RefundStatus
{
    Requested,
    Completed,
    Cancelled
}