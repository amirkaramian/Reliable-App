using MyReliableSite.Application.Common.Interfaces;
using MyReliableSite.Application.Wrapper;
using MyReliableSite.Shared.DTOs.Incomes;
using MyReliableSite.Shared.DTOs.Transaction;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyReliableSite.Application.Incomes.Interfaces;
public interface IIncomeService : ITransientService
{
    Task<PaginatedResult<IncomeDto>> SearchAsync(IncomeListFilter filter);
    Task<Result<IncomeDetailsDto>> GetIncomingDetailsAsync(Guid id);

    Task<string> GetIncomeOverview();

    Task<string> GetIncomeForecast();

    Task<PaginatedResult<TransactionDto>> GetIncomeTransactionOverviewAsync(TransactionListFilter filter, string transactionType);
    Task<Result<IncomeHistoryDto>> GetIncomingHistoryAsync();
}
