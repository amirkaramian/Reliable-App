using Mapster;
using Microsoft.Extensions.Localization;
using MyReliableSite.Application.Abstractions.Services.Identity;
using MyReliableSite.Application.Common.Interfaces;
using MyReliableSite.Application.Orders.Interfaces;
using MyReliableSite.Application.Storage;
using MyReliableSite.Application.Transactions.Interfaces;
using MyReliableSite.Application.WHMCS;
using MyReliableSite.Application.Wrapper;
using MyReliableSite.Domain.Billing;
using MyReliableSite.Domain.Billing.Events;
using MyReliableSite.Domain.Common;
using MyReliableSite.Domain.Identity;
using MyReliableSite.Domain.Products;
using MyReliableSite.Domain.WHMSC;
using MyReliableSite.Infrastructure.CSVValidator;
using MyReliableSite.Infrastructure.Persistence.Contexts;
using MyReliableSite.Shared.DTOs.Identity;
using MyReliableSite.Shared.DTOs.Orders;
using MyReliableSite.Shared.DTOs.Products;
using MyReliableSite.Shared.DTOs.WHMCS;
using Newtonsoft.Json;
using Org.BouncyCastle.Asn1.Ocsp;
using static MyReliableSite.Domain.Constants.PermissionConstants;

namespace MyReliableSite.Infrastructure.WHMCS.Services;

public class WHMCSImportService : IWHMCSImportService
{
    private readonly IFileStorageService _file;
    private readonly IStringLocalizer<WHMCSImportService> _localizer;
    private readonly IRepositoryAsync _repository;
    private readonly ICurrentUser _user;
    private readonly IIdentityService _identityService;
    private readonly ITransaction _transaction;
    private readonly IUserService _userService;
    private readonly ApplicationDbContext _context;
    private readonly IOrderService _orderService;

    public WHMCSImportService(
        IRepositoryAsync repository,
        IStringLocalizer<WHMCSImportService> localizer,
        IFileStorageService file,
        ApplicationDbContext context,
        ICurrentUser user,
        IIdentityService identityService,
        ITransaction transaction,
        IUserService userService,
        IOrderService orderService)
    {
        _repository = repository;
        _localizer = localizer;
        _file = file;
        _context = context;
        _user = user;
        _identityService = identityService;
        _transaction = transaction;
        _userService = userService;
        _orderService = orderService;
    }

    public async Task<List<IResult>> ImportData(ImportWHMSCRequest importWHMSCRequest, string origin)
    {
        List<IResult> result = new List<IResult>();
        switch (importWHMSCRequest.WHMCSFileType)
        {
            case WHMCSFileType.Clients:
                result = await ImportClientsRange(ReadImportJson<WHMSCClientDto>(importWHMSCRequest.Content), origin, importWHMSCRequest.ClientId);
                break;
            case WHMCSFileType.Transactions:
                result = await ImportTransactionsRangeAsync(ReadImportJson<WHMSCTransaction>(importWHMSCRequest.Content), origin);
                break;
            case WHMCSFileType.Invoices:
                result = await ImportInvoicesRange(ReadImportJson<WHMSCInvoice>(importWHMSCRequest.Content), origin);
                break;
            case WHMCSFileType.Domains:
                await ImportDomainsRange(ReadImportJson<WHMSCDomain>(importWHMSCRequest.Content));
                break;
            case WHMCSFileType.Services:
                result = await ImportServicesRange(ReadImportJson<WHMSCService>(importWHMSCRequest.Content));
                break;
            default:
                break;
        }

        return result;

    }

    public async Task<ImportWHMSCResponse> ValidateTheData(ImportWHMSCRequest importWHMSCRequest)
    {

        string productThumbnailPath = null;
        if (importWHMSCRequest.JsonFile != null)
            productThumbnailPath = await _file.UploadCSVFileAsync<ImportWHMSCRequest>(importWHMSCRequest.JsonFile, FileType.CSV);

        DateTime start = DateTime.Now;

        string fileName;
        switch (importWHMSCRequest.WHMCSFileType)
        {
            case WHMCSFileType.Clients:
                fileName = "clients-whmcs-config.json";
                break;
            case WHMCSFileType.Transactions:
                fileName = "transactions-whmcs-config.json";
                break;
            case WHMCSFileType.Invoices:
                fileName = "invoices-whmcs-config.json";
                break;
            case WHMCSFileType.Domains:
                fileName = "domains-whmcs-config.json";
                break;
            case WHMCSFileType.Services:
                fileName = "services-whmcs-config.json";
                break;
            default:
                fileName = string.Empty;
                break;
        }

        Validator validator = Validator.FromJson(File.ReadAllText(Path.Combine(Directory.GetCurrentDirectory(), "configurations/WHMCSconfig/" + fileName)));
        FileSourceReader source = new FileSourceReader(productThumbnailPath);
        List<RowValidationError> errors = new List<RowValidationError>();

        foreach (RowValidationError current in validator.Validate(source))
        {
            errors.Add(current);
        }

        return new ImportWHMSCResponse() { Content = ConvertToJSON(File.ReadAllText(Path.Combine(Directory.GetCurrentDirectory(), productThumbnailPath)), source), Success = errors.Count == 0, RowValidationErrors = errors.Adapt<List<RowValidationErrorDto>>() };
    }

    private string ConvertToJSON(string csv, ISourceReader reader)
    {
        int totalRowsChecked = 0;
        var readerList = reader.ReadLines("\n", 0);

        var listObjResult = new List<Dictionary<string, string>>();
        string[] headers = new string[] { };

        foreach (string line in reader.ReadLines("\n", headers.Count()))
        {
            totalRowsChecked++;
            if (totalRowsChecked == 1)
            {
            }
            else if (totalRowsChecked == 2)
            {
                headers = ColumnSplitter.Split(line, ",");
            }
            else
            {
                if (string.IsNullOrWhiteSpace(line)) continue;
                if (string.IsNullOrEmpty(line)) continue;

                string[] data = ColumnSplitter.Split(line, ",");

                var objResult = new Dictionary<string, string>();
                for (int h = 0; h < headers.Length; h++)
                {
                    objResult.Add(headers[h], data[h]);
                }

                listObjResult.Add(objResult);
            }
        }

        return JsonConvert.SerializeObject(listObjResult);
    }

    private async Task<List<IResult>> ImportClientsRange(IEnumerable<WHMSCClientDto> clients, string origin, string parentID)
    {
        List<IResult> results = new List<IResult>();
        foreach (var item in clients)
        {
            results.Add(await _identityService.RegisterImportClientUserAsync(
                new RegisterClientRequest()
                {
                    FullName = item.FirstName + " " + item.LastName,
                    FirstName = item.FirstName,
                    LastName = item.LastName,
                    Email = item.Email,
                    Password = "123Pa$$word!",
                    ConfirmPassword = "123Pa$$word!",
                    CompanyName = item.CompanyName,
                    Address1 = item.Address1,
                    Address2 = item.Address2,
                    City = item.City,
                    State_Region = item.State,
                    Country = item.Country,
                    PhoneNumber = item.PhoneNumber,
                    ZipCode = item.Postcode,
                    Status = item.Status.ToLower() == "active",
                    ParentID = parentID,
                    OldUserId = item.ID
                },
                origin));
        }

        return results;
    }

    private async Task<List<IResult>> ImportTransactionsRangeAsync(IEnumerable<WHMSCTransaction> transactions, string origin)
    {
        List<IResult> results = new List<IResult>();

        int currentUserId = 0;
        foreach (var transaction in transactions)
        {
            string userId = string.Empty;
            var user = await _userService.GetUserProfileByFullNameAsync(transaction.ClientName, transaction.UserID);
            userId = user.Data?.Id.ToString();
            currentUserId = transaction.UserID;

            if (string.IsNullOrEmpty(userId))
            {

                // userId = _user.GetUserId().ToString();
                continue;
            }

            var oldOrder = await _repository.FirstByConditionAsync<Order>(m => m.InvoiceNo == transaction.InvoiceID.ToString());
            if (oldOrder == null)
            {
                continue;
            }

            var orderId = oldOrder.Id;
            var orderDetail = oldOrder;

            // Order Transaction as its Amount In
            decimal totalAmount = transaction.AmountIn > 0 ? transaction.AmountIn : transaction.AmountOut;

            var trannsaction = new Transaction(userId, TransactionType.Order, transaction.AmountIn, orderDetail.OrderNo, "Order", orderId, TransactionByRole.Client, TransactionStatus.Completed, _user.GetUserId().ToString());

            Guid transactionID = Guid.Empty;
            try
            {
                transactionID = await _repository.CreateAsync<Transaction>(trannsaction);
                await _repository.SaveChangesAsync();

                if (transactionID != Guid.Empty)
                {
                    results.Add(await Result<string>.SuccessAsync(string.Format(_localizer["Transaciton {0} is not added successfully."], transaction.ID)));
                }
            }
            catch (Exception)
            {
                results.Add(await Result<string>.FailAsync(string.Format(_localizer["Transaciton {0} is not added."], transaction.ID)));
            }

        }

        return results;
    }

    private async Task<List<IResult>> ImportInvoicesRange(IEnumerable<WHMSCInvoice> invoices, string origin)
    {
        List<IResult> results = new List<IResult>();

        int currentUserId = 0;
        foreach (var invoice in invoices)
        {
            string userId = string.Empty;
            var user = await _userService.GetUserProfileByFullNameAsync(invoice.ClientName, invoice.UserID);
            userId = user.Data?.Id.ToString();
            currentUserId = invoice.UserID;

            if (string.IsNullOrEmpty(userId))
            {
                continue;
            }

            CreateProductRequestWHMCS createProduct = new CreateProductRequestWHMCS();

            createProduct.Name = "Order for Imported Invoice " + invoice.ID;
            createProduct.Notes = "Order for Imported Invoice " + invoice.ID;
            createProduct.Status = ProductStatus.Active;
            createProduct.PaymentType = PaymentType.Recurring; // Biennially Monthly
            createProduct.NextDueDate = invoice.DueDate;
            createProduct.AssignedToClientId = userId;
            createProduct.BillingCycle = BillingCycle.Monthly;
            createProduct.ProductLineItems = new List<CreateProductLineItemRequest>();
            createProduct.ProductLineItems.Add(
                       new CreateProductLineItemRequest()
                       {
                           LineItem = invoice.ID.ToString(),
                           Price = +invoice.Subtotal,
                           PriceType = PriceType.Recurring
                       });

            CreateOrderRequestWHMCS createOrderRequest = new CreateOrderRequestWHMCS()
            {
                Products = new List<CreateProductRequestWHMCS>() { createProduct },
                OrderForClientId = userId,
                OrderStatus = OrderStatus.Paid,
                Notes = invoice.Notes,
                InvoiceNo = invoice.ID,
                Total = invoice.Total,
                Subtotal = invoice.Subtotal,
                Credit = invoice.Credit,
                Tenant = "Admin",
                AdminAssigned = new List<string>() { _user.GetUserId().ToString() },

            };

            // ADD OrderId here.
            try
            {
                var resp = await _orderService.CreateOrderWHMCSAsync(createOrderRequest);

                if (resp.Data != Guid.Empty)
                {
                    results.Add(await Result<string>.SuccessAsync(string.Format(_localizer["Invoice {0} is added successfully."], invoice.ID)));
                }
            }
            catch (Exception)
            {
                results.Add(await Result<string>.FailAsync(string.Format(_localizer["Invoice {0} is not added or already exists."], invoice.ID)));
            }
        }

        return results;

        // But a Bill/Invoice required Bill Product Line items which contains information about the products price name etc
        // Entity is BillProductLineItem

    }

    private async Task<List<IResult>> ImportDomainsRange(IEnumerable<WHMSCDomain> domains)
    {
        List<IResult> results = new List<IResult>();

        int currentUserId = 0;
        foreach (var domain in domains)
        {
            string userId = string.Empty;
            if (currentUserId != domain.UserID)
            {
                var user = await _userService.GetUserProfileByFullNameAsync(domain.ClientName, domain.UserID);
                userId = user.Data?.Id.ToString();
                currentUserId = domain.UserID;
            }

            if (string.IsNullOrEmpty(userId))
            {
                continue;
            }

            var oldOrder = await _repository.FirstByConditionAsync<Order>(m => m.OldOrderId == domain.OrderID);
            if (oldOrder == null)
            {
                CreateProductRequestWHMCS createProduct = new CreateProductRequestWHMCS();

                createProduct.Name = domain.DomainName;
                createProduct.Notes = domain.Notes;
                createProduct.Status = ProductStatus.Active;
                createProduct.PaymentType = PaymentType.Recurring; // Biennially Monthly
                createProduct.NextDueDate = domain.NextDueDate;
                createProduct.ExpiryDate = domain.ExpiryDate;
                createProduct.RecurringAmount = domain.RecurringAmount;
                createProduct.AssignedToClientId = userId;
                createProduct.BillingCycle = BillingCycle.Monthly;
                createProduct.ProductLineItems.Add(
                           new CreateProductLineItemRequest()
                           {
                               LineItem = domain.DomainName,
                               Price = domain.FirstPayment,
                               PriceType = domain.RecurringAmount > 0 ? PriceType.Recurring : PriceType.OneTime
                           });

                CreateOrderRequestWHMCS createOrderRequest = new CreateOrderRequestWHMCS()
                {
                    Products = new List<CreateProductRequestWHMCS>() { createProduct },
                    OrderForClientId = userId,
                    OrderStatus = OrderStatus.Paid,
                    Notes = domain.Notes
                };

                // ADD OrderId here.
                results.Add(await _orderService.CreateOrderWHMCSAsync(createOrderRequest));
            }
            else
            {

                var orderId = oldOrder.Id;
                var orderDetail = oldOrder;
                if (orderDetail.Products != null && orderDetail.Products.Count > 0)
                {
                    orderDetail.Products.First().ProductLineItems.Add(
                        new ProductLineItems()
                        {
                            LineItem = domain.DomainName,
                            Price = domain.FirstPayment,
                            PriceType = domain.RecurringAmount > 0 ? PriceType.Recurring : PriceType.OneTime
                        });
                }
                else
                {
                    var thisOrderToUpdate = orderDetail.Adapt<UpdateOrderRequest>();
                    var createProduct = new Product();
                    createProduct.Name = domain.DomainName;
                    createProduct.Notes = domain.Notes;
                    createProduct.Status = ProductStatus.Active;
                    createProduct.PaymentType = PaymentType.Recurring; // Biennially Monthly
                    createProduct.NextDueDate = domain.NextDueDate;
                    createProduct.AssignedToClientId = userId;
                    createProduct.BillingCycle = BillingCycle.Monthly;
                    createProduct.ProductLineItems.Add(new ProductLineItems()
                    {
                        LineItem = domain.DomainName,
                        Price = domain.FirstPayment,
                        PriceType = domain.RecurringAmount > 0 ? PriceType.Recurring : PriceType.OneTime
                    });

                    orderDetail.Products.Add(createProduct);

                    orderDetail.DomainEvents.Add(new StatsChangedEvent());

                    await _repository.UpdateAsync<Order>(orderDetail);
                    await _repository.SaveChangesAsync();
                    results.Add(await _orderService.UpdateOrderAsync(thisOrderToUpdate, orderDetail.Id));
                }

            }
        }

        return results;
    }

    private async Task<List<IResult>> ImportServicesRange(IEnumerable<WHMSCService> services)
    {
        List<IResult> results = new List<IResult>();
        List<string> orderIds = services.Select(m => m.OrderID).Distinct().ToList();
        foreach (string orderid in orderIds)
        {
            var productList = services.Where(m => m.OrderID == orderid).ToList();
            var firstDto = productList.First();
            var user = await _userService.GetUserProfileByFullNameAsync(firstDto.ClientName, firstDto.UserID);
            string userId = user.Data?.Id.ToString();
            if (string.IsNullOrEmpty(userId))
            {
                continue;

                // userId = _user.GetUserId().ToString();
            }

            List<CreateProductRequestWHMCS> createProductRequest = new List<CreateProductRequestWHMCS>();
            var productIDs = productList.Select(m => m.ProductID).Distinct().ToList();

            foreach (string id in productIDs)
            {
                var product = productList.First(m => m.ProductID == id);

                var lineItems = productList.Where(m => m.ProductID == id);
                CreateProductRequestWHMCS createProduct = new CreateProductRequestWHMCS();
                Enum.TryParse(product.Status, out ProductStatus productStatus);
                Enum.TryParse(product.BillingCycle, out BillingCycle billingCycle);
                createProduct.ProductLineItems = new();
                foreach (var item in lineItems)
                {
                    createProduct.ProductLineItems.Add(
                        new CreateProductLineItemRequest()
                        {
                            LineItem = item.DomainName,
                            Price = item.FirstPaymentAmount,
                            PriceType = item.RecurringAmount > 0 ? PriceType.Recurring : PriceType.OneTime
                        });
                }

                createProduct.Name = product.DomainName;
                createProduct.Notes = product.Notes;
                createProduct.Status = productStatus;
                createProduct.PaymentType = PaymentType.OneTime; // Biennially Monthly
                createProduct.NextDueDate = product.NextDue;
                createProduct.AssignedToClientId = userId;
                createProduct.BillingCycle = billingCycle;

                createProduct.OldOrderId = product.OrderID;
                createProduct.OldProductId = product.ProductID;
                createProduct.ServerId = product.ServerID;
                createProduct.DomainName = product.DomainName;
                createProduct.DedicatedIP = product.DedicatedIP;
                createProduct.AssginedIPs = product.AssginedIPs;
                createProduct.AdminAssigned = _user.GetUserId().ToString();

                createProductRequest.Add(createProduct);
            }

            CreateOrderRequestWHMCS createOrderRequest = new CreateOrderRequestWHMCS()
            {
                Products = createProductRequest,
                OrderForClientId = userId,
                OrderStatus = OrderStatus.Draft,
                Notes = firstDto.Notes,
                Tenant = "Admin",
                AdminAssigned = new List<string>() { _user.GetUserId().ToString() },
            };

            // ADD OrderId here.
            try
            {
                var resp = await _orderService.CreateOrderWHMCSAsync(createOrderRequest);

                if (resp.Data != Guid.Empty)
                {
                    results.Add(await Result<string>.SuccessAsync(string.Format(_localizer["Order {0} is added successfully."], orderid)));
                }
            }
            catch (Exception)
            {
                results.Add(await Result<string>.FailAsync(string.Format(_localizer["Order {0} is not added or already exists."], orderid)));
            }

        }

        return results;
    }

    private async Task<IEnumerable<Guid>> ImportServicesRange_old(IEnumerable<WHMSCService> services)
    {
        int currentUserId = 0;
        List<Product> products = new List<Product>();
        foreach (var product in services)
        {
            string userId = string.Empty;
            if (currentUserId != product.UserID)
            {
                var user = await _userService.GetUserProfileByFullNameAsync(product.ClientName, product.UserID);
                userId = user.Data?.Id.ToString();
                currentUserId = product.UserID;
            }

            if (string.IsNullOrEmpty(userId))
            {
                continue;

                // userId = _user.GetUserId().ToString();
            }

            Enum.TryParse(product.Status, out ProductStatus productStatus);
            Enum.TryParse(product.BillingCycle, out BillingCycle billingCycle);
            var productInsert = new Product(
                product.DomainName,
                product.Notes,
                null,
                productStatus,
                PaymentType.OneTime, // Biennially Monthly
                product.Notes,
                product.CreatedOn,
                product.NextDue,
                null,
                null,
                null,
                userId,
                billingCycle,
                product.DedicatedIP,
                product.AssginedIPs);

            productInsert.OldOrderId = product.OrderID;
            productInsert.OldProductId = product.ProductID;
            productInsert.ServerId = product.ServerID;
            productInsert.DomainName = product.DomainName;

            // productInsert.SuspendReason = product.SuspendReason;
            products.Add(productInsert);
        }

        return await _repository.CreateRangeAsync<Product>(products);
    }

    private IEnumerable<T> ReadImportJson<T>(string data)
    {
        return JsonConvert.DeserializeObject<IEnumerable<T>>(data);
    }

    private bool convertStatus(string status)
    {
        return status.ToLower().Equals("active");
    }

    private bool convertCreationDate(string creationDate)
    {
        return DateTime.TryParse(creationDate, out DateTime date);
    }

    private bool isDataExists()
    {
        return true;
    }

}
