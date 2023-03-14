using Microsoft.AspNetCore.Mvc;

using MyReliableSite.Application.PaymentGateways.Interfaces;
using MyReliableSite.Domain.Constants;
using MyReliableSite.Infrastructure.Identity.Permissions;
using MyReliableSite.Infrastructure.Swagger;
using MyReliableSite.Shared.DTOs.PaymentGateways;
using MyReliableSite.Application.Wrapper;

namespace MyReliableSite.Admin.API.Controllers.PaymentGateways;
[Route("api/[controller]")]
[ApiController]

public class PaymentGatewaysController : BaseController
{
    private readonly IPaymentGatewayService _paymentGatewayService;

    public PaymentGatewaysController(IPaymentGatewayService paymentGatewayService)
    {
        _paymentGatewayService = paymentGatewayService ?? throw new ArgumentNullException(nameof(paymentGatewayService));
    }

    /// <summary>
    /// retrive the PaymentGateway against specific id.
    /// </summary>
    /// <response code="200">PaymentGateway returns.</response>
    /// <response code="400">PaymentGateway not found.</response>
    /// <response code="500">Oops! Can't lookup your record right now.</response>
    [ProducesResponseType(typeof(Result<PaymentGatewayDto>), 200)]
    [ProducesResponseType(typeof(IDictionary<string, string>), 400)]
    [ProducesResponseType(500)]
    [HttpGet("{id:guid}")]
    [MustHavePermission(PermissionConstants.PaymentGateways.View)]
    [SwaggerHeader("tenant", "PaymentGateways", "Search", "Input your tenant to access this API i.e. admin for test", "admin", true)]
    public async Task<IActionResult> GetAsync(Guid id) => Ok(await _paymentGatewayService.GetAsync(id));

    /// <summary>
    /// List of records with pagination &amp; filters.
    /// </summary>
    /// <response code="200">PaymentGateways List returns.</response>
    /// <response code="400">PaymentGateway not found.</response>
    /// <response code="500">Oops! Can't lookup your record right now.</response>
    [HttpPost("search")]
    [ProducesResponseType(typeof(PaginatedResult<PaymentGatewayDto>), 200)]
    [ProducesResponseType(typeof(IDictionary<string, string>), 400)]
    [ProducesResponseType(500)]
    [MustHavePermission(PermissionConstants.PaymentGateways.Search)]
    [SwaggerHeader("tenant", "PaymentGateways", "Search", "Input your tenant to access this API i.e. admin for test", "admin", true)]
    public async Task<IActionResult> SearchAsync(PaymentGatewayListFilter filter)
    {
        return Ok(await _paymentGatewayService.SearchAsync(filter));
    }

    /// <summary>
    /// Create an PaymentGateway.
    /// </summary>
    /// <response code="200">PaymentGateway created.</response>
    /// <response code="400">PaymentGateway already exists.</response>
    /// <response code="500">Oops! Can't lookup your product right now.</response>
    [HttpPost]
    [ProducesResponseType(typeof(Result<PaymentGatewayDto>), 200)]
    [ProducesResponseType(typeof(IDictionary<string, string>), 400)]
    [ProducesResponseType(500)]
    [MustHavePermission(PermissionConstants.PaymentGateways.Create)]
    [SwaggerHeader("tenant", "PaymentGateways", "Search", "Input your tenant to access this API i.e. admin for test", "admin", true)]
    public async Task<IActionResult> CreateAsync(CreatePaymentGatewayRequest request)
    {
        return Ok(await _paymentGatewayService.CreateAsync(request));
    }

    /// <summary>
    /// update a specific PaymentGateway by unique id.
    /// </summary>
    /// <response code="200">PaymentGateway updated.</response>
    /// <response code="404">PaymentGateway not found.</response>
    /// <response code="500">Oops! Can't lookup your product right now.</response>
    [ProducesResponseType(typeof(Result<PaymentGatewayDto>), 200)]
    [ProducesResponseType(404)]
    [ProducesResponseType(500)]
    [HttpPut("{id:guid}")]
    [MustHavePermission(PermissionConstants.PaymentGateways.Update)]
    [SwaggerHeader("tenant", "PaymentGateways", "Search", "Input your tenant to access this API i.e. admin for test", "admin", true)]
    public async Task<IActionResult> UpdateAsync(UpdatePaymentGatewayRequest request, Guid id)
    {
        return Ok(await _paymentGatewayService.UpdateAsync(id, request));
    }

    /// <summary>
    /// Delete a specific PaymentGateway by unique id.
    /// </summary>
    /// <response code="200">PaymentGateway deleted.</response>
    /// <response code="404">PaymentGateway not found.</response>
    /// <response code="500">Oops! Can't lookup your product right now.</response>
    [ProducesResponseType(typeof(Result<bool>), 200)]
    [ProducesResponseType(404)]
    [ProducesResponseType(500)]
    [HttpDelete("{id:guid}")]
    [MustHavePermission(PermissionConstants.PaymentGateways.Remove)]
    [SwaggerHeader("tenant", "PaymentGateways", "Search", "Input your tenant to access this API i.e. admin for test", "admin", true)]
    public async Task<IActionResult> DeleteAsync(Guid id) => Ok(await _paymentGatewayService.DeleteAsync(id));
}
