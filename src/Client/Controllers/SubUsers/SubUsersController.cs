using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MyReliableSite.Application.Abstractions.Services.Identity;
using MyReliableSite.Application.ManageModule.Interfaces;
using MyReliableSite.Application.Wrapper;
using MyReliableSite.Domain.Constants;
using MyReliableSite.Infrastructure.Identity.Permissions;
using MyReliableSite.Infrastructure.Identity.Services;
using MyReliableSite.Infrastructure.Swagger;
using MyReliableSite.Shared.DTOs.Identity;
using MyReliableSite.Shared.DTOs.ManageModule;
using MyReliableSite.Shared.DTOs.ManageUserApiKey;
using Swashbuckle.AspNetCore.Annotations;

namespace MyReliableSite.Client.API.Controllers.SubUsers;

[ApiController]
[Route("api/[controller]")]
public class SubUsersController : BaseController
{
    private readonly IUserService _userService;
    private readonly IUserModuleManagementService _service;

    public SubUsersController(IUserService userService, IUserModuleManagementService service)
    {
        _userService = userService;
        _service = service;
    }

    /// <summary>
    /// Add new sub user.
    /// </summary>
    /// <response code="200">Create subuser success.</response>
    /// <response code="500">Oops! Can't lookup your record right now.</response>
    [ProducesResponseType(200)]
    [ProducesResponseType(500)]
    [HttpPost("add-new-subuser")]
    [AllowAnonymous]
    [SwaggerHeader("tenant", "SubUsers", "Create", "Input your tenant to access this API i.e. admin for test", "admin", true, true)]
    [MustHavePermission(PermissionConstants.SubUsers.Create)]
    public async Task<IActionResult> AddNewSubUserAsync(CreateSubUserRequest request)
    {
        string origin = GenerateOrigin();
        return Ok(await _userService.CreateSubUserAsync(request, origin));
    }

    /// <summary>
    /// Update sub user.
    /// </summary>
    /// <response code="200">Create subuser success.</response>
    /// <response code="500">Oops! Can't lookup your record right now.</response>
    [ProducesResponseType(200)]
    [ProducesResponseType(500)]
    [HttpPost("update-subuser/{subUserId}")]
    [AllowAnonymous]
    [SwaggerHeader("tenant", "SubUsers", "Update", "Input your tenant to access this API i.e. admin for test", "admin", true, true)]
    [MustHavePermission(PermissionConstants.SubUsers.Update)]
    public async Task<IActionResult> UpdateSubUserAsync(CreateSubUserRequest request, string subUserId)
    {
        string origin = GenerateOrigin();
        return Ok(await _userService.UpdateSubUserAsync(request, subUserId, origin));
    }

    /// <summary>
    /// List of records with pagination.
    /// </summary>
    /// <response code="200">SubUsers List returns.</response>
    /// <response code="400">SubUsers not found.</response>
    /// <response code="500">Oops! Can't lookup your record right now.</response>
    [ProducesResponseType(typeof(Result<List<UserDetailsDto>>), 200)]
    [ProducesResponseType(typeof(IDictionary<string, string>), 400)]
    [ProducesResponseType(500)]
    [HttpGet("GetAllSubUsersByClientIDAsync")]
    [AllowAnonymous]
    [SwaggerHeader("tenant", "SubUsers", "View", "Input your tenant to access this API i.e. admin for test", "admin", true, true)]
    [MustHavePermission(PermissionConstants.SubUsers.ListAll)]
    public async Task<IActionResult> GetAllSubUsersByClientIDAsync(string clientId)
    {
        var users = await _userService.GetAllSubUsersByClientIDAsync(clientId);
        return Ok(users);
    }

    /// <summary>
    /// List of records with pagination.
    /// </summary>
    /// <response code="200">SubUsers List returns.</response>
    /// <response code="400">SubUsers not found.</response>
    /// <response code="500">Oops! Can't lookup your record right now.</response>
    [ProducesResponseType(typeof(Result<List<UserDetailsDto>>), 200)]
    [ProducesResponseType(typeof(IDictionary<string, string>), 400)]
    [ProducesResponseType(500)]
    [HttpGet("get-all-subusers")]
    [AllowAnonymous]
    [SwaggerHeader("tenant", "SubUsers", "View", "Input your tenant to access this API i.e. admin for test", "admin", true, true)]
    [MustHavePermission(PermissionConstants.SubUsers.ListAll)]
    public async Task<IActionResult> GetAllAsync()
    {
        var users = await _userService.GetAllSubUsersAsync();
        return Ok(users);
    }

    /// <summary>
    /// retrive the SubUser against specific id.
    /// </summary>
    /// <response code="200">SubUser returns.</response>
    /// <response code="400">SubUser not found.</response>
    /// <response code="500">Oops! Can't lookup your record right now.</response>
    [HttpGet("id")]
    [ProducesResponseType(typeof(Result<UserDetailsDto>), 200)]
    [ProducesResponseType(typeof(IDictionary<string, string>), 400)]
    [ProducesResponseType(500)]
    [SwaggerHeader("tenant", "SubUsers", "View", "Input your tenant to access this API i.e. admin for test", "admin", true, true)]
    [MustHavePermission(PermissionConstants.SubUsers.View)]
    [SwaggerOperation(Summary = "Retrive the SubUser against specific id.")]
    public async Task<IActionResult> GetByIdAsync(string id)
    {
        var user = await _userService.GetSubUserAsync(id);
        return Ok(user);
    }

    /// <summary>
    /// Retrive the roles of a subuser against specific id.
    /// </summary>
    /// <response code="200">SubUser returns.</response>
    /// <response code="400">SubUser not found.</response>
    /// <response code="500">Oops! Can't lookup your record right now.</response>
    [HttpGet("{id}/roles")]
    [ProducesResponseType(typeof(PaginatedResult<UserRolesResponse>), 200)]
    [ProducesResponseType(typeof(IDictionary<string, string>), 400)]
    [ProducesResponseType(500)]
    [SwaggerHeader("tenant", "SubUsers", "View", "Input your tenant to access this API i.e. admin for test", "admin", true, true)]
    [MustHavePermission(PermissionConstants.SubUsers.View)]
    [SwaggerOperation(Summary = "Retrive the roles of a subuser against specific id.")]
    public async Task<IActionResult> GetRolesAsync(string id)
    {
        var userRoles = await _userService.GetSubUserRolesAsync(id);
        return Ok(userRoles);
    }

    /// <summary>
    /// Retrive the permissions of a subuser against specific id.
    /// </summary>
    /// <response code="200">SubUser returns.</response>
    /// <response code="400">SubUser not found.</response>
    /// <response code="500">Oops! Can't lookup your record right now.</response>
    [HttpGet("{id}/permissions")]
    [ProducesResponseType(typeof(PaginatedResult<List<PermissionDto>>), 200)]
    [ProducesResponseType(typeof(IDictionary<string, string>), 400)]
    [ProducesResponseType(500)]
    [SwaggerHeader("tenant", "SubUsers", "View", "Input your tenant to access this API i.e. admin for test", "admin", true, true)]
    [MustHavePermission(PermissionConstants.SubUsers.View)]
    [SwaggerOperation(Summary = "Retrive the permissions of a subuser against specific id.")]
    public async Task<IActionResult> GetPermissionsAsync(string id)
    {
        var userPermissions = await _userService.GetSubUserPermissionsAsync(id);
        return Ok(userPermissions);
    }

    /// <summary>
    /// Assign roles to a specific sub user id.
    /// </summary>
    /// <response code="200">SubUser updated.</response>
    /// <response code="404">SubUser not found.</response>
    /// <response code="500">Oops! Can't lookup your product right now.</response>
    [HttpPost("{id}/roles")]
    [ProducesResponseType(typeof(Result<string>), 200)]
    [ProducesResponseType(typeof(IDictionary<string, string>), 400)]
    [ProducesResponseType(500)]
    [SwaggerHeader("tenant", "SubUsers", "Update", "Input your tenant to access this API i.e. admin for test", "admin", true, true)]
    [MustHavePermission(PermissionConstants.SubUsers.Update)]
    [SwaggerOperation(Summary = "Assign roles to a specific sub user id.")]
    public async Task<IActionResult> AssignRolesAsync(string id, UserRolesRequest request)
    {
        var result = await _userService.AssignSubUserRolesAsync(id, request);
        return Ok(result);
    }

    /// <summary>
    /// Delete a specific SubUser by unique id.
    /// </summary>
    /// <response code="200">SubUser deleted.</response>
    /// <response code="404">SubUser not found.</response>
    /// <response code="500">Oops! Can't lookup your product right now.</response>
    [HttpDelete("{id}/Delete")]
    [ProducesResponseType(typeof(Result<Guid>), 200)]
    [ProducesResponseType(404)]
    [ProducesResponseType(500)]
    [SwaggerHeader("tenant", "SubUsers", "Remove", "Input your tenant to access this API i.e. admin for test", "admin", true)]
    [MustHavePermission(PermissionConstants.SubUsers.Remove)]
    public async Task<IActionResult> DeleteAsync(Guid id)
    {
        return Ok(await _userService.DeleteUserAsync(id));
    }

    /// <summary>
    /// Deactivate a specific SubUser by unique id.
    /// </summary>
    /// <response code="200">SubUser Deactivated.</response>
    /// <response code="404">SubUser not found.</response>
    /// <response code="500">Oops! Can't lookup your product right now.</response>
    [HttpPut("{id}/Deactivate")]
    [ProducesResponseType(typeof(Result<Guid>), 200)]
    [ProducesResponseType(404)]
    [ProducesResponseType(500)]
    [SwaggerHeader("tenant", "SubUsers", "Update", "Input your tenant to access this API i.e. admin for test", "admin", true)]
    [MustHavePermission(PermissionConstants.SubUsers.Update)]
    public async Task<IActionResult> DeactiveAsync(string id)
    {
        return Ok(await _userService.DeactiveUserAsync(id));
    }

    /// <summary>
    /// Activate a specific SubUser by unique id.
    /// </summary>
    /// <response code="200">SubUser Activated.</response>
    /// <response code="404">SubUser not found.</response>
    /// <response code="500">Oops! Can't lookup your product right now.</response>
    [HttpPut("{id}/Activate")]
    [ProducesResponseType(typeof(Result<Guid>), 200)]
    [ProducesResponseType(404)]
    [ProducesResponseType(500)]
    [SwaggerHeader("tenant", "SubUsers", "Update", "Input your tenant to access this API i.e. admin for test", "admin", true)]
    [MustHavePermission(PermissionConstants.SubUsers.Update)]
    public async Task<IActionResult> ActiveAsync(string id)
    {
        return Ok(await _userService.ActiveUserAsync(id));
    }

    /// <summary>
    /// Create an Sub User Module.
    /// </summary>
    /// <response code="200">Sub User Module created.</response>
    /// <response code="400">Sub User Module already exists.</response>
    [HttpPost("subusermodule")]
    [ProducesResponseType(typeof(Result<Guid>), 200)]
    [ProducesResponseType(typeof(IDictionary<string, string>), 400)]
    [ProducesResponseType(500)]
    [SwaggerHeader("tenant", "SubUsers", "Update", "Input your tenant to access this API i.e. admin for test", "admin", true)]
    [MustHavePermission(PermissionConstants.SubUsers.Update)]
    public async Task<IActionResult> CreateSubUserModuleManagementAsync(CreateUserModuleManagementRequest request)
    {
        return Ok(await _service.CreateUserModuleManagementAsync(request));
    }

    /// <summary>
    /// update a specific Sub User Module by sub user id.
    /// </summary>
    /// <response code="200">SubUser Module updated.</response>
    /// <response code="404">SubUser Module not found.</response>
    /// <response code="500">Oops! Can't lookup your product right now.</response>
    [HttpPut("subusermodule")]
    [ProducesResponseType(typeof(Result<Guid>), 200)]
    [ProducesResponseType(404)]
    [ProducesResponseType(500)]
    [SwaggerHeader("tenant", "SubUsers", "Update", "Input your tenant to access this API i.e. admin for test", "admin", true)]
    [MustHavePermission(PermissionConstants.SubUsers.Update)]
    public async Task<IActionResult> UpdateSubUserModuleManagementAsync(UpdateUserModuleManagementRequest request)
    {
        return Ok(await _service.UpdateSubUserModuleManagementAsync(request));
    }

    /// <summary>
    /// update a specific Sub User Module by sub user id.
    /// </summary>
    /// <response code="200">SubUser Module updated.</response>
    /// <response code="404">SubUser Module not found.</response>
    /// <response code="500">Oops! Can't lookup your product right now.</response>
    [HttpPut("subusermodulelist")]
    [ProducesResponseType(typeof(Result<Guid>), 200)]
    [ProducesResponseType(404)]
    [ProducesResponseType(500)]
    [SwaggerHeader("tenant", "SubUsers", "Update", "Input your tenant to access this API i.e. admin for test", "admin", true)]
    [MustHavePermission(PermissionConstants.SubUsers.Update)]
    public async Task<IActionResult> UpdateSubUserModuleListAsync(UpdateSubUserModuleListRequest request)
    {
        return Ok(await _service.UpdateSubUserModuleListAsync(request));
    }

    private string GenerateOrigin()
    {
        string baseUrl = $"{this.Request.Scheme}://{this.Request.Host.Value}{this.Request.PathBase.Value}";
        string origin = string.IsNullOrEmpty(Request.Headers["origin"].ToString()) ? baseUrl : Request.Headers["origin"].ToString();
        return origin;
    }

}
