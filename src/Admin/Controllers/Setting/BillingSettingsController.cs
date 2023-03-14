using Microsoft.AspNetCore.Mvc;
using MyReliableSite.Application.Settings.Interfaces;
using MyReliableSite.Application.Wrapper;
using MyReliableSite.Domain.Constants;
using MyReliableSite.Infrastructure.Identity.Permissions;
using MyReliableSite.Infrastructure.Swagger;
using MyReliableSite.Shared.DTOs.Settings;

namespace MyReliableSite.Admin.API.Controllers.Setting;

public class BillingSettingsController : BaseController
{
    private readonly ISettingService _service;

    public BillingSettingsController(ISettingService service)
    {
        _service = service;
    }

    /// <summary>
    /// Create an Billing Setting.
    /// </summary>
    /// <response code="200">Billing Setting created.</response>
    /// <response code="400">Billing Setting already exists.</response>
    /// <response code="500">Oops! Can't lookup your product right now.</response>
    [HttpPost]
    [ProducesResponseType(typeof(Result<Guid>), 200)]
    [ProducesResponseType(typeof(IDictionary<string, string>), 400)]
    [ProducesResponseType(500)]
    [HttpPost]
    [SwaggerHeader("tenant", "Setting", "Register", "Input your tenant to access this API i.e. admin for test", "admin", true)]
    [MustHavePermission(PermissionConstants.Settings.Register)]
    public async Task<IActionResult> CreateAsync(CreateBillingSettingRequest request)
    {
        return Ok(await _service.CreateBillingSettingAsync(request));
    }

    /// <summary>
    /// update a specific Billing Setting by unique id.
    /// </summary>
    /// <response code="200">Billing Setting updated.</response>
    /// <response code="404">Billing Setting not found.</response>
    /// <response code="500">Oops! Can't lookup your product right now.</response>
    [HttpPut("{id}")]
    [ProducesResponseType(typeof(Result<Guid>), 200)]
    [ProducesResponseType(404)]
    [ProducesResponseType(500)]
    [SwaggerHeader("tenant", "Setting", "Update", "Input your tenant to access this API i.e. admin for test", "admin", true)]
    [MustHavePermission(PermissionConstants.Settings.Update)]
    public async Task<IActionResult> UpdateAsync(UpdateBillingSettingRequest request, Guid id)
    {
        return Ok(await _service.UpdateBillingSettingAsync(request, id));
    }

    /// <summary>
    /// retrive the Billing Setting against specific id.
    /// </summary>
    /// <response code="200">Billing Setting returns.</response>
    /// <response code="400">Billing Setting not found.</response>
    /// <response code="500">Oops! Can't lookup your record right now.</response>
    [ProducesResponseType(typeof(Result<SettingBillingDetailsDto>), 200)]
    [ProducesResponseType(typeof(IDictionary<string, string>), 400)]
    [ProducesResponseType(500)]
    [HttpGet("{id}")]
    [SwaggerHeader("tenant", "Setting", "View", "Input your tenant to access this API i.e. admin for test", "admin", true)]
    [MustHavePermission(PermissionConstants.Settings.View)]
    public async Task<IActionResult> GetAsync(Guid id)
    {
        var result = await _service.GetBillingSettingDetailsAsync(id);
        return Ok(result);
    }

    /// <summary>
    /// retrive the Billing Setting against Tenant.
    /// </summary>
    /// <response code="200">Billing Setting returns.</response>
    /// <response code="400">Billing Setting not found.</response>
    /// <response code="500">Oops! Can't lookup your record right now.</response>
    [ProducesResponseType(typeof(Result<SettingBillingDetailsDto>), 200)]
    [ProducesResponseType(typeof(IDictionary<string, string>), 400)]
    [ProducesResponseType(500)]
    [HttpGet("getsettingswithtenant/{tenant}")]
    [SwaggerHeader("tenant", "Setting", "View", "Input your tenant to access this API i.e. admin for test", "admin", true)]
    [MustHavePermission(PermissionConstants.Settings.View)]
    public async Task<IActionResult> GetBillingSettingsWithTenantAsync(string tenant)
    {
        var result = await _service.GetBillingSettingDetailsAsync(tenant);
        return Ok(result);
    }
}
