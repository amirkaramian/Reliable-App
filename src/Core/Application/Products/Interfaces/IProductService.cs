using MyReliableSite.Application.Common.Interfaces;
using MyReliableSite.Application.Wrapper;
using MyReliableSite.Domain.Products;
using MyReliableSite.Shared.DTOs.Products;
using MyReliableSite.Shared.DTOs.Scripting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyReliableSite.Application.Products.Interfaces;
public interface IProductService : ITransientService
{
    Task<Result<ProductDetailDto>> GetProductDetailsAsync(Guid id);
    Task<PaginatedResult<ProductDto>> SearchAsync(ProductListFilter filter);
    Task<Product> CreateProductAsync(CreateProductRequest request);
    Task<Product> CreateProductWHMCSAsync(CreateProductRequestWHMCS request);
    Task<Result<Guid>> CreateProductAsync(CreateProductRequest request, bool isFromOrder = false);
    Task<Result<Guid>> UpdateProductAsync(UpdateProductRequest request, Guid id);
    Task<Result<Guid>> DeleteProductAsync(Guid id);
    Task<Result<Guid>> AssignProductToClientAsync(Guid id, string clientId);
    Task<Result<Guid>> UnassignProductToClientAsync(Guid id);
    Task<Result<List<ProductDto>>> AvailableProductsForClients();
    Task<Result<List<ProductDto>>> GetProductsListAsync();
    Task<Result<List<ProductEXL>>> GetProductListAsync(string userId, DateTime startDate, DateTime endDate);
    Task<Result<Guid>> SuspensionOfProductAsync(Guid id, string suspendedReason);
    Task<Result<Guid>> UnSuspensionOfProductAsync(Guid id);
    Task<Result<Guid>> ActivateProductAsync(Guid id);
    Task<Result<Guid>> PendingProductAsync(Guid id);
    Task<Result<Guid>> TerminationOfProductAsync(Guid id);

    Task<Result<Guid>> CancellationRequestOfProductAsync(Guid id, bool sendemail);
    Task<Result<ProductCounts>> GetProductCounts();
    Task<Result<Guid>> RemoveCancellationRequest(Guid id);
    Task<Result<Guid>> CancellationRequestEOB(Guid id);
    Task<PaginatedResult<ProductDto>> GetProductsWithStatus(string filter, ProductListFilter listFilter);
    Task<Result<Guid>> SendCancelRequestEmail(Guid id);
    Task<Result<Guid>> ConfirmCancelRequestAsync(Guid id, string token);
    Task<Result<Guid>> ChangeProductModule(Guid id, ProductModuleRequest request);
    Task<PaginatedResult<ProductDto>> SearchAsClientAsync(ProductListFilter filter);
}
