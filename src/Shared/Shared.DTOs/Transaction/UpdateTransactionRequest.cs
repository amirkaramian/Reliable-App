using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyReliableSite.Shared.DTOs.Transaction;
public class UpdateTransactionRequest : IDto
{
    public string Notes { get; set; }
    public TransactionStatus TransactionStatus { get; set; }
    public string Tenant { get; set; }
}
