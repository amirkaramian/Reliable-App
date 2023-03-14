using FluentValidation;
using Mapster;
using Microsoft.Extensions.Localization;
using MyReliableSite.Application.Abstractions.Services.Identity;
using MyReliableSite.Application.Common.Interfaces;
using MyReliableSite.Application.Exceptions;
using MyReliableSite.Application.Incomes.Interfaces;
using MyReliableSite.Application.Transactions.Services;
using MyReliableSite.Application.Wrapper;
using MyReliableSite.Domain.Billing;
using MyReliableSite.Domain.Common.Contracts;
using MyReliableSite.Domain.Products;
using MyReliableSite.Shared.DTOs.Incomes;
using MyReliableSite.Shared.DTOs.Products;
using MyReliableSite.Shared.DTOs.Transaction;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using static MyReliableSite.Domain.Constants.PermissionConstants;

namespace MyReliableSite.Application.Incomes.Services;
public class IncomeService : IIncomeService
{
    private readonly IStringLocalizer<TransactionService> _localizer;
    private readonly IRepositoryAsync _repository;
    private readonly ICurrentUser _user;
    private readonly IUserService _userService;

    public IncomeService()
    {
    }

    public IncomeService(IRepositoryAsync repository, IStringLocalizer<TransactionService> localizer, ICurrentUser user, IUserService userService)
    {
        _repository = repository;
        _localizer = localizer;
        _user = user;
        _userService = userService;
    }

    public async Task<PaginatedResult<IncomeDto>> SearchAsync(IncomeListFilter filter)
    {
        var incomes = await _repository.GetSearchResultsAsync<Transaction, IncomeDto>(filter.PageNumber, filter.PageSize, filter.OrderBy, filter.OrderType, filter.AdvancedSearch, filter.Keyword, x => x.TransactionType == Domain.Billing.TransactionType.Order && ((filter.StartDate == null || x.CreatedOn >= filter.StartDate) && (filter.EndDate == null || x.CreatedOn <= filter.EndDate)));

        var userDetails = await _userService.GetAllAsync(incomes.Data.Select(x => x.TransactionBy));

        foreach (var tran in incomes.Data)
        {
            var user = userDetails.Data.FirstOrDefault(x => x.Id == Guid.Parse(tran.TransactionBy));

            if (user != null)
            {
                tran.FullName = user.FullName;
                tran.UserImagePath = user.ImageUrl;
            }
        }

        return incomes;
    }

    public async Task<Result<IncomeDetailsDto>> GetIncomingDetailsAsync(Guid id)
    {
        var income = await _repository.GetByIdAsync<Transaction, IncomeDetailsDto>(id);
        if (income == null) throw new EntityNotFoundException(string.Format(_localizer["income.notfound"]));

        return await Result<IncomeDetailsDto>.SuccessAsync(income);
    }

    public async Task<string> GetIncomeOverview()
    {
        var dateofincome = DateTime.Now.Date.AddDays(-30);

        var incomeOverview = await _repository.FindByConditionAsync<Transaction>(x => x.CreatedOn >= dateofincome);
        var jsonAttributes = new Dictionary<string, List<Dictionary<string, string>>>();
        var jsonIncomeOverviewData = new List<Dictionary<string, string>>();

        var jsonIncomeRefundOverViewData = new List<Dictionary<string, string>>();

        foreach (var income in incomeOverview)
        {
            string incomeDate = income.CreatedOn.ToShortDateString();

            if (income.TransactionType == Domain.Billing.TransactionType.Order || income.TransactionType == Domain.Billing.TransactionType.Invoice)
            {
                jsonIncomeOverviewData.Add(JsonIncomeOverviewDataAttribute(income.Adapt<TransactionDto>()));
            }
            else
            {
                jsonIncomeRefundOverViewData.Add(JsonIncomeRefundOverviewDataAttribute(income.Adapt<TransactionDto>()));
            }
        }

        jsonAttributes.Add("income", jsonIncomeOverviewData);

        jsonAttributes.Add("refund", jsonIncomeRefundOverViewData);

        string jsonIncomeRefund = JsonConvert.SerializeObject(jsonAttributes);

        return jsonIncomeRefund;
    }

    public Dictionary<string, string> JsonIncomeOverviewDataAttribute(TransactionDto item)
    {

        var jsonItemDataAttributes = new Dictionary<string, string>();

        #region Item Data

        if (!string.IsNullOrEmpty(item.CreatedOn.ToString()) && !string.IsNullOrEmpty(item.Total.ToString()))
            jsonItemDataAttributes.Add(item.CreatedOn.ToString("dd/MM/yyyy"), item.Total.ToString());

        // if (!string.IsNullOrEmpty(item.Total.ToString()))
        //    jsonItemDataAttributes.Add("total", item.Total.ToString());
        #endregion
        return jsonItemDataAttributes;

    }

    public Dictionary<string, string> JsonIncomeRefundOverviewDataAttribute(TransactionDto item)
    {

        var jsonItemDataAttributes = new Dictionary<string, string>();

        #region Item Data

        if (!string.IsNullOrEmpty(item.CreatedOn.ToString()) && !string.IsNullOrEmpty(item.Total.ToString()))
            jsonItemDataAttributes.Add(item.CreatedOn.ToString("dd/MM/yyyy"), item.Total.ToString());

        #endregion
        return jsonItemDataAttributes;

    }

    public async Task<PaginatedResult<TransactionDto>> GetIncomeTransactionOverviewAsync(TransactionListFilter filter, string transactionType)
    {
        PaginatedResult<TransactionDto> transactionList;

        if (transactionType == "Income")
        {
            // var incomeOverview = await _repository.FindByConditionAsync<Transaction>(x => x.TransactionType == Domain.Billing.TransactionType.Order || x.TransactionType == Domain.Billing.TransactionType.Invoice);

            var incomeOverview = await _repository.GetSearchResultsAsync<Transaction, TransactionDto>(filter.PageNumber, filter.PageSize, filter.OrderBy, filter.OrderType, filter.AdvancedSearch, filter.Keyword, x => (x.TransactionType == Domain.Billing.TransactionType.Order || x.TransactionType == Domain.Billing.TransactionType.Invoice) && ((filter.StartDate == null || x.CreatedOn >= filter.StartDate) && (filter.EndDate == null || x.CreatedOn <= filter.EndDate)) && (x.TransactionStatus == (Domain.Billing.TransactionStatus)filter.TransactionStatus));

            // transactionList = incomeOverview.Adapt<List<TransactionDto>>();
            transactionList = incomeOverview;
        }
        else
        {
            // var incomeOverview = await _repository.FindByConditionAsync<Transaction>(x => x.TransactionType == Domain.Billing.TransactionType.Refund);

            var incomeOverview = await _repository.GetSearchResultsAsync<Transaction, TransactionDto>(filter.PageNumber, filter.PageSize, filter.OrderBy, filter.OrderType, filter.AdvancedSearch, filter.Keyword, x => (x.TransactionType == Domain.Billing.TransactionType.Refund) && ((filter.StartDate == null || x.CreatedOn >= filter.StartDate) && (filter.EndDate == null || x.CreatedOn <= filter.EndDate)) && (x.TransactionStatus == (Domain.Billing.TransactionStatus)filter.TransactionStatus));

            // var incomeOverview = await _repository.GetSearchResultsAsync<Transaction, TransactionDto>(filter.PageNumber, filter.PageSize, filter.OrderBy, filter.OrderType, filter.AdvancedSearch, filter.Keyword, x => (x.TransactionType == Domain.Billing.TransactionType.Refund));

            transactionList = incomeOverview;

            // transactionList = incomeOverview.Adapt<List<TransactionDto>>();
        }

        return transactionList;
    }

    public async Task<string> GetIncomeForecast()
    {
        var monthly = await GetTransactionMonthyListAsync();
        var quarterly = await GetTransactionQuarterlyListAsync();
        var semiAnnually = await GetTransactionSemiAnnuallyListAsync();
        var annually = await GetTransactionAnnuallyListAsync();
        var biennially = await GetTransactionBienniallyListAsync();
        var trienially = await GetTransactionTrieniallyListAsync();

        var jsonIncomeForecastAttributes = new Dictionary<string, string>();

        var totalMonthly = monthly.Data.Select(l => l.RunningMonthlyTotal).ToList();
        var totalQuarterly = quarterly.Data.Select(l => l.RunningQuarterlyTotal).ToList();
        var totalSemiAnnually = semiAnnually.Data.Select(l => l.RunningSemiAnnuallyTotal).ToList();
        var totalAnnually = annually.Data.Select(l => l.RunningAnnuallyTotal).ToList();
        var totalBiennially = biennially.Data.Select(l => l.RunningBienniallyTotal).ToList();
        var totalTrienially = trienially.Data.Select(l => l.RunningTrieniallyTotal).ToList();

        var totalMonthlyYear = totalMonthly[0] * 12;

        var totalQuartelyYear = totalQuarterly[0] * 4;

        var totalsemiAnnuallyYearly = totalSemiAnnually[0] * 2;

        var totalAnnuallyYearly = totalAnnually[0] * 1;

        var totalBienniallyYearly = totalBiennially[0] / 2;

        var totalTrieniallyYearly = totalTrienially[0] / 3;

        var totalforecastedAnnualIncome = (totalMonthlyYear + totalQuartelyYear + totalsemiAnnuallyYearly + totalAnnuallyYearly + totalBienniallyYearly + totalTrieniallyYearly) / 6;

        jsonIncomeForecastAttributes.Add("Monthly", JsonConvert.SerializeObject(totalMonthly[0]));
        jsonIncomeForecastAttributes.Add("Quarterly", JsonConvert.SerializeObject(totalQuarterly[0]));
        jsonIncomeForecastAttributes.Add("SemiAnnually", JsonConvert.SerializeObject(totalSemiAnnually[0]));
        jsonIncomeForecastAttributes.Add("Annually", JsonConvert.SerializeObject(totalAnnually[0]));
        jsonIncomeForecastAttributes.Add("Biennially", JsonConvert.SerializeObject(totalBiennially[0]));
        jsonIncomeForecastAttributes.Add("Trienially", JsonConvert.SerializeObject(totalTrienially[0]));
        jsonIncomeForecastAttributes.Add("forecastedAnnualIncome", string.Format("{0:0.00}", totalforecastedAnnualIncome));
        string jsonIncomeForecast = JsonConvert.SerializeObject(jsonIncomeForecastAttributes);
        return jsonIncomeForecast;

    }

    public async Task<Result<List<TransactionDto>>> GetTransactionMonthyListAsync()
    {
        var monthly = await _repository.QueryWithDtoAsync<TransactionDto>($@"with Monthly1 as (
                                                                                        select Total,CreatedOn
                                                                                        from Transactions where CreatedOn >= DATEADD(M, -1, GETDATE())),
                                                                                        Monthly2 as (
	                                                                                        Select CreatedOn, SUM(Total) as TotalAmount from Monthly1 Group by CreatedOn
                                                                                        )
                                                                                        select CreatedOn, TotalAmount
	                                                                                        , (Select SUM(TotalAmount) from Monthly2) as RunningMonthlyTotal from Monthly2");
        return await Result<List<TransactionDto>>.SuccessAsync(monthly.ToList());
    }

    public async Task<Result<List<TransactionDto>>> GetTransactionQuarterlyListAsync()
    {
        var quarterly = await _repository.QueryWithDtoAsync<TransactionDto>($@"with Quarterly1 as (
                                                                                                select Total,CreatedOn as Quarterly
                                                                                                from Transactions where CreatedOn >= DATEADD(M, -3, GETDATE())),
                                                                                                Quarterly2 as (
	                                                                                                Select Quarterly, SUM(Total) as TotalAmount from Quarterly1 Group by Quarterly
                                                                                                )
                                                                                                select Quarterly, TotalAmount
	                                                                                                , (Select SUM(TotalAmount) from Quarterly2) as RunningQuarterlyTotal from Quarterly2");
        return await Result<List<TransactionDto>>.SuccessAsync(quarterly.ToList());
    }

    public async Task<Result<List<TransactionDto>>> GetTransactionSemiAnnuallyListAsync()
    {
        var semiannually = await _repository.QueryWithDtoAsync<TransactionDto>($@"with SemiAnnually1 as (
                                                                                                        select Total,CreatedOn as SemiAnnually
                                                                                                        from Transactions where CreatedOn >= DATEADD(M, -6, GETDATE())),
                                                                                                        SemiAnnually2 as (
	                                                                                                        Select SemiAnnually, SUM(Total) as TotalAmount from SemiAnnually1 Group by SemiAnnually
                                                                                                        )
                                                                                                        select SemiAnnually, TotalAmount
	                                                                                                        , (Select SUM(TotalAmount) from SemiAnnually2) as RunningSemiAnnuallyTotal from SemiAnnually2");
        return await Result<List<TransactionDto>>.SuccessAsync(semiannually.ToList());
    }

    public async Task<Result<List<TransactionDto>>> GetTransactionAnnuallyListAsync()
    {
        var annually = await _repository.QueryWithDtoAsync<TransactionDto>($@"with Annually1 as (
                                                                                                select Total,CreatedOn as Annually
                                                                                                from Transactions where CreatedOn > DATEADD(Year, -1, GETDATE())),
                                                                                                Annually2 as (
	                                                                                                Select Annually, SUM(Total) as TotalAmount from Annually1 Group by Annually
                                                                                                )
                                                                                                select Annually,TotalAmount
	                                                                                                , (Select SUM(TotalAmount) from Annually2) as RunningAnnuallyTotal from Annually2");
        return await Result<List<TransactionDto>>.SuccessAsync(annually.ToList());
    }

    public async Task<Result<List<TransactionDto>>> GetTransactionBienniallyListAsync()
    {
        var biennially = await _repository.QueryWithDtoAsync<TransactionDto>($@"with Biennially1 as (
                                                                                                    select Total,CreatedOn as Biennially
                                                                                                    from Transactions where CreatedOn >= DATEADD(YEAR, -2, GETDATE())),
                                                                                                    Biennially2 as (
	                                                                                                    Select Biennially, SUM(Total) as TotalAmount from Biennially1 Group by Biennially
                                                                                                    )
                                                                                                    select Biennially, TotalAmount
	                                                                                                    , (Select SUM(TotalAmount) from Biennially2) as RunningBienniallyTotal from Biennially2");
        return await Result<List<TransactionDto>>.SuccessAsync(biennially.ToList());
    }

    public async Task<Result<List<TransactionDto>>> GetTransactionTrieniallyListAsync()
    {
        var trienially = await _repository.QueryWithDtoAsync<TransactionDto>($@"with Trienially1 as (
                                                                                                    select Total,CreatedOn as Trienially
                                                                                                    from Transactions where CreatedOn >= DATEADD(YEAR, -3, GETDATE())),
                                                                                                    Trienially2 as (
	                                                                                                    Select Trienially, SUM(Total) as TotalAmount from Trienially1 Group by Trienially
                                                                                                    )
                                                                                                    select Trienially, TotalAmount
	                                                                                                    , (Select SUM(TotalAmount) from Trienially2) as RunningTrieniallyTotal from Trienially2");
        return await Result<List<TransactionDto>>.SuccessAsync(trienially.ToList());
    }

    public async Task<Result<IncomeHistoryDto>> GetIncomingHistoryAsync()
    {
        var incomeHistory = new IncomeHistoryDto()
        {
            IncomeOveralls = new List<IncomeOverallDto>()
        };

        var incomes = await _repository.GetListAsync<Transaction>(x => x.TransactionBy == _user.GetUserId().ToString() && x.CreatedOn.Year >= DateTime.Now.AddYears(-3).Year);
        if (incomes == null) return await Result<IncomeHistoryDto>.SuccessAsync(incomeHistory);
        var history = (from inc in incomes
                       group inc by new { Year = inc.CreatedOn.Year, Month = inc.CreatedOn.ToString("MMMM") } into grp
                       select new
                       {
                           Year = grp.Key.Year.ToString(),
                           Month = grp.Key.Month,
                           Income = grp.Sum(x => x.Total)
                       }).ToList();
        var overal = history.GroupBy(x => x.Year).Select(x => new IncomeOverallDto()
        {
            Year = x.Key,
            Total = x.Sum(y => y.Income),
            MontlyIncomes = history.Where(y => y.Year == x.Key).Select(x => new IncomeHistoryMontlyDto() { Month = x.Month, Income = x.Income }).ToList()
        }).ToList();
        incomeHistory.IncomeOveralls.AddRange(overal);
        return await Result<IncomeHistoryDto>.SuccessAsync(incomeHistory);
    }
}