using Microsoft.AspNetCore.Mvc;
using MyReliableSite.Application.Common.Interfaces;
using MyReliableSite.Application.Wrapper;
using MyReliableSite.Domain.Constants;
using MyReliableSite.Infrastructure.Identity.Permissions;
using MyReliableSite.Infrastructure.Swagger;
using MyReliableSite.Shared.DTOs.EmailTemplates;
using MyReliableSite.Shared.DTOs.Filters;

namespace MyReliableSite.Admin.API.Controllers.EmailTemplates;

public class TemplateVariablesController : BaseController
{
    private readonly ITemplateVariableService _templateVariableService;

    public TemplateVariablesController(ITemplateVariableService templateVariableService)
    {
        _templateVariableService = templateVariableService ?? throw new ArgumentNullException(nameof(templateVariableService));
    }

    /// <summary>
    /// retrive the TemplateVariable against specific id.
    /// </summary>
    /// <response code="200">TemplateVariable returns.</response>
    /// <response code="400">TemplateVariable not found.</response>
    /// <response code="500">Oops! Can't lookup your record right now.</response>
    [ProducesResponseType(typeof(Result<TemplateVariableDto>), 200)]
    [ProducesResponseType(typeof(IDictionary<string, string>), 400)]
    [ProducesResponseType(500)]
    [HttpGet("{id:guid}")]
    [MustHavePermission(PermissionConstants.TemplateVariables.Read)]
    [SwaggerHeader("tenant", "TemplateVariables", "Read", "Input your tenant to access this API i.e. admin for test", "admin", true)]
    public async Task<IActionResult> GetAsync(Guid id) => Ok(await _templateVariableService.GetAsync(id));

    /// <summary>
    /// List of records with pagination &amp; filters.
    /// </summary>
    /// <response code="200">TemplateVariables List returns.</response>
    /// <response code="400">TemplateVariable not found.</response>
    /// <response code="500">Oops! Can't lookup your record right now.</response>
    [ProducesResponseType(typeof(PaginatedResult<TemplateVariableDto>), 200)]
    [ProducesResponseType(typeof(IDictionary<string, string>), 400)]
    [ProducesResponseType(500)]
    [HttpPost("search")]
    [MustHavePermission(PermissionConstants.TemplateVariables.Search)]
    [SwaggerHeader("tenant", "TemplateVariables", "Search", "Input your tenant to access this API i.e. admin for test", "admin", true)]
    public async Task<IActionResult> SearchAsync(PaginationFilter filter)
    {
        return Ok(await _templateVariableService.SearchAsync(filter));
    }

    /// <summary>
    /// Create an TemplateVariable.
    /// </summary>
    /// <response code="200">TemplateVariable created.</response>
    /// <response code="400">TemplateVariable already exists.</response>
    /// <response code="500">Oops! Can't lookup your product right now.</response>
    [ProducesResponseType(typeof(Result<TemplateVariableDto>), 200)]
    [ProducesResponseType(typeof(IDictionary<string, string>), 400)]
    [ProducesResponseType(500)]
    [HttpPost]
    [MustHavePermission(PermissionConstants.TemplateVariables.Create)]
    [SwaggerHeader("tenant", "TemplateVariables", "Create", "Input your tenant to access this API i.e. admin for test", "admin", true)]
    public async Task<IActionResult> CreateAsync(CreateTemplateVariableRequest request)
    {
        return Ok(await _templateVariableService.CreateAsync(request));
    }

    /// <summary>
    /// update a specific TemplateVariable by unique id.
    /// </summary>
    /// <response code="200">TemplateVariable updated.</response>
    /// <response code="404">TemplateVariable not found.</response>
    /// <response code="500">Oops! Can't lookup your product right now.</response>
    [ProducesResponseType(typeof(Result<TemplateVariableDto>), 200)]
    [ProducesResponseType(404)]
    [ProducesResponseType(500)]
    [HttpPut("{id:guid}")]
    [MustHavePermission(PermissionConstants.TemplateVariables.Update)]
    [SwaggerHeader("tenant", "TemplateVariables", "Update", "Input your tenant to access this API i.e. admin for test", "admin", true)]
    public async Task<IActionResult> UpdateAsync(UpdateTemplateVariableRequest request, Guid id)
    {
        return Ok(await _templateVariableService.UpdateAsync(id, request));
    }

    /// <summary>
    /// Delete a specific TemplateVariable by unique id.
    /// </summary>
    /// <response code="200">TemplateVariable deleted.</response>
    /// <response code="404">TemplateVariable not found.</response>
    /// <response code="500">Oops! Can't lookup your product right now.</response>
    [ProducesResponseType(typeof(Result<bool>), 200)]
    [ProducesResponseType(404)]
    [ProducesResponseType(500)]
    [SwaggerHeader("tenant", "TemplateVariables", "Remove", "Input your tenant to access this API i.e. admin for test", "", true)]
    [HttpDelete("{id:guid}")]
    [MustHavePermission(PermissionConstants.TemplateVariables.Delete)]
    public async Task<IActionResult> DeleteAsync(Guid id) => Ok(await _templateVariableService.DeleteAsync(id));
}
