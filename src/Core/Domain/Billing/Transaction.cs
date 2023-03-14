using MyReliableSite.Domain.Common;
using MyReliableSite.Domain.Common.Contracts;
using MyReliableSite.Domain.Contracts;
using System.ComponentModel.DataAnnotations.Schema;

namespace MyReliableSite.Domain.Billing;

public class Transaction : AuditableEntity, IMustHaveTenant
{
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int TransactionNo { get; set; }
    public int ReferenceNo { get; set; }
    public Guid ReferenceId { get; set; }
    public string TransactionBy { get; set; }
    public string ActionTakenBy { get; set; }
    public TransactionType TransactionType { get; set; }
    public decimal Total { get; set; }
    public string Tenant { get; set; }
    public string Notes { get; set; }
    public TransactionByRole TransactionByRole { get; set; }
    public TransactionStatus TransactionStatus { get; set; }
    public decimal? RefundRetainPercentage { get; set; }
    public decimal? TotalAfterRefundRetain { get; set; }
    public PaymentMethod PaymentMethod { get; set; }
    public Transaction(string transactionBy, TransactionType transactionType, decimal total, int referenceNo, string notes, Guid referenceId, TransactionByRole transactionByRole, TransactionStatus transactionStatus, string actionTakenBy)
    {
        TransactionBy = transactionBy;
        TransactionType = transactionType;
        Total = total;
        ReferenceNo = referenceNo;
        Notes = notes;
        ReferenceId = referenceId;
        TransactionByRole = transactionByRole;
        TransactionStatus = transactionStatus;
        ActionTakenBy = actionTakenBy;
    }

    public Transaction Update(string notes, TransactionStatus transactionStatus, string actionTakenBy, decimal? refundRetainPercentage, decimal? totalAfterRefundRetain)
    {
        if (notes != null && !Notes.NullToString().Equals(notes)) Notes = notes;
        if (transactionStatus != TransactionStatus) TransactionStatus = transactionStatus;
        if (actionTakenBy != null && !ActionTakenBy.NullToString().Equals(actionTakenBy)) ActionTakenBy = actionTakenBy;
        if (refundRetainPercentage != RefundRetainPercentage) RefundRetainPercentage = refundRetainPercentage;
        if (totalAfterRefundRetain != TotalAfterRefundRetain) TotalAfterRefundRetain = totalAfterRefundRetain;
        return this;
    }
}

public enum TransactionStatus
{
    Pending,
    Completed,
    Cancelled
}

public enum TransactionType
{
    Order,
    Refund,
    Invoice,
    Credit,
    Payment
}

public enum PaymentMethod
{
    CreditCard,
    PayPal,
    Cash
}

public enum TransactionByRole
{
    Admin,
    Client
}