using Mapster;
using Microsoft.Extensions.Localization;
using MyReliableSite.Application.Abstractions.Services.Identity;
using MyReliableSite.Application.Common.Interfaces;
using MyReliableSite.Application.Exceptions;
using MyReliableSite.Application.Transactions.Interfaces;
using MyReliableSite.Application.Wrapper;
using MyReliableSite.Domain.Billing;
using MyReliableSite.Domain.Billing.Events;
using MyReliableSite.Domain.Common.Contracts;
using MyReliableSite.Domain.KnowledgeBase;
using MyReliableSite.Shared.DTOs.Billing;
using MyReliableSite.Shared.DTOs.Settings;
using MyReliableSite.Shared.DTOs.Transaction;

namespace MyReliableSite.Application.Transactions.Services;
public class TransactionService : ITransaction
{
    private readonly IStringLocalizer<TransactionService> _localizer;
    private readonly IRepositoryAsync _repository;
    private readonly ICurrentUser _user;
    private readonly IUserService _userService;

    public TransactionService()
    {
    }

    public TransactionService(IRepositoryAsync repository, IStringLocalizer<TransactionService> localizer, ICurrentUser user, IUserService userService)
    {
        _repository = repository;
        _localizer = localizer;
        _user = user;
        _userService = userService;
    }

    public async Task<Result<List<TransactionEXL>>> GetTransactionListAsync(string userId, DateTime startDate, DateTime endDate)
    {

        var transactions = await _repository.QueryWithDtoAsync<TransactionEXL>($@"SELECT T.Id, T.TransactionBy,T.Tenant, T.CreatedBy,T.CreatedOn, T.LastModifiedBy, T.LastModifiedOn,T.ReferenceNo, CAST(T.Total AS REAL) AS Total,T.TransactionNo, T.TransactionBy, T.TransactionStatus,T.Notes, T.ReferenceId, T.ActionTakenBy, T.TransactionByRole, T.PaymentMethod, T.AdminAsClient, T.RefundRetainPercentage, T.TotalAfterRefundRetain
                                                                                                        FROM Transactions T
                                                                                                        WHERE ((CONVERT(date, [T].[CreatedOn]) >= '{startDate}') AND (CONVERT(date, [T].[CreatedOn]) <= '{endDate}')) and DeletedOn is null and CreatedBy = '{userId}' ORDER BY T.CreatedOn ASC");
        return await Result<List<TransactionEXL>>.SuccessAsync(transactions.ToList());
    }

    public async Task<Result<TransactionDetailsDto>> GetTransactionDetailsAsync(Guid id)
    {
        var transaction = await _repository.GetByIdAsync<Transaction, TransactionDetailsDto>(id);

        return await Result<TransactionDetailsDto>.SuccessAsync(transaction);
    }

    public async Task<PaginatedResult<TransactionDto>> SearchAsync(TransactionListFilter filter)
    {
        var transactions = await _repository.GetSearchResultsAsync<Transaction, TransactionDto>(filter.PageNumber, filter.PageSize, filter.OrderBy, filter.OrderType, filter.AdvancedSearch, filter.Keyword, x => (filter.StartDate == null || x.CreatedOn >= filter.StartDate) && (filter.EndDate == null || x.CreatedOn <= filter.EndDate));

        if (transactions != null && transactions.Data != null && transactions.Data.Any())
        {
            var userDetails = await _userService.GetAllAsync(transactions.Data.Select(x => x.TransactionBy));

            foreach (var tran in transactions.Data)
            {
                var user = userDetails.Data.FirstOrDefault(x => x.Id == Guid.Parse(tran.TransactionBy));

                if (user != null)
                {
                    tran.FullName = user.FullName;
                    tran.UserImagePath = user.ImageUrl;
                }
            }
        }

        return transactions;
    }

    public async Task<Result<Guid>> UpdateTransactionAsync(UpdateTransactionRequest request, Guid id)
    {
        var billingSetting = await _repository.FirstByConditionAsync<BillingSetting>(x => x.Tenant.ToLower() == request.Tenant.ToLower());
        if (billingSetting == null) throw new EntityNotFoundException(string.Format(_localizer["billingSetting.notfound"]));

        var transaction = await _repository.GetByIdAsync<Transaction>(id);
        if (transaction == null) throw new EntityNotFoundException(string.Format(_localizer["transaction.notfound"], id));

        decimal refundRetainPercentage = billingSetting.RefundRetainPercentage;
        decimal? totalAfterRefundRetain = transaction.TotalAfterRefundRetain;

        var refund = await _repository.FirstByConditionAsync<Refund>(x => x.Id == transaction.ReferenceId && x.RefundStatus == RefundStatus.Requested);

        if (refund != null && refund.RefundStatus == RefundStatus.Requested)
        {
            if (request.TransactionStatus == Shared.DTOs.Transaction.TransactionStatus.Completed)
            {
                if (refundRetainPercentage != 0)
                    totalAfterRefundRetain = (refundRetainPercentage / 100) * transaction.Total;
            }

            // Update Refund Finance
            var refundToUpdate = refund.Update(request.Notes, refund.Total, totalAfterRefundRetain.Value, RefundStatus.Completed, _user.GetUserId());
            await _repository.UpdateAsync(refundToUpdate);
        }

        transaction = transaction.Update(request.Notes, (Domain.Billing.TransactionStatus)request.TransactionStatus, _user.GetUserId().ToString(), refundRetainPercentage, totalAfterRefundRetain);

        transaction.DomainEvents.Add(new StatsChangedEvent());

        await _repository.UpdateAsync<Transaction>(transaction);

        await _repository.SaveChangesAsync();

        return await Result<Guid>.SuccessAsync(id);
    }
}