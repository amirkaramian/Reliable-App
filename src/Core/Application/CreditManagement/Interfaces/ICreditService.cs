using MyReliableSite.Application.Common.Interfaces;
using MyReliableSite.Application.Wrapper;
using MyReliableSite.Domain.Billing;
using MyReliableSite.Shared.DTOs.Billing;
using MyReliableSite.Shared.DTOs.Bills;
using MyReliableSite.Shared.DTOs.Brands;
using MyReliableSite.Shared.DTOs.CreditManagement;
using MyReliableSite.Shared.DTOs.Orders;
using MyReliableSite.Shared.DTOs.Products;
using MyReliableSite.Shared.DTOs.Refund;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyReliableSite.Application.CreditManagement.Interfaces;
public interface ICreditService : ITransientService
{
    Task<Result<ClientCreditInfoDto>> CreateCreditAsync(CreateCreditRequest request);
    Task<Result<ClientCreditInfoDto>> CreateCreditAdminAsync(CreateCreditRequest request);
    Task<Result<Guid>> RemoveCreditAsync(Guid guid);
    Task<PaginatedResult<CreditSearchDto>> SearchAsync(CreditListFilter filter);
    Task<Result<ClientCreditInfoDto>> DecreaseCreditAsync(DecreaseCreditRequest request);
    Task<Result<ClientCreditInfoDto>> ClientAddCreditAsync(ClientCreateCreditRequest request);
    Task<Result<ClientCreditInfoDto>> GetCreditInfo(CreditInfoRequest request);
    Task<Result<decimal>> GetClientCreditInfo(string clientId = null);
    Task<Result<Guid>> MakeCreditPaymentRequest(MakeCreditPaymentRequest request);
    Task<Result<List<CreditEXL>>> GetCreditListAsync(string clientId, DateTime startDate, DateTime endDate);
    Task<Result<Guid>> MakeAllInvoicePaymentRequest(MakeAllInvoiceCreditPaymentRequest request);

}
