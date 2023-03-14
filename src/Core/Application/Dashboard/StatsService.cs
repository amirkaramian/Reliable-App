using Mapster;
using MyReliableSite.Application.Abstractions.Services.Identity;
using MyReliableSite.Application.Common.Interfaces;
using MyReliableSite.Application.Departments.Interfaces;
using MyReliableSite.Application.Orders.Interfaces;
using MyReliableSite.Application.Wrapper;
using MyReliableSite.Domain.Billing;
using MyReliableSite.Domain.Categories;
using MyReliableSite.Domain.Departments;
using MyReliableSite.Domain.KnowledgeBase;
using MyReliableSite.Domain.Products;
using MyReliableSite.Domain.Tickets;
using MyReliableSite.Shared.DTOs;
using MyReliableSite.Shared.DTOs.Dashboard;
using MyReliableSite.Shared.DTOs.Orders;
using MyReliableSite.Shared.DTOs.Tickets;
using System.Collections.Generic;
using System.Linq;

namespace MyReliableSite.Application.Dashboard;

public class StatsService : IStatsService
{
    private readonly IUserService _userService;
    private readonly IDepartmentService _departmentService;
    private readonly IOrderService _ordertService;
    private readonly IRoleService _roleService;
    private readonly IRepositoryAsync _repository;
    private readonly ICurrentUser _currentUser;

    public StatsService(IRepositoryAsync repository, IRoleService roleService, IUserService userService, IDepartmentService departmentService, IOrderService ordertService, ICurrentUser currentUser)
    {
        _repository = repository;
        _roleService = roleService;
        _userService = userService;
        _departmentService = departmentService;
        _ordertService = ordertService;
        _currentUser = currentUser;
    }

    public async Task<IResult<StatsDto>> GetDataAsync()
    {
        var stats = new StatsDto
        {
            ArticleCount = await _repository.GetCountAsync<Article>(),
            CategoryCount = await _repository.GetCountAsync<Category>(),
            UserCount = await _userService.GetCountAsync(),
            RoleCount = await _roleService.GetCountAsync(),
        };
        return await Result<StatsDto>.SuccessAsync(stats);
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
}