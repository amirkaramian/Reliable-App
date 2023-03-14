using MyReliableSite.Application.Common.Interfaces;
using MyReliableSite.Application.Wrapper;
using MyReliableSite.Shared.DTOs.Orders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyReliableSite.Application.Orders.Interfaces;
public interface IOrderService : ITransientService
{
    Task<Result<OrderDetailsDto>> GetOrderDetailsAsync(Guid id);
    Task<PaginatedResult<OrderDto>> SearchAsync(OrderListFilter filter);

    Task<Result<List<OrderDto>>> GetAllOrdersByAdminID(string adminId);

    Task<Result<List<OrderDetailsDto>>> GetOrderDetailsListAsync();
    Task<Result<Guid>> CreateOrderAsync(CreateOrderRequest request);
    Task<Result<Guid>> CreateOrderWHMCSAsync(CreateOrderRequestWHMCS request);
    Task<Result<Guid>> UpdateOrderAsync(UpdateOrderRequest request, Guid id);
    Task<Result<Guid>> UpdateOrderAdminAsync(List<string> adminAssignedId, Guid id);
    Task<Result<Guid>> DeleteOrderAsync(Guid id);
}
