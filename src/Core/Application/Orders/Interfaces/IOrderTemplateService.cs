using MyReliableSite.Application.Common.Interfaces;
using MyReliableSite.Application.Wrapper;
using MyReliableSite.Shared.DTOs.Orders;

namespace MyReliableSite.Application.Orders.Interfaces;
public interface IOrderTemplateService : ITransientService
{
    Task<List<OrderTemplateDto>> GetAllAsync(string tenant);
    Task<Result<OrderTemplateDetailsDto>> GetOrderTemplateDetailsAsync(Guid id);
    Task<PaginatedResult<OrderTemplateDto>> SearchAsync(OrderTemplateListFilter filter);
    Task<Result<Guid>> CreateOrderTemplateAsync(CreateOrderTemplateRequest request);
    Task<Result<Guid>> UpdateOrderTemplateAsync(UpdateOrderTemplateRequest request, Guid id);
    Task<Result<Guid>> DeleteOrderTemplateAsync(Guid id);
}
