using Microsoft.Extensions.Localization;
using MyReliableSite.Application.Abstractions.Services.Identity;
using MyReliableSite.Application.Common.Interfaces;
using MyReliableSite.Application.Orders.Interfaces;
using MyReliableSite.Application.Wrapper;
using MyReliableSite.Shared.DTOs.Orders;
using MyReliableSite.Domain.Billing;
using MyReliableSite.Domain.Products;
using MyReliableSite.Application.Specifications;
using MyReliableSite.Domain.Billing.Events;
using MyReliableSite.Application.Exceptions;
using MyReliableSite.Shared.DTOs.Notifications;
using MyReliableSite.Shared.DTOs.Notifications.Enums;
using MyReliableSite.Application.Products.Interfaces;
using MyReliableSite.Shared.DTOs.Products;
using Mapster;
using MyReliableSite.Domain.Common.Contracts;
using MyReliableSite.Domain.Tickets;
using MyReliableSite.Shared.DTOs.Identity;
using MyReliableSite.Application.Storage;
using MyReliableSite.Domain.ArticleFeedbacks;
using MyReliableSite.Shared.DTOs.ArticleFeedbacks;
using IronPython.Runtime.Operations;
using System.Linq.Expressions;
using MyReliableSite.Application.Extensions;

namespace MyReliableSite.Application.Orders.Services;
public class OrderService : IOrderService
{
    private readonly IStringLocalizer<OrderService> _localizer;
    private readonly IRepositoryAsync _repository;
    private readonly ICurrentUser _user;
    private readonly INotificationService _notificationService;
    private readonly IUserService _userService;
    private readonly IProductService _productService;
    private readonly IMailService _mailService;

    public OrderService()
    {
    }

    public OrderService(IRepositoryAsync repository, IProductService productService, IStringLocalizer<OrderService> localizer, ICurrentUser user, INotificationService notificationService, IUserService userService, IMailService mailService)
    {
        _repository = repository;
        _localizer = localizer;
        _user = user;
        _notificationService = notificationService;
        _userService = userService;
        _productService = productService;
        _mailService = mailService;
    }

    public async Task<Result<OrderDetailsDto>> GetOrderDetailsAsync(Guid id)
    {
        var spec = new BaseSpecification<Order>();
        spec.Includes.Add(a => a.Bill);
        spec.Includes.Add(a => a.Products);
        spec.Includes.Add(a => a.OrderProductLineItems);

        var order = await _repository.GetByIdAsync<Order, OrderDetailsDto>(id, spec);
        if (order == null) throw new EntityNotFoundException(string.Format(_localizer["order.notfound"], id));

        if (order.Bill != null)
        {
            order.SubTotal = order.Bill.SubTotal;
            order.TotalPrice = order.SubTotal + order.Bill.VAT;

        }

        foreach (var item in order.Products)
        {
            var productDepartments = await _repository.GetListAsync<ProductDepartments>(m => m.ProductId == item.Id);

            item.ProductDepartments = productDepartments.Adapt<IList<ProductDepartmentDto>>();
        }

        if (!string.IsNullOrEmpty(order.AdminAssigned))
        {
            order.AdminUserInfos = new List<AdminUserInfo>();
            var adminIds = order.AdminAssigned.split(",");
            foreach (string item in adminIds)
            {

                var userDetails = await _userService.GetAllAsync(new List<string> { order.ClientId, item });

                if (userDetails.Data.Count > 0)
                {
                    if (!string.IsNullOrEmpty(order.ClientId))
                    {

                        var user = userDetails.Data.FirstOrDefault(x => x.Id == Guid.Parse(order.ClientId));

                        if (user != null)
                        {
                            order.ClientFullName = user.FullName;
                        }
                    }

                    if (!string.IsNullOrEmpty(order.AdminAssigned))
                    {

                        var user = userDetails.Data.FirstOrDefault(x => x.Id == Guid.Parse(item));

                        if (user != null)
                        {
                            order.AdminUserInfos.Add(new AdminUserInfo()
                            {
                                AdminAssignedFullName = user.FullName,
                                AdminAssigned = item,
                            });
                            order.AdminAssignedFullName = user.FullName;
                            order.AdminAssigned = item;
                        }

                    }
                }
            }
        }

        if (order.OrderProductLineItems != null)
        {
            order.OrderProductLineItems = order.OrderProductLineItems.OrderByDescending(m => m.CreatedOn).ToList();
        }

        return await Result<OrderDetailsDto>.SuccessAsync(order);
    }

    public async Task<PaginatedResult<OrderDto>> SearchAsync(OrderListFilter filter)
    {
        var filters = new Filters<Order>();
        filters.Add(filter.OrderNo.HasValue, x => x.OrderNo == filter.OrderNo);
        filters.Add(filter.OrderStatus.HasValue, x => x.Status == (OrderStatus)filter.OrderStatus);
        filters.Add(filter.Amount.HasValue, x => x.Total == filter.Amount);
        filters.Add(filter.StartDate.HasValue && filter.EndDate.HasValue, x => x.CreatedOn.Date >= filter.StartDate.Value.Date && x.CreatedOn <= filter.EndDate.Value.Date);
        filters.Add(filter.StartDate.HasValue && !filter.EndDate.HasValue, x => x.CreatedOn.Date >= filter.StartDate.Value.Date);
        filters.Add(!filter.StartDate.HasValue && filter.EndDate.HasValue, x => x.CreatedOn.Date <= filter.EndDate.Value.Date);
        filters.Add(!string.IsNullOrEmpty(filter.AdminAssigned), x => x.AdminAssigned.Contains(filter.AdminAssigned));
        filters.Add(!string.IsNullOrEmpty(filter.ClientId), x => x.ClientId == filter.ClientId);

        PaginatedResult<OrderDto> orders = null;

        OrderStatus statusEnum = OrderStatus.Pending;
        string status = string.Empty;

        if (filter.AdvancedSearch != null && filter.AdvancedSearch.Fields.FirstOrDefault(x => x.ToLower() == "status") != null)
        {
            status = filter.AdvancedSearch?.Keyword;
            statusEnum = (OrderStatus)Convert.ToInt32(status);
        }

        orders = await _repository.GetSearchResultsAsync<Order, OrderDto>(filter.PageNumber, filter.PageSize, filter.OrderBy, filter.OrderType, filters, filter.AdvancedSearch, filter.Keyword);

        var bills = await _repository.FindByConditionAsync<Bill>(bill => orders.Data.Select(order => order.Id).Contains(bill.OrderId));

        var userIds = orders.Data.Select(x => x.ClientId);

        var adminAssignedIds = Combine(orders.Data.Select(x => x.AdminAssigned));
        userIds = userIds.Concat(adminAssignedIds);

        var userDetails = await _userService.GetAllAsync(userIds);
        if (userDetails.Data.Count() > 0 || bills.Count() > 0)
        {
            foreach (var order in orders.Data)
            {
                order.AdminUserInfos = new List<AdminUserInfo>();
                if (bills.Any() && order.Id != Guid.Empty)
                {
                    var bill = bills.FirstOrDefault(x => x.OrderId == order.Id);

                    if (bill != null)
                    {
                        order.SubTotal = bill.SubTotal;
                        order.TotalPrice = order.SubTotal + bill.VAT;
                        order.BillNo = bill.BillNo;
                    }
                }

                if (userDetails.Data.Count > 0)
                {
                    if (!string.IsNullOrEmpty(order.ClientId))
                    {
                        var user = userDetails.Data.FirstOrDefault(x => x.Id == Guid.Parse(order.ClientId));

                        if (user != null)
                        {
                            order.ClientFullName = user.FullName;
                        }
                    }

                    if (!string.IsNullOrEmpty(string.Concat(order.AdminAssigned)))
                    {
                        var adminIds = order.AdminAssigned;
                        foreach (string item in adminIds)
                        {
                            if (!Guid.TryParse(item, out var adminId))
                            {
                                continue;
                            }

                            var user = userDetails.Data.FirstOrDefault(x => x.Id == adminId);

                            if (user != null)
                            {
                                order.AdminUserInfos.Add(new AdminUserInfo()
                                {
                                    AdminAssignedFullName = user.FullName,
                                    AdminAssigned = item,
                                });
                                order.AdminAssignedFullName = user.FullName;
                                order.AdminAssigned = adminIds;
                            }
                        }

                    }

                }
            }
        }

        return orders;
    }

    public async Task<Result<List<OrderDto>>> GetAllOrdersByAdminID(string adminId)
    {
        var orders = await _repository.FindByConditionAsync<Order>(x => x.DeletedOn == null && x.AdminAssigned.Contains(adminId));
        var orderDetailsDtos = orders.Adapt<List<OrderDto>>();

        var bills = await _repository.FindByConditionAsync<Bill>(bill => orders.Select(order => order.Id).Contains(bill.OrderId));

        var userIds = orders.Select(x => x.ClientId).ToList();

        // var adminAssignedIds = orders.Select(x => x.AdminAssigned);

        // userIds = userIds.Concat(adminAssignedIds);

        userIds.Add(adminId);

        var userDetails = await _userService.GetAllAsync(userIds);
        if (userDetails.Data.Count() > 0 || bills.Count() > 0)
        {
            foreach (var order in orderDetailsDtos)
            {
                if (bills.Any() && order.Id != Guid.Empty)
                {
                    var bill = bills.FirstOrDefault(x => x.OrderId == order.Id);

                    if (bill != null)
                    {
                        order.SubTotal = bill.SubTotal;
                        order.TotalPrice = order.SubTotal + bill.VAT;
                        order.BillNo = bill.BillNo;
                    }
                }

                if (userDetails.Data.Count > 0)
                {
                    if (!string.IsNullOrEmpty(order.ClientId))
                    {
                        var user = userDetails.Data.FirstOrDefault(x => x.Id == Guid.Parse(order.ClientId));

                        if (user != null)
                        {
                            order.ClientFullName = user.FullName;
                        }
                    }

                    if (!string.IsNullOrEmpty(string.Concat(order.AdminAssigned)) && order.AdminAssigned.Contains(adminId))
                    {

                        var user = userDetails.Data.FirstOrDefault(x => x.Id == Guid.Parse(adminId));

                        if (user != null)
                        {
                            order.AdminAssignedFullName = user.FullName;
                        }
                    }

                }
            }
        }

        return await Result<List<OrderDto>>.SuccessAsync(orderDetailsDtos);
    }

    public async Task<Result<List<OrderDetailsDto>>> GetOrderDetailsListAsync()
    {
        var orders = await _repository.FindByConditionAsync<Order>(x => x.DeletedOn == null);
        var orderDetailsDtos = orders.Adapt<List<OrderDetailsDto>>();

        return await Result<List<OrderDetailsDto>>.SuccessAsync(orderDetailsDtos);
    }

    public async Task<Result<Guid>> CreateOrderAsync(CreateOrderRequest request)
    {
        var setting = await _repository.FirstByConditionAsync<Setting>(x => x.Tenant.ToLower() == request.Tenant.ToLower());
        if (setting == null) throw new EntityNotFoundException(string.Format(_localizer["setting.notfound"]));

        List<string> userAdminId = request.AdminAssigned;
        await ValidatePermision(userAdminId);

        // New Product

        Order order = new Order(request.OrderForClientId, userAdminId, request.CustomerIP, request.Notes, request.OrderStatus, 0, request.Notify);

        decimal total = 0;
        foreach (var productDto in request.Products)
        {
            if (string.IsNullOrEmpty(productDto.AssignedToClientId))
            {
                productDto.AssignedToClientId = request.OrderForClientId;
            }

            if (productDto.OrderId == Guid.Empty)
            {
                productDto.OrderId = order.Id;
            }

            if (string.IsNullOrEmpty(productDto.AdminAssigned))
            {
                productDto.AdminAssigned = userAdminId.FirstOrDefault();
            }

            var product = await _productService.CreateProductAsync(productDto);

            decimal subTotal = 0;

            // Add Line Items for Audit
            foreach (var item in product.ProductLineItems)
            {
                order.OrderProductLineItems.Add(new OrderProductLineItem(item.LineItem, item.Price, order.Id, product.Id, item.Id));
                subTotal += item.Price;
            }

            // Store SubTotal without VAT
            total += subTotal;
            order.Products.Add(product);
        }

        order.Total = total + setting.VAT ?? 0;

        order.Bill = new Bill(string.Empty, request.OrderForClientId, setting.VAT ?? 0, DateTime.UtcNow.AddDays(setting.DefaultBillDueDays), 0);

        if (order.Bill != null) order.Bill.SubTotal = total;
        var orderId = await _repository.CreateAsync<Order>(order);

        await _repository.SaveChangesAsync();

        string billNo = $"{setting.BillPrefix}{order.OrderNo}";

        // Update Bill Or Invoice No
        var billUpdate = order.Bill.Update(billNo);

        await _repository.UpdateAsync<Bill>(billUpdate);

        // Add Order Transaction
        if (request.OrderStatus != OrderStatus.Draft)
            await _repository.CreateAsync<Transaction>(new Transaction(userAdminId.FirstOrDefault(), TransactionType.Order, total, order.OrderNo, request.Notes, order.Id, TransactionByRole.Client, TransactionStatus.Pending, userAdminId.FirstOrDefault()));

        billUpdate.DomainEvents.Add(new StatsChangedEvent());

        await _repository.SaveChangesAsync();

        // Notify to ther Admins has Order Update Permissions
        string message = "Hello [[fullName]], a Order has been created.";

        // await _notificationService.SendMessageToAdminsHavingPermissionAsync(new BasicNotification() { Message = message, Label = BasicNotification.LabelType.Information, NotificationType = NotificationType.ORDER_CREATED, TargetUserTypes = NotificationTargetUserTypes.Admins, Data = new { OrderId = orderId } }, PermissionConstants.Orders.Update);

        await _notificationService.SendMessageToSuperAdminsHavingPermissionAsync(new BasicNotification() { Message = message, Label = BasicNotification.LabelType.Information, NotificationType = NotificationType.ORDER_CREATED, TargetUserTypes = NotificationTargetUserTypes.SuperAdmins, NotifyModelId = orderId, Data = new { OrderId = orderId } });

        // if (request.OrderStatus != OrderStatus.Draft)
        {
            string billMessage = $"Hello [[fullName]], a invoice is generated for your order No {order.OrderNo}";
            await _notificationService.SendMessageToUserAsync(Convert.ToString(_user.GetUserId()), new BasicNotification() { Message = billMessage, Label = BasicNotification.LabelType.Information, NotificationType = NotificationType.BILL_CREATED, TargetUserTypes = NotificationTargetUserTypes.Clients, NotifyModelId = orderId, Data = new { OrderId = orderId, BillId = order.Bill.Id } });
        }

        // send email to admin when order create
        var adminUser = await _userService.GetAsync(request.OrderForClientId);
        string createOrderUrl = $"https://admin.myreliablesite.m2mbeta.com/admin/dashboard/billing/orders/your-orders/list/edit/{orderId}";
        await _mailService.SendEmailViaSMTPTemplate(new List<UserDetailsDto> { adminUser.Data }, Shared.DTOs.EmailTemplates.EmailTemplateType.OrderCreated, $"Order No : {order.OrderNo.ToString()}", null, createOrderUrl);

        foreach (string admin in request.AdminAssigned)
        {
            if (!string.IsNullOrWhiteSpace(admin))
            {
                var assignedUserData = await _userService.GetAsync(admin);
                if (assignedUserData != null && assignedUserData.Data != null)
                {
                    var assignedUser = assignedUserData.Data;
                    await _mailService.SendEmailViaSMTPTemplate(new List<UserDetailsDto> { assignedUser }, Shared.DTOs.EmailTemplates.EmailTemplateType.OrderAssignment, $"Order No : {order.OrderNo.ToString()}", null, createOrderUrl);
                }
            }
        }

        return await Result<Guid>.SuccessAsync(orderId);
    }

    public async Task<Result<Guid>> CreateOrderWHMCSAsync(CreateOrderRequestWHMCS request)
    {
        var setting = await _repository.FirstByConditionAsync<Setting>(x => x.Tenant.ToLower() == request.Tenant.ToLower());
        if (setting == null) throw new EntityNotFoundException(string.Format(_localizer["setting.notfound"]));

        List<string> userAdminId = request.AdminAssigned;
        await ValidatePermision(userAdminId);

        // New Product
        Order order = new Order(request.OrderForClientId, userAdminId, request.CustomerIP, request.Notes, request.OrderStatus, request.Total);
        order.InvoiceNo = request.InvoiceNo.ToString();

        // Generate Bill/Invoice
        // if (request.OrderStatus != OrderStatus.Draft)
        {
            order.Bill = new Bill(order.InvoiceNo, request.OrderForClientId, setting.VAT ?? 0, DateTime.UtcNow.AddDays(setting.DefaultBillDueDays), order.Total);

        }

        decimal total = 0;
        foreach (var productDto in request.Products)
        {
            if (string.IsNullOrEmpty(productDto.AssignedToClientId))
            {
                productDto.AssignedToClientId = request.OrderForClientId;
            }

            if (productDto.OrderId == Guid.Empty)
            {
                productDto.OrderId = order.Id;
            }

            if (string.IsNullOrEmpty(productDto.AdminAssigned))
            {
                productDto.AdminAssigned = userAdminId.FirstOrDefault();
            }

            var product = await _productService.CreateProductWHMCSAsync(productDto);

            decimal subTotal = 0;

            // Add Line Items for Audit
            foreach (var item in product.ProductLineItems)
            {
                order.OrderProductLineItems.Add(new OrderProductLineItem(item.LineItem, item.Price, order.Id, product.Id, item.Id));
                subTotal += item.Price;
            }

            // Store SubTotal without VAT
            total += subTotal;
            order.Products.Add(product);
        }

        if (order.Bill != null) order.Bill.SubTotal = total;

        order.Total = total + setting.VAT ?? 0;

        var orderId = await _repository.CreateAsync<Order>(order);

        await _repository.SaveChangesAsync();

        // if (request.OrderStatus != OrderStatus.Draft)
        {
            string billNo = $"{setting.BillPrefix}{order.OrderNo}";

            // Update Bill Or Invoice No
            var billUpdate = order.Bill.Update(billNo);

            await _repository.UpdateAsync<Bill>(billUpdate);

            // Add Order Transaction
            if (request.OrderStatus != OrderStatus.Draft)
                await _repository.CreateAsync<Transaction>(new Transaction(userAdminId.FirstOrDefault(), TransactionType.Order, total, order.OrderNo, request.Notes, order.Id, TransactionByRole.Client, TransactionStatus.Pending, userAdminId.FirstOrDefault()));

            billUpdate.DomainEvents.Add(new StatsChangedEvent());
        }

        await _repository.SaveChangesAsync();

        // Notify to ther Admins has Order Update Permissions
        string message = "Hello [[fullName]], a Order has been created.";

        // await _notificationService.SendMessageToAdminsHavingPermissionAsync(new BasicNotification() { Message = message, Label = BasicNotification.LabelType.Information, NotificationType = NotificationType.ORDER_CREATED, TargetUserTypes = NotificationTargetUserTypes.Admins, Data = new { OrderId = orderId } }, PermissionConstants.Orders.Update);

        await _notificationService.SendMessageToSuperAdminsHavingPermissionAsync(new BasicNotification() { Message = message, Label = BasicNotification.LabelType.Information, NotificationType = NotificationType.ORDER_CREATED, TargetUserTypes = NotificationTargetUserTypes.SuperAdmins, NotifyModelId = orderId, Data = new { OrderId = orderId } });

        // if (request.OrderStatus != OrderStatus.Draft)
        {
            string billMessage = $"Hello [[fullName]], a invoice is generated for your order No {order.OrderNo}";
            await _notificationService.SendMessageToUserAsync(Convert.ToString(_user.GetUserId()), new BasicNotification() { Message = billMessage, Label = BasicNotification.LabelType.Information, NotificationType = NotificationType.BILL_CREATED, TargetUserTypes = NotificationTargetUserTypes.Clients, NotifyModelId = orderId, Data = new { OrderId = orderId, BillId = order.Bill.Id } });
        }

        return await Result<Guid>.SuccessAsync(orderId);
    }

    private async Task createorUpdateBill(Order order, decimal total)
    {
        // Generate Bill/Invoice
        if (order.Status != OrderStatus.Draft)
        {

            string userId = _user.GetUserId().ToString();
            var setting = await _repository.FirstByConditionAsync<Setting>(x => x.Tenant.ToLower() == order.Tenant.ToLower());
            if (setting == null) throw new EntityNotFoundException(string.Format(_localizer["setting.notfound"]));

            if (order.Bill == null)
            {
                var bill = new Bill($"{setting.BillPrefix}{order.OrderNo}", userId, setting.VAT ?? 0, DateTime.UtcNow.AddDays(setting.DefaultBillDueDays), order.Total);
                bill.Order = order;
                bill.OrderId = order.Id;
                await _repository.CreateAsync<Bill>(bill);

                bill.DomainEvents.Add(new StatsChangedEvent());
                await _repository.SaveChangesAsync();
                string billMessage = $"Hello [[fullName]], a invoice is generated for your order No {order.OrderNo}";
                await _notificationService.SendMessageToUserAsync(Convert.ToString(_user.GetUserId()), new BasicNotification() { Message = billMessage, Label = BasicNotification.LabelType.Information, NotificationType = NotificationType.BILLS, TargetUserTypes = NotificationTargetUserTypes.Clients, NotifyModelId = order.Id, Data = new { OrderId = order.Id, BillId = order.Bill.Id } });
            }
        }

    }

    /*
    public async Task<Result<Guid>> CreateOrderAsync(CreateOrderRequest request)
    {
        var setting = await _repository.FirstByConditionAsync<Setting>(x => x.Tenant.ToLower() == request.Tenant.ToLower());
        if (setting == null) throw new EntityNotFoundException(string.Format(_localizer["setting.notfound"]));

        string userId = _user.GetUserId().ToString();

        // New Product
        Order order = new Order(userId, request.CustomerIP, request.Notes, Domain.Billing.OrderStatus.Pending);

        // Generate Bill/Invoice
        order.Bills = new Bill(string.Empty, userId, setting.VAT ?? 0, DateTime.UtcNow.AddDays(setting.DefaultBillDueDays), request.ProductId, 0);

        var specs = new BaseSpecification<Product>();
        specs.Includes.Add(x => x.ProductLineItems);

        // Get Product Detail with its product lines item details
        var product = await _repository.GetByIdAsync<Product>(request.ProductId, specs);

        decimal subTotal = 0;

        // Add Line Items for Audit
        foreach (var item in product.ProductLineItems)
        {
            order.Bills.BilledProductLineItems.Add(new BilledProductLineItem(item.LineItem, item.Price));
            subTotal += item.Price;
        }

        // Store SubTotal without VAT
        order.Bills.SubTotal = subTotal;

        var orderId = await _repository.CreateAsync<Order>(order);

        await _repository.SaveChangesAsync();

        string billNo = $"{setting.BillPrefix}{order.OrderNo}";

        // Update Bill Or Invoice No
        var billUpdate = order.Bills.Update(billNo);

        await _repository.UpdateAsync<Bill>(billUpdate);

        // Add Order Transaction
        decimal total = subTotal + setting.VAT ?? 0;
        await _repository.CreateAsync<Transaction>(new Transaction(userId, TransactionType.Order, total, order.OrderNo, request.Notes, order.Id, TransactionByRole.Client, TransactionStatus.Pending, null));

        billUpdate.DomainEvents.Add(new StatsChangedEvent());

        await _repository.SaveChangesAsync();

        // Notify to ther Admins has Order Update Permissions
        string message = "Hello [[fullName]], a Order has been created.";
        await _notificationService.SendMessageToAdminsHavingPermissionAsync(new BasicNotification() { Message = message, Label = BasicNotification.LabelType.Information, NotificationType = NotificationType.ORDER_CREATED, TargetUserTypes = NotificationTargetUserTypes.Admins, Data = new { OrderId = orderId } }, PermissionConstants.Orders.Update);

        await _notificationService.SendMessageToSuperAdminsHavingPermissionAsync(new BasicNotification() { Message = message, Label = BasicNotification.LabelType.Information, NotificationType = NotificationType.ORDER_CREATED, TargetUserTypes = NotificationTargetUserTypes.SuperAdmins, Data = new { OrderId = orderId } });

        string billMessage = $"Hello [[fullName]], a invoice is generated for your order No {order.OrderNo}";
        await _notificationService.SendMessageToUserAsync(Convert.ToString(_user.GetUserId()), new BasicNotification() { Message = billMessage, Label = BasicNotification.LabelType.Information, NotificationType = NotificationType.BILL_CREATED, TargetUserTypes = NotificationTargetUserTypes.Clients, Data = new { OrderId = orderId, BillId = order.Bills.Id } });

        return await Result<Guid>.SuccessAsync(orderId);
    }*/

    public async Task<Result<Guid>> UpdateOrderAsync(UpdateOrderRequest request, Guid id)
    {
        var order = await _repository.GetByIdAsync<Order>(id);
        if (order == null) throw new EntityNotFoundException(string.Format(_localizer["order.notfound"], id));

        await ValidatePermision(request.AdminAssignedId);

        var orderStatus = order.Status;
        var updatedOrder = order.Update(request.Notes, request.Status, request.AdminAssignedId);

        TransactionByRole role = TransactionByRole.Admin;

        if (_user.IsInRole("Client"))
            role = TransactionByRole.Client;

        decimal total = 0;
        if ((order.Bill == null && request.Status != OrderStatus.Draft) || (role == TransactionByRole.Client && request.Status == OrderStatus.Cancelled))
        {
            var orderFinancialDetail = await GetOrderDetailsAsync(id);
            total = orderFinancialDetail.Data.TotalPrice;
        }

        // Add Transaction and Update Bill Payment Status to Paid if Order is Paid
        if (request.Status == OrderStatus.Paid && order.Status != OrderStatus.Paid)
        {
            await _repository.CreateAsync<Transaction>(new Transaction(_user.GetUserId().ToString(), TransactionType.Order, total, order.OrderNo, request.Notes, order.Id, role, TransactionStatus.Completed, string.Empty));
        }

        // Cancelled Order become refund if paid by customer
        if (role == TransactionByRole.Client && request.Status == OrderStatus.Cancelled)
        {
            // Does Client have paid against this Order then procced for the refund
            var transaction = await _repository.FirstByConditionAsync<Transaction>(x => x.ReferenceId == order.Id && x.TransactionByRole == TransactionByRole.Client && x.TransactionBy == _user.GetUserId().ToString() && x.TransactionStatus == TransactionStatus.Completed, AsNoTracking: false);

            if (transaction != null)
            {
                var refund = new Refund(request.Notes, total, total, Domain.Billing.RefundStatus.Requested, order.Id, _user.GetUserId(), null);

                await _repository.CreateAsync<Refund>(refund);
                await _repository.CreateAsync<Transaction>(new Transaction(_user.GetUserId().ToString(), TransactionType.Refund, total, refund.RefundNo, request.Notes, refund.Id, role, TransactionStatus.Pending, string.Empty));
            }
        }

        updatedOrder.DomainEvents.Add(new StatsChangedEvent());

        await _repository.UpdateAsync<Order>(updatedOrder);
        await _repository.SaveChangesAsync();

        if (order.Bill == null && request.Status != OrderStatus.Draft)
        {
            await createorUpdateBill(updatedOrder, updatedOrder.Total);
        }

        if (request.Status == OrderStatus.Paid)
        {
            var adminUser = await _userService.GetAsync(request.AdminAssignedId.FirstOrDefault());
            string invoiceUrl = $"https://admin.myreliablesite.m2mbeta.com/admin/dashboard/billing/invoices/list/details/{order.Bill.Id}";
            await _mailService.SendEmailViaSMTPTemplate(new List<UserDetailsDto> { adminUser.Data }, Shared.DTOs.EmailTemplates.EmailTemplateType.Invoice, $"Invoice Order No :{order.Id}", null, invoiceUrl);
        }

        if (request.Status != OrderStatus.Paid)
        {
            var client = await _userService.GetAsync(order.ClientId);
            string orderUrl = $"https://admin.myreliablesite.m2mbeta.com/admin/dashboard/billing/orders/your-orders/list/edit/{order.Id}";
            await _mailService.SendEmailViaSMTPTemplate(new List<UserDetailsDto> { client.Data }, Shared.DTOs.EmailTemplates.EmailTemplateType.Invoice, $"Order No :{order.Id}", null, orderUrl, order.Status.ToString());
        }

        // Notify to ther Admins has Order Update Permissions
        string message = "Hello [[fullName]], a Order has been updated.";

        // await _notificationService.SendMessageToAdminsHavingPermissionAsync(new BasicNotification() { Message = message, Label = BasicNotification.LabelType.Information, NotificationType = NotificationType.ORDER_UPDATED, TargetUserTypes = NotificationTargetUserTypes.Admins, Data = new { OrderId = order.Id } }, PermissionConstants.Orders.Update);
        await _notificationService.SendMessageToSuperAdminsHavingPermissionAsync(new BasicNotification() { Message = message, Label = BasicNotification.LabelType.Information, NotificationType = NotificationType.ORDER_UPDATED, TargetUserTypes = NotificationTargetUserTypes.SuperAdmins, NotifyModelId = order.Id, Data = new { OrderId = order.Id } });

        return await Result<Guid>.SuccessAsync(id);
    }

    public async Task<Result<Guid>> UpdateOrderAdminAsync(List<string> adminAssignedId, Guid id)
    {
        var order = await _repository.GetByIdAsync<Order>(id);
        if (order == null) throw new EntityNotFoundException(string.Format(_localizer["order.notfound"], id));
        await ValidatePermision(adminAssignedId);
        var updatedOrder = order.Update(adminAssignedId);

        updatedOrder.DomainEvents.Add(new StatsChangedEvent());

        await _repository.UpdateAsync<Order>(updatedOrder);
        await _repository.SaveChangesAsync();

        // Notify to ther Admins has Order Update Permissions
        string message = "Hello [[fullName]], a Order has been updated.";

        await _notificationService.SendMessageToUserAsync(adminAssignedId.FirstOrDefault(), new BasicNotification() { Message = message, Label = BasicNotification.LabelType.Information, NotificationType = NotificationType.ORDER_UPDATED, TargetUserTypes = NotificationTargetUserTypes.Clients, NotifyModelId = order.Id, Data = new { OrderId = order.Id } });
        await _notificationService.SendMessageToSuperAdminsHavingPermissionAsync(new BasicNotification() { Message = message, Label = BasicNotification.LabelType.Information, NotificationType = NotificationType.ORDER_UPDATED, TargetUserTypes = NotificationTargetUserTypes.SuperAdmins, NotifyModelId = order.Id, Data = new { OrderId = order.Id } });

        return await Result<Guid>.SuccessAsync(id);
    }

    private async Task ValidatePermision(List<string> adminAssignedId)
    {
        var settings = await _repository.FindByConditionAsync<UserAppSetting>(x => adminAssignedId.Contains(x.UserId));
        var setting = settings.FirstOrDefault();

        Expression<Func<Order, bool>> fnc = x => 1 != 1;

        foreach (string item in adminAssignedId)
            fnc = fnc.Or<Order>(x => x.AdminAssignedString.Contains(item));

        var orders = await _repository.FindByConditionAsync<Order>(fnc);
        if (setting is null || setting.Tenant == "Client")
            return;
        if (orders != null && orders.Any() && !setting.CanTakeOrders)
            throw new EntityNotFoundException(string.Format(_localizer["order.multipleAdmin"], adminAssignedId));
    }

    public async Task<Result<Guid>> DeleteOrderAsync(Guid id)
    {
        var spec = new BaseSpecification<Order>();
        spec.Includes.Add(a => a.Bill);
        spec.Includes.Add(a => a.Products);
        spec.Includes.Add(a => a.OrderProductLineItems);
        var order = await _repository.GetByIdAsync<Order>(id, spec);
        if (order == null) throw new EntityNotFoundException(string.Format(_localizer["order.notfound"], id));

        foreach (var product in order.Products)
        {

            product.DeletedOn = DateTime.UtcNow;
            product.DeletedBy = _user.GetUserId();
        }

        foreach (var item in order.OrderProductLineItems)
        {
            item.DeletedOn = DateTime.UtcNow;
            item.DeletedBy = _user.GetUserId();
        }

        if (order.Bill != null)
        {
            order.Bill.DeletedBy = _user.GetUserId();
            order.Bill.DeletedOn = DateTime.UtcNow;
        }

        order.DeletedOn = DateTime.UtcNow;
        order.DeletedBy = _user.GetUserId();

        order.DomainEvents.Add(new StatsChangedEvent());

        await _repository.SaveChangesAsync();
        return await Result<Guid>.SuccessAsync(id);
    }

    private List<string> Combine(IEnumerable<List<string>> list)
    {
        List<string> result = new List<string>();
        foreach (var item in list)
        {
            result.AddRange(item);
        }

        return result;
    }
}
