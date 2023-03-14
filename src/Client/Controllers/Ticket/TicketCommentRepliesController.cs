using Microsoft.AspNetCore.Mvc;
using MyReliableSite.Application.Tickets.Interfaces;
using MyReliableSite.Application.Wrapper;
using MyReliableSite.Domain.Constants;
using MyReliableSite.Infrastructure.Identity.Permissions;
using MyReliableSite.Infrastructure.Swagger;
using MyReliableSite.Shared.DTOs.Tickets;
using Swashbuckle.AspNetCore.Annotations;

namespace MyReliableSite.Client.API.Controllers.Tickets;
[Route("api/[controller]")]
[ApiController]
public class TicketCommentRepliesController : BaseController
{
    private readonly ITicketCommentReplyService _service;
    private readonly ITicketCommentService _commentservice;
    private readonly IConfiguration _config;
    public TicketCommentRepliesController(ITicketCommentReplyService service, ITicketCommentService commentservice, IConfiguration config)
    {
        _service = service;
        _commentservice = commentservice;
        _config = config;
    }

    [ProducesResponseType(typeof(Result<TicketCommentReplyDto>), 200)]
    [ProducesResponseType(typeof(IDictionary<string, string>), 400)]
    [ProducesResponseType(500)]
    [HttpGet("{id}")]
    [SwaggerHeader("tenant", "Tickets", "View", "Input your tenant to access this API i.e. client for test", "client", true)]
    [MustHavePermission(PermissionConstants.Tickets.View)]
    public async Task<IActionResult> GetAsync(Guid id)
    {
        var department = await _service.GetTicketCommentReplyAsync(id);
        return Ok(department);
    }

    [ProducesResponseType(typeof(Result<Guid>), 200)]
    [ProducesResponseType(typeof(IDictionary<string, string>), 400)]
    [ProducesResponseType(500)]
    [HttpPost("createclientcommentreply")]
    [SwaggerHeader("tenant", "Tickets", "Create", "Input your tenant to access this API i.e. client for test", "client", true)]
    [MustHavePermission(PermissionConstants.Tickets.Create)]
    public async Task<IActionResult> CreateAsync(CreateTicketCommentReplyRequest request)
    {
        return Ok(await _service.CreateTicketCommentReplyAsync(request));
    }

    [ProducesResponseType(typeof(Result<Guid>), 200)]
    [ProducesResponseType(typeof(IDictionary<string, string>), 400)]
    [ProducesResponseType(500)]
    [HttpPost("createClientComment")]
    [SwaggerHeader("tenant", "Tickets", "Create", "Input your tenant to access this API i.e. client for test", "client", true)]
    [MustHavePermission(PermissionConstants.Tickets.Create)]
    public async Task<IActionResult> CreateAsync(CreateTicketCommentRequest request)
    {
        return Ok(await _commentservice.CreateTicketCommentAsync(request));
    }

    [ProducesResponseType(typeof(Result<Guid>), 200)]
    [ProducesResponseType(typeof(IDictionary<string, string>), 400)]
    [ProducesResponseType(500)]
    [HttpDelete("deleteclientcomment/{id}")]
    [SwaggerHeader("tenant", "Tickets", "Remove", "Input your tenant to access this API i.e. admin for test", "admin", true)]
    [MustHavePermission(PermissionConstants.Tickets.Remove)]
    public async Task<IActionResult> DeleteAsync(Guid id)
    {
        var departmentId = await _commentservice.DeleteTicketCommentAsync(id);
        return Ok(departmentId);
    }

    [ProducesResponseType(typeof(Result<Guid>), 200)]
    [ProducesResponseType(typeof(IDictionary<string, string>), 400)]
    [ProducesResponseType(500)]
    [HttpDelete("deleteclientcommentreply/{id}")]
    [SwaggerHeader("tenant", "Tickets", "Remove", "Input your tenant to access this API i.e. admin for test", "admin", true)]
    [MustHavePermission(PermissionConstants.Tickets.Remove)]
    public async Task<IActionResult> DeleteClientCommentReplyAsync(Guid id)
    {
        var departmentId = await _commentservice.DeleteClientCommentReplyAsync(id);
        return Ok(departmentId);
    }
}