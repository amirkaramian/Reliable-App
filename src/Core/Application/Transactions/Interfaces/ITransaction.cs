using MyReliableSite.Application.Common.Interfaces;
using MyReliableSite.Application.Wrapper;
using MyReliableSite.Shared.DTOs.Billing;
using MyReliableSite.Shared.DTOs.Settings;
using MyReliableSite.Shared.DTOs.Transaction;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyReliableSite.Application.Transactions.Interfaces;
public interface ITransaction : ITransientService
{
    Task<Result<TransactionDetailsDto>> GetTransactionDetailsAsync(Guid id);
    Task<PaginatedResult<TransactionDto>> SearchAsync(TransactionListFilter filter);
    Task<Result<Guid>> UpdateTransactionAsync(UpdateTransactionRequest request, Guid id);

    Task<Result<List<TransactionEXL>>> GetTransactionListAsync(string userId, DateTime startDate, DateTime endDate);
}
