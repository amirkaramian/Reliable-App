using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MyReliableSite.Application.ManageModule.Interfaces;
using MyReliableSite.Application.Wrapper;
using MyReliableSite.Domain.Constants;
using MyReliableSite.Infrastructure.Identity.Permissions;
using MyReliableSite.Infrastructure.Swagger;
using MyReliableSite.Shared.DTOs.ManageModule;
using Swashbuckle.AspNetCore.Annotations;

namespace MyReliableSite.Admin.API.Controllers.ManageModule;
[Route("api/[controller]")]
[ApiController]

public class UserModuleManagementController : BaseController
{
    private readonly IUserModuleManagementService _service;
    private readonly IConfiguration _config;
    public UserModuleManagementController(IUserModuleManagementService service, IConfiguration config)
    {
        _service = service;
        _config = config;
    }

    /// <summary>
    /// List of records with pagination &amp; filters.
    /// </summary>
    /// <response code="200">User Module Management List returns.</response>
    /// <response code="400">User Module Management not found.</response>
    /// <response code="500">Oops! Can't lookup your record right now.</response>
    [ProducesResponseType(typeof(PaginatedResult<UserModuleDto>), 200)]
    [ProducesResponseType(typeof(IDictionary<string, string>), 400)]
    [ProducesResponseType(500)]
    [HttpPost("search")]
    [SwaggerHeader("tenant", "ManageModule", "Search", "Input your tenant to access this API i.e. admin for test", "admin", true)]
    [MustHavePermission(PermissionConstants.ModuleManagements.Search)]
    [SwaggerOperation(Summary = "Search user ModuleManagements using available Filters.")]
    public async Task<IActionResult> SearchAsync(UserModuleManagementListFilter filter)
    {
        var moduleManagements = await _service.SearchAsync(filter);
        return Ok(moduleManagements);
    }

    /// <summary>
    /// retrive the User Module Management against specific id.
    /// </summary>
    /// <response code="200">User Module Management returns.</response>
    /// <response code="400">User Module Management not found.</response>
    /// <response code="500">Oops! Can't lookup your record right now.</response>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(Result<AdminGroupModuleDto>), 200)]
    [ProducesResponseType(typeof(IDictionary<string, string>), 400)]
    [ProducesResponseType(500)]
    [SwaggerHeader("tenant", "ManageModule", "Search", "Input your tenant to access this API i.e. admin for test", "admin", true)]
    [MustHavePermission(PermissionConstants.ModuleManagements.View)]
    public async Task<IActionResult> GetAsync(Guid id)
    {
        var moduleManagement = await _service.GetUserModuleManagementAsync(id);
        return Ok(moduleManagement);
    }

    /// <summary>
    /// retrive the User Module Management against specific user id.
    /// </summary>
    /// <response code="200">User Module Management returns.</response>
    /// <response code="400">User Module Management not found.</response>
    /// <response code="500">Oops! Can't lookup your record right now.</response>
    [ProducesResponseType(typeof(Result<List<AdminGroupModuleDto>>), 200)]
    [ProducesResponseType(typeof(IDictionary<string, string>), 400)]
    [ProducesResponseType(500)]
    [HttpGet("getmodulebyuser/{userid}")]
    [SwaggerHeader("tenant", "ManageModule", "Search", "Input your tenant to access this API i.e. admin for test", "admin", true)]
    [MustHavePermission(PermissionConstants.ModuleManagements.View)]
    public async Task<IActionResult> GetModuleByTenantAsync(string userid)
    {
        return Ok(await _service.GetUserModuleManagementByUserIdAsync(userid));
    }

    /// <summary>
    /// Create an User Module.
    /// </summary>
    /// <response code="200">User Module created.</response>
    /// <response code="400">User Module already exists.</response>
    /// <response code="500">Oops! Can't lookup your product right now.</response>
    [HttpPost]
    [ProducesResponseType(typeof(Result<Guid>), 200)]
    [ProducesResponseType(typeof(IDictionary<string, string>), 400)]
    [ProducesResponseType(500)]
    [SwaggerHeader("tenant", "ManageModule", "Create", "Input your tenant to access this API i.e. admin for test", "admin", true)]
    [MustHavePermission(PermissionConstants.ModuleManagements.Create)]
    public async Task<IActionResult> CreateAsync(CreateUserModuleManagementRequest request)
    {
        return Ok(await _service.CreateUserModuleManagementAsync(request));
    }

    /// <summary>
    /// update a specific User Module by unique id.
    /// </summary>
    /// <response code="200">User Module updated.</response>
    /// <response code="404">User Module not found.</response>
    /// <response code="500">Oops! Can't lookup your product right now.</response>
    [HttpPut("{id}")]
    [ProducesResponseType(typeof(Result<Guid>), 200)]
    [ProducesResponseType(404)]
    [ProducesResponseType(500)]
    [SwaggerHeader("tenant", "ManageModule", "Update", "Input your tenant to access this API i.e. admin for test", "admin", true)]
    [MustHavePermission(PermissionConstants.ModuleManagements.Update)]
    public async Task<IActionResult> UpdateAsync(UpdateUserModuleManagementRequest request, Guid id)
    {
        return Ok(await _service.UpdateUserModuleManagementAsync(request, id));
    }

    /// <summary>
    /// Delete a specific User Module by unique id.
    /// </summary>
    /// <response code="200">User Module deleted.</response>
    /// <response code="404">User Module not found.</response>
    /// <response code="500">Oops! Can't lookup your product right now.</response>
    [ProducesResponseType(typeof(Result<Guid>), 200)]
    [ProducesResponseType(404)]
    [ProducesResponseType(500)]
    [HttpDelete("{id}")]
    [SwaggerHeader("tenant", "ManageModule", "Remove", "Input your tenant to access this API i.e. admin for test", "admin", true)]
    [MustHavePermission(PermissionConstants.ModuleManagements.Remove)]
    public async Task<IActionResult> DeleteAsync(Guid id)
    {
        var moduleManagementId = await _service.DeleteUserModuleManagementAsync(id);
        return Ok(moduleManagementId);
    }
}
