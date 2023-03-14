using Microsoft.AspNetCore.Mvc;
using MyReliableSite.Application.Settings.Interfaces;
using MyReliableSite.Application.Wrapper;
using MyReliableSite.Domain.Constants;
using MyReliableSite.Infrastructure.Identity.Permissions;
using MyReliableSite.Infrastructure.Swagger;
using MyReliableSite.Shared.DTOs.Settings;

namespace MyReliableSite.Admin.API.Controllers.Setting;

public class SettingsController : BaseController
{
    private readonly ISettingService _service;

    public SettingsController(ISettingService service)
    {
        _service = service;
    }

    /// <summary>
    /// Create an Setting.
    /// </summary>
    /// <response code="200">Setting created.</response>
    /// <response code="400">Setting already exists.</response>
    /// <response code="500">Oops! Can't lookup your product right now.</response>
    [HttpPost]
    [ProducesResponseType(typeof(Result<Guid>), 200)]
    [ProducesResponseType(typeof(IDictionary<string, string>), 400)]
    [ProducesResponseType(500)]
    [SwaggerHeader("tenant", "Setting", "Register", "Input your tenant to access this API i.e. admin for test", "admin", true)]
    [MustHavePermission(PermissionConstants.Settings.Register)]
    public async Task<IActionResult> CreateAsync(CreateSettingRequest request)
    {
        return Ok(await _service.CreateSettingAsync(request));
    }

    /// <summary>
    /// update a specific Setting by unique id.
    /// </summary>
    /// <response code="200">Setting updated.</response>
    /// <response code="404">Setting not found.</response>
    /// <response code="500">Oops! Can't lookup your product right now.</response>
    [HttpPut("{id}")]
    [ProducesResponseType(typeof(Result<Guid>), 200)]
    [ProducesResponseType(404)]
    [ProducesResponseType(500)]
    [SwaggerHeader("tenant", "Setting", "Update", "Input your tenant to access this API i.e. admin for test", "admin", true)]
    [MustHavePermission(PermissionConstants.Settings.Update)]
    public async Task<IActionResult> UpdateAsync(UpdateSettingRequest request, Guid id)
    {
        return Ok(await _service.UpdateSettingAsync(request, id));
    }

    /// <summary>
    /// retrive the Setting against specific id.
    /// </summary>
    /// <response code="200">Setting returns.</response>
    /// <response code="400">Setting not found.</response>
    /// <response code="500">Oops! Can't lookup your record right now.</response>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(Result<SettingSupportDetailsDto>), 200)]
    [ProducesResponseType(typeof(IDictionary<string, string>), 400)]
    [ProducesResponseType(500)]
    [SwaggerHeader("tenant", "Setting", "View", "Input your tenant to access this API i.e. admin for test", "admin", true)]
    [MustHavePermission(PermissionConstants.Settings.View)]
    public async Task<IActionResult> GetAsync(Guid id)
    {
        var result = await _service.GetSettingDetailsAsync(id);
        return Ok(result);
    }

    /// <summary>
    /// retrive the Setting against specific tenant.
    /// </summary>
    /// <response code="200">Setting returns.</response>
    /// <response code="400">Setting not found.</response>
    /// <response code="500">Oops! Can't lookup your record right now.</response>
    [HttpGet("{tenant}")]
    [ProducesResponseType(typeof(Result<SettingDetailsDto>), 200)]
    [ProducesResponseType(typeof(IDictionary<string, string>), 400)]
    [ProducesResponseType(500)]
    [HttpGet("getsettingswithtenant/{tenant}")]
    [SwaggerHeader("tenant", "Setting", "View", "Input your tenant to access this API i.e. admin for test", "admin", true)]
    [MustHavePermission(PermissionConstants.Settings.View)]
    public async Task<IActionResult> GetSettingsWithTenantAsync(string tenant)
    {
        var result = await _service.GetSettingDetailsAsync(tenant);
        return Ok(result);
    }
}
