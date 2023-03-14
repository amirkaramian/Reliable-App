﻿using Microsoft.AspNetCore.Mvc;
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
public class TicketCommentsController : BaseController
{
    private readonly ITicketCommentService _service;
    private readonly IConfiguration _config;
    public TicketCommentsController(ITicketCommentService service, IConfiguration config)
    {
        _service = service;
        _config = config;
    }

    [ProducesResponseType(typeof(PaginatedResult<TicketCommentDto>), 200)]
    [ProducesResponseType(typeof(IDictionary<string, string>), 400)]
    [ProducesResponseType(500)]
    [HttpPost("search")]
    [SwaggerHeader("tenant", "Tickets", "Search", "Input your tenant to access this API i.e. admin for test", "admin", true)]
    [MustHavePermission(PermissionConstants.Tickets.Search)]
    [SwaggerOperation(Summary = "Search TicketComments using available Filters.")]
    public async Task<IActionResult> SearchAsync(TicketCommentListFilter filter)
    {
        var departments = await _service.SearchAsync(filter);
        return Ok(departments);
    }

    [ProducesResponseType(typeof(Result<TicketCommentDto>), 200)]
    [ProducesResponseType(typeof(IDictionary<string, string>), 400)]
    [ProducesResponseType(500)]
    [HttpGet("{id}")]
    [SwaggerHeader("tenant", "Tickets", "View", "Input your tenant to access this API i.e. admin for test", "admin", true)]
    [MustHavePermission(PermissionConstants.Tickets.View)]
    public async Task<IActionResult> GetAsync(Guid id)
    {
        var department = await _service.GetTicketCommentAsync(id);
        return Ok(department);
    }

    [ProducesResponseType(typeof(Result<Guid>), 200)]
    [ProducesResponseType(typeof(IDictionary<string, string>), 400)]
    [ProducesResponseType(500)]
    [HttpPost]
    [SwaggerHeader("tenant", "Tickets", "Create", "Input your tenant to access this API i.e. admin for test", "admin", true)]
    [MustHavePermission(PermissionConstants.Tickets.Create)]
    public async Task<IActionResult> CreateAsync(CreateTicketCommentRequest request)
    {
        return Ok(await _service.CreateTicketCommentAsync(request));
    }

    [ProducesResponseType(typeof(Result<Guid>), 200)]
    [ProducesResponseType(typeof(IDictionary<string, string>), 400)]
    [ProducesResponseType(500)]
    [HttpPut("{id}")]
    [SwaggerHeader("tenant", "Tickets", "Update", "Input your tenant to access this API i.e. admin for test", "admin", true)]
    [MustHavePermission(PermissionConstants.Tickets.Update)]
    public async Task<IActionResult> UpdateAsync(UpdateTicketCommentRequest request, Guid id)
    {
        return Ok(await _service.UpdateTicketCommentAsync(request, id));
    }

    [ProducesResponseType(typeof(Result<Guid>), 200)]
    [ProducesResponseType(typeof(IDictionary<string, string>), 400)]
    [ProducesResponseType(500)]
    [HttpDelete("{id}")]
    [SwaggerHeader("tenant", "Tickets", "Remove", "Input your tenant to access this API i.e. admin for test", "admin", true)]
    [MustHavePermission(PermissionConstants.Tickets.Remove)]
    public async Task<IActionResult> DeleteAsync(Guid id)
    {
        var departmentId = await _service.DeleteTicketCommentAsync(id);
        return Ok(departmentId);
    }
}