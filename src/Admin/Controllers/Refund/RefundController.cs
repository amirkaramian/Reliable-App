using Microsoft.AspNetCore.Mvc;
using MyReliableSite.Application.Bills.Interfaces;
using MyReliableSite.Application.CreditManagement.Interfaces;
using MyReliableSite.Application.Wrapper;
using MyReliableSite.Domain.Constants;
using MyReliableSite.Infrastructure.Identity.Permissions;
using MyReliableSite.Infrastructure.Swagger;
using MyReliableSite.Shared.DTOs.CreditManagement;
using MyReliableSite.Shared.DTOs.Refund;
using Org.BouncyCastle.Asn1.Ocsp;

namespace MyReliableSite.Admin.API.Controllers.CreditManagment;
public class RefundController : BaseController
{
    private readonly IBillService _billService;

    public RefundController(IBillService billService)
    {
        _billService = billService;
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
    [SwaggerHeader("tenant", "Refunds", "Create", "Input your tenant to access this API i.e. admin for test", "admin", true)]
    [MustHavePermission(PermissionConstants.Refunds.Create)]
    public async Task<IActionResult> CreateRefundtAsync(CreateRefundRequest request)
    {
        return Ok(await _billService.CreateRefundAdminAsync(request));
    }

    /// <summary>
    /// List of Refund with pagination &amp; filters.
    /// </summary>
    /// <response code="200">Refunds List returns.</response>
    /// <response code="400">Refunds not found.</response>
    /// <response code="500">Oops! Can't lookup your record right now.</response>
    [HttpPost("search")]
    [ProducesResponseType(typeof(PaginatedResult<RefundDto>), 200)]
    [ProducesResponseType(typeof(IDictionary<string, string>), 400)]
    [ProducesResponseType(500)]
    [MustHavePermission(PermissionConstants.Refunds.Search)]
    [SwaggerHeader("tenant", "Refunds", "Search", "Input your tenant to access this API i.e. admin for test", "admin", true)]
    public async Task<IActionResult> SearchRefundAccountAsync(RefundListFilter filter)
    {
        return Ok(await _billService.SearchRefundAsync(filter));
    }
}
