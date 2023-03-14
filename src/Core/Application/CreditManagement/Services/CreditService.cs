using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IronPython.Runtime;
using Mapster;
using MediatR;
using Microsoft.Extensions.Localization;
using MyReliableSite.Application.Abstractions.Services.Identity;
using MyReliableSite.Application.Bills.Services;
using MyReliableSite.Application.Common.Interfaces;
using MyReliableSite.Application.CreditManagement.Interfaces;
using MyReliableSite.Application.Exceptions;
using MyReliableSite.Application.ManageModule.Interfaces;
using MyReliableSite.Application.Storage;
using MyReliableSite.Application.Wrapper;
using MyReliableSite.Domain.Billing;
using MyReliableSite.Domain.Billing.Events.TemplateVariables;
using MyReliableSite.Domain.Categories;
using MyReliableSite.Domain.Common.Contracts;
using MyReliableSite.Domain.Products;
using MyReliableSite.Shared.DTOs.Billing;
using MyReliableSite.Shared.DTOs.Bills;
using MyReliableSite.Shared.DTOs.Brands;
using MyReliableSite.Shared.DTOs.CreditManagement;
using MyReliableSite.Shared.DTOs.ManageModule;
using MyReliableSite.Shared.DTOs.Orders;
using MyReliableSite.Shared.DTOs.Products;
using MyReliableSite.Shared.DTOs.Refund;
using Newtonsoft.Json;
using Polly.Caching;
using static MyReliableSite.Domain.Billing.Credit;
using static MyReliableSite.Domain.Constants.PermissionConstants;

namespace MyReliableSite.Application.CreditManagement.Services;
public class CreditService : ICreditService
{
    private readonly IStringLocalizer<CreditService> _localizer;
    private readonly IRepositoryAsync _repository;
    private readonly IUserService _userService;
    private readonly ICurrentUser _currentUser;
    private readonly IUserModuleManagementService _service;
    public CreditService(IStringLocalizer<CreditService> localizer, IRepositoryAsync repository, IUserService userService, ICurrentUser currentUser, IUserModuleManagementService service)
    {
        _localizer = localizer;
        _repository = repository;
        _userService = userService;
        _currentUser = currentUser;
        _service = service;
    }

    public async Task<Result<ClientCreditInfoDto>> ClientAddCreditAsync(ClientCreateCreditRequest request)
    {
        var setting = await _repository.FirstByConditionAsync<Setting>(x => x.Tenant.ToLower() == request.Tenant.ToLower());
        if (setting == null) throw new EntityNotFoundException(string.Format(_localizer["setting.notfound"]));
        var appSetting = await _repository.FirstByConditionAsync<UserAppSetting>(x => x.Tenant.ToLower() == request.Tenant.ToLower() && x.UserId == _currentUser.GetUserId().ToString());
        decimal? maxCredit = null;
        if (appSetting == null)
        {
            var billingSetting = await _repository.FirstByConditionAsync<BillingSetting>(x => x.Tenant.ToLower() == request.Tenant.ToLower());
            if (billingSetting == null) throw new EntityNotFoundException(string.Format(_localizer["billingSetting.notfound"]));
            if (request.Amount > billingSetting.MaxCreditAmount) throw new CustomException(string.Format(_localizer["billingSetting.InvalidAmount"]));
            maxCredit = billingSetting.MaxCreditAmount;
        }
        else
        {
            if (request.Amount > appSetting.MaxCreditAmount) throw new CustomException(string.Format(_localizer["appsetting.maxcreditlimit"]));
            maxCredit = appSetting.MaxCreditAmount;
        }

        var credit = new Credit(_currentUser.GetUserId(), request.Amount, DateTime.Now, request.Description);
        var creditInfo = await GetCreditInfo(new CreditInfoRequest(credit.Id, _currentUser.GetUserId()) { Tenant = request.Tenant });
        if (maxCredit.HasValue && creditInfo != null && creditInfo.Data.Balance + request.Amount > maxCredit)
        {
            throw new CustomException(string.Format(_localizer["appsetting.maxcreditlimit"]));
        }

        creditInfo.Data.Balance += request.Amount;
        var creditId = await _repository.CreateAsync(credit);
        _ = await _repository.SaveChangesAsync();
        return await Result<ClientCreditInfoDto>.SuccessAsync(creditInfo.Data);
    }

    public async Task<Result<ClientCreditInfoDto>> CreateCreditAsync(CreateCreditRequest request)
    {
        var setting = await _repository.FirstByConditionAsync<Setting>(x => x.Tenant.ToLower() == request.Tenant.ToLower());
        if (setting == null) throw new EntityNotFoundException(string.Format(_localizer["setting.notfound"]));
        var appSetting = await _repository.FirstByConditionAsync<UserAppSetting>(x => x.Tenant.ToLower() == request.Tenant.ToLower() && x.UserId == request.ClientId.ToString());
        decimal? maxCredit = null;
        if (appSetting == null)
        {
            var billingSetting = await _repository.FirstByConditionAsync<BillingSetting>(x => x.Tenant.ToLower() == request.Tenant.ToLower());
            if (billingSetting == null) throw new EntityNotFoundException(string.Format(_localizer["billingSetting.notfound"]));
            if (request.Amount > billingSetting.MaxCreditAmount) throw new CustomException(string.Format(_localizer["appsetting.maxcreditlimit"]));
            maxCredit = billingSetting.MaxCreditAmount;
        }
        else
        {
            if (request.Amount > appSetting.MaxCreditAmount) throw new CustomException(string.Format(_localizer["appsetting.maxcreditlimit"]));
            maxCredit = appSetting.MaxCreditAmount;
        }

        var credit = new Credit(request.ClientId, request.Amount, DateTime.Now, request.Description);
        var creditInfo = await GetCreditInfo(new CreditInfoRequest(credit.Id, request.ClientId) { Tenant = request.Tenant });
        if (maxCredit.HasValue && creditInfo != null && creditInfo.Data.Balance + request.Amount > maxCredit)
        {
            throw new CustomException(string.Format(_localizer["appsetting.maxcreditlimit"]));
        }

        creditInfo.Data.Balance += request.Amount;
        var creditId = await _repository.CreateAsync(credit);
        int res = await _repository.SaveChangesAsync();
        return await Result<ClientCreditInfoDto>.SuccessAsync(creditInfo.Data);
    }

    public async Task<Result<ClientCreditInfoDto>> CreateCreditAdminAsync(CreateCreditRequest request)
    {
        var setting = await _repository.FirstByConditionAsync<Setting>(x => x.Tenant.ToLower() == request.Tenant.ToLower());
        if (setting == null) throw new EntityNotFoundException(string.Format(_localizer["setting.notfound"]));
        var appSetting = await _repository.FirstByConditionAsync<UserAppSetting>(x => x.Tenant.ToLower() == request.Tenant.ToLower() && x.UserId == request.ClientId.ToString());
        decimal? maxCredit = null;
        if (appSetting == null)
        {
            var billingSetting = await _repository.FirstByConditionAsync<BillingSetting>(x => x.Tenant.ToLower() == request.Tenant.ToLower());
            if (billingSetting == null) throw new EntityNotFoundException(string.Format(_localizer["billingSetting.notfound"]));
            if (request.Amount > billingSetting.MaxCreditAmount) throw new CustomException(string.Format(_localizer["appsetting.maxcreditlimit"]));
            maxCredit = billingSetting.MaxCreditAmount;
        }
        else
        {
            if (request.Amount > appSetting.MaxCreditAmount) throw new CustomException(string.Format(_localizer["appsetting.maxcreditlimit"]));
            maxCredit = appSetting.MaxCreditAmount;
        }

        var credit = new Credit(request.ClientId, request.Amount, DateTime.Now, request.Description);
        var creditInfo = await GetCreditInfo(new CreditInfoRequest(credit.Id, request.ClientId) { Tenant = request.Tenant });
        if (maxCredit.HasValue && creditInfo != null && creditInfo.Data.Balance + request.Amount > maxCredit)
        {
            throw new CustomException(string.Format(_localizer["appsetting.maxcreditlimit"]));
        }

        creditInfo.Data.Balance += request.Amount;

        var creditId = await _repository.CreateAsync(credit);

        var transaction = new Transaction(
            _currentUser.GetUserId().ToString(),
            TransactionType.Credit,
            request.Amount,
            0,
            request.Notes,
            creditId,
            TransactionByRole.Admin,
            TransactionStatus.Completed,
            string.Empty);

        var id = await _repository.CreateAsync<Transaction>(transaction);

        int res = await _repository.SaveChangesAsync();

        creditInfo.Data.TransactionStatus = MyReliableSite.Shared.DTOs.Transaction.TransactionStatus.Completed;
        return await Result<ClientCreditInfoDto>.SuccessAsync(creditInfo.Data);
    }

    public async Task<PaginatedResult<CreditSearchDto>> SearchAsync(CreditListFilter filter)
    {
        var filters = new Filters<Credit>();
        {
            filters.Add(!string.IsNullOrEmpty(filter.ClientId), x => x.UserId == Guid.Parse(filter.ClientId));
        }

        return await _repository.GetSearchResultsAsync<Credit, CreditSearchDto>(filter.PageNumber, filter.PageSize, filter.OrderBy, filter.OrderType, filters, filter.AdvancedSearch, filter.Keyword);
    }

    public async Task<Result<Guid>> RemoveCreditAsync(Guid id)
    {
        var creditToDelete = await _repository.RemoveByIdAsync<Credit>(id);
        _ = await _repository.SaveChangesAsync();
        return await Result<Guid>.SuccessAsync(id);
    }

    public async Task<Result<ClientCreditInfoDto>> DecreaseCreditAsync(DecreaseCreditRequest request)
    {
        var setting = await _repository.FirstByConditionAsync<Setting>(x => x.Tenant.ToLower() == request.Tenant.ToLower());
        if (setting == null) throw new EntityNotFoundException(string.Format(_localizer["setting.notfound"]));

        var credit = await _repository.FirstByConditionAsync<Credit>(x => x.UserId == request.ClientId);
        if (credit == null) throw new EntityNotFoundException(string.Format(_localizer["credit.notfound"]));

        var creditInfo = await GetCreditInfo(new CreditInfoRequest(credit.Id, request.ClientId) { Tenant = request.Tenant });
        decimal newBalance = creditInfo.Data.Balance - request.DecreaseAmount;
        if (newBalance < 0)
            throw new CustomException(string.Format(_localizer["credit.NegetiveCreditBalance"]), null, System.Net.HttpStatusCode.Forbidden);
        credit = new Credit(request.ClientId, request.DecreaseAmount, DateTime.Now, request.Description, (byte)Credit.CreditTransactionTypes.Decrease);
        var creditId = await _repository.CreateAsync(credit);
        _ = await _repository.SaveChangesAsync();
        var info = new ClientCreditInfoDto(credit.Id, request.ClientId, newBalance);
        return await Result<ClientCreditInfoDto>.SuccessAsync(info);
    }

    public async Task<Result<ClientCreditInfoDto>> GetCreditInfo(CreditInfoRequest request)
    {
        var creditList = await _repository.GetListAsync<Credit>(x => x.UserId == request.ClientId && x.DeletedBy == null);
        var refunds = await _repository.GetListAsync<Refund>(x => x.RequestById == request.Id && x.RefundStatus == Domain.Billing.RefundStatus.Completed);
        var trx = await _repository.GetListAsync<Transaction>(x => x.TransactionBy == request.ClientId.ToString()
        && x.TransactionStatus == TransactionStatus.Completed && x.TransactionType == TransactionType.Invoice);
        decimal addAmount = creditList.Where(x => x.CreditTransactionType == (byte)CreditTransactionTypes.Increase).Sum(x => x.Amount) + refunds.Sum(x => x.Total);
        decimal minusAmount = creditList.Where(x => x.CreditTransactionType == (byte)CreditTransactionTypes.Decrease).Sum(x => x.Amount) + trx.Sum(x => x.Total);
        var creditInfo = new ClientCreditInfoDto(request.Id, request.ClientId, addAmount - minusAmount);
        return await Result<ClientCreditInfoDto>.SuccessAsync(creditInfo);
    }

    public async Task<Result<Guid>> MakeCreditPaymentRequest(MakeCreditPaymentRequest request)
    {
        var setting = await _repository.FirstByConditionAsync<Setting>(x => x.Tenant.ToLower() == request.Tenant.ToLower());
        if (setting == null)
            throw new EntityNotFoundException(string.Format(_localizer["setting.notfound"]));
        var appSetting = await _repository.FirstByConditionAsync<UserAppSetting>(x => x.Tenant.ToLower() == request.Tenant.ToLower() && x.UserId == _currentUser.GetUserId().ToString());
        if (appSetting == null || !appSetting.IsActiveOrPendingProduct)
            throw new EntityNotFoundException(string.Format(_localizer["usersetting.creditpaymentnotpctive"]));
        var creditInfo = await GetCreditInfo(new CreditInfoRequest(_currentUser.GetUserId()) { Tenant = request.Tenant });
        if (creditInfo.Data.Balance < request.TotalAmount)
            throw new CustomException(string.Format(_localizer["credit.insufficientCreditBalance"]), null, System.Net.HttpStatusCode.Forbidden);
        var bill = await _repository.FirstByConditionAsync<Bill>(x => x.BillNo == request.InvoiceNumber);
        if (bill == null) throw new EntityNotFoundException(string.Format(_localizer["bill.notfound"]));
        var order = await _repository.FirstByConditionAsync<Order>(x => x.Id == bill.OrderId);
        if (order == null) throw new EntityNotFoundException(string.Format(_localizer["order.notfound"]));
        var transaction = await _repository.FirstByConditionAsync<Transaction>(x => x.ReferenceId == order.Id && (x.TransactionStatus == TransactionStatus.Pending || x.TransactionStatus == TransactionStatus.Completed));
        if (transaction != null && transaction.Id != Guid.Empty)
            throw new CustomException(string.Format(_localizer["transaction.paymentInProcess"]), null);

        var id = await _repository.CreateAsync<Transaction>(
              new Transaction(
              _currentUser.GetUserId().ToString(),
              TransactionType.Invoice,
              request.TotalAmount,
              order.OrderNo,
              request.Notes,
              order.Id,
              TransactionByRole.Client,
              TransactionStatus.Completed,
              string.Empty));
        order.Status = OrderStatus.Completed;
        await _repository.SaveChangesAsync();
        return await Result<Guid>.SuccessAsync(id);
    }

    public async Task<Result<decimal>> GetClientCreditInfo(string clientId = null)
    {
        string userId = string.IsNullOrEmpty(clientId) ? _currentUser.GetUserId().ToString() : clientId;
        var user = await _userService.GetUserProfileAsync(userId);
        if (user?.Data != null && !string.IsNullOrEmpty(user.Data.ParentID))
            userId = user.Data.ParentID;
        var creditInfo = await GetCreditInfo(new CreditInfoRequest(Guid.Parse(userId)) { Tenant = _currentUser.GetTenant() });
        return await Result<decimal>.SuccessAsync(creditInfo.Data.Balance);
    }

    public async Task<Result<Guid>> MakeAllInvoicePaymentRequest(MakeAllInvoiceCreditPaymentRequest request)
    {
        var setting = await _repository.FirstByConditionAsync<Setting>(x => x.Tenant.ToLower() == request.Tenant.ToLower());
        if (setting == null)
            throw new EntityNotFoundException(string.Format(_localizer["setting.notfound"]));
        var appSetting = await _repository.FirstByConditionAsync<UserAppSetting>(x => x.Tenant.ToLower() == request.Tenant.ToLower() && x.UserId == _currentUser.GetUserId().ToString());
        if (appSetting == null || !appSetting.IsActiveOrPendingProduct)
            throw new EntityNotFoundException(string.Format(_localizer["usersetting.creditpaymentnotpctive"]));

        var bills = await _repository.FindByConditionAsync<Bill>(x => x.UserId == _currentUser.GetUserId().ToString());
        if (bills == null) throw new EntityNotFoundException(string.Format(_localizer["bill.notfound"]));
        var orderIds = bills.Select(x => x.OrderId).ToList();
        var orders = await _repository.FindByConditionAsync<Order>(x => orderIds.Contains(x.Id));
        if (orders == null) throw new EntityNotFoundException(string.Format(_localizer["order.notfound"]));
        var orderids = orders.Select(x => x.Id).ToList();
        var transaction = await _repository.FindByConditionAsync<Transaction>(x => orderids.Contains(x.ReferenceId) && (x.TransactionStatus == TransactionStatus.Pending || x.TransactionStatus == TransactionStatus.Completed));
        if (transaction != null && !transaction.Any())
            throw new CustomException(string.Format(_localizer["bill.nobilltopay"]), null);

        decimal sumAmount = transaction.Sum(x => x.Total);
        var creditInfo = await GetCreditInfo(new CreditInfoRequest(_currentUser.GetUserId()) { Tenant = request.Tenant });
        if (sumAmount > creditInfo.Data.Balance)
            throw new CustomException(string.Format(_localizer["credit.insufficientCreditBalance"]), null, System.Net.HttpStatusCode.Forbidden);
        foreach (var order in orders)
        {
            var id = await _repository.CreateAsync<Transaction>(
                  new Transaction(
                  _currentUser.GetUserId().ToString(),
                  TransactionType.Invoice,
                  transaction.First(x => x.ReferenceId == order.Id).Total,
                  order.OrderNo,
                  $"{request.Notes} pay for invoice {order.InvoiceNo}",
                  order.Id,
                  TransactionByRole.Client,
                  TransactionStatus.Completed,
                  string.Empty));
            order.Status = OrderStatus.Completed;
            await _repository.UpdateAsync<Order>(order);
        }

        int cnt = await _repository.SaveChangesAsync();
        return await Result<Guid>.SuccessAsync(cnt.ToString());
    }

    public async Task<Result<List<CreditEXL>>> GetCreditListAsync(string clientId, DateTime startDate, DateTime endDate)
    {

        var credits = await _repository.QueryWithDtoAsync<CreditEXL>($@"SELECT C.*
                                                                                                        FROM Credit C
                                                                                                        WHERE ((CONVERT(date, [C].[CreatedOn]) >= '{startDate}') AND (CONVERT(date, [C].[CreatedOn]) <= '{endDate}')) and DeletedOn is null and UserId = '{clientId}' ORDER BY C.CreatedOn ASC");
        return await Result<List<CreditEXL>>.SuccessAsync(credits.ToList());
    }

    private bool GetUserPermision(List<UserModuleDto> modules, string module)
    {
        var permissionForThisController = modules.FirstOrDefault(m => m.Name.Equals(module, StringComparison.OrdinalIgnoreCase));
        if (permissionForThisController == null)
            return false;
        var permissions = JsonConvert.DeserializeObject<Dictionary<string, bool>>(permissionForThisController.PermissionDetail);
        return permissions.TryGetValue("View", out bool isActive) && isActive;
    }
}
