using Microsoft.AspNetCore.Mvc;
using MyReliableSite.Application.Bills.Interfaces;
using MyReliableSite.Application.Wrapper;
using MyReliableSite.Domain.Constants;
using MyReliableSite.Infrastructure.Identity.Permissions;
using MyReliableSite.Infrastructure.Swagger;
using MyReliableSite.Shared.DTOs.Bills;
using MyReliableSite.Shared.DTOs.CreditManagement;
using MyReliableSite.Shared.DTOs.Orders;
using MyReliableSite.Shared.DTOs.Transaction;
using Swashbuckle.AspNetCore.Annotations;

namespace MyReliableSite.Admin.API.Controllers.Bills;

public class BillsController : BaseController
{
    private readonly IBillService _billService;

    public BillsController(IBillService billService)
    {
        _billService = billService;
    }

    /// <summary>
    /// retrive the Bills against specific id.
    /// </summary>
    /// <response code="200">Bills returns.</response>
    /// <response code="404">Bills not found.</response>
    /// <response code="500">Oops! Can't lookup your article feedback right now.</response>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(Result<BillDetailDto>), 200)]
    [ProducesResponseType(404)]
    [ProducesResponseType(500)]
    [SwaggerHeader("tenant", "Bills", "View", "Input your tenant to access this API i.e. admin for test", "admin", true)]
    [MustHavePermission(PermissionConstants.Bills.View)]
    public async Task<IActionResult> GetAsync(Guid id)
    {
        var bill = await _billService.GetBillDetailsAsync(id);

        return Ok(bill);
    }

    /// <summary>
    /// List of records with pagination &amp; filters.
    /// </summary>
    /// <response code="200">Bills List returns.</response>
    /// <response code="500">Oops! Can't lookup your record right now.</response>
    [HttpPost("search")]
    [ProducesResponseType(typeof(PaginatedResult<BillDto>), 200)]
    [ProducesResponseType(500)]
    [SwaggerHeader("tenant", "Bills", "Search", "Input your tenant to access this API i.e. admin for test", "admin", true)]
    [MustHavePermission(PermissionConstants.Bills.Search)]
    [SwaggerOperation(Summary = "Search Bills using available Filters.")]
    public async Task<IActionResult> SearchAsync(BillListFilter filter)
    {
        var bills = await _billService.SearchAsync(filter);
        return Ok(bills);
    }

    /// <summary>
    /// List of records without pagination &amp; filters.
    /// </summary>
    /// <response code="200">Bills List returns.</response>
    /// <response code="500">Oops! Can't lookup your record right now.</response>
    [HttpGet("getallinvoices")]
    [ProducesResponseType(typeof(IEnumerable<BillDetailDto>), 200)]
    [ProducesResponseType(typeof(IDictionary<string, string>), 400)]
    [SwaggerHeader("tenant", "Invoices", "View", "Input your tenant to access this API i.e. admin for test", "admin", true)]
    [MustHavePermission(PermissionConstants.Invoices.View)]
    [SwaggerOperation(Summary = "Get List of Invoices without paging")]
    public async Task<IActionResult> GetAllBillsDetailAsync()
    {
        var bills = await _billService.GetAllBillsDetailAsync();

        return Ok(bills);
    }

    /// <summary>
    /// List of records with pagination &amp; filters.
    /// </summary>
    /// <response code="200">Bills List returns.</response>
    /// <response code="500">Oops! Can't lookup your record right now.</response>
    [HttpPost("getallinvoices/{productId:guid}")]
    [ProducesResponseType(typeof(IEnumerable<BillDetailDto>), 200)]
    [ProducesResponseType(typeof(IDictionary<string, string>), 400)]
    [SwaggerHeader("tenant", "Invoices", "View", "Input your tenant to access this API i.e. admin for test", "admin", true)]
    [MustHavePermission(PermissionConstants.Invoices.View)]
    [SwaggerOperation(Summary = "Get List of Invoices in a single product")]
    public async Task<IActionResult> GetAllBillsFromProductAsync(Guid productId, [FromBody] BillListFilter billListFilter)
    {
        var bills = await _billService.GetAllBillsFromProductAsync(billListFilter, productId);
        return Ok(bills);
    }

    /// <summary>
    /// List of records without pagination &amp; filters.
    /// </summary>
    /// <response code="200">Bills List returns.</response>
    /// <response code="500">Oops! Can't lookup your record right now.</response>
    [HttpGet("getallpaidinvoices")]
    [ProducesResponseType(typeof(IEnumerable<BillDetailDto>), 200)]
    [ProducesResponseType(typeof(IDictionary<string, string>), 400)]
    [SwaggerHeader("tenant", "Invoices", "View", "Input your tenant to access this API i.e. admin for test", "admin", true)]
    [MustHavePermission(PermissionConstants.Invoices.View)]
    [SwaggerOperation(Summary = "Get List of Paid Invoices without paging")]
    public async Task<IActionResult> GetAllPaidBillsDetailAsync()
    {
        var bills = await _billService.GetAllPaidBillsDetailAsync();
        return Ok(bills);
    }

    /// <summary>
    /// List of records with pagination &amp; filters.
    /// </summary>
    /// <response code="200">Transaction List returns.</response>
    /// <response code="500">Oops! Can't lookup your record right now.</response>
    [HttpPost("transactions/search")]
    [ProducesResponseType(typeof(PaginatedResult<TransactionDto>), 200)]
    [ProducesResponseType(500)]
    [SwaggerHeader("tenant", "Invoices", "Search", "Input your tenant to access this API i.e. admin for test", "admin", true)]
    [MustHavePermission(PermissionConstants.Invoices.Search)]
    [SwaggerOperation(Summary = "Search Transaction using available Filters.")]
    public async Task<IActionResult> SearchAsync(TransactionListFilter filter)
    {
        var transaction = await _billService.SearchtransactionsAsync(filter);
        return Ok(transaction);
    }

    /// <summary>
    /// Get unpaid invoices.
    /// </summary>
    /// <response code="200">Transaction List returns.</response>
    /// <response code="500">Oops! Can't lookup your record right now.</response>
    [HttpGet("unpaid/{clientId}")]
    [ProducesResponseType(typeof(PaginatedResult<TransactionDto>), 200)]
    [ProducesResponseType(500)]
    [SwaggerHeader("tenant", "Invoices", "Search", "Input your tenant to access this API i.e. admin for test", "admin", true)]
    [MustHavePermission(PermissionConstants.Invoices.Search)]
    [SwaggerOperation(Summary = "Search Transaction using available Filters.")]
    public async Task<IActionResult> GetUnpaidInvoicesAsync(Guid clientId)
    {
        return Ok(await _billService.GetUnpaidInvoices(clientId));
    }

    /// <summary>
    /// Make invoice paymeny By Admin.
    /// </summary>
    /// <response code="200">bill paied.</response>
    /// <response code="404">Settings not found.</response>
    /// <response code="500">Oops! Can't create payment right now.</response>
    [HttpPost("makeinvoicepayment")]
    [ProducesResponseType(typeof(Result<Guid>), 200)]
    [ProducesResponseType(404)]
    [ProducesResponseType(500)]
    [SwaggerHeader("tenant", "Invoices", "Create", "Input your tenant to access this API i.e. admin for test", "admin", true)]
    [MustHavePermission(PermissionConstants.Invoices.Create)]
    public async Task<IActionResult> MakeInvoicePaymentAsync(MakeInvoicePaymentRequest request)
    {
        return Ok(await _billService.MakeInvoicePaymentRequest(request));
    }

    /// <summary>
    /// Make paymnet of all invoices of current.
    /// </summary>
    /// <response code="200">bills paied.</response>
    /// <response code="404">Settings not found.</response>
    /// <response code="500">Oops! Can't create credit right now.</response>
    [HttpPost("makeallinvoicepayment")]
    [ProducesResponseType(typeof(Result<List<TransactionDetailsDto>>), 200)]
    [ProducesResponseType(404)]
    [ProducesResponseType(500)]
    [SwaggerHeader("tenant", "Credits", "Create", "Input your tenant to access this API i.e. admin for test", "client", true)]
    [MustHavePermission(PermissionConstants.Credits.Create)]
    public async Task<IActionResult> MakeAllInvoicePaymentsOfCurrentUser(MakeAllInvoiceCreditPaymentRequest request)
    {
        return Ok(await _billService.MakeAllInvoicePaymentsOfCurrentUser(request));
    }
}
