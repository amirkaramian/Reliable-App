using Microsoft.AspNetCore.Mvc;
using MyReliableSite.Application.SmtpConfigurations.Interfaces;
using MyReliableSite.Application.Wrapper;
using MyReliableSite.Domain.Constants;
using MyReliableSite.Infrastructure.Identity.Permissions;
using MyReliableSite.Infrastructure.Swagger;
using MyReliableSite.Shared.DTOs.SmtpConfigurations;

namespace MyReliableSite.Admin.API.Controllers.SmtpConfigurations;

public class SmtpConfigurationsController : BaseController
{
    private readonly ISmtpConfigurationService _smtpConfigurationService;

    public SmtpConfigurationsController(ISmtpConfigurationService smtpConfigurationService)
    {
        _smtpConfigurationService = smtpConfigurationService ?? throw new ArgumentNullException(nameof(smtpConfigurationService));
    }

    /// <summary>
    /// retrive the SmtpConfiguration against specific id.
    /// </summary>
    /// <response code="200">SmtpConfiguration returns.</response>
    /// <response code="400">SmtpConfiguration not found.</response>
    /// <response code="500">Oops! Can't lookup your record right now.</response>
    [ProducesResponseType(typeof(Result<SmtpConfigurationDto>), 200)]
    [ProducesResponseType(typeof(IDictionary<string, string>), 400)]
    [ProducesResponseType(500)]
    [HttpGet("{id:guid}")]
    [MustHavePermission(PermissionConstants.SmtpConfigurations.Read)]
    [SwaggerHeader("tenant", "SmtpConfigurations", "Get", "Input your tenant to access this API i.e. admin for test", "admin", true)]
    public async Task<IActionResult> GetAsync(Guid id) => Ok(await _smtpConfigurationService.GetAsync(id));

    /// <summary>
    /// retrive the SmtpConfiguration against specific brand id.
    /// </summary>
    /// <response code="200">SmtpConfiguration returns.</response>
    /// <response code="400">SmtpConfiguration not found.</response>
    /// <response code="500">Oops! Can't lookup your record right now.</response>
    [ProducesResponseType(typeof(Result<SmtpConfigurationDto>), 200)]
    [ProducesResponseType(typeof(IDictionary<string, string>), 400)]
    [ProducesResponseType(500)]
    [HttpGet("by-brand/{brandId:guid}")]
    [MustHavePermission(PermissionConstants.SmtpConfigurations.Read)]
    [SwaggerHeader("tenant", "SmtpConfigurations", "Get", "Input your tenant to access this API i.e. admin for test", "admin", true)]
    public async Task<IActionResult> GetByBrandAsync(Guid brandId) => Ok(await _smtpConfigurationService.GetByBrandAsync(brandId));

    /// <summary>
    /// List of records with pagination &amp; filters.
    /// </summary>
    /// <response code="200">SmtpConfiguration List returns.</response>
    /// <response code="400">SmtpConfiguration not found.</response>
    /// <response code="500">Oops! Can't lookup your record right now.</response>
    [HttpPost("search")]
    [ProducesResponseType(typeof(PaginatedResult<SmtpConfigurationDto>), 200)]
    [ProducesResponseType(typeof(IDictionary<string, string>), 400)]
    [ProducesResponseType(500)]
    [MustHavePermission(PermissionConstants.SmtpConfigurations.Search)]
    [SwaggerHeader("tenant", "SmtpConfigurations", "Search", "Input your tenant to access this API i.e. admin for test", "admin", true)]
    public async Task<IActionResult> SearchAsync(SmtpConfigurationListFilter filter)
    {
        return Ok(await _smtpConfigurationService.SearchAsync(filter));
    }

    /// <summary>
    /// Create an SmtpConfiguration.
    /// </summary>
    /// <response code="200">SmtpConfiguration created.</response>
    /// <response code="400">SmtpConfiguration already exists.</response>
    /// <response code="500">Oops! Can't lookup your product right now.</response>
    [ProducesResponseType(typeof(Result<SmtpConfigurationDto>), 200)]
    [ProducesResponseType(typeof(IDictionary<string, string>), 400)]
    [ProducesResponseType(500)]
    [HttpPost]
    [MustHavePermission(PermissionConstants.SmtpConfigurations.Create)]
    [SwaggerHeader("tenant", "SmtpConfigurations", "Create", "Input your tenant to access this API i.e. admin for test", "admin", true)]
    public async Task<IActionResult> CreateAsync(CreateSmtpConfigurationRequest request)
    {
        return Ok(await _smtpConfigurationService.CreateAsync(request));
    }

    /// <summary>
    /// update a specific SmtpConfiguration by unique id.
    /// </summary>
    /// <response code="200">SmtpConfiguration updated.</response>
    /// <response code="404">SmtpConfiguration not found.</response>
    /// <response code="500">Oops! Can't lookup your product right now.</response>
    [ProducesResponseType(typeof(Result<SmtpConfigurationDto>), 200)]
    [ProducesResponseType(404)]
    [ProducesResponseType(500)]
    [HttpPut("{id:guid}")]
    [MustHavePermission(PermissionConstants.SmtpConfigurations.Update)]
    [SwaggerHeader("tenant", "SmtpConfigurations", "Update", "Input your tenant to access this API i.e. admin for test", "admin", true)]
    public async Task<IActionResult> UpdateAsync(UpdateSmtpConfigurationRequest request, Guid id)
    {
        return Ok(await _smtpConfigurationService.UpdateAsync(id, request));
    }

    /// <summary>
    /// update a specific SmtpConfiguration by brand id.
    /// </summary>
    /// <response code="200">SmtpConfiguration updated.</response>
    /// <response code="404">SmtpConfiguration not found.</response>
    /// <response code="500">Oops! Can't lookup your product right now.</response>
    [ProducesResponseType(typeof(Result<SmtpConfigurationDto>), 200)]
    [ProducesResponseType(404)]
    [ProducesResponseType(500)]
    [HttpPut("by-brand/{brandId:guid}/{departmentId:guid}")]
    [MustHavePermission(PermissionConstants.SmtpConfigurations.Update)]
    [SwaggerHeader("tenant", "SmtpConfigurations", "Update", "Input your tenant to access this API i.e. admin for test", "admin", true)]
    public async Task<IActionResult> UpdateByBrandAsync(UpdateSmtpConfigurationRequest request, Guid brandId, Guid departmentId)
    {
        return Ok(await _smtpConfigurationService.UpdateForBrandAsync(brandId, departmentId, request));
    }

    /// <summary>
    /// Delete a specific SmtpConfiguration by unique id.
    /// </summary>
    /// <response code="200">SmtpConfiguration deleted.</response>
    /// <response code="404">SmtpConfiguration not found.</response>
    /// <response code="500">Oops! Can't lookup your product right now.</response>
    [ProducesResponseType(typeof(Result<bool>), 200)]
    [ProducesResponseType(404)]
    [ProducesResponseType(500)]
    [HttpDelete("{id:guid}")]
    [MustHavePermission(PermissionConstants.SmtpConfigurations.Delete)]
    [SwaggerHeader("tenant", "SmtpConfigurations", "Delete", "Input your tenant to access this API i.e. admin for test", "admin", true)]
    public async Task<IActionResult> DeleteAsync(Guid id) => Ok(await _smtpConfigurationService.DeleteAsync(id));

    /// <summary>
    /// Delete a specific SmtpConfiguration by brand id.
    /// </summary>
    /// <response code="200">SmtpConfiguration deleted.</response>
    /// <response code="404">SmtpConfiguration not found.</response>
    /// <response code="500">Oops! Can't lookup your product right now.</response>
    [ProducesResponseType(typeof(Result<bool>), 200)]
    [ProducesResponseType(404)]
    [ProducesResponseType(500)]
    [HttpDelete("by-brand/{brandId:guid}")]
    [MustHavePermission(PermissionConstants.SmtpConfigurations.Delete)]
    [SwaggerHeader("tenant", "SmtpConfigurations", "Delete", "Input your tenant to access this API i.e. admin for test", "admin", true)]
    public async Task<IActionResult> DeleteForBrandAsync(Guid brandId) => Ok(await _smtpConfigurationService.DeleteForBrandAsync(brandId));
}
