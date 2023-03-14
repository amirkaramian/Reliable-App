using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyReliableSite.Shared.DTOs.Transaction;
public class TransactionDto : IDto
{
    public Guid Id { get; set; }
    public int TransactionNo { get; set; }
    public int ReferenceNo { get; set; }
    public Guid ReferenceId { get; set; }
    public DateTime CreatedOn { get; set; }
    public DateTime? LastModifiedOn { get; set; }
    public string TransactionBy { get; set; }
    public string ActionTakenBy { get; set; }
    public TransactionType TransactionType { get; set; }
    public decimal Total { get; set; }
    public decimal TotalAmount { get; set; }
    public decimal RunningMonthlyTotal { get; set; }
    public decimal RunningQuarterlyTotal { get; set; }
    public decimal RunningSemiAnnuallyTotal { get; set; }
    public decimal RunningAnnuallyTotal { get; set; }
    public decimal RunningBienniallyTotal { get; set; }
    public decimal RunningTrieniallyTotal { get; set; }
    public string Notes { get; set; }
    public TransactionByRole TransactionByRole { get; set; }
    public TransactionStatus TransactionStatus { get; set; }
    public decimal? RefundRetainPercentage { get; set; }
    public decimal? TotalAfterRefundRetain { get; set; }
    public string UserImagePath { get; set; }
    public string FullName { get; set; }
    public string Description { get; set; }
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
    Refund
}

public enum TransactionByRole
{
    Admin,
    Client
}