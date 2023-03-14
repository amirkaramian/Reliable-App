using Microsoft.AspNetCore.Mvc;
using MyReliableSite.Application.Tickets.Interfaces;
using MyReliableSite.Application.Wrapper;
using MyReliableSite.Domain.Constants;
using MyReliableSite.Infrastructure.Identity.Permissions;
using MyReliableSite.Infrastructure.Swagger;
using MyReliableSite.Shared.DTOs.Tickets;
using Swashbuckle.AspNetCore.Annotations;

namespace MyReliableSite.Admin.API.Controllers.Tickets;
[Route("api/[controller]")]
[ApiController]
public class TicketsController : BaseController
{
    private readonly ITicketService _service;
    private readonly IConfiguration _config;
    public TicketsController(ITicketService service, IConfiguration config)
    {
        _service = service;
        _config = config;
    }

    [ProducesResponseType(typeof(PaginatedResult<TicketDto>), 200)]
    [ProducesResponseType(typeof(IDictionary<string, string>), 400)]
    [ProducesResponseType(500)]
    [HttpPost("search")]
    [SwaggerHeader("tenant", "Tickets", "Search", "Input your tenant to access this API i.e. admin for test", "admin", true)]
    [MustHavePermission(PermissionConstants.Tickets.Search)]
    [SwaggerOperation(Summary = "Search Tickets using available Filters.")]
    public async Task<IActionResult> SearchAsync(TicketListFilter filter)
    {
        var departments = await _service.SearchAsync(filter);
        return Ok(departments);
    }

    [ProducesResponseType(typeof(Result<TicketDto>), 200)]
    [ProducesResponseType(typeof(IDictionary<string, string>), 400)]
    [ProducesResponseType(500)]
    [HttpGet("{id}")]
    [SwaggerHeader("tenant", "Tickets", "View", "Input your tenant to access this API i.e. admin for test", "admin", true)]
    [MustHavePermission(PermissionConstants.Tickets.View)]
    public async Task<IActionResult> GetAsync(Guid id)
    {
        var department = await _service.GetTicketAsyncWithCommentAndReplies(id);
        return Ok(department);
    }

    [ProducesResponseType(typeof(Result<List<TicketDto>>), 200)]
    [ProducesResponseType(typeof(IDictionary<string, string>), 400)]
    [ProducesResponseType(500)]
    [HttpGet("getticketsbyclientid/{id}")]
    [SwaggerHeader("tenant", "Tickets", "View", "Input your tenant to access this API i.e. admin for test", "admin", true)]
    [MustHavePermission(PermissionConstants.Tickets.View)]
    public async Task<IActionResult> GetTicketsByClientIdAsync(Guid id)
    {
        var tickets = await _service.GetTicketByClientIdAsync(id);
        return Ok(tickets);
    }

    [ProducesResponseType(typeof(Result<List<TicketHistoryDto>>), 200)]
    [ProducesResponseType(typeof(IDictionary<string, string>), 400)]
    [ProducesResponseType(500)]
    [HttpGet("gettickethistory/{id}")]
    [SwaggerHeader("tenant", "Tickets", "View", "Input your tenant to access this API i.e. admin for test", "admin", true)]
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
    [SwaggerHeader("tenant", "Tickets", "View", "Input your tenant to access this API i.e. admin for test", "admin", true)]
    [MustHavePermission(PermissionConstants.Tickets.View)]
    public async Task<IActionResult> GetCurrentUserTicketsAsync()
    {
        var tickets = await _service.GetCurrentUserTicketsAsync();
        return Ok(tickets);
    }

    [ProducesResponseType(typeof(Result<Guid>), 200)]
    [ProducesResponseType(typeof(IDictionary<string, string>), 400)]
    [ProducesResponseType(500)]
    [HttpPost]
    [SwaggerHeader("tenant", "Tickets", "Create", "Input your tenant to access this API i.e. admin for test", "admin", true)]
    [MustHavePermission(PermissionConstants.Tickets.Create)]
    public async Task<IActionResult> CreateAsync(CreateTicketRequest request)
    {
        return Ok(await _service.CreateTicketAsync(request));
    }

    [ProducesResponseType(typeof(Result<Guid>), 200)]
    [ProducesResponseType(typeof(IDictionary<string, string>), 400)]
    [ProducesResponseType(500)]
    [HttpPut("{id}")]
    [SwaggerHeader("tenant", "Tickets", "Update", "Input your tenant to access this API i.e. admin for test", "admin", true)]
    [MustHavePermission(PermissionConstants.Tickets.Update)]
    public async Task<IActionResult> UpdateAsync(UpdateTicketRequest request, Guid id)
    {
        return Ok(await _service.UpdateTicketAsync(request, id));
    }

    [ProducesResponseType(typeof(Result<Guid>), 200)]
    [ProducesResponseType(typeof(IDictionary<string, string>), 400)]
    [ProducesResponseType(500)]
    [HttpDelete("{id}")]
    [SwaggerHeader("tenant", "Tickets", "Remove", "Input your tenant to access this API i.e. admin for test", "admin", true)]
    [MustHavePermission(PermissionConstants.Tickets.Remove)]
    public async Task<IActionResult> DeleteAsync(Guid id)
    {
        var departmentId = await _service.DeleteTicketAsync(id);
        return Ok(departmentId);
    }

    [ProducesResponseType(typeof(Result<Guid>), 200)]
    [ProducesResponseType(typeof(IDictionary<string, string>), 400)]
    [ProducesResponseType(500)]
    [HttpGet("support/duration")]
    [SwaggerHeader("tenant", "Tickets", "View", "Input your tenant to access this API i.e. admin for test", "admin", true)]
    [MustHavePermission(PermissionConstants.Tickets.Remove)]
    public async Task<IActionResult> GetSupportDurationAsync([FromQuery]SupportHistoryFilterDto filterDto)
    {
        var departmentId = await _service.GetSupportDurationAsync(filterDto);
        return Ok(departmentId);
    }

    [ProducesResponseType(typeof(Result<Guid>), 200)]
    [ProducesResponseType(typeof(IDictionary<string, string>), 400)]
    [ProducesResponseType(500)]
    [HttpGet("support/responsetime")]
    [SwaggerHeader("tenant", "Tickets", "View", "Input your tenant to access this API i.e. admin for test", "admin", true)]
    [MustHavePermission(PermissionConstants.Tickets.Remove)]
    public async Task<IActionResult> GetSupportResponseTimeAsync([FromQuery] SupportHistoryFilterDto filterDto)
    {
        var departmentId = await _service.GetResponseTimeTicketAsync(filterDto);
        return Ok(departmentId);
    }

    [ProducesResponseType(typeof(Result<Guid>), 200)]
    [ProducesResponseType(typeof(IDictionary<string, string>), 400)]
    [ProducesResponseType(500)]
    [HttpGet("support/replycount")]
    [SwaggerHeader("tenant", "Tickets", "View", "Input your tenant to access this API i.e. admin for test", "admin", true)]
    [MustHavePermission(PermissionConstants.Tickets.Remove)]
    public async Task<IActionResult> GetSupportReplyCountAsync([FromQuery] SupportHistoryFilterDto filterDto)
    {
        var departmentId = await _service.GetReplyTicketAsync(filterDto);
        return Ok(departmentId);
    }
}