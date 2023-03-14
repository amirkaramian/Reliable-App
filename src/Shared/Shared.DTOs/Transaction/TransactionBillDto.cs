using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace MyReliableSite.Shared.DTOs.Transaction;
public class TransactionBillDto : IDto
{
    public Guid Id { get; set; }
    public int TransactionNo { get; set; }
    public int ReferenceNo { get; set; }
    public Guid ReferenceId { get; set; }
    [JsonPropertyName("date")]
    public DateTime CreatedOn { get; set; }
    public string TransactionBy { get; set; }
    public string ActionTakenBy { get; set; }
    public TransactionType TransactionType { get; set; }
    [JsonPropertyName("amount")]
    public decimal Total { get; set; }
    public string Notes { get; set; }
    public TransactionByRole TransactionByRole { get; set; }
    public TransactionStatus TransactionStatus { get; set; }
    public decimal? RefundRetainPercentage { get; set; }
    public decimal? TotalAfterRefundRetain { get; set; }
    public string UserImagePath { get; set; }
    public string FullName { get; set; }
    [JsonPropertyName("desciption")]
    public string Description { get; set; }
}