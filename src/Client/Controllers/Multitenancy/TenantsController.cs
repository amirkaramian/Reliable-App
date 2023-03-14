using Microsoft.AspNetCore.Mvc;
using MyReliableSite.Application.Multitenancy;
using MyReliableSite.Domain.Constants;
using MyReliableSite.Infrastructure.Identity.Permissions;
using MyReliableSite.Infrastructure.Swagger;
using MyReliableSite.Shared.DTOs.Multitenancy;
using Swashbuckle.AspNetCore.Annotations;

namespace MyReliableSite.Client.API.Controllers.Multitenancy;

[ApiController]
[Route("api/[controller]")]
public class TenantsController : ControllerBase
{
    private readonly ITenantManager _tenantService;

    public TenantsController(ITenantManager tenantService)
    {
        _tenantService = tenantService;
    }

    [HttpGet("{key}")]
    [SwaggerHeader("tenant,gen-api-key", "Articles", "Search", "Input your tenant to access this API i.e. admin for test", "admin", true, true)]
    [MustHavePermission(RootPermissions.Tenants.View)]
    [SwaggerOperation(Summary = "Get Tenant Details.")]
    public async Task<IActionResult> GetAsync(string key)
    {
        var tenant = await _tenantService.GetByKeyAsync(key);
        return Ok(tenant);
    }
}
