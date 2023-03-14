using MyReliableSite.Shared.DTOs.Transaction;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyReliableSite.Shared.DTOs.Incomes;
public class IncomeDetailsDto : IDto
{
    public Guid Id { get; set; }
    public int TransactionNo { get; set; }
    public int ReferenceNo { get; set; }
    public Guid ReferenceId { get; set; }
    public string TransactionBy { get; set; }
    public string ActionTakenBy { get; set; }
    public TransactionType TransactionType { get; set; }
    public decimal Total { get; set; }
    public TransactionStatus TransactionStatus { get; set; }
    public decimal? RefundRetainPercentage { get; set; }
    public decimal? TotalAfterRefundRetain { get; set; }
}
