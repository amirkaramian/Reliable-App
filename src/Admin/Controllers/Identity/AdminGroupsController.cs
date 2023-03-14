using Microsoft.AspNetCore.Mvc;
using MyReliableSite.Application.Identity.Interfaces;
using MyReliableSite.Application.Wrapper;
using MyReliableSite.Domain.Constants;
using MyReliableSite.Infrastructure.Identity.Permissions;
using MyReliableSite.Infrastructure.Swagger;
using MyReliableSite.Shared.DTOs.Identity;
using Swashbuckle.AspNetCore.Annotations;

namespace MyReliableSite.Admin.API.Controllers.Identity;

[Route("api/[controller]")]
[ApiController]
public class AdminGroupsController : BaseController
{
    private readonly IAdminGroupService _service;
    private readonly IConfiguration _config;
    public AdminGroupsController(IAdminGroupService service, IConfiguration config)
    {
        _service = service;
        _config = config;
    }

    /// <summary>
    /// List of records with pagination &amp; filters.
    /// </summary>
    /// <response code="200">Identity List returns.</response>
    /// <response code="500">Oops! Can't lookup your record right now.</response>
    [HttpPost("search")]
    [ProducesResponseType(typeof(PaginatedResult<AdminGroupDto>), 200)]
    [ProducesResponseType(500)]
    [SwaggerHeader("tenant", "Identity", "Search", "Input your tenant to access this API i.e. admin for test", "admin", true)]
    [MustHavePermission(PermissionConstants.AdminGroups.Search)]
    [SwaggerOperation(Summary = "Search AdminGroups using available Filters.")]
    public async Task<IActionResult> SearchAsync(AdminGroupListFilter filter)
    {
        var adminGroups = await _service.SearchAsync(filter);
        return Ok(adminGroups);
    }

    /// <summary>
    /// retrive the Identity against specific id.
    /// </summary>
    /// <response code="200">Identity returns.</response>
    /// <response code="400">Identity not found.</response>
    /// <response code="500">Oops! Can't lookup your record right now.</response>
    [ProducesResponseType(typeof(Result<AdminGroupDto>), 200)]
    [ProducesResponseType(typeof(IDictionary<string, string>), 400)]
    [ProducesResponseType(500)]
    [HttpGet("{id}")]
    [SwaggerHeader("tenant", "Identity", "View", "Input your tenant to access this API i.e. admin for test", "admin", true)]
    [MustHavePermission(PermissionConstants.AdminGroups.View)]
    public async Task<IActionResult> GetAsync(Guid id)
    {
        var adminGroup = await _service.GetAdminGroupAsync(id);
        return Ok(adminGroup);
    }

    /// <summary>
    /// Create an Identity.
    /// </summary>
    /// <response code="200">Identity created.</response>
    /// <response code="400">Identity already exists.</response>
    /// <response code="500">Oops! Can't lookup your product right now.</response>
    [ProducesResponseType(typeof(Result<Guid>), 200)]
    [ProducesResponseType(typeof(IDictionary<string, string>), 400)]
    [ProducesResponseType(500)]
    [HttpPost]
    [SwaggerHeader("tenant", "Identity", "Create", "Input your tenant to access this API i.e. admin for test", "admin", true)]
    [MustHavePermission(PermissionConstants.AdminGroups.Create)]
    public async Task<IActionResult> CreateAsync(CreateAdminGroupRequest request)
    {
        return Ok(await _service.CreateAdminGroupAsync(request));
    }

    /// <summary>
    /// update a specific Identity by unique id.
    /// </summary>
    /// <response code="200">Identity updated.</response>
    /// <response code="404">Identity not found.</response>
    /// <response code="500">Oops! Can't lookup your product right now.</response>
    [ProducesResponseType(typeof(Result<Guid>), 200)]
    [ProducesResponseType(404)]
    [ProducesResponseType(500)]
    [HttpPut("{id}")]
    [SwaggerHeader("tenant", "Identity", "Update", "Input your tenant to access this API i.e. admin for test", "admin", true)]
    [MustHavePermission(PermissionConstants.AdminGroups.Update)]
    public async Task<IActionResult> UpdateAsync(UpdateAdminGroupRequest request, Guid id)
    {
        return Ok(await _service.UpdateAdminGroupAsync(request, id));
    }

    /// <summary>
    /// Delete a specific Identity by unique id.
    /// </summary>
    /// <response code="200">Identity deleted.</response>
    /// <response code="404">Identity not found.</response>
    /// <response code="500">Oops! Can't lookup your product right now.</response>
    [HttpDelete("{id}")]
    [ProducesResponseType(typeof(Result<Guid>), 200)]
    [ProducesResponseType(404)]
    [ProducesResponseType(500)]
    [SwaggerHeader("tenant", "Identity", "Remove", "Input your tenant to access this API i.e. admin for test", "admin", true)]
    [MustHavePermission(PermissionConstants.AdminGroups.Remove)]
    public async Task<IActionResult> DeleteAsync(Guid id)
    {
        var adminGroupId = await _service.DeleteAdminGroupAsync(id);
        return Ok(adminGroupId);
    }
}
