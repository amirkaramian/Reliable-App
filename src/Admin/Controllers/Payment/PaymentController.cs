using Microsoft.AspNetCore.Mvc;
using MyReliableSite.Application.PaymentGateways.Interfaces;
using MyReliableSite.Application.Wrapper;
using MyReliableSite.Domain.Common.Contracts;
using MyReliableSite.Domain.Constants;
using MyReliableSite.Infrastructure.Identity.Permissions;
using MyReliableSite.Infrastructure.Swagger;
using MyReliableSite.Shared.DTOs.PaymentGateways;

namespace MyReliableSite.Admin.API.Controllers.Payment;
public class PaymentController : BaseController
{
    private readonly IPaymentGatewayService _paymentGatewayService;

    public PaymentController(IPaymentGatewayService paymentGatewayService)
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
}
