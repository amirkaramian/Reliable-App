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

public class OrdersController : BaseController
{
    private readonly IOrderService _orderService;

    public OrdersController(IOrderService orderService)
    {
        _orderService = orderService;
    }

    /// <summary>
    /// Create a Order (Transactions and Bills will be automatically generated) and a notification will send to all the admins.
    /// </summary>
    /// <response code="200">Order updated.</response>
    /// <response code="404">Settings not found.</response>
    /// <response code="500">Oops! Can't lookup your product right now.</response>
    [HttpPost]
    [ProducesResponseType(typeof(Result<Guid>), 200)]
    [ProducesResponseType(404)]
    [ProducesResponseType(500)]
    [SwaggerHeader("tenant", "Orders", "Create", "Input your tenant to access this API i.e. admin for test", "client", true)]
    [MustHavePermission(PermissionConstants.Orders.Create)]
    public async Task<IActionResult> CreateAsync(CreateOrderRequest request)
    {
        return Ok(await _orderService.CreateOrderAsync(request));
    }

    /// <summary>
    /// Retrive the order against specific id.
    /// </summary>
    /// <response code="200">Order returns.</response>
    /// <response code="404">Order not found.</response>
    /// <response code="500">Oops! Can't lookup your record right now.</response>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(Result<OrderDetailsDto>), 200)]
    [ProducesResponseType(404)]
    [ProducesResponseType(500)]
    [SwaggerHeader("tenant", "Orders", "View", "Input your tenant to access this API i.e. admin for test", "admin", true)]
    [MustHavePermission(PermissionConstants.Orders.View)]
    public async Task<IActionResult> GetAsync(Guid id)
    {
        var order = await _orderService.GetOrderDetailsAsync(id);
        return Ok(order);
    }

    /// <summary>
    /// Retrive the order against specific id.
    /// </summary>
    /// <response code="200">Order returns.</response>
    /// <response code="404">Order not found.</response>
    /// <response code="500">Oops! Can't lookup your record right now.</response>
    [HttpGet("getallordersbyadminid{id}")]
    [ProducesResponseType(typeof(Result<OrderDto>), 200)]
    [ProducesResponseType(404)]
    [ProducesResponseType(500)]
    [SwaggerHeader("tenant", "Orders", "View", "Input your tenant to access this API i.e. admin for test", "admin", true)]
    [MustHavePermission(PermissionConstants.Orders.View)]
    public async Task<IActionResult> GetAllOrdersByAdminID(string id)
    {
        var order = await _orderService.GetAllOrdersByAdminID(id);
        return Ok(order);
    }

    /// <summary>
    /// List of records with pagination &amp; filters.
    /// </summary>
    /// <response code="200">Orders List returns.</response>
    /// <response code="500">Oops! Can't lookup your record right now.</response>
    [HttpPost("search")]
    [ProducesResponseType(typeof(PaginatedResult<OrderDto>), 200)]
    [ProducesResponseType(500)]
    [SwaggerHeader("tenant", "Orders", "Search", "Input your tenant to access this API i.e. admin for test", "admin", true)]
    [MustHavePermission(PermissionConstants.Products.Search)]
    [SwaggerOperation(Summary = "Search Orders using available Filters.")]
    public async Task<IActionResult> SearchAsync(OrderListFilter filter)
    {
        var orders = await _orderService.SearchAsync(filter);
        return Ok(orders);
    }

    /// <summary>
    /// Update a specific order by unique id.
    /// </summary>
    /// <response code="200">Order updated.</response>
    /// <response code="404">Order not found.</response>
    /// <response code="500">Oops! Can't lookup your product right now.</response>
    [HttpPut("{id:guid}")]
    [ProducesResponseType(typeof(Result<Guid>), 200)]
    [ProducesResponseType(404)]
    [ProducesResponseType(500)]
    [MustHavePermission(PermissionConstants.Orders.Update)]
    [SwaggerHeader("tenant", "Orders", "Update", "Input your tenant to access this API i.e. admin for test", "admin", true)]
    public async Task<IActionResult> UpdateAsync(UpdateOrderRequest request, Guid id)
    {
        return Ok(await _orderService.UpdateOrderAsync(request, id));
    }

    /// <summary>
    /// Update a specific order by unique id.
    /// </summary>
    /// <response code="200">Order updated.</response>
    /// <response code="404">Order not found.</response>
    /// <response code="500">Oops! Can't lookup your product right now.</response>
    [HttpPut("updateorderadminasync/{adminid}/{id:guid}")]
    [ProducesResponseType(typeof(Result<Guid>), 200)]
    [ProducesResponseType(404)]
    [ProducesResponseType(500)]
    [MustHavePermission(PermissionConstants.Orders.Update)]
    [SwaggerHeader("tenant", "Orders", "Update", "Input your tenant to access this API i.e. admin for test", "admin", true)]
    public async Task<IActionResult> UpdateOrderAdminAsync(List<string> adminid, Guid id)
    {
        return Ok(await _orderService.UpdateOrderAdminAsync(adminid, id));
    }

    /// <summary>
    /// Delete a specific order by unique id.
    /// </summary>
    /// <response code="200">Order deleted.</response>
    /// <response code="404">Order not found.</response>
    /// <response code="500">Oops! Can't lookup your product right now.</response>
    [HttpDelete("{id}")]
    [ProducesResponseType(typeof(Result<Guid>), 200)]
    [ProducesResponseType(404)]
    [ProducesResponseType(500)]
    [SwaggerHeader("tenant", "Orders", "Remove", "Input your tenant to access this API i.e. admin for test", "admin", true)]
    [MustHavePermission(PermissionConstants.Orders.Remove)]
    public async Task<IActionResult> DeleteAsync(Guid id)
    {
        var orderId = await _orderService.DeleteOrderAsync(id);
        return Ok(orderId);
    }

    /// <summary>
    /// Update a specific order by unique id.
    /// </summary>
    /// <response code="200">Order updated.</response>
    /// <response code="404">Order not found.</response>
    /// <response code="500">Oops! Can't lookup your product right now.</response>
    [HttpPut("updateorderadmin1async/{adminid}/{id:guid}")]
    [ProducesResponseType(typeof(Result<Guid>), 200)]
    [ProducesResponseType(404)]
    [ProducesResponseType(500)]
    [MustHavePermission(PermissionConstants.Orders.Update)]
    [SwaggerHeader("tenant", "Orders", "Update", "Input your tenant to access this API i.e. admin for test", "admin", true)]
    public async Task<IActionResult> UpdateOrderAdmin1Async(List<string> adminid, Guid id)
    {
        return Ok(await _orderService.UpdateOrderAdminAsync(adminid, id));
    }
}
