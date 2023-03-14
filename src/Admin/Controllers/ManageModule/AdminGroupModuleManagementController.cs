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
public class AdminGroupModuleManagementController : BaseController
{
    private readonly IAdminGroupModuleManagementService _service;
    private readonly IConfiguration _config;
    public AdminGroupModuleManagementController(IAdminGroupModuleManagementService service, IConfiguration config)
    {
        _service = service;
        _config = config;
    }

    /// <summary>
    /// List of records with pagination &amp; filters.
    /// </summary>
    /// <response code="200">ManageModule List returns.</response>
    /// <response code="400">ManageModule not found.</response>
    /// <response code="500">Oops! Can't lookup your record right now.</response>
    [ProducesResponseType(typeof(PaginatedResult<AdminGroupModuleDto>), 200)]
    [ProducesResponseType(typeof(IDictionary<string, string>), 400)]
    [ProducesResponseType(500)]
    [HttpPost("search")]
    [SwaggerHeader("tenant", "ManageModule", "Search", "Input your tenant to access this API i.e. admin for test", "admin", true)]
    [MustHavePermission(PermissionConstants.ModuleManagements.Search)]
    [SwaggerOperation(Summary = "Search ModuleManagements using available Filters.")]
    public async Task<IActionResult> SearchAsync(AdminGroupModuleManagementListFilter filter)
    {
        var moduleManagements = await _service.SearchAsync(filter);
        return Ok(moduleManagements);
    }

    /// <summary>
    /// retrive the AdminGroupModuleManagement against specific id.
    /// </summary>
    /// <response code="200">Admin Group ModuleManagement returns.</response>
    /// <response code="400">Admin Group ModuleManagement not found.</response>
    /// <response code="500">Oops! Can't lookup your record right now.</response>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(Result<AdminGroupModuleDto>), 200)]
    [ProducesResponseType(typeof(IDictionary<string, string>), 400)]
    [ProducesResponseType(500)]
    [SwaggerHeader("tenant", "ManageModule", "View", "Input your tenant to access this API i.e. admin for test", "admin", true)]
    [MustHavePermission(PermissionConstants.ModuleManagements.View)]
    public async Task<IActionResult> GetAsync(Guid id)
    {
        var moduleManagement = await _service.GetAdminGroupModuleManagementAsync(id);
        return Ok(moduleManagement);
    }

    /// <summary>
    /// retrive the AdminGroupModuleManagement against specific admin group id.
    /// </summary>
    /// <response code="200">Admin Group ModuleManagement returns.</response>
    /// <response code="400">Admin Group ModuleManagement not found.</response>
    /// <response code="500">Oops! Can't lookup your record right now.</response>
    [ProducesResponseType(typeof(Result<List<AdminGroupModuleDto>>), 200)]
    [ProducesResponseType(typeof(IDictionary<string, string>), 400)]
    [ProducesResponseType(500)]
    [HttpGet("getmodulebyadmingroup/{admingroupid}")]
    [SwaggerHeader("tenant", "ManageModule", "Search", "Input your tenant to access this API i.e. admin for test", "admin", true)]
    [MustHavePermission(PermissionConstants.ModuleManagements.View)]
    public async Task<IActionResult> GetModuleByAdminGroupAsync(string admingroupid)
    {
        return Ok(await _service.GetAdminGroupModuleManagementByAdminGroupIdAsync(admingroupid));
    }

    /// <summary>
    /// Create an AdminGroup module.
    /// </summary>
    /// <response code="200">AdminGroup module created.</response>
    /// <response code="400">AdminGroup module already exists.</response>
    /// <response code="500">Oops! Can't lookup your product right now.</response>
    [HttpPost]
    [ProducesResponseType(typeof(Result<Guid>), 200)]
    [ProducesResponseType(typeof(IDictionary<string, string>), 400)]
    [ProducesResponseType(500)]
    [SwaggerHeader("tenant", "ManageModule", "Create", "Input your tenant to access this API i.e. admin for test", "admin", true)]
    [MustHavePermission(PermissionConstants.ModuleManagements.Create)]
    public async Task<IActionResult> CreateAsync(CreateAdminGroupModuleManagementRequest request)
    {
        return Ok(await _service.CreateAdminGroupModuleManagementAsync(request));
    }

    /// <summary>
    /// update a specific AdminGroup module by unique id.
    /// </summary>
    /// <response code="200">AdminGroup module updated.</response>
    /// <response code="404">AdminGroup module not found.</response>
    /// <response code="500">Oops! Can't lookup your product right now.</response>
    [HttpPut("{id}")]
    [ProducesResponseType(typeof(Result<Guid>), 200)]
    [ProducesResponseType(404)]
    [ProducesResponseType(500)]
    [SwaggerHeader("tenant", "ManageModule", "Update", "Input your tenant to access this API i.e. admin for test", "admin", true)]
    [MustHavePermission(PermissionConstants.ModuleManagements.Update)]
    public async Task<IActionResult> UpdateAsync(UpdateAdminGroupModuleManagementRequest request, Guid id)
    {
        return Ok(await _service.UpdateAdminGroupModuleManagementAsync(request, id));
    }

    /// <summary>
    /// Delete a specific AdminGroup module by unique id.
    /// </summary>
    /// <response code="200">AdminGroup module deleted.</response>
    /// <response code="404">AdminGroup module not found.</response>
    /// <response code="500">Oops! Can't lookup your product right now.</response>
    [HttpDelete("{id}")]
    [ProducesResponseType(typeof(Result<Guid>), 200)]
    [ProducesResponseType(404)]
    [ProducesResponseType(500)]
    [SwaggerHeader("tenant", "ManageModule", "Remove", "Input your tenant to access this API i.e. admin for test", "admin", true)]
    [MustHavePermission(PermissionConstants.ModuleManagements.Remove)]
    public async Task<IActionResult> DeleteAsync(Guid id)
    {
        var moduleManagementId = await _service.DeleteAdminGroupModuleManagementAsync(id);
        return Ok(moduleManagementId);
    }

    /// <summary>
    /// Delete a AdminGroup By AdminGroupId.
    /// </summary>
    /// <response code="200">AdminGroup deleted.</response>
    /// <response code="404">AdminGroup not found.</response>
    /// <response code="500">Oops! Can't lookup your product right now.</response>
    [ProducesResponseType(typeof(Result<Guid>), 200)]
    [ProducesResponseType(404)]
    [ProducesResponseType(500)]
    [HttpDelete("deleteAdminGroupByAdminGroupId/{id}")]
    [SwaggerHeader("tenant", "ManageModule", "Remove", "Input your tenant to access this API i.e. admin for test", "admin", true)]
    [MustHavePermission(PermissionConstants.ModuleManagements.Remove)]
    public async Task<IActionResult> DeletedminGroupByAdminGroupIdAsync(string id)
    {
        var moduleManagementId = await _service.DeleteAdminGroupByAdminGroupIdModuleManagementAsync(id);
        return Ok(moduleManagementId);
    }
}
