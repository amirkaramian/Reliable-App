using Microsoft.AspNetCore.Mvc;
using MyReliableSite.Application.Abstractions.Services.Identity;
using MyReliableSite.Application.Wrapper;
using MyReliableSite.Domain.Constants;
using MyReliableSite.Infrastructure.Identity.Permissions;
using MyReliableSite.Infrastructure.Swagger;
using MyReliableSite.Shared.DTOs.Identity;

namespace MyReliableSite.Admin.API.Controllers.Identity;

[ApiController]
[Route("api/[controller]")]
public class RolesController : ControllerBase
{
    private readonly IRoleService _roleService;

    public RolesController(IRoleService roleService)
    {
        _roleService = roleService;
    }

    /// <summary>
    /// List of records with pagination &amp; filters.
    /// </summary>
    /// <response code="200">Roles List returns.</response>
    /// <response code="400">Role not found.</response>
    /// <response code="500">Oops! Can't lookup your record right now.</response>
    [ProducesResponseType(typeof(PaginatedResult<List<RoleDto>>), 200)]
    [ProducesResponseType(typeof(IDictionary<string, string>), 400)]
    [ProducesResponseType(500)]
    [HttpGet("all")]
    [SwaggerHeader("tenant", "Roles", "Search", "Input your tenant to access this API i.e. admin for test", "admin", true)]
    [MustHavePermission(PermissionConstants.Roles.ListAll)]
    public async Task<IActionResult> GetListAsync()
    {
        var roles = await _roleService.GetListAsync();
        return Ok(roles);
    }

    /// <summary>
    /// retrive the Role against specific id.
    /// </summary>
    /// <response code="200">Role returns.</response>
    /// <response code="400">Role not found.</response>
    /// <response code="500">Oops! Can't lookup your record right now.</response>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(Result<RoleDto>), 200)]
    [ProducesResponseType(typeof(IDictionary<string, string>), 400)]
    [ProducesResponseType(500)]
    [SwaggerHeader("tenant", "Roles", "Search", "Input your tenant to access this API i.e. admin for test", "admin", true)]
    [MustHavePermission(PermissionConstants.Roles.View)]
    public async Task<IActionResult> GetByIdAsync(string id)
    {
        var roles = await _roleService.GetByIdAsync(id);
        return Ok(roles);
    }

    /// <summary>
    /// retrive the Role's permissions against specific id.
    /// </summary>
    /// <response code="200">Role's permissions returns.</response>
    /// <response code="400">Role not found.</response>
    /// <response code="500">Oops! Can't lookup your record right now.</response>
    [ProducesResponseType(typeof(Result<List<PermissionDto>>), 200)]
    [ProducesResponseType(typeof(IDictionary<string, string>), 400)]
    [ProducesResponseType(500)]
    [HttpGet("{id}/permissions")]
    [SwaggerHeader("tenant", "Roles", "Search", "Input your tenant to access this API i.e. admin for test", "admin", true)]
    public async Task<IActionResult> GetPermissionsAsync(string id)
    {
        var roles = await _roleService.GetPermissionsAsync(id);
        return Ok(roles);
    }

    /// <summary>
    /// update a specific Role's permissions by unique id.
    /// </summary>
    /// <response code="200">Role's permissions updated.</response>
    /// <response code="404">Role not found.</response>
    /// <response code="500">Oops! Can't lookup your product right now.</response>
    [HttpPut("{id}")]
    [ProducesResponseType(typeof(Result<string>), 200)]
    [ProducesResponseType(404)]
    [ProducesResponseType(500)]
    [HttpPut("{id}/permissions")]
    [SwaggerHeader("tenant", "Roles", "Search", "Input your tenant to access this API i.e. admin for test", "admin", true)]
    public async Task<IActionResult> UpdatePermissionsAsync(string id, List<UpdatePermissionsRequest> request)
    {
        var roles = await _roleService.UpdatePermissionsAsync(id, request);
        return Ok(roles);
    }

    /// <summary>
    /// Create an Role.
    /// </summary>
    /// <response code="200">Role created.</response>
    /// <response code="400">Role already exists.</response>
    /// <response code="500">Oops! Can't lookup your product right now.</response>
    [HttpPost]
    [ProducesResponseType(typeof(Result<string>), 200)]
    [ProducesResponseType(typeof(IDictionary<string, string>), 400)]
    [ProducesResponseType(500)]
    [HttpPost]
    [SwaggerHeader("tenant", "Roles", "Register", "Input your tenant to access this API i.e. admin for test", "admin", true)]
    [MustHavePermission(PermissionConstants.Roles.Register)]
    public async Task<IActionResult> RegisterRoleAsync(RoleRequest request)
    {
        var response = await _roleService.RegisterRoleAsync(request);
        return Ok(response);
    }

    /// <summary>
    /// Delete a specific Role by unique id.
    /// </summary>
    /// <response code="200">Role deleted.</response>
    /// <response code="404">Role not found.</response>
    /// <response code="500">Oops! Can't lookup your product right now.</response>
    [ProducesResponseType(typeof(Result<string>), 200)]
    [ProducesResponseType(404)]
    [ProducesResponseType(500)]
    [HttpDelete("{id}")]
    [SwaggerHeader("tenant", "Roles", "Search", "Input your tenant to access this API i.e. admin for test", "admin", true)]
    [MustHavePermission(PermissionConstants.Roles.Remove)]
    public async Task<IActionResult> DeleteAsync(string id)
    {
        var response = await _roleService.DeleteAsync(id);
        return Ok(response);
    }
}
