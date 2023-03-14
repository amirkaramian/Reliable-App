using Microsoft.AspNetCore.Mvc;
using MyReliableSite.Application.Dashboard.Interfaces;
using MyReliableSite.Application.Tickets.Interfaces;
using MyReliableSite.Application.Wrapper;
using MyReliableSite.Domain.Constants;
using MyReliableSite.Infrastructure.Identity.Permissions;
using MyReliableSite.Infrastructure.Swagger;
using MyReliableSite.Shared.DTOs.Orders;
using MyReliableSite.Shared.DTOs.Tickets;
using Swashbuckle.AspNetCore.Annotations;

namespace MyReliableSite.Client.API.Controllers.Ticket;
[Route("api/[controller]")]
[ApiController]
public class TicketsController : Controller
{
    private readonly ITicketService _service;
    private readonly IConfiguration _config;
    public TicketsController(ITicketService service, IConfiguration config)
    {
        _service = service;
        _config = config;
    }

    /// <summary>
    /// List of records with pagination &amp; filters.
    /// </summary>
    /// <response code="200">Bills List returns.</response>
    /// <response code="500">Oops! Can't lookup your record right now.</response>
    [HttpPost("search")]
    [ProducesResponseType(typeof(PaginatedResult<TicketDto>), 200)]
    [ProducesResponseType(500)]
    [SwaggerHeader("tenant", "Tickets", "Search", "Input your tenant to access this API i.e. client for test", "client", true)]
    [MustHavePermission(PermissionConstants.Tickets.Search)]
    [SwaggerOperation(Summary = "Search Invoices using available Filters.")]
    public async Task<IActionResult> SearchAsync(TicketListFilter filter)
    {
        var bills = await _service.SearchAsync(filter);
        return Ok(bills);
    }

    [ProducesResponseType(typeof(Result<TicketDto>), 200)]
    [ProducesResponseType(typeof(IDictionary<string, string>), 400)]
    [ProducesResponseType(500)]
    [HttpGet("{id}")]
    [SwaggerHeader("tenant", "Tickets", "View", "Input your tenant to access this API i.e. client for test", "client", true)]
    [MustHavePermission(PermissionConstants.Tickets.View)]
    public async Task<IActionResult> GetAsync(Guid id)
    {
        var department = await _service.GetTicketAsyncWithCommentAndReplies(id);
        return Ok(department);
    }

    [ProducesResponseType(typeof(Result<Guid>), 200)]
    [ProducesResponseType(typeof(IDictionary<string, string>), 400)]
    [ProducesResponseType(500)]
    [HttpPut("close/{id}")]
    [SwaggerHeader("tenant", "Tickets", "Update", "Input your tenant to access this API i.e. client for test", "client", true)]
    [MustHavePermission(PermissionConstants.Tickets.Update)]
    public async Task<IActionResult> UpdateAsync(Guid id)
    {
        return Ok(await _service.CloseTicketAsync(id));
    }

    [ProducesResponseType(typeof(Result<Guid>), 200)]
    [ProducesResponseType(typeof(IDictionary<string, string>), 400)]
    [ProducesResponseType(500)]
    [HttpPost("create")]
    [SwaggerHeader("tenant", "Tickets", "Create", "Input your tenant to access this API i.e. client for test", "client", true)]
    [MustHavePermission(PermissionConstants.Tickets.Create)]
    public async Task<IActionResult> CreateAsync(CreateTicketRequest request)
    {
        return Ok(await _service.CreateTicketAsync(request));
    }

    [ProducesResponseType(typeof(Result<List<TicketHistoryDto>>), 200)]
    [ProducesResponseType(typeof(IDictionary<string, string>), 400)]
    [ProducesResponseType(500)]
    [HttpGet("gettickethistory/{id}")]
    [SwaggerHeader("tenant", "Tickets", "View", "Input your tenant to access this API i.e. client for test", "client", true)]
    [MustHavePermission(PermissionConstants.Tickets.View)]
    public async Task<IActionResult> GetTicketHistoryAsync(Guid id)
    {
        var tickets = await _service.GetTicketHistoryAsync(id);
        return Ok(tickets);
    }

    [ProducesResponseType(typeof(Result<List<TicketDto>>), 200)]
    [ProducesResponseType(typeof(IDictionary<string, string>), 400)]
    [ProducesResponseType(500)]
    [HttpGet("getloggedinuserassigntickets")]
    [SwaggerHeader("tenant", "Tickets", "View", "Input your tenant to access this API i.e. client for test", "client", true)]
    [MustHavePermission(PermissionConstants.Tickets.View)]
    public async Task<IActionResult> GetCurrentUserTicketsAsync()
    {
        var tickets = await _service.GetCurrentUserTicketsAsync();
        return Ok(tickets);
    }
}
