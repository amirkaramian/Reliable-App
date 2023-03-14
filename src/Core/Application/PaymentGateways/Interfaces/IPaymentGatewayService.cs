using MyReliableSite.Application.Common.Interfaces;
using MyReliableSite.Application.Wrapper;
using MyReliableSite.Shared.DTOs.PaymentGateways;
using MyReliableSite.Shared.DTOs.Filters;

namespace MyReliableSite.Application.PaymentGateways.Interfaces;

public interface IPaymentGatewayService : ITransientService
{
    Task<Result<PaymentGatewayDto>> GetAsync(Guid id);

    Task<PaginatedResult<PaymentGatewayDto>> SearchAsync(PaymentGatewayListFilter filters);

    Task<Result<PaymentGatewayDto>> CreateAsync(CreatePaymentGatewayRequest request);

    Task<Result<PaymentGatewayDto>> UpdateAsync(Guid id, UpdatePaymentGatewayRequest request);

    Task<Result<bool>> DeleteAsync(Guid id);
}
