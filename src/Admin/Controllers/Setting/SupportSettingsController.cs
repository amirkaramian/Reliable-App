using Microsoft.AspNetCore.Mvc;
using MyReliableSite.Application.Settings.Interfaces;
using MyReliableSite.Application.Wrapper;
using MyReliableSite.Domain.Constants;
using MyReliableSite.Infrastructure.Identity.Permissions;
using MyReliableSite.Infrastructure.Swagger;
using MyReliableSite.Shared.DTOs.Settings;

namespace MyReliableSite.Admin.API.Controllers.Setting;

public class SupportSettingsController : BaseController
{
    private readonly ISettingService _service;

    public SupportSettingsController(ISettingService service)
    {
        _service = service;
    }

    /// <summary>
    /// Create an Support Setting.
    /// </summary>
    /// <response code="200">Support Setting created.</response>
    /// <response code="400">Support Setting already exists.</response>
    /// <response code="500">Oops! Can't lookup your product right now.</response>
    [HttpPost]
    [ProducesResponseType(typeof(Result<Guid>), 200)]
    [ProducesResponseType(typeof(IDictionary<string, string>), 400)]
    [ProducesResponseType(500)]
    [SwaggerHeader("tenant", "Setting", "Register", "Input your tenant to access this API i.e. admin for test", "admin", true)]
    [MustHavePermission(PermissionConstants.Settings.Register)]
    public async Task<IActionResult> CreateAsync(CreateSupportSettingRequest request)
    {
        return Ok(await _service.CreateSupportSettingAsync(request));
    }

    /// <summary>
    /// update a specific Support Setting by unique id.
    /// </summary>
    /// <response code="200">Support Setting updated.</response>
    /// <response code="404">Support Setting not found.</response>
    /// <response code="500">Oops! Can't lookup your product right now.</response>
    [HttpPut("{id}")]
    [ProducesResponseType(typeof(Result<Guid>), 200)]
    [ProducesResponseType(404)]
    [ProducesResponseType(500)]
    [SwaggerHeader("tenant", "Setting", "Update", "Input your tenant to access this API i.e. admin for test", "admin", true)]
    [MustHavePermission(PermissionConstants.Settings.Update)]
    public async Task<IActionResult> UpdateAsync(UpdateSupportSettingRequest request, Guid id)
    {
        return Ok(await _service.UpdateSupportSettingAsync(request, id));
    }

    /// <summary>
    /// retrive the Support Setting against specific id.
    /// </summary>
    /// <response code="200">Support Setting returns.</response>
    /// <response code="400">Support Setting not found.</response>
    /// <response code="500">Oops! Can't lookup your record right now.</response>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(Result<SettingSupportDetailsDto>), 200)]
    [ProducesResponseType(typeof(IDictionary<string, string>), 400)]
    [ProducesResponseType(500)]
    [SwaggerHeader("tenant", "Setting", "View", "Input your tenant to access this API i.e. admin for test", "admin", true)]
    [MustHavePermission(PermissionConstants.Settings.View)]
    public async Task<IActionResult> GetAsync(Guid id)
    {
        var result = await _service.GetSupportSettingDetailsAsync(id);
        return Ok(result);
    }

    /// <summary>
    /// retrive the Support Setting against specific tenant.
    /// </summary>
    /// <response code="200">Support Setting returns.</response>
    /// <response code="400">Support Setting not found.</response>
    /// <response code="500">Oops! Can't lookup your record right now.</response>
    [HttpGet("{tenant}")]
    [ProducesResponseType(typeof(Result<SettingSupportDetailsDto>), 200)]
    [ProducesResponseType(typeof(IDictionary<string, string>), 400)]
    [ProducesResponseType(500)]
    [HttpGet("getsettingswithtenant/{tenant}")]
    [SwaggerHeader("tenant", "Setting", "View", "Input your tenant to access this API i.e. admin for test", "admin", true)]
    [MustHavePermission(PermissionConstants.Settings.View)]
    public async Task<IActionResult> GetSupportSettingsWithTenantAsync(string tenant)
    {
        var result = await _service.GetSupportSettingDetailsAsync(tenant);
        return Ok(result);
    }
}
