using Microsoft.Extensions.Localization;
using MyReliableSite.Application.Abstractions.Services.Identity;
using MyReliableSite.Application.Common.Interfaces;
using MyReliableSite.Application.CreditManagement.Services;
using MyReliableSite.Application.Dashboard.Interfaces;
using MyReliableSite.Application.ManageModule.Interfaces;
using MyReliableSite.Application.Specifications;
using MyReliableSite.Application.Wrapper;
using MyReliableSite.Domain.Billing;
using MyReliableSite.Domain.Constants;
using MyReliableSite.Domain.Departments;
using MyReliableSite.Domain.Identity;
using MyReliableSite.Domain.KnowledgeBase;
using MyReliableSite.Domain.Products;
using MyReliableSite.Domain.Tickets;
using MyReliableSite.Domain.WebHooksDomain;
using MyReliableSite.Shared.DTOs;
using MyReliableSite.Shared.DTOs.CreditManagement;
using MyReliableSite.Shared.DTOs.Dashboard;
using MyReliableSite.Shared.DTOs.Departments;
using MyReliableSite.Shared.DTOs.ManageModule;
using MyReliableSite.Shared.DTOs.Orders;
using MyReliableSite.Shared.DTOs.Tickets;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Collections.Specialized.BitVector32;

namespace MyReliableSite.Application.Dashboard;
public class DashboardService : IDashboardService
{
    private readonly ICurrentUser _currentUser;
    private readonly IRepositoryAsync _repository;
    private readonly IUserService _userService;
    private readonly IUserModuleManagementService _service;
    private readonly IStringLocalizer<CreditService> _localizer;
    public DashboardService(IRepositoryAsync repository, ICurrentUser currentUser, IUserService userService, IUserModuleManagementService service, IStringLocalizer<CreditService> localizer)
    {
        _repository = repository;
        _currentUser = currentUser;
        _userService = userService;
        _service = service;
        _localizer = localizer;
    }

    public async Task<Result<DashboardDto>> GetAsync()
    {
        DashboardDto dashboardDto = new DashboardDto();

        DateTime lastMonthLastDate = DateTime.Today.AddDays(0 - DateTime.Today.Day);
        DateTime lastMonthFirstDate = lastMonthLastDate.AddDays(1 - lastMonthLastDate.Day);

        // Orders
        var orders = await _repository.FindByConditionAsync<Order>(x => x.CreatedOn.Date <= lastMonthLastDate.Date && x.CreatedOn >= lastMonthFirstDate.Date);

        dashboardDto.AllOrders = orders.GroupBy(x => new { x.CreatedOn.Date }).Select(group => new DashboardKeyValueDto { Key = group.Key, Value = group.Count() });
        dashboardDto.CompletedOrders = orders.Where(x => x.Status == OrderStatus.Completed).GroupBy(x => new { x.CreatedOn.Date }).Select(group => new DashboardKeyValueDto { Key = group.Key, Value = group.Count() });

        // Income Overview
        var incomesTransactions = await _repository.FindByConditionAsync<Transaction>(x => x.CreatedOn.Date <= lastMonthLastDate.Date && x.CreatedOn >= lastMonthFirstDate.Date);
        incomesTransactions = incomesTransactions.DistinctBy(x => new { x.ReferenceId, x.TransactionStatus });

        // Income (Order Transaction which are completed mean which are paid)
        dashboardDto.Incomes = incomesTransactions.Where(x => x.TransactionType == TransactionType.Order && x.TransactionStatus == TransactionStatus.Completed).GroupBy(x => new { x.CreatedOn.Date }).Select(group => new DashboardKeyValueDto { Key = group.Key, Value = group.Sum(x => x.Total) });
        dashboardDto.Refunds = incomesTransactions.Where(x => x.TransactionType == TransactionType.Refund && x.TransactionStatus == TransactionStatus.Completed).GroupBy(x => new { x.CreatedOn.Date }).Select(group => new DashboardKeyValueDto { Key = group.Key, Value = group.Sum(x => x.TotalAfterRefundRetain) });

        // Tickets Department Wise
        var sepec = new BaseSpecification<Ticket>();
        sepec.Includes.Add(x => x.Department);

        var tickets = await _repository.FindByConditionAsync<Ticket>(x => x.AssignedTo == _currentUser.GetUserId().ToString(), specification: sepec);
        dashboardDto.TicketsDepartment = tickets.GroupBy(x => new { x.DepartmentId, x.Department?.Name }).Select(group => new DashboardKeyValueDto { Key = group.Key.Name, Value = group.Count() });

        return await Result<DashboardDto>.SuccessAsync(dashboardDto);
    }

    public async Task<Result<DataCountDto>> GetDataCountsAsync()
    {

        var admins = await _userService.GetAllByUserRoleAsync(RoleConstants.Admin);
        var clients = await _userService.GetAllByUserRoleAsync(RoleConstants.Client);
        var superadmins = await _userService.GetAllByUserRoleAsync(RoleConstants.SuperAdmin);
        var orders = await _repository.GetListAsync<Order>(m => m.DeletedOn == null);
        BaseSpecification<Ticket> specification = new BaseSpecification<Ticket>();
        specification.Includes.Add(x => x.Department);

        var tickets = await _repository.FindByConditionAsync<Ticket>(m => m.DeletedOn == null && m.TicketStatus != Shared.DTOs.Tickets.TicketStatus.Closed && m.TicketStatus != Shared.DTOs.Tickets.TicketStatus.ClosedAndLocked, true, specification);
        var departments = await _repository.GetListAsync<Department>(m => tickets.Select(m => m.DepartmentId).Distinct().ToList().Contains(m.Id.ToString()));
        foreach (var ticket in tickets)
        {

            if (!string.IsNullOrEmpty(ticket.DepartmentId) && ticket.Department == null)
            {
                ticket.Department = departments.Find(m => m.Id == Guid.Parse(ticket.DepartmentId));
            }
        }

        var mytickets = tickets.Where(m => m.AssignedTo == _currentUser.GetUserId().ToString());
        var products = await _repository.GetListAsync<Product>(m => m.DeletedOn == null);

        return await Result<DataCountDto>.SuccessAsync(new DataCountDto
        {
            Orders = new Dictionary<string, int>()
            {
                { "Assigned",  orders.Count(m => m.Total == 0) },
                { "NotCompleted", orders.Count(m => m.Status != OrderStatus.Completed)},
                { "All", orders.Count(m => m.Status != OrderStatus.Accepted && m.Status != OrderStatus.Cancelled) }
            },
            /* Products = new Dictionary<string, int>()
             {
                 { "Drafts", products.Count(m => m.Status == Shared.DTOs.Products.ProductStatus.Pending) },
                 { "FeedbackNotReviewed", 0 },
                 { "All", products.Count() }
             },*/
            Tickets = new Dictionary<string, object>()
            {
                { "All", tickets.Count() },
                { "UnAssignedTotal", tickets.Count(m => string.IsNullOrEmpty(m.AssignedTo)) },
                { "UnAssigned", tickets.Where(m => string.IsNullOrEmpty(m.AssignedTo)).GroupBy(m => m.Department.Name).Select(group => new { Department = group.Key, Count = group.Count()})},
                { "ActiveDeptGroupCount", mytickets.Where(m => m.TicketStatus == Shared.DTOs.Tickets.TicketStatus.Active).GroupBy(m => m.Department.Name).Select(group => new { Department = group.Key, Count = group.Count()})},
                { "WaitingDeptGroupCount", mytickets.Where(m => m.TicketStatus == Shared.DTOs.Tickets.TicketStatus.Waiting).GroupBy(m => m.Department.Name).Select(group => new { Department = group.Key, Count = group.Count()})},
                { "Active", mytickets.Count(m => m.TicketStatus == Shared.DTOs.Tickets.TicketStatus.Active) },
                { "InFollowUp", tickets.Count(m => m.TicketStatus == Shared.DTOs.Tickets.TicketStatus.FollowUp) },
                { "Disabled", tickets.Count(m => m.TicketStatus == Shared.DTOs.Tickets.TicketStatus.Disabled) },
                { "Waiting", mytickets.Count(m => m.TicketStatus == Shared.DTOs.Tickets.TicketStatus.Waiting) },
                { "AssignedToAnyone", tickets.Count(m => !string.IsNullOrEmpty(m.AssignedTo)) },
                { "DeptGroupCount", tickets.GroupBy(m => m.Department.Name).Select(group => new { Department = group.Key, Count = group.Count()})},
                { "AssignedToMe", mytickets.GroupBy(m => m.Department.Name).Select(group => new { Department = group.Key, Count = group.Count()})},
                { "totalAssignedToMe", mytickets.Count() }
            },
            KnowledgeBase = new Dictionary<string, int>()
            {
                { "ReviewedCount",  await _repository.CountByConditionAsync<Article>(m => m.DeletedOn == null && m.ArticleFeedbacks.IsReviewed)},
                { "NotReviewedCount",  await _repository.CountByConditionAsync<Article>(m => m.DeletedOn == null && !m.ArticleFeedbacks.IsReviewed)}
            },
            BillCount = await _repository.CountByConditionAsync<Bill>(m => m.DeletedOn == null),
            AdminGroupCount = await _repository.CountByConditionAsync<AdminGroup>(m => m.DeletedOn == null),
            TransactionsCount = await _repository.CountByConditionAsync<Transaction>(m => m.DeletedOn == null),
            WebHooksCount = await _repository.CountByConditionAsync<WebHook>(m => m.DeletedOn == null),
            AdminsCount = admins.Data?.Count ?? 0,
            ClientsCount = clients.Data?.Count ?? 0,
            SuperAdminCounts = superadmins.Data?.Count ?? 0
        });
    }

    public async Task<Result<ClientDashboardDto>> GetCleintDataAsync()
    {
        var stats = new ClientDashboardDto()
        {
            TicketItems = new List<TicketItemDto>(),
        };

        string userId = _currentUser.GetUserId().ToString();
        var modules = await _service.GetUserModuleManagementByUserIdAsync(userId);
        var msg = new List<string>();
        var user = await _userService.GetUserProfileAsync(userId);
        if (user?.Data != null && !string.IsNullOrEmpty(user.Data.ParentID))
            userId = user.Data.ParentID;
        if (GetUserPermision(modules.Data, "Tickets"))
        {
            var ticketData = await _repository.FindByConditionAsync<Ticket>(x => x.ClientId == userId || x.AssignedTo == userId);
            if (ticketData != null && ticketData.Any())
            {
                var tickets = ticketData.Where(x => x.AssignedTo == userId).OrderByDescending(x => x.CreatedOn).Take(5).ToList();

                var departmentIds = tickets.Select(x => x.DepartmentId).ToList();
                var deptList = await _repository.FindByConditionAsync<Department>(x => departmentIds.Contains(x.Id.ToString()));
                var ticketIds = tickets.Select(x => x.Id).ToList();

                tickets.ForEach(item =>
                {
                    var subtrac = DateTime.Now.Subtract(item.CreatedOn);
                    int commentCount = _repository.GetCountAsync<TicketComment>(x => x.Ticket.Id == item.Id).Result;
                    stats.TicketItems.Add(new TicketItemDto()
                    {
                        Id = item.Id,
                        AssignedTo = item.ClientFullName,
                        DepartmentName = deptList.FirstOrDefault(x => x.Id.ToString() == item.DepartmentId)?.Name,
                        IdleTime = subtrac.TotalDays > 1 ? $"{(int)subtrac.TotalDays} days" : subtrac.TotalHours > 1 ? $"{(int)subtrac.TotalHours} hours" : $"{(int)subtrac.TotalMinutes} min",
                        Status = item.TicketStatus.ToString(),
                        Title = item.TicketTitle,
                        MessageCount = commentCount
                    });
                });
                stats.TicketStatistics = new TicketStatistics
                {
                    OpenTotal = ticketData.Where(x => x.TicketStatus == TicketStatus.Active || x.TicketStatus == TicketStatus.Waiting).Count(),
                    WatingToAgentCount = ticketData.Where(x => x.ClientId == userId && (x.TicketStatus == TicketStatus.Active || x.TicketStatus == TicketStatus.Waiting)).Count(),
                    WatingToClientCount = ticketData.Where(x => x.AssignedTo == userId && (x.TicketStatus == TicketStatus.Active || x.TicketStatus == TicketStatus.Waiting)).Count(),
                };
            }
        }
        else
        {
            msg.Add(string.Format(_localizer["modulesetting.nopermision"], "tickets"));
        }

        if (GetUserPermision(modules.Data, "Products"))
        {
            var productService = await _repository.FindByConditionAsync<Product>(x => x.Status != Shared.DTOs.Products.ProductStatus.Cancelled && x.AssignedToClientId == userId);

            stats.ProductServiceStatistics = new ProductStatistics()
            {
                Total = productService.Count(),
                ActiveCount = productService.Where(x => x.Status == Shared.DTOs.Products.ProductStatus.Active).Count(),
                PendingCount = productService.Where(x => x.Status == Shared.DTOs.Products.ProductStatus.Pending).Count(),
                SuspendedCount = productService.Where(x => x.Status == Shared.DTOs.Products.ProductStatus.Suspended).Count(),
            };
        }
        else { msg.Add(string.Format(_localizer["modulesetting.nopermision"], "products")); }
        if (GetUserPermision(modules.Data, "Bills"))
        {
            var billslist = await _repository.GetListAsync<Bill>(m => m.DeletedOn == null);
            stats.InvoiceStatistics = await GetInvoiceStatistics(userId);
        }
        else
        {
            msg.Add(string.Format(_localizer["modulesetting.nopermision"], "bills"));
        }

        return await Result<ClientDashboardDto>.SuccessAsync(stats, msg);
    }

    private async Task<InvoiceStatistics> GetInvoiceStatistics(string currentUser)
    {
        var stats = new InvoiceStatistics();
        var order = await _repository.GetListAsync<Order>(x => x.ClientId == currentUser);
        if (order == null && !order.Any())
            return stats;
        var list = order.Where(x => x.Status == OrderStatus.Pending).Select(x => x.Id).ToList();
        stats.UnPaidCount = list.Any() ? await _repository.CountByConditionAsync<Bill>(x => list.Contains(x.OrderId)) : 0;
        list = order.Where(x => x.Status == OrderStatus.Processing).Select(x => x.Id).ToList();
        stats.OverdueCount = list.Any() ? await _repository.CountByConditionAsync<Bill>(x => list.Contains(x.OrderId)) : 0;
        return stats;
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