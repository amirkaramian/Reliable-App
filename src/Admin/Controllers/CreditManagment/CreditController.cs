using Microsoft.AspNetCore.Mvc;
using MyReliableSite.Application.CreditManagement.Interfaces;
using MyReliableSite.Application.Wrapper;
using MyReliableSite.Domain.Constants;
using MyReliableSite.Infrastructure.Identity.Permissions;
using MyReliableSite.Infrastructure.Swagger;
using MyReliableSite.Shared.DTOs.CreditManagement;
using Org.BouncyCastle.Asn1.Ocsp;

namespace MyReliableSite.Admin.API.Controllers.CreditManagment;
public class CreditController : BaseController
{
    private readonly ICreditService _creditService;

    public CreditController(ICreditService creditService)
    {
        _creditService = creditService;
    }

    /// <summary>
    /// Create account credit By Admin.
    /// </summary>
    /// <response code="200">Credit created.</response>
    /// <response code="404">Settings not found.</response>
    /// <response code="500">Oops! Can't create credit right now.</response>
    [HttpPost("add")]
    [ProducesResponseType(typeof(Result<Guid>), 200)]
    [ProducesResponseType(404)]
    [ProducesResponseType(500)]
    [SwaggerHeader("tenant", "Credits", "Create", "Input your tenant to access this API i.e. admin for test", "admin", true)]
    [MustHavePermission(PermissionConstants.Credits.Create)]
    public async Task<IActionResult> CreateCreditAccountAsync(CreateCreditRequest request)
    {
        return Ok(await _creditService.CreateCreditAdminAsync(request));
    }

    /// <summary>
    /// decrease credit amount value .
    /// </summary>
    /// <response code="200">Brand updated.</response>
    /// <response code="404">Brand not found.</response>
    /// <response code="500">Oops! Can't lookup your product right now.</response>
    [ProducesResponseType(typeof(Result<CreditDto>), 200)]
    [ProducesResponseType(404)]
    [ProducesResponseType(500)]
    [HttpPost("decrease")]
    [MustHavePermission(PermissionConstants.Brands.Update)]
    [SwaggerHeader("tenant", "Credits", "decrease", "Input your tenant to access this API i.e. admin for test", "admin", true)]
    public async Task<IActionResult> DecreaseCreditAsync(DecreaseCreditRequest request)
    {
        return Ok(await _creditService.DecreaseCreditAsync(request));
    }

    [HttpDelete("remove/{id}")]
    [SwaggerHeader("tenant", "Credits", "Delete", "Input your tenant to access this API i.e. admin for test", "admin", true)]
    [MustHavePermission(PermissionConstants.Credits.Remove)]
    public async Task<IActionResult> DeleteCreditAccountAsync(Guid id)
    {
        var creditId = await _creditService.RemoveCreditAsync(id);
        return Ok(creditId);
    }

    /// <summary>
    /// List of Credit with pagination &amp; filters.
    /// </summary>
    /// <response code="200">Credits List returns.</response>
    /// <response code="400">Credit not found.</response>
    /// <response code="500">Oops! Can't lookup your record right now.</response>
    [HttpPost("search")]
    [ProducesResponseType(typeof(PaginatedResult<CreditDto>), 200)]
    [ProducesResponseType(typeof(IDictionary<string, string>), 400)]
    [ProducesResponseType(500)]
    [MustHavePermission(PermissionConstants.Brands.Search)]
    [SwaggerHeader("tenant", "Credits", "Search", "Input your tenant to access this API i.e. admin for test", "admin", true)]
    public async Task<IActionResult> SearchCreditAccountAsync(CreditListFilter filter)
    {
        return Ok(await _creditService.SearchAsync(filter));
    }

    /// <summary>
    /// Get credit information include balance for Client.
    /// </summary>
    /// <response code="200">Credit added.</response>
    /// <response code="404">Settings not found.</response>
    /// <response code="500">Oops! Can't create credit right now.</response>
    [HttpGet("balance/{clientId}")]
    [ProducesResponseType(typeof(Result<Guid>), 200)]
    [ProducesResponseType(404)]
    [ProducesResponseType(500)]
    [SwaggerHeader("tenant", "Credits", "View", "Input your tenant to access this API i.e. admin for test", "admin", true)]
    [MustHavePermission(PermissionConstants.Credits.View)]
    public async Task<IActionResult> GetCreditInfoAsync(Guid clientId)
    {
        return Ok(await _creditService.GetClientCreditInfo(clientId.ToString()));
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
    [SwaggerHeader("tenant", "Credits", "Create", "Input your tenant to access this API i.e. admin for test", "admin", true)]
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
    [SwaggerHeader("tenant", "Credits", "Create", "Input your tenant to access this API i.e. admin for test", "admin", true)]
    [MustHavePermission(PermissionConstants.Credits.Create)]
    public async Task<IActionResult> MakeAllInvoicePaymentWithCreditAsync(MakeAllInvoiceCreditPaymentRequest request)
    {
        return Ok(await _creditService.MakeAllInvoicePaymentRequest(request));
    }
}
