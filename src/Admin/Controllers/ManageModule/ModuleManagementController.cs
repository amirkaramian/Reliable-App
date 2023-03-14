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
public class ModuleManagementController : BaseController
{
    private readonly IModuleManagementService _service;
    private readonly IConfiguration _config;
    public ModuleManagementController(IModuleManagementService service, IConfiguration config)
    {
        _service = service;
        _config = config;
    }

    /// <summary>
    /// List of records with pagination &amp; filters.
    /// </summary>
    /// <response code="200">Module List returns.</response>
    /// <response code="400">Module not found.</response>
    /// <response code="500">Oops! Can't lookup your record right now.</response>
    [ProducesResponseType(typeof(PaginatedResult<ModuleDto>), 200)]
    [ProducesResponseType(typeof(IDictionary<string, string>), 400)]
    [ProducesResponseType(500)]
    [HttpPost("search")]
    [SwaggerHeader("tenant", "ManageModule", "Search", "Input your tenant to access this API i.e. admin for test", "admin", true)]
    [MustHavePermission(PermissionConstants.ModuleManagements.Search)]
    [SwaggerOperation(Summary = "Search ModuleManagements using available Filters.")]
    public async Task<IActionResult> SearchAsync(ModuleManagementListFilter filter)
    {
        var moduleManagements = await _service.SearchAsync(filter);
        return Ok(moduleManagements);
    }

    /// <summary>
    /// retrive the Modules against specific id.
    /// </summary>
    /// <response code="200">Modules returns.</response>
    /// <response code="400">Modules not found.</response>
    /// <response code="500">Oops! Can't lookup your record right now.</response>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(Result<ModuleDto>), 200)]
    [ProducesResponseType(typeof(IDictionary<string, string>), 400)]
    [ProducesResponseType(500)]
    [SwaggerHeader("tenant", "ManageModule", "Get", "Input your tenant to access this API i.e. admin for test", "admin", true)]
    [MustHavePermission(PermissionConstants.ModuleManagements.View)]
    public async Task<IActionResult> GetAsync(Guid id)
    {
        var moduleManagement = await _service.GetModuleManagementAsync(id);
        return Ok(moduleManagement);
    }

    /// <summary>
    /// retrive the Modules against specific tenant.
    /// </summary>
    /// <response code="200">Modules returns.</response>
    /// <response code="400">Modules not found.</response>
    /// <response code="500">Oops! Can't lookup your record right now.</response>
    [ProducesResponseType(typeof(Result<List<ModuleDto>>), 200)]
    [ProducesResponseType(typeof(IDictionary<string, string>), 400)]
    [ProducesResponseType(500)]
    [HttpGet("getmodulebytenant/{tenant}")]
    [SwaggerHeader("tenant", "ManageModule", "Get", "Input your tenant to access this API i.e. admin for test", "admin", true)]
    [MustHavePermission(PermissionConstants.ModuleManagements.View)]
    public async Task<IActionResult> GetModuleByTenantAsync(string tenant)
    {
        return Ok(await _service.GetModuleManagementByTenantIdAsync(tenant));
    }

    /// <summary>
    /// Create a Module.
    /// </summary>
    /// <response code="200">Module created.</response>
    /// <response code="400">Module already exists.</response>
    /// <response code="500">Oops! Can't lookup your product right now.</response>
    [HttpPost]
    [ProducesResponseType(typeof(Result<Guid>), 200)]
    [ProducesResponseType(typeof(IDictionary<string, string>), 400)]
    [ProducesResponseType(500)]
    [SwaggerHeader("tenant", "ManageModule", "Create", "Input your tenant to access this API i.e. admin for test", "admin", true)]
    [MustHavePermission(PermissionConstants.ModuleManagements.Create)]
    public async Task<IActionResult> CreateAsync(CreateModuleManagementRequest request)
    {
        string filePath = _config.GetValue<string>("MiddlewareSettings:ModuleManageFileNameWithPath");
        return Ok(await _service.CreateModuleManagementAsync(request, filePath));
    }

    /// <summary>
    /// update a specific Module by unique id.
    /// </summary>
    /// <response code="200">Module updated.</response>
    /// <response code="404">Module not found.</response>
    /// <response code="500">Oops! Can't lookup your product right now.</response>
    [HttpPut("{id}")]
    [ProducesResponseType(typeof(Result<Guid>), 200)]
    [ProducesResponseType(404)]
    [ProducesResponseType(500)]
    [SwaggerHeader("tenant", "ManageModule", "Update", "Input your tenant to access this API i.e. admin for test", "admin", true)]
    [MustHavePermission(PermissionConstants.ModuleManagements.Update)]
    public async Task<IActionResult> UpdateAsync(UpdateModuleManagementRequest request, Guid id)
    {
        string filePath = _config.GetValue<string>("MiddlewareSettings:ModuleManageFileNameWithPath");
        return Ok(await _service.UpdateModuleManagementAsync(request, id, filePath));
    }

    /// <summary>
    /// Delete a specific Module by unique id.
    /// </summary>
    /// <response code="200">Module deleted.</response>
    /// <response code="404">Module not found.</response>
    /// <response code="500">Oops! Can't lookup your product right now.</response>
    [HttpDelete("{id}")]
    [ProducesResponseType(typeof(Result<Guid>), 200)]
    [ProducesResponseType(404)]
    [ProducesResponseType(500)]
    [SwaggerHeader("tenant", "ManageModule", "Remove", "Input your tenant to access this API i.e. admin for test", "admin", true)]
    [MustHavePermission(PermissionConstants.ModuleManagements.Remove)]
    public async Task<IActionResult> DeleteAsync(Guid id)
    {
        string filePath = _config.GetValue<string>("MiddlewareSettings:ModuleManageFileNameWithPath");
        var moduleManagementId = await _service.DeleteModuleManagementAsync(id, filePath);
        return Ok(moduleManagementId);
    }
}
