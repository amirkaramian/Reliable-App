using MyReliableSite.Shared.DTOs.Transaction;
using System.ComponentModel.DataAnnotations.Schema;

namespace MyReliableSite.Shared.DTOs.Billing;

public class TransactionEXL
{
    public Guid Id { get; set; }
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int TransactionNo { get; set; }
    public int ReferenceNo { get; set; }
    public Guid ReferenceId { get; set; }
    public string TransactionBy { get; set; }
    public Guid CreatedBy { get; set; }
    public string CreatedOn { get; set; }
    public Guid LastModifiedBy { get; set; }
    public Guid DeletedBy { get; set; }
    public Guid AdminAsClient { get; set; }
    public string LastModifiedOn { get; set; }
    public string DeletedOn { get; set; }
    public string ActionTakenBy { get; set; }
    public TransactionType TransactionType { get; set; }
    public decimal Total { get; set; }
    public string Tenant { get; set; }
    public string Notes { get; set; }
    public TransactionByRole TransactionByRole { get; set; }
    public TransactionStatus TransactionStatus { get; set; }
    public decimal? RefundRetainPercentage { get; set; }
    public decimal? TotalAfterRefundRetain { get; set; }
}