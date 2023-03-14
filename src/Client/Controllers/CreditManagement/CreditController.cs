using Microsoft.AspNetCore.Mvc;
using MyReliableSite.Application.CreditManagement.Interfaces;
using MyReliableSite.Application.Wrapper;
using MyReliableSite.Domain.Constants;
using MyReliableSite.Infrastructure.Identity.Permissions;
using MyReliableSite.Infrastructure.Swagger;
using MyReliableSite.Shared.DTOs.CreditManagement;
using Org.BouncyCastle.Asn1.Ocsp;

namespace MyReliableSite.Client.API.Controllers.CreditManagement;

public class CreditController : BaseController
{
    private readonly ICreditService _creditService;

    public CreditController(ICreditService creditService)
    {
        _creditService = creditService;
    }

    /// <summary>
    /// Add credit By Client.
    /// </summary>
    /// <response code="200">Credit added.</response>
    /// <response code="404">Settings not found.</response>
    /// <response code="500">Oops! Can't create credit right now.</response>
    [HttpPost("add")]
    [ProducesResponseType(typeof(Result<Guid>), 200)]
    [ProducesResponseType(404)]
    [ProducesResponseType(500)]
    [SwaggerHeader("tenant", "Credits", "Create", "Input your tenant to access this API i.e. admin for test", "client", true)]
    [MustHavePermission(PermissionConstants.Credits.Create)]
    public async Task<IActionResult> AddCreditAsync(ClientCreateCreditRequest request)
    {
        return Ok(await _creditService.ClientAddCreditAsync(request));
    }

    /// <summary>
    /// Get credit information include balance By Client.
    /// </summary>
    /// <response code="200">Credit added.</response>
    /// <response code="404">Settings not found.</response>
    /// <response code="500">Oops! Can't create credit right now.</response>
    [HttpGet]
    [ProducesResponseType(typeof(Result<Guid>), 200)]
    [ProducesResponseType(404)]
    [ProducesResponseType(500)]
    [SwaggerHeader("tenant", "Credits", "View", "Input your tenant to access this API i.e. admin for test", "client", true)]
    [MustHavePermission(PermissionConstants.Credits.View)]
    public async Task<IActionResult> GetCreditInfoAsync()
    {
        return Ok(await _creditService.GetClientCreditInfo());
    }

    /// <summary>
    /// Make credit paymeny By Client.
    /// </summary>
    /// <response code="200">bill paied.</response>
    /// <response code="404">Settings not found.</response>
    /// <response code="500">Oops! Can't create credit right now.</response>
    [HttpPost("payment")]
    [ProducesResponseType(typeof(Result<Guid>), 200)]
    [ProducesResponseType(404)]
    [ProducesResponseType(500)]
    [SwaggerHeader("tenant", "Credits", "Create", "Input your tenant to access this API i.e. admin for test", "client", true)]
    [MustHavePermission(PermissionConstants.Credits.Create)]
    public async Task<IActionResult> MakePaymentWithCreditAsync(MakeCreditPaymentRequest request)
    {
        return Ok(await _creditService.MakeCreditPaymentRequest(request));
    }

    /// <summary>
    /// Make credit paymeny By Client for all invoices.
    /// </summary>
    /// <response code="200">bills paied.</response>
    /// <response code="404">Settings not found.</response>
    /// <response code="500">Oops! Can't create credit right now.</response>
    [HttpPost("payment/all")]
    [ProducesResponseType(typeof(Result<Guid>), 200)]
    [ProducesResponseType(404)]
    [ProducesResponseType(500)]
    [SwaggerHeader("tenant", "Credits", "Create", "Input your tenant to access this API i.e. admin for test", "client", true)]
    [MustHavePermission(PermissionConstants.Credits.Create)]
    public async Task<IActionResult> MakeAllInvoicePaymentWithCreditAsync(MakeAllInvoiceCreditPaymentRequest request)
    {
        return Ok(await _creditService.MakeAllInvoicePaymentRequest(request));
    }
}
