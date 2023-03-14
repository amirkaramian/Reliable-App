using MyReliableSite.Application.Common.Interfaces;
using MyReliableSite.Application.Wrapper;
using MyReliableSite.Shared.DTOs.Bills;
using MyReliableSite.Shared.DTOs.CreditManagement;
using MyReliableSite.Shared.DTOs.Orders;
using MyReliableSite.Shared.DTOs.Refund;
using MyReliableSite.Shared.DTOs.Transaction;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyReliableSite.Application.Bills.Interfaces;
public interface IBillService : ITransientService
{
    Task<PaginatedResult<BillDto>> SearchAsync(BillListFilter filter);
    Task<PaginatedResult<BillDto>> GetAllBillsDetailAsync(BillListFilter filter);
    Task<Result<List<BillDetailDto>>> GetAllBillsDetailAsync();
    Task<Result<List<BillDetailDto>>> GetAllPaidBillsDetailAsync();
    Task<Result<BillDetailDto>> GetBillDetailsAsync(Guid id);
    Task<PaginatedResult<TransactionBillDto>> SearchtransactionsAsync(TransactionListFilter filter);
    Task<PaginatedResult<BillDto>> GetAllBillsFromProductAsync(BillListFilter filter, Guid productId);
    Task<Result<int>> GetUnpaidInvoices(Guid clientid);
    Task<Result<TransactionDetailsDto>> MakeInvoicePaymentRequest(MakeInvoicePaymentRequest request);
    Task<Result<ClientCreditInfoDto>> CreateRefundAdminAsync(CreateRefundRequest request);
    Task<PaginatedResult<RefundDto>> SearchRefundAsync(RefundListFilter filter);
    Task<Result<List<TransactionDetailsDto>>> MakeAllInvoicePaymentsOfCurrentUser(MakeAllInvoiceCreditPaymentRequest request);
}
