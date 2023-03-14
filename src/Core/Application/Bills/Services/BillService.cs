using Mapster;
using Microsoft.Extensions.Localization;
using MyReliableSite.Application.Abstractions.Services.Identity;
using MyReliableSite.Application.Bills.Interfaces;
using MyReliableSite.Application.Common.Interfaces;
using MyReliableSite.Application.Exceptions;
using MyReliableSite.Application.Specifications;
using MyReliableSite.Application.Storage;
using MyReliableSite.Application.Wrapper;
using MyReliableSite.Domain.ArticleFeedbacks;
using MyReliableSite.Domain.Billing;
using MyReliableSite.Domain.Common.Contracts;
using MyReliableSite.Domain.ManageUserApiKey;
using MyReliableSite.Domain.Products;
using MyReliableSite.Shared.DTOs.ArticleFeedbacks;
using MyReliableSite.Shared.DTOs.Bills;
using MyReliableSite.Shared.DTOs.Identity;
using MyReliableSite.Shared.DTOs.Orders;
using MyReliableSite.Shared.DTOs.Products;
using MyReliableSite.Shared.DTOs.Transaction;
using MyReliableSite.Shared.DTOs.CreditManagement;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using MyReliableSite.Shared.DTOs.Refund;
using static MyReliableSite.Domain.Billing.Credit;

namespace MyReliableSite.Application.Bills.Services;

public class BillService : IBillService
{
    private readonly IStringLocalizer<BillService> _localizer;
    private readonly IRepositoryAsync _repository;
    private readonly IUserService _userService;
    private readonly ICurrentUser _currentUser;
    private readonly IFileStorageService _fileStorageService;
    public BillService()
    {
    }

    public BillService(IRepositoryAsync repository, IStringLocalizer<BillService> localizer, IUserService userService, ICurrentUser currentUser, IFileStorageService fileStorageService)
    {
        _repository = repository;
        _localizer = localizer;
        _userService = userService;
        _currentUser = currentUser;
        _fileStorageService = fileStorageService ?? throw new ArgumentNullException(nameof(fileStorageService));
    }

    public async Task<PaginatedResult<BillDto>> SearchAsync(BillListFilter filter)
    {
        PaginatedResult<BillDto> bills = null;

        if (filter.AdvancedSearch != null && filter.AdvancedSearch.Fields.FirstOrDefault(x => x == "status") != null)
        {
            string status = filter.AdvancedSearch?.Keyword;
            var statusEnum = (OrderStatus)Convert.ToInt32(status);

            var ordersStatus = await _repository.FindByConditionAsync<Order>(x => x.Status == statusEnum);

            bills = await _repository.GetSearchResultsAsync<Bill, BillDto>(filter.PageNumber, filter.PageSize, filter.OrderBy, filter.OrderType, null, filter.Keyword, x => ordersStatus.Select(y => y.Id).Contains(x.OrderId) && ((filter.StartDate == null || x.DueDate >= filter.StartDate) && (filter.EndDate == null || x.DueDate <= filter.EndDate)));
        }
        else
        {
            bills = await _repository.GetSearchResultsAsync<Bill, BillDto>(filter.PageNumber, filter.PageSize, filter.OrderBy, filter.OrderType, filter.AdvancedSearch, filter.Keyword, x => (filter.StartDate == null || x.DueDate >= filter.StartDate) && (filter.EndDate == null || x.DueDate <= filter.EndDate));
        }

        var products = await _repository.FindByConditionAsync<Product>(product => bills.Data.Select(x => x.ProductId).Contains(product.Id));

        var orderSpec = new BaseSpecification<Order>();
        orderSpec.Includes.Add(x => x.Bill);

        var orders = await _repository.FindByConditionAsync<Order>(x => bills.Data.Select(x => x.OrderId).Contains(x.Bill.OrderId), specification: orderSpec);

        var product = products.Adapt<List<OrderedProductDetailDto>>();
        foreach (var bill in bills.Data)
        {
            bill.Status = (Shared.DTOs.Orders.OrderStatus)orders.FirstOrDefault(x => x.Bill.Id == bill.Id).Status;
            bill.Product = product.FirstOrDefault(x => x.Id == bill.ProductId);
        }

        var userDetails = await _userService.GetAllAsync(bills.Data.Select(x => x.UserId));

        foreach (var item in bills.Data)
        {
            var user = userDetails.Data.FirstOrDefault(x => x.Id == Guid.Parse(item.UserId));

            if (user != null)
            {
                item.FullName = user.FullName;
                if (!string.IsNullOrEmpty(user.ImageUrl))
                {
                    item.UserImagePath = await _fileStorageService.ReturnBase64StringOfImageFileAsync(user.ImageUrl);
                }
            }

            var settnigs = await _repository.FirstByConditionAsync<Setting>(x => x.Tenant.ToLower() == item.Tenant.ToLower());
            var userDetail = await _userService.GetUserProfileAsync(item.UserId);

            // ISSUED FOR - Get company name from user detail if no company name found i will get full name of client.
            if (userDetail != null && userDetail.Data != null)
            {
                if (string.IsNullOrEmpty(userDetail.Data.CompanyName))
                {
                    item.IssuedFor = userDetail.Data.FullName;
                }
                else
                {
                    item.IssuedFor = userDetail.Data.CompanyName;
                }

                item.IssueForImage = userDetail.Data.ImageUrl;
            }

            // ISSUED TO - get brand name if any brand assigned to the client and if no brand is assigned then i will get company name from settings.
            var clientBarnd = await _repository.FirstByConditionAsync<Brand>(x => x.ClientAssigned == item.UserId);
            if (clientBarnd != null)
            {
                item.IssuedBy = clientBarnd.CompanyName;
                item.IssueByImage = await _fileStorageService.ReturnBase64StringOfImageFileAsync(clientBarnd.LogoUrl);
            }
            else
            {
                if (settnigs != null)
                {
                    item.IssuedBy = settnigs.CompanyName;
                }

                if (!string.IsNullOrEmpty(userDetail?.Data?.ImageUrl))
                {
                    item.IssueByImage = await _fileStorageService.ReturnBase64StringOfImageFileAsync(clientBarnd.LogoUrl);
                }
            }
        }

        return bills;
    }

    public async Task<PaginatedResult<BillDto>> GetAllBillsDetailAsync(BillListFilter filter)
    {
        PaginatedResult<BillDto> bills = null;

        if (filter.AdvancedSearch != null && filter.AdvancedSearch.Fields.FirstOrDefault(x => x == "status") != null)
        {
            string status = filter.AdvancedSearch?.Keyword;
            var statusEnum = (OrderStatus)Convert.ToInt32(status);

            var ordersStatus = await _repository.FindByConditionAsync<Order>(x => x.Status == statusEnum);

            bills = await _repository.GetSearchResultsAsync<Bill, BillDto>(filter.PageNumber, filter.PageSize, filter.OrderBy, filter.OrderType, null, filter.Keyword, x => x.UserId == _currentUser.GetUserId().ToString() && ordersStatus.Select(y => y.Id).Contains(x.OrderId) && ((filter.StartDate == null || x.DueDate >= filter.StartDate) && (filter.EndDate == null || x.DueDate <= filter.EndDate)));
        }
        else
        {
            bills = await _repository.GetSearchResultsAsync<Bill, BillDto>(filter.PageNumber, filter.PageSize, filter.OrderBy, filter.OrderType, filter.AdvancedSearch, filter.Keyword, x => x.UserId == _currentUser.GetUserId().ToString() && (filter.StartDate == null || x.DueDate >= filter.StartDate) && (filter.EndDate == null || x.DueDate <= filter.EndDate));
        }

        var products = await _repository.FindByConditionAsync<Product>(product => bills.Data.Select(x => x.ProductId).Contains(product.Id));

        var orderSpec = new BaseSpecification<Order>();
        orderSpec.Includes.Add(x => x.Bill);

        var orders = await _repository.FindByConditionAsync<Order>(x => bills.Data.Select(x => x.OrderId).Contains(x.Bill.OrderId), specification: orderSpec);

        var product = products.Adapt<List<OrderedProductDetailDto>>();
        foreach (var bill in bills.Data)
        {
            bill.Status = (Shared.DTOs.Orders.OrderStatus)orders.FirstOrDefault(x => x.Bill.Id == bill.Id).Status;
            bill.Product = product.FirstOrDefault(x => x.Id == bill.ProductId);
        }

        var userDetails = await _userService.GetAllAsync(bills.Data.Select(x => x.UserId));

        foreach (var item in bills.Data)
        {
            var user = userDetails.Data.FirstOrDefault(x => x.Id == Guid.Parse(item.UserId));

            if (user != null)
            {
                item.FullName = user.FullName;
                if (!string.IsNullOrEmpty(user.ImageUrl))
                {
                    item.UserImagePath = await _fileStorageService.ReturnBase64StringOfImageFileAsync(user.ImageUrl);
                }
            }

            var settnigs = await _repository.FirstByConditionAsync<Setting>(x => x.Tenant.ToLower() == item.Tenant.ToLower());
            var userDetail = await _userService.GetUserProfileAsync(item.UserId);

            // ISSUED FOR - Get company name from user detail if no company name found i will get full name of client.
            if (userDetail != null && userDetail.Data != null)
            {
                if (string.IsNullOrEmpty(userDetail.Data.CompanyName))
                {
                    item.IssuedFor = userDetail.Data.FullName;
                }
                else
                {
                    item.IssuedFor = userDetail.Data.CompanyName;
                }

                item.IssueForImage = userDetail.Data.ImageUrl;
            }

            // ISSUED TO - get brand name if any brand assigned to the client and if no brand is assigned then i will get company name from settings.
            var clientBarnd = await _repository.FirstByConditionAsync<Brand>(x => x.ClientAssigned == item.UserId);
            if (clientBarnd != null)
            {
                item.IssuedBy = clientBarnd.CompanyName;
                item.IssueByImage = await _fileStorageService.ReturnBase64StringOfImageFileAsync(clientBarnd.LogoUrl);
            }
            else
            {
                if (settnigs != null)
                {
                    item.IssuedBy = settnigs.CompanyName;
                }

                if (!string.IsNullOrEmpty(userDetail?.Data?.ImageUrl))
                {
                    item.IssueByImage = await _fileStorageService.ReturnBase64StringOfImageFileAsync(clientBarnd.LogoUrl);
                }
            }
        }

        return bills;
    }

    public async Task<PaginatedResult<BillDto>> GetAllBillsFromProductAsync(BillListFilter filter, Guid productId)
    {
        PaginatedResult<BillDto> bills = null;
        bills = await _repository.GetSearchResultsAsync<Bill, BillDto>(filter.PageNumber, filter.PageSize, filter.OrderBy, filter.OrderType, filter.AdvancedSearch, filter.Keyword, x => x.UserId == _currentUser.GetUserId().ToString() && x.Order.Products.Select(p => p.Id).Contains(productId));
        var products = await _repository.FindByConditionAsync<Product>(product => bills.Data.Select(x => x.ProductId).Contains(product.Id));

        var orderSpec = new BaseSpecification<Order>();
        orderSpec.Includes.Add(x => x.Bill);

        var orders = await _repository.FindByConditionAsync<Order>(x => bills.Data.Select(x => x.OrderId).Contains(x.Bill.OrderId), specification: orderSpec);

        var product = products.Adapt<List<OrderedProductDetailDto>>();
        foreach (var bill in bills.Data)
        {
            bill.Status = (Shared.DTOs.Orders.OrderStatus)orders.FirstOrDefault(x => x.Bill.Id == bill.Id).Status;
            bill.Product = product.FirstOrDefault(x => x.Id == bill.ProductId);
        }

        var userDetails = await _userService.GetAllAsync(bills.Data.Select(x => x.UserId));

        foreach (var item in bills.Data)
        {
            var user = userDetails.Data.FirstOrDefault(x => x.Id == Guid.Parse(item.UserId));

            if (user != null)
            {
                item.FullName = user.FullName;
                if (!string.IsNullOrEmpty(user.ImageUrl))
                {
                    item.UserImagePath = await _fileStorageService.ReturnBase64StringOfImageFileAsync(user.ImageUrl);
                }
            }

            var settnigs = await _repository.FirstByConditionAsync<Setting>(x => x.Tenant.ToLower() == item.Tenant.ToLower());
            var userDetail = await _userService.GetUserProfileAsync(item.UserId);

            // ISSUED FOR - Get company name from user detail if no company name found i will get full name of client.
            if (userDetail != null && userDetail.Data != null)
            {
                if (string.IsNullOrEmpty(userDetail.Data.CompanyName))
                {
                    item.IssuedFor = userDetail.Data.FullName;
                }
                else
                {
                    item.IssuedFor = userDetail.Data.CompanyName;
                }

                item.IssueForImage = userDetail.Data.ImageUrl;
            }

            // ISSUED TO - get brand name if any brand assigned to the client and if no brand is assigned then i will get company name from settings.
            var clientBarnd = await _repository.FirstByConditionAsync<Brand>(x => x.ClientAssigned == item.UserId);
            if (clientBarnd != null)
            {
                item.IssuedBy = clientBarnd.CompanyName;
                item.IssueByImage = await _fileStorageService.ReturnBase64StringOfImageFileAsync(clientBarnd.LogoUrl);
            }
            else
            {
                if (settnigs != null)
                {
                    item.IssuedBy = settnigs.CompanyName;
                }

                if (!string.IsNullOrEmpty(userDetail?.Data?.ImageUrl))
                {
                    item.IssueByImage = await _fileStorageService.ReturnBase64StringOfImageFileAsync(clientBarnd.LogoUrl);
                }
            }
        }

        return bills;
    }

    public async Task<Result<List<BillDetailDto>>> GetAllBillsDetailAsync()
    {
        var billslist = await _repository.GetListAsync<Bill>(m => m.DeletedOn == null);
        var billslistDetail = billslist.Adapt<List<BillDetailDto>>();

        foreach (var billDetail in billslistDetail)
        {
            var spec = new BaseSpecification<Order>();
            spec.Includes.Add(a => a.Products);
            spec.Includes.Add(a => a.OrderProductLineItems);
            var order = await _repository.GetByIdAsync<Order>(billDetail.OrderId, spec);
            var user = await _userService.GetAsync(order.ClientId);

            billDetail.Products = order.Products.Adapt<List<OrderedProductDetailDto>>();
            billDetail.OrderProductLineItems = order.OrderProductLineItems.OrderBy(m => m.CreatedOn).Adapt<List<OrderProductLineItemsDto>>();
            billDetail.Status = order.Status;
            billDetail.SubTotal = billDetail.SubTotal;
            billDetail.TotalPrice = billDetail.SubTotal + billDetail.VAT;
            billDetail.User = user.Data;

            var settnigs = await _repository.FirstByConditionAsync<Setting>(x => x.Tenant.ToLower() == billDetail.Tenant.ToLower());
            var userDetail = await _userService.GetUserProfileAsync(billDetail.UserId);

            // ISSUED FOR - Get company name from user detail if no company name found i will get full name of client.
            if (userDetail != null && userDetail.Data != null)
            {
                if (string.IsNullOrEmpty(userDetail.Data.CompanyName))
                {
                    billDetail.IssuedFor = userDetail.Data.FullName;
                }
                else
                {
                    billDetail.IssuedFor = userDetail.Data.CompanyName;
                }

                billDetail.IssueForImage = userDetail.Data.ImageUrl;
            }

            // ISSUED TO - get brand name if any brand assigned to the client and if no brand is assigned then i will get company name from settings.
            var clientBarnd = await _repository.FirstByConditionAsync<Brand>(x => x.ClientAssigned == billDetail.UserId);
            if (clientBarnd != null)
            {
                billDetail.IssuedBy = clientBarnd.CompanyName;
                billDetail.IssueByImage = await _fileStorageService.ReturnBase64StringOfImageFileAsync(clientBarnd.LogoUrl);
            }
            else
            {
                if (settnigs != null)
                {
                    billDetail.IssuedBy = settnigs.CompanyName;
                }

                if (!string.IsNullOrEmpty(userDetail?.Data?.ImageUrl))
                {
                    billDetail.IssueByImage = await _fileStorageService.ReturnBase64StringOfImageFileAsync(clientBarnd.LogoUrl);
                }
            }
        }

        return await Result<List<BillDetailDto>>.SuccessAsync(billslistDetail);
    }

    public async Task<Result<List<BillDetailDto>>> GetAllPaidBillsDetailAsync()
    {
        var billslist = await _repository.GetListAsync<Bill>(m => m.DeletedOn == null);
        var billslistDetail = billslist.Adapt<List<BillDetailDto>>();

        foreach (var billDetail in billslistDetail)
        {
            var spec = new BaseSpecification<Order>();
            spec.Includes.Add(a => a.Products);
            spec.Includes.Add(a => a.OrderProductLineItems);
            var order = await _repository.GetByIdAsync<Order>(billDetail.OrderId, spec);
            var user = await _userService.GetAsync(order.ClientId);

            billDetail.Products = order.Products.Adapt<List<OrderedProductDetailDto>>();
            billDetail.OrderProductLineItems = order.OrderProductLineItems.OrderBy(m => m.CreatedOn).Adapt<List<OrderProductLineItemsDto>>();
            billDetail.Status = order.Status;
            billDetail.SubTotal = billDetail.SubTotal;
            billDetail.TotalPrice = billDetail.SubTotal + billDetail.VAT;
            billDetail.User = user.Data;

            var settnigs = await _repository.FirstByConditionAsync<Setting>(x => x.Tenant.ToLower() == billDetail.Tenant.ToLower());
            var userDetail = await _userService.GetUserProfileAsync(billDetail.UserId);

            // ISSUED FOR - Get company name from user detail if no company name found i will get full name of client.
            if (userDetail != null && userDetail.Data != null)
            {
                if (string.IsNullOrEmpty(userDetail.Data.CompanyName))
                {
                    billDetail.IssuedFor = userDetail.Data.FullName;
                }
                else
                {
                    billDetail.IssuedFor = userDetail.Data.CompanyName;
                }

                billDetail.IssueForImage = userDetail.Data.ImageUrl;
            }

            // ISSUED TO - get brand name if any brand assigned to the client and if no brand is assigned then i will get company name from settings.
            var clientBarnd = await _repository.FirstByConditionAsync<Brand>(x => x.ClientAssigned == billDetail.UserId);
            if (clientBarnd != null)
            {
                billDetail.IssuedBy = clientBarnd.CompanyName;
                billDetail.IssueByImage = await _fileStorageService.ReturnBase64StringOfImageFileAsync(clientBarnd.LogoUrl);
            }
            else
            {
                if (settnigs != null)
                {
                    billDetail.IssuedBy = settnigs.CompanyName;
                }

                if (!string.IsNullOrEmpty(userDetail?.Data?.ImageUrl))
                {
                    billDetail.IssueByImage = await _fileStorageService.ReturnBase64StringOfImageFileAsync(clientBarnd.LogoUrl);
                }
            }
        }

        return await Result<List<BillDetailDto>>.SuccessAsync(billslistDetail.Where(x => x.Status.Equals(Shared.DTOs.Orders.OrderStatus.Paid)).ToList());
    }

    public async Task<Result<BillDetailDto>> GetBillDetailsAsync(Guid id)
    {

        var billDetail = await _repository.GetByIdAsync<Bill, BillDetailDto>(id);
        if (billDetail == null) throw new EntityNotFoundException(string.Format(_localizer["bill.notfound"]));

        var spec = new BaseSpecification<Order>();
        spec.Includes.Add(a => a.Products);
        spec.Includes.Add(a => a.OrderProductLineItems);
        var order = await _repository.GetByIdAsync<Order>(billDetail.OrderId, spec);
        var user = await _userService.GetAsync(order.ClientId);

        billDetail.Products = order.Products.Adapt<List<OrderedProductDetailDto>>();
        billDetail.OrderProductLineItems = order.OrderProductLineItems.OrderBy(m => m.CreatedOn).Adapt<List<OrderProductLineItemsDto>>();
        billDetail.Status = order.Status;
        billDetail.Notes = order.Notes;
        billDetail.SubTotal = billDetail.SubTotal;
        billDetail.TotalPrice = billDetail.SubTotal + billDetail.VAT;
        billDetail.User = user.Data;

        var settnigs = await _repository.FirstByConditionAsync<Setting>(x => x.Tenant.ToLower() == billDetail.Tenant.ToLower());
        var userDetail = await _userService.GetUserProfileAsync(billDetail.UserId);

        // ISSUED FOR - Get company name from user detail if no company name found i will get full name of client.
        if (userDetail != null && userDetail.Data != null)
        {
            if (string.IsNullOrEmpty(userDetail.Data.CompanyName))
            {
                billDetail.IssuedFor = userDetail.Data.FullName;
            }
            else
            {
                billDetail.IssuedFor = userDetail.Data.CompanyName;
            }

            billDetail.IssueForImage = userDetail.Data.ImageUrl;
        }

        // ISSUED TO - get brand name if any brand assigned to the client and if no brand is assigned then i will get company name from settings.
        var clientBarnd = await _repository.FirstByConditionAsync<Brand>(x => x.ClientAssigned == billDetail.UserId);
        if (clientBarnd != null)
        {
            billDetail.IssuedBy = clientBarnd.CompanyName;
            billDetail.IssueByImage = await _fileStorageService.ReturnBase64StringOfImageFileAsync(clientBarnd.LogoUrl);
        }
        else
        {
            if (settnigs != null)
            {
                billDetail.IssuedBy = settnigs.CompanyName;
            }

            if (!string.IsNullOrEmpty(userDetail?.Data?.ImageUrl))
            {
                billDetail.IssueByImage = await _fileStorageService.ReturnBase64StringOfImageFileAsync(clientBarnd.LogoUrl);
            }
        }

        return await Result<BillDetailDto>.SuccessAsync(billDetail);
    }

    public async Task<PaginatedResult<TransactionBillDto>> SearchtransactionsAsync(TransactionListFilter filter)
    {
        var transactions = await _repository.GetSearchResultsAsync<Transaction, TransactionBillDto>(filter.PageNumber, filter.PageSize, filter.OrderBy, filter.OrderType, filter.AdvancedSearch, filter.Keyword, x => (filter.StartDate == null || x.CreatedOn >= filter.StartDate) && (filter.EndDate == null || x.CreatedOn <= filter.EndDate));

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

    public async Task<Result<int>> GetUnpaidInvoices(Guid clientid)
    {
        var order = await _repository.GetListAsync<Order>(x => x.ClientId == clientid.ToString());
        if (order == null || !order.Any())
            return await Result<int>.SuccessAsync(0);
        var list = order.Select(x => x.Id).ToList();
        var billDetail = await _repository.GetListAsync<Bill>(x => list.Contains(x.OrderId));
        if (billDetail == null || !billDetail.Any())
            return await Result<int>.SuccessAsync(0);
        var trx = await _repository.GetListAsync<Transaction>(x => list.Contains(x.ReferenceId) && (x.TransactionStatus != Domain.Billing.TransactionStatus.Pending && x.TransactionStatus != Domain.Billing.TransactionStatus.Completed));
        if (trx == null || !trx.Any())
            return await Result<int>.SuccessAsync(0);
        return await Result<int>.SuccessAsync(trx.Count);
    }

    public async Task<Result<TransactionDetailsDto>> MakeInvoicePaymentRequest(MakeInvoicePaymentRequest request)
    {
        var setting = await _repository.FirstByConditionAsync<Setting>(x => x.Tenant.ToLower() == request.Tenant.ToLower());
        if (setting == null)
            throw new EntityNotFoundException(string.Format(_localizer["setting.notfound"]));
        var appSetting = await _repository.FirstByConditionAsync<UserAppSetting>(x => x.Tenant.ToLower() == request.Tenant.ToLower() && x.UserId == _currentUser.GetUserId().ToString());
        if (appSetting == null || !appSetting.IsActiveOrPendingProduct)
            throw new EntityNotFoundException(string.Format(_localizer["usersetting.invoicepaymentnotpctive"]));
        var bill = await _repository.FirstByConditionAsync<Bill>(x => x.BillNo == request.InvoiceNumber);
        if (bill == null) throw new EntityNotFoundException(string.Format(_localizer["invoice.notfound"]));
        var order = await _repository.FirstByConditionAsync<Order>(x => x.Id == bill.OrderId);
        if (order == null) throw new EntityNotFoundException(string.Format(_localizer["order.notfound"]));
        var transaction = await _repository.FirstByConditionAsync<Transaction>(x => x.ReferenceId == order.Id && (x.TransactionStatus == MyReliableSite.Domain.Billing.TransactionStatus.Pending || x.TransactionStatus == MyReliableSite.Domain.Billing.TransactionStatus.Completed));
        if (transaction != null && transaction.Id != Guid.Empty)
            throw new CustomException(string.Format(_localizer["transaction.paymentInProcess"]), null);

        var id = await _repository.CreateAsync<Transaction>(
              new Transaction(
              _currentUser.GetUserId().ToString(),
              MyReliableSite.Domain.Billing.TransactionType.Payment,
              request.TotalAmount,
              order.OrderNo,
              $"{request.Notes} pay for invoice {order.InvoiceNo}",
              order.Id,
              MyReliableSite.Domain.Billing.TransactionByRole.Admin,
              MyReliableSite.Domain.Billing.TransactionStatus.Completed,
              string.Empty));
        order.Status = OrderStatus.Paid;
        await _repository.SaveChangesAsync();

        var transactionDetails = await _repository.GetByIdAsync<Transaction, TransactionDetailsDto>(id);

        return await Result<TransactionDetailsDto>.SuccessAsync(transactionDetails);
    }

    public async Task<PaginatedResult<RefundDto>> SearchRefundAsync(RefundListFilter filter)
    {
        var filters = new Filters<Refund>();
        {
            filters.Add(!string.IsNullOrEmpty(filter.RequestById), x => x.RequestById == Guid.Parse(filter.RequestById));
        }

        return await _repository.GetSearchResultsAsync<Refund, RefundDto>(filter.PageNumber, filter.PageSize, filter.OrderBy, filter.OrderType, filters, filter.AdvancedSearch, filter.Keyword);
    }

    public async Task<Result<ClientCreditInfoDto>> CreateRefundAdminAsync(CreateRefundRequest request)
    {
        var setting = await _repository.FirstByConditionAsync<Setting>(x => x.Tenant.ToLower() == request.Tenant.ToLower());
        if (setting == null) throw new EntityNotFoundException(string.Format(_localizer["setting.notfound"]));
        var appSetting = await _repository.FirstByConditionAsync<UserAppSetting>(x => x.Tenant.ToLower() == request.Tenant.ToLower() && x.UserId == request.RequestById.ToString());

        var transaction = await _repository.FirstByConditionAsync<Transaction>(x => x.ReferenceId == request.OrderId && x.TransactionByRole == Domain.Billing.TransactionByRole.Admin && x.TransactionBy == _currentUser.GetUserId().ToString() && x.TransactionStatus == Domain.Billing.TransactionStatus.Completed, AsNoTracking: false);
        if (transaction == null) throw new EntityNotFoundException(string.Format(_localizer["transaction.notfound"]));

        var refund = new Refund(request.Notes, transaction.Total, 0, Domain.Billing.RefundStatus.Requested, request.OrderId, _currentUser.GetUserId(), null);

        var refundId = await _repository.CreateAsync<Refund>(refund);
        await _repository.CreateAsync<Transaction>(new Transaction(_currentUser.GetUserId().ToString(), Domain.Billing.TransactionType.Refund, transaction.Total, refund.RefundNo, request.Notes, refundId, Domain.Billing.TransactionByRole.Admin, Domain.Billing.TransactionStatus.Completed, string.Empty));

        decimal? maxCredit = null;
        if (appSetting == null)
        {
            var billingSetting = await _repository.FirstByConditionAsync<BillingSetting>(x => x.Tenant.ToLower() == request.Tenant.ToLower());
            if (billingSetting == null) throw new EntityNotFoundException(string.Format(_localizer["billingSetting.notfound"]));
            if (transaction.Total > billingSetting.MaxCreditAmount) throw new CustomException(string.Format(_localizer["appsetting.maxcreditlimit"]));
            maxCredit = billingSetting.MaxCreditAmount;
        }
        else
        {
            if (transaction.Total > appSetting.MaxCreditAmount) throw new CustomException(string.Format(_localizer["appsetting.maxcreditlimit"]));
            maxCredit = appSetting.MaxCreditAmount;
        }

        var credit = new Credit(request.RequestById, transaction.Total, DateTime.Now, request.Notes);
        var creditInfo = await GetCreditInfo(new CreditInfoRequest(credit.Id, request.RequestById) { Tenant = request.Tenant });
        if (maxCredit.HasValue && creditInfo != null && creditInfo.Data.Balance + transaction.Total > maxCredit)
        {
            throw new CustomException(string.Format(_localizer["appsetting.maxcreditlimit"]));
        }

        creditInfo.Data.Balance += transaction.Total;

        var creditId = await _repository.CreateAsync(credit);

        var transactionCredit = new Transaction(
            _currentUser.GetUserId().ToString(),
            Domain.Billing.TransactionType.Credit,
            transaction.Total,
            0,
            request.Notes,
            creditId,
            Domain.Billing.TransactionByRole.Admin,
            Domain.Billing.TransactionStatus.Completed,
            string.Empty);

        var id = await _repository.CreateAsync<Transaction>(transactionCredit);

        int res = await _repository.SaveChangesAsync();

        creditInfo.Data.TransactionStatus = MyReliableSite.Shared.DTOs.Transaction.TransactionStatus.Completed;
        return await Result<ClientCreditInfoDto>.SuccessAsync(creditInfo.Data);
    }

    public async Task<Result<ClientCreditInfoDto>> GetCreditInfo(CreditInfoRequest request)
    {
        var creditList = await _repository.GetListAsync<Credit>(x => x.UserId == request.ClientId && x.DeletedBy == null);
        var refunds = await _repository.GetListAsync<Refund>(x => x.RequestById == request.Id && x.RefundStatus == Domain.Billing.RefundStatus.Completed);
        var trx = await _repository.GetListAsync<Transaction>(x => x.TransactionBy == request.ClientId.ToString()
        && x.TransactionStatus == Domain.Billing.TransactionStatus.Completed && x.TransactionType == Domain.Billing.TransactionType.Invoice);
        decimal addAmount = creditList.Where(x => x.CreditTransactionType == (byte)CreditTransactionTypes.Increase).Sum(x => x.Amount) + refunds.Sum(x => x.Total);
        decimal minusAmount = creditList.Where(x => x.CreditTransactionType == (byte)CreditTransactionTypes.Decrease).Sum(x => x.Amount) + trx.Sum(x => x.Total);
        var creditInfo = new ClientCreditInfoDto(request.Id, request.ClientId, addAmount - minusAmount);
        return await Result<ClientCreditInfoDto>.SuccessAsync(creditInfo);
    }

    public async Task<Result<List<TransactionDetailsDto>>> MakeAllInvoicePaymentsOfCurrentUser(MakeAllInvoiceCreditPaymentRequest request)
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
        var transaction = await _repository.FindByConditionAsync<Transaction>(x => orderids.Contains(x.ReferenceId) && (x.TransactionStatus == Domain.Billing.TransactionStatus.Pending || x.TransactionStatus == Domain.Billing.TransactionStatus.Completed));
        if (transaction != null && !transaction.Any())
            throw new CustomException(string.Format(_localizer["bill.nobilltopay"]), null);

        decimal sumAmount = transaction.Sum(x => x.Total);

        foreach (var order in orders)
        {
            var id = await _repository.CreateAsync<Transaction>(
                  new Transaction(
                  _currentUser.GetUserId().ToString(),
                  Domain.Billing.TransactionType.Payment,
                  transaction.First(x => x.ReferenceId == order.Id).Total,
                  order.OrderNo,
                  $"{request.Notes} pay for invoice {order.InvoiceNo}",
                  order.Id,
                  Domain.Billing.TransactionByRole.Admin,
                  Domain.Billing.TransactionStatus.Completed,
                  string.Empty));
            order.Status = OrderStatus.Completed;
            await _repository.UpdateAsync<Order>(order);
        }

        int cnt = await _repository.SaveChangesAsync();

        var transactionlist = await _repository.GetListAsync<Transaction>(m => m.DeletedOn == null);
        transactionlist = transactionlist.Where(t => orders.Any(o => o.Id == t.Id)).ToList();
        var transactionlistDetail = transactionlist.Adapt<List<TransactionDetailsDto>>();
        return await Result<List<TransactionDetailsDto>>.SuccessAsync(transactionlistDetail);

    }

}
