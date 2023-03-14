using Microsoft.AspNetCore.Mvc;
using MyReliableSite.Application.Abstractions.Services.Identity;
using MyReliableSite.Application.Products.Interfaces;
using MyReliableSite.Application.Wrapper;
using MyReliableSite.Domain.Constants;
using MyReliableSite.Infrastructure.Identity.Permissions;
using MyReliableSite.Infrastructure.Swagger;
using MyReliableSite.Shared.DTOs.Products;
using MyReliableSite.Shared.DTOs.Scripting;
using Swashbuckle.AspNetCore.Annotations;

namespace MyReliableSite.Admin.API.Controllers.Products;

public class ProductsController : BaseController
{
    private readonly IProductService _productService;

    public ProductsController(IProductService productService)
    {
        _productService = productService;
    }

    /// <summary>
    /// Retrive the products against specific id.
    /// </summary>
    /// <response code="200">Products returns.</response>
    /// <response code="404">Products not found.</response>
    /// <response code="500">Oops! Can't lookup your record right now.</response>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(Result<ProductDetailDto>), 200)]
    [ProducesResponseType(404)]
    [ProducesResponseType(500)]
    [SwaggerHeader("tenant", "Products", "View", "Input your tenant to access this API i.e. admin for test", "admin", true)]
    [MustHavePermission(PermissionConstants.Products.View)]
    public async Task<IActionResult> GetAsync(Guid id)
    {
        var product = await _productService.GetProductDetailsAsync(id);
        return Ok(product);
    }

    /// <summary>
    /// List of records with pagination &amp; filters.
    /// </summary>
    /// <response code="200">Products List returns.</response>
    /// <response code="500">Oops! Can't lookup your record right now.</response>
    [HttpPost("search")]
    [ProducesResponseType(typeof(PaginatedResult<ProductDto>), 200)]
    [ProducesResponseType(500)]
    [SwaggerHeader("tenant", "Products", "Search", "Input your tenant to access this API i.e. admin for test", "admin", true)]
    [MustHavePermission(PermissionConstants.Products.Search)]
    [SwaggerOperation(Summary = "Search Products using available Filters.")]
    public async Task<IActionResult> SearchAsync(ProductListFilter filter)
    {
        var products = await _productService.SearchAsync(filter);
        return Ok(products);
    }

    /// <summary>
    /// Create a product.
    /// </summary>
    /// <response code="200">Product created.</response>
    /// <response code="400">Product already exists.</response>
    /// <response code="500">Oops! Can't lookup your product right now.</response>
    [HttpPost]
    [ProducesResponseType(typeof(Result<Guid>), 200)]
    [ProducesResponseType(400)]
    [ProducesResponseType(500)]
    [SwaggerHeader("tenant", "Products", "Create", "Input your tenant to access this API i.e. admin for test", "admin", true)]
    [MustHavePermission(PermissionConstants.Products.Create)]
    public async Task<IActionResult> CreateAsync(CreateProductRequest request)
    {
        return Ok(await _productService.CreateProductAsync(request));
    }

    /// <summary>
    /// Update a product.
    /// </summary>
    /// <response code="200">Product created.</response>
    /// <response code="400">Product already exists.</response>
    /// <response code="500">Oops! Can't lookup your product right now.</response>
    [HttpPost("module/{id:guid}")]
    [ProducesResponseType(typeof(Result<Guid>), 200)]
    [ProducesResponseType(400)]
    [ProducesResponseType(500)]
    [SwaggerHeader("tenant", "Products", "Update", "Input your tenant to access this API i.e. admin for test", "admin", true)]
    [MustHavePermission(PermissionConstants.Products.Update)]
    public async Task<IActionResult> ChangeModuleAsync(Guid id, ProductModuleRequest request)
    {
        return Ok(await _productService.ChangeProductModule(id, request));
    }

    /// <summary>
    /// Update a product by unique id.
    /// </summary>
    /// <response code="200">Product updated.</response>
    /// <response code="404">Product not found.</response>
    /// <response code="500">Oops! Can't lookup your product right now.</response>
    [HttpPut("{id:guid}")]
    [ProducesResponseType(typeof(Result<Guid>), 200)]
    [ProducesResponseType(404)]
    [ProducesResponseType(500)]
    [MustHavePermission(PermissionConstants.Products.Update)]
    [SwaggerHeader("tenant", "Products", "Update", "Input your tenant to access this API i.e. admin for test", "admin", true)]
    public async Task<IActionResult> UpdateAsync(UpdateProductRequest request, Guid id)
    {
        return Ok(await _productService.UpdateProductAsync(request, id));
    }

    /// <summary>
    /// Assign product to client.
    /// </summary>
    /// <response code="200">Product assigned to client.</response>
    /// <response code="404">Product not found.</response>
    /// <response code="400">Product already assigned to other client.</response>
    /// <response code="500">Oops! Can't lookup your product right now.</response>
    [HttpPut("{id:guid}/assign/{clientId}")]
    [ProducesResponseType(typeof(Result<Guid>), 200)]
    [ProducesResponseType(404)]
    [ProducesResponseType(400)]
    [ProducesResponseType(500)]
    [MustHavePermission(PermissionConstants.Products.Update)]
    [SwaggerHeader("tenant", "Products", "Update", "Input your tenant to access this API i.e. admin for test", "admin", true)]
    public async Task<IActionResult> AssignProductToClientAsync(Guid id, string clientId)
    {
        return Ok(await _productService.AssignProductToClientAsync(id, clientId));
    }

    /// <summary>
    /// Unassign specific product from client.
    /// </summary>
    /// <response code="200">Product unassigned from client.</response>
    /// <response code="404">Product not found.</response>
    /// <response code="500">Oops! Can't lookup your product right now.</response>
    [HttpPut("{id:guid}/unassign")]
    [ProducesResponseType(typeof(Result<Guid>), 200)]
    [ProducesResponseType(404)]
    [ProducesResponseType(500)]
    [MustHavePermission(PermissionConstants.Products.Update)]
    [SwaggerHeader("tenant", "Products", "Update", "Input your tenant to access this API i.e. admin for test", "admin", true)]
    public async Task<IActionResult> UnassignProductToClientAsync(Guid id)
    {
        return Ok(await _productService.UnassignProductToClientAsync(id));
    }

    /// <summary>
    /// List of available products which are unassigned to the clients.
    /// </summary>
    /// <response code="200">Products List returns.</response>
    /// <response code="500">Oops! Can't lookup your record right now.</response>
    [HttpGet("unassigned")]
    [ProducesResponseType(typeof(PaginatedResult<ProductDto>), 200)]
    [ProducesResponseType(500)]
    [SwaggerHeader("tenant", "Products", "Search", "Input your tenant to access this API i.e. admin for test", "admin", true)]
    [MustHavePermission(PermissionConstants.Products.Search)]
    [SwaggerOperation(Summary = "List of available products which are unassigned to the clients.")]
    public async Task<IActionResult> ListOfUnassignedProducts()
    {
        var products = await _productService.AvailableProductsForClients();
        return Ok(products);
    }

    /// <summary>
    /// Delete a specific product by unique id.
    /// </summary>
    /// <response code="200">Product deleted.</response>
    /// <response code="404">Product not found.</response>
    /// <response code="500">Oops! Can't lookup your product right now.</response>
    [HttpDelete("{id}")]
    [ProducesResponseType(typeof(Result<Guid>), 200)]
    [ProducesResponseType(404)]
    [ProducesResponseType(500)]
    [SwaggerHeader("tenant", "Products", "Remove", "Input your tenant to access this API i.e. admin for test", "admin", true)]
    [MustHavePermission(PermissionConstants.Products.Remove)]
    public async Task<IActionResult> DeleteAsync(Guid id)
    {
        var productId = await _productService.DeleteProductAsync(id);
        return Ok(productId);
    }

    /// <summary>
    /// suspension of specific product from client.
    /// </summary>
    /// <response code="200">Product suspension from client.</response>
    /// <response code="404">Product not found.</response>
    /// <response code="500">Oops! Can't lookup your product right now.</response>
    [HttpPut("{id:guid}/{suspendedreason}/suspension")]
    [ProducesResponseType(typeof(Result<Guid>), 200)]
    [ProducesResponseType(404)]
    [ProducesResponseType(500)]
    [MustHavePermission(PermissionConstants.Products.Update)]
    [SwaggerHeader("tenant", "Products", "Update", "Input your tenant to access this API i.e. admin for test", "admin", true)]
    public async Task<IActionResult> SuspensionOfProductAsync(Guid id, string suspendedreason)
    {
        return Ok(await _productService.SuspensionOfProductAsync(id, suspendedreason));
    }

    /// <summary>
    /// cancellation of specific product from client.
    /// </summary>
    /// <response code="200">Product cancellation from client.</response>
    /// <response code="404">Product not found.</response>
    /// <response code="500">Oops! Can't lookup your product right now.</response>
    [HttpPut("{id:guid}/{sendemail:bool}/cancellation")]
    [ProducesResponseType(typeof(Result<Guid>), 200)]
    [ProducesResponseType(404)]
    [ProducesResponseType(500)]
    [MustHavePermission(PermissionConstants.Products.Update)]
    [SwaggerHeader("tenant", "Products", "Update", "Input your tenant to access this API i.e. admin for test", "admin", true)]
    public async Task<IActionResult> CancellationRequestOfProductAsync(Guid id, bool sendemail)
    {
        return Ok(await _productService.CancellationRequestOfProductAsync(id, sendemail));
    }

    /// <summary>
    /// Unassign unsuspension product from client.
    /// </summary>
    /// <response code="200">Product unsuspension from client.</response>
    /// <response code="404">Product not found.</response>
    /// <response code="500">Oops! Can't lookup your product right now.</response>
    [HttpPut("{id:guid}/unsuspension")]
    [ProducesResponseType(typeof(Result<Guid>), 200)]
    [ProducesResponseType(404)]
    [ProducesResponseType(500)]
    [MustHavePermission(PermissionConstants.Products.Update)]
    [SwaggerHeader("tenant", "Products", "Update", "Input your tenant to access this API i.e. admin for test", "admin", true)]
    public async Task<IActionResult> UnSuspensionOfProductAsync(Guid id)
    {
        return Ok(await _productService.UnSuspensionOfProductAsync(id));
    }

    /// <summary>
    /// Unassign unsuspension product from client.
    /// </summary>
    /// <response code="200">Product unsuspension from client.</response>
    /// <response code="404">Product not found.</response>
    /// <response code="500">Oops! Can't lookup your product right now.</response>
    [HttpPut("{id:guid}/pendingproduct")]
    [ProducesResponseType(typeof(Result<Guid>), 200)]
    [ProducesResponseType(404)]
    [ProducesResponseType(500)]
    [MustHavePermission(PermissionConstants.Products.Update)]
    [SwaggerHeader("tenant", "Products", "Update", "Input your tenant to access this API i.e. admin for test", "admin", true)]
    public async Task<IActionResult> PendingProductAsync(Guid id)
    {
        return Ok(await _productService.PendingProductAsync(id));
    }

    /// <summary>
    /// renewal of specific product from client.
    /// </summary>
    /// <response code="200">Product renewal from client.</response>
    /// <response code="404">Product not found.</response>
    /// <response code="500">Oops! Can't lookup your product right now.</response>
    [HttpPut("{id:guid}/activateproduct")]
    [ProducesResponseType(typeof(Result<Guid>), 200)]
    [ProducesResponseType(404)]
    [ProducesResponseType(500)]
    [MustHavePermission(PermissionConstants.Products.Update)]
    [SwaggerHeader("tenant", "Products", "Update", "Input your tenant to access this API i.e. admin for test", "admin", true)]
    public async Task<IActionResult> ActivateProductAsync(Guid id)
    {
        return Ok(await _productService.ActivateProductAsync(id));
    }

    /// <summary>
    /// termination of specific product from client.
    /// </summary>
    /// <response code="200">Product termination from client.</response>
    /// <response code="404">Product not found.</response>
    /// <response code="500">Oops! Can't lookup your product right now.</response>
    [HttpPut("{id:guid}/termination")]
    [ProducesResponseType(typeof(Result<Guid>), 200)]
    [ProducesResponseType(404)]
    [ProducesResponseType(500)]
    [MustHavePermission(PermissionConstants.Products.Update)]
    [SwaggerHeader("tenant", "Products", "Update", "Input your tenant to access this API i.e. admin for test", "admin", true)]
    public async Task<IActionResult> TerminationOfProductAsync(Guid id)
    {
        return Ok(await _productService.TerminationOfProductAsync(id));
    }
}
