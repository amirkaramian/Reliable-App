using Microsoft.AspNetCore.Mvc;
using MyReliableSite.Application.Abstractions.Services.Identity;
using MyReliableSite.Application.Products.Interfaces;
using MyReliableSite.Application.Wrapper;
using MyReliableSite.Domain.Constants;
using MyReliableSite.Infrastructure.Identity.Permissions;
using MyReliableSite.Infrastructure.Swagger;
using MyReliableSite.Shared.DTOs.Products;
using Swashbuckle.AspNetCore.Annotations;

namespace MyReliableSite.Client.API.Controllers.Products;

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
    [SwaggerHeader("tenant", "Products", "View", "Input your tenant to access this API i.e. client", "client", true)]
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
    [SwaggerHeader("tenant", "Products", "Search", "Input your tenant to access this API i.e. client", "client", true)]
    [MustHavePermission(PermissionConstants.Products.Search)]
    [SwaggerOperation(Summary = "Search Products using available Filters.")]
    public async Task<IActionResult> SearchAsync(ProductListFilter filter)
    {
        var products = await _productService.SearchAsClientAsync(filter);
        return Ok(products);
    }

    /// <summary>
    /// List of records without pagination &amp; filters.
    /// </summary>
    /// <response code="200">Products List returns.</response>
    /// <response code="500">Oops! Can't lookup your record right now.</response>
    [HttpGet("listasync")]
    [ProducesResponseType(typeof(PaginatedResult<ProductDto>), 200)]
    [ProducesResponseType(500)]
    [SwaggerHeader("tenant", "Products", "View", "Input your tenant to access this API i.e. client", "client", true)]
    [MustHavePermission(PermissionConstants.Products.Search)]
    [SwaggerOperation(Summary = "List of records without pagination &amp; filters.")]
    public async Task<IActionResult> ListAsync()
    {
        var products = await _productService.GetProductsListAsync();
        return Ok(products);
    }

    /// <summary>
    /// fetch list of products using status.
    /// </summary>
    /// <response code="200">Products List returns.</response>
    /// <response code="500">Oops! Can't lookup your record right now.</response>
    [HttpPost("getproducts/{status}")]
    [ProducesResponseType(typeof(PaginatedResult<ProductDto>), 200)]
    [ProducesResponseType(500)]
    [SwaggerHeader("tenant", "Products", "View", "Input your tenant to access this API i.e. client", "client", true)]
    [MustHavePermission(PermissionConstants.Products.View)]
    [SwaggerOperation(Summary = "fetch list of products using status..")]
    public async Task<IActionResult> GetProductsByStatus(string status, [FromBody] ProductListFilter filter)
    {
        var products = await _productService.GetProductsWithStatus(status, filter);
        return Ok(products);
    }

    /// <summary>
    /// Gets the count of Active/Suspended/Pending/Cancelled Products.
    /// </summary>
    /// <response code="200">Products Counts returns.</response>
    /// <response code="500">Oops! Can't lookup your record right now.</response>
    [HttpGet("productcounts")]
    [ProducesResponseType(typeof(Result<ProductCounts>), 200)]
    [ProducesResponseType(500)]
    [SwaggerHeader("tenant", "Products", "View", "Input your tenant to access this API i.e. client", "client", true)]
    [MustHavePermission(PermissionConstants.Products.View)]
    [SwaggerOperation(Summary = "Gets the count of Active/Suspended/Pending/Cancelled Products")]
    public async Task<IActionResult> GetProductCounst()
    {
        var counts = await _productService.GetProductCounts();
        return Ok(counts);
    }

    /// <summary>
    /// cancellation of specific product from client.
    /// </summary>
    /// <response code="200">Product cancellation from client.</response>
    /// <response code="404">Product not found.</response>
    /// <response code="500">Oops! Can't lookup your product right now.</response>
    [HttpPut("{id:guid}/cancellation")]
    [ProducesResponseType(typeof(Result<Guid>), 200)]
    [ProducesResponseType(404)]
    [ProducesResponseType(500)]
    [MustHavePermission(PermissionConstants.Products.View)]
    [SwaggerHeader("tenant", "Products", "Update", "Input your tenant to access this API i.e. admin for test", "admin", true)]
    public async Task<IActionResult> CancellationRequestOfProductAsync(Guid id)
    {
        return Ok(await _productService.SendCancelRequestEmail(id));
    }

    /// <summary>
    /// cancellation of specific product from client.
    /// </summary>
    /// <response code="200">Product cancellation from client.</response>
    /// <response code="404">Product not found.</response>
    /// <response code="500">Oops! Can't lookup your product right now.</response>
    [HttpPut("{id:guid}/cancellation/confirm")]
    [ProducesResponseType(typeof(Result<Guid>), 200)]
    [ProducesResponseType(404)]
    [ProducesResponseType(500)]
    [MustHavePermission(PermissionConstants.Products.View)]
    [SwaggerHeader("tenant", "Products", "Update", "Input your tenant to access this API i.e. admin for test", "admin", true)]
    public async Task<IActionResult> ConfirmCancellationRequestAsync(Guid id, [FromQuery] string token)
    {
        return Ok(await _productService.ConfirmCancelRequestAsync(id, token));
    }

    /// <summary>
    /// cancellation of specific product from client on end of next billing date.
    /// </summary>
    /// <response code="200">Product cancellation from client.</response>
    /// <response code="404">Product not found.</response>
    /// <response code="500">Oops! Can't lookup your product right now.</response>
    [HttpPut("{id:guid}/cancellation/endofbilling")]
    [ProducesResponseType(typeof(Result<Guid>), 200)]
    [ProducesResponseType(404)]
    [ProducesResponseType(500)]
    [MustHavePermission(PermissionConstants.Products.View)]
    [SwaggerHeader("tenant", "Products", "Update", "Input your tenant to access this API i.e. admin for test", "admin", true)]
    public async Task<IActionResult> CancellationRequestEOB(Guid id)
    {
        return Ok(await _productService.CancellationRequestEOB(id));
    }

    /// <summary>
    /// remove cancellation of specific product.
    /// </summary>
    /// <response code="200">Product cancellation from client.</response>
    /// <response code="404">Product not found.</response>
    /// <response code="500">Oops! Can't lookup your product right now.</response>
    [HttpPut("{id:guid}/cancellation/remove")]
    [ProducesResponseType(typeof(Result<Guid>), 200)]
    [ProducesResponseType(404)]
    [ProducesResponseType(500)]
    [MustHavePermission(PermissionConstants.Products.View)]
    [SwaggerHeader("tenant", "Products", "Update", "Input your tenant to access this API i.e. admin for test", "admin", true)]
    public async Task<IActionResult> CancellationRequestRemoval(Guid id)
    {
        return Ok(await _productService.RemoveCancellationRequest(id));
    }

    /// <summary>
    /// List of available products which are unassigned to the clients.
    /// </summary>
    /// <response code="200">Products List returns.</response>
    /// <response code="500">Oops! Can't lookup your record right now.</response>
    [HttpGet("unassigned")]
    [ProducesResponseType(typeof(PaginatedResult<ProductDto>), 200)]
    [ProducesResponseType(500)]
    [SwaggerHeader("tenant", "Products", "Search", "Input your tenant to access this API i.e. client", "client", true)]
    [MustHavePermission(PermissionConstants.Products.Search)]
    [SwaggerOperation(Summary = "List of available products which are unassigned to the clients.")]
    public async Task<IActionResult> ListOfUnassignedProducts()
    {
        var products = await _productService.AvailableProductsForClients();
        return Ok(products);
    }
}
