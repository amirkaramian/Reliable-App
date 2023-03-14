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
public class TicketCommentRepliesController : BaseController
{
    private readonly ITicketCommentReplyService _service;
    private readonly IConfiguration _config;
    public TicketCommentRepliesController(ITicketCommentReplyService service, IConfiguration config)
    {
        _service = service;
        _config = config;
    }

    [ProducesResponseType(typeof(Result<TicketCommentReplyDto>), 200)]
    [ProducesResponseType(typeof(IDictionary<string, string>), 400)]
    [ProducesResponseType(500)]
    [HttpGet("{id}")]
    [SwaggerHeader("tenant", "Tickets", "View", "Input your tenant to access this API i.e. admin for test", "admin", true)]
    [MustHavePermission(PermissionConstants.Tickets.View)]
    public async Task<IActionResult> GetAsync(Guid id)
    {
        var department = await _service.GetTicketCommentReplyAsync(id);
        return Ok(department);
    }

    [ProducesResponseType(typeof(Result<Guid>), 200)]
    [ProducesResponseType(typeof(IDictionary<string, string>), 400)]
    [ProducesResponseType(500)]
    [HttpPost]
    [SwaggerHeader("tenant", "Tickets", "Create", "Input your tenant to access this API i.e. admin for test", "admin", true)]
    [MustHavePermission(PermissionConstants.Tickets.Create)]
    public async Task<IActionResult> CreateAsync(CreateTicketCommentReplyRequest request)
    {
        return Ok(await _service.CreateTicketCommentReplyAsync(request));
    }

    [ProducesResponseType(typeof(Result<Guid>), 200)]
    [ProducesResponseType(typeof(IDictionary<string, string>), 400)]
    [ProducesResponseType(500)]
    [HttpPut("{id}")]
    [SwaggerHeader("tenant", "Tickets", "Update", "Input your tenant to access this API i.e. admin for test", "admin", true)]
    [MustHavePermission(PermissionConstants.Tickets.Update)]
    public async Task<IActionResult> UpdateAsync(UpdateTicketCommentReplyRequest request, Guid id)
    {
        return Ok(await _service.UpdateTicketCommentReplyAsync(request, id));
    }

    [ProducesResponseType(typeof(Result<Guid>), 200)]
    [ProducesResponseType(typeof(IDictionary<string, string>), 400)]
    [ProducesResponseType(500)]
    [HttpDelete("{id}")]
    [SwaggerHeader("tenant", "Tickets", "Remove", "Input your tenant to access this API i.e. admin for test", "admin", true)]
    [MustHavePermission(PermissionConstants.Tickets.Remove)]
    public async Task<IActionResult> DeleteAsync(Guid id)
    {
        var departmentId = await _service.DeleteTicketCommentReplyAsync(id);
        return Ok(departmentId);
    }
}