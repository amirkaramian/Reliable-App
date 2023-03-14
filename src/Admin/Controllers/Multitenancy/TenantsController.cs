using Microsoft.AspNetCore.Mvc;
using MyReliableSite.Application.Multitenancy;
using MyReliableSite.Application.Wrapper;
using MyReliableSite.Domain.Constants;
using MyReliableSite.Infrastructure.Identity.Permissions;
using MyReliableSite.Infrastructure.Swagger;
using MyReliableSite.Shared.DTOs.Multitenancy;
using Swashbuckle.AspNetCore.Annotations;

namespace MyReliableSite.Admin.API.Controllers.Multitenancy;

[ApiController]
[Route("api/[controller]")]
public class TenantsController : ControllerBase
{
    private readonly ITenantManager _tenantService;

    public TenantsController(ITenantManager tenantService)
    {
        _tenantService = tenantService;
    }

    /// <summary>
    /// retrive the Tenant against specific id.
    /// </summary>
    /// <response code="200">Tenant returns.</response>
    /// <response code="400">Tenant not found.</response>
    /// <response code="500">Oops! Can't lookup your record right now.</response>
    [ProducesResponseType(typeof(Result<TenantDto>), 200)]
    [ProducesResponseType(typeof(IDictionary<string, string>), 400)]
    [ProducesResponseType(500)]
    [HttpGet("{key}")]
    [SwaggerHeader("tenant", "Multitenancy", "Search", "Input your tenant to access this API i.e. admin for test", "admin", true)]
    [MustHavePermission(RootPermissions.Tenants.View)]
    [SwaggerOperation(Summary = "Get Tenant Details.")]
    public async Task<IActionResult> GetAsync(string key)
    {
        var tenant = await _tenantService.GetByKeyAsync(key);
        return Ok(tenant);
    }

    /// <summary>
    /// retrive the Tenant against specific id.
    /// </summary>
    /// <response code="200">Tenant returns.</response>
    /// <response code="400">Tenant not found.</response>
    /// <response code="500">Oops! Can't lookup your record right now.</response>
    [ProducesResponseType(typeof(Result<List<TenantDto>>), 200)]
    [ProducesResponseType(typeof(IDictionary<string, string>), 400)]
    [ProducesResponseType(500)]
    [HttpGet]
    [MustHavePermission(RootPermissions.Tenants.ListAll)]
    [SwaggerHeader("tenant", "Multitenancy", "View", "Input your tenant to access this API i.e. admin for test", "admin", true)]
    [SwaggerOperation(Summary = "Get all the available Tenants.")]
    public async Task<IActionResult> GetAllAsync()
    {
        var tenants = await _tenantService.GetAllAsync();
        return Ok(tenants);
    }

    /// <summary>
    /// Create an Tenant.
    /// </summary>
    /// <response code="200">Tenant created.</response>
    /// <response code="400">Tenant already exists.</response>
    /// <response code="500">Oops! Can't lookup your product right now.</response>
    [HttpPost]
    [ProducesResponseType(typeof(Result<Guid>), 200)]
    [ProducesResponseType(typeof(IDictionary<string, string>), 400)]
    [ProducesResponseType(500)]
    [MustHavePermission(RootPermissions.Tenants.Create)]
    [SwaggerHeader("tenant", "Multitenancy", "Create", "Input your tenant to access this API i.e. admin for test", "admin", true)]
    [SwaggerOperation(Summary = "Create a new Tenant.")]
    public async Task<IActionResult> CreateAsync(CreateTenantRequest request)
    {
        var tenantId = await _tenantService.CreateTenantAsync(request);
        return Ok(tenantId);
    }

    /// <summary>
    /// update a specific Tenant by unique id.
    /// </summary>
    /// <response code="200">Tenant updated.</response>
    /// <response code="404">Tenant not found.</response>
    /// <response code="500">Oops! Can't lookup your product right now.</response>
    [ProducesResponseType(typeof(Result<Guid>), 200)]
    [ProducesResponseType(404)]
    [ProducesResponseType(500)]
    [HttpPost("upgrade")]
    [SwaggerHeader("tenant", "Multitenancy", "Update", "Input your tenant to access this API i.e. admin for test", "admin", true)]
    [MustHavePermission(RootPermissions.Tenants.UpgradeSubscription)]
    [SwaggerOperation(Summary = "Upgrade Subscription of Tenant.")]
    public async Task<IActionResult> UpgradeSubscriptionAsync(UpgradeSubscriptionRequest request)
    {
        return Ok(await _tenantService.UpgradeSubscriptionAsync(request));
    }

    /// <summary>
    /// update a specific Tenant by unique id.
    /// </summary>
    /// <response code="200">Tenant updated.</response>
    /// <response code="404">Tenant not found.</response>
    /// <response code="500">Oops! Can't lookup your product right now.</response>
    [ProducesResponseType(typeof(Result<Guid>), 200)]
    [ProducesResponseType(404)]
    [ProducesResponseType(500)]
    [HttpPost("{id}/deactivate")]
    [SwaggerHeader("tenant", "Multitenancy", "Update", "Input your tenant to access this API i.e. admin for test", "admin", true)]
    [MustHavePermission(RootPermissions.Tenants.Update)]
    [SwaggerOperation(Summary = "Deactivate Tenant.")]
    public async Task<IActionResult> DeactivateTenantAsync(string id)
    {
        return Ok(await _tenantService.DeactivateTenantAsync(id));
    }

    /// <summary>
    /// update a specific Tenant by unique id.
    /// </summary>
    /// <response code="200">Tenant updated.</response>
    /// <response code="404">Tenant not found.</response>
    /// <response code="500">Oops! Can't lookup your product right now.</response>
    [ProducesResponseType(typeof(Result<Guid>), 200)]
    [ProducesResponseType(404)]
    [ProducesResponseType(500)]
    [HttpPost("{id}/activate")]
    [SwaggerHeader("tenant", "Multitenancy", "Update", "Input your tenant to access this API i.e. admin for test", "admin", true)]
    [MustHavePermission(RootPermissions.Tenants.Update)]
    [SwaggerOperation(Summary = "Activate Tenant.")]
    public async Task<IActionResult> ActivateTenantAsync(string id)
    {
        return Ok(await _tenantService.ActivateTenantAsync(id));
    }
}
