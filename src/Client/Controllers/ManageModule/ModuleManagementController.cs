using Microsoft.AspNetCore.Mvc;
using MyReliableSite.Application.ManageModule.Interfaces;
using MyReliableSite.Domain.Constants;
using MyReliableSite.Infrastructure.Identity.Permissions;
using MyReliableSite.Infrastructure.Swagger;
using MyReliableSite.Shared.DTOs.ManageModule;
using Swashbuckle.AspNetCore.Annotations;

namespace MyReliableSite.Client.API.Controllers.ManageModule;
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

    [HttpGet("getmodulebytenant/{tenant}")]
    [SwaggerHeader("tenant", "Settings", "View", "Input your tenants to access this API i.e. admin for test", "admin", true)]
    [MustHavePermission(PermissionConstants.ModuleManagements.View)]
    public async Task<IActionResult> GetModuleByTenantAsync(string tenant)
    {
        return Ok(await _service.GetModuleManagementByTenantIdAsync(tenant));
    }
}
