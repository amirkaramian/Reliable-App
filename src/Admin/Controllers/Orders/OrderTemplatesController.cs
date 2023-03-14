using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MyReliableSite.Application.Orders.Interfaces;
using MyReliableSite.Application.Wrapper;
using MyReliableSite.Domain.Constants;
using MyReliableSite.Infrastructure.Identity.Permissions;
using MyReliableSite.Infrastructure.Swagger;
using MyReliableSite.Shared.DTOs.Orders;
using Swashbuckle.AspNetCore.Annotations;

namespace MyReliableSite.Admin.API.Controllers.Orders;

public class OrderTemplatesController : BaseController
{
    private readonly IOrderTemplateService _orderService;

    public OrderTemplatesController(IOrderTemplateService orderService)
    {
        _orderService = orderService;
    }

    /// <summary>
    /// Retrive the order's Template against specific id.
    /// </summary>
    /// <response code="200">Order's Template returns.</response>
    /// <response code="404">Order's Template not found.</response>
    /// <response code="500">Oops! Can't lookup your record right now.</response>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(Result<OrderTemplateDetailsDto>), 200)]
    [ProducesResponseType(404)]
    [ProducesResponseType(500)]
    [SwaggerHeader("tenant", "Orders", "View", "Input your tenant to access this API i.e. admin for test", "admin", true)]
    [MustHavePermission(PermissionConstants.Orders.View)]
    public async Task<IActionResult> GetAsync(Guid id)
    {
        var order = await _orderService.GetOrderTemplateDetailsAsync(id);
        return Ok(order);
    }

    /// <summary>
    /// Retrive the order's Template against specific tenant.
    /// </summary>
    /// <response code="200">Order's Template returns.</response>
    /// <response code="404">Order's Template not found.</response>
    /// <response code="500">Oops! Can't lookup your record right now.</response>
    [HttpGet("getordertemplates/{tenant}")]
    [ProducesResponseType(typeof(Result<OrderTemplateDetailsDto>), 200)]
    [ProducesResponseType(404)]
    [ProducesResponseType(500)]
    [SwaggerHeader("tenant", "Orders", "View", "Input your tenant to access this API i.e. admin for test", "admin", true)]
    [MustHavePermission(PermissionConstants.Orders.View)]
    public async Task<IActionResult> GetAllAsync(string tenant)
    {
        var order = await _orderService.GetAllAsync(tenant);
        return Ok(order);
    }

    /// <summary>
    /// List of records with pagination &amp; filters.
    /// </summary>
    /// <response code="200">Order's Template List returns.</response>
    /// <response code="500">Oops! Can't lookup your record right now.</response>
    [HttpPost("search")]
    [ProducesResponseType(typeof(PaginatedResult<OrderTemplateDto>), 200)]
    [ProducesResponseType(500)]
    [SwaggerHeader("tenant", "Orders", "Search", "Input your tenant to access this API i.e. admin for test", "admin", true)]
    [MustHavePermission(PermissionConstants.Products.Search)]
    [SwaggerOperation(Summary = "Search Orders using available Filters.")]
    public async Task<IActionResult> SearchAsync(OrderTemplateListFilter filter)
    {
        var orders = await _orderService.SearchAsync(filter);
        return Ok(orders);
    }

    /// <summary>
    /// Create order's Template.
    /// </summary>
    /// <response code="200">Order's Template create.</response>
    /// <response code="404">Order's Template not found.</response>
    /// <response code="500">Oops! Can't lookup your product right now.</response>
    [HttpPost]
    [ProducesResponseType(typeof(Result<Guid>), 200)]
    [ProducesResponseType(500)]
    [MustHavePermission(PermissionConstants.Orders.Create)]
    [SwaggerHeader("tenant", "Orders", "Update", "Input your tenant to access this API i.e. admin for test", "admin", true)]
    public async Task<IActionResult> CreateAsync(CreateOrderTemplateRequest request)
    {
        return Ok(await _orderService.CreateOrderTemplateAsync(request));
    }

    /// <summary>
    /// Update a specific order's Template by unique id.
    /// </summary>
    /// <response code="200">order's Template updated.</response>
    /// <response code="404">order's Template not found.</response>
    /// <response code="500">Oops! Can't lookup your product right now.</response>
    [HttpPut("{id:guid}")]
    [ProducesResponseType(typeof(Result<Guid>), 200)]
    [ProducesResponseType(404)]
    [ProducesResponseType(500)]
    [MustHavePermission(PermissionConstants.Orders.Update)]
    [SwaggerHeader("tenant", "Orders", "Update", "Input your tenant to access this API i.e. admin for test", "admin", true)]
    public async Task<IActionResult> UpdateAsync(UpdateOrderTemplateRequest request, Guid id)
    {
        return Ok(await _orderService.UpdateOrderTemplateAsync(request, id));
    }

    /// <summary>
    /// Delete a specific order's Template by unique id.
    /// </summary>
    /// <response code="200">order's Template deleted.</response>
    /// <response code="404">order's Template not found.</response>
    /// <response code="500">Oops! Can't lookup your product right now.</response>
    [HttpDelete("{id}")]
    [ProducesResponseType(typeof(Result<Guid>), 200)]
    [ProducesResponseType(404)]
    [ProducesResponseType(500)]
    [SwaggerHeader("tenant", "Orders", "Remove", "Input your tenant to access this API i.e. admin for test", "admin", true)]
    [MustHavePermission(PermissionConstants.Orders.Remove)]
    public async Task<IActionResult> DeleteAsync(Guid id)
    {
        var orderId = await _orderService.DeleteOrderTemplateAsync(id);
        return Ok(orderId);
    }
}
