using Microsoft.AspNetCore.Mvc;
using MyReliableSite.Application.ManageUserApiKey.Interfaces;
using MyReliableSite.Application.Wrapper;
using MyReliableSite.Domain.Constants;
using MyReliableSite.Infrastructure.Identity.Permissions;
using MyReliableSite.Infrastructure.Swagger;
using MyReliableSite.Shared.DTOs.ManageUserApiKey;
using MyReliableSite.Shared.DTOs.ManageSubUserApiKey;
using Swashbuckle.AspNetCore.Annotations;
using System.Collections;

namespace MyReliableSite.Client.API.Controllers.ManageSubUserApiKey;

[Route("api/[controller]")]
[ApiController]
public class ManageSubUserApiKeyController : BaseController
{
    private readonly IAPIKeyPairService _service;
    private readonly IConfiguration _config;
    public ManageSubUserApiKeyController(IAPIKeyPairService service, IConfiguration config)
    {
        _service = service;
        _config = config;
    }

    /// <summary>
    /// List of records with pagination &amp; filters.
    /// </summary>
    /// <response code="200">ManageSubUserApiKey List returns.</response>
    /// <response code="400">ManageSubUserApiKey not found.</response>
    /// <response code="500">Oops! Can't lookup your record right now.</response>
    [HttpPost("search")]
    [ProducesResponseType(typeof(PaginatedResult<APIKeyPairDto>), 200)]
    [ProducesResponseType(typeof(IDictionary<string, string>), 400)]
    [ProducesResponseType(500)]
    [SwaggerHeader("tenant", "ManageSubUserApiKey", "Search", "Input your tenant to access this API i.e. admin for test", "admin", true, true)]
    [MustHavePermission(PermissionConstants.SubUserAPIKeyPairs.Search)]
    [SwaggerOperation(Summary = "Search APIKeyPairs using available Filters.")]
    public async Task<IActionResult> SearchAsync(APIKeyPairListFilter filter)
    {
        var aPIKeyPairs = await _service.SearchAsync(filter);
        return Ok(aPIKeyPairs);
    }

    /// <summary>
    /// List of records without pagination &amp; filters.
    /// </summary>
    /// <response code="200">ManageSubUserApiKey List returns.</response>
    /// <response code="400">ManageSubUserApiKey not found.</response>
    /// <response code="500">Oops! Can't lookup your record right now.</response>
    [HttpGet("GetAllAsync")]
    [ProducesResponseType(typeof(IEnumerable<APIKeyPairDto>), 200)]
    [ProducesResponseType(typeof(IDictionary<string, string>), 400)]
    [ProducesResponseType(500)]
    [SwaggerHeader("tenant", "ManageSubUserApiKey", "View", "Input your tenant to access this API i.e. admin for test", "admin", true, true)]
    [MustHavePermission(PermissionConstants.SubUserAPIKeyPairs.View)]
    [SwaggerOperation(Summary = "Search APIKeyPairs using available Filters.")]
    public async Task<IActionResult> GetAllAsync()
    {
        var aPIKeyPairs = await _service.GetAllAsync();
        return Ok(aPIKeyPairs);
    }

    /// <summary>
    /// retrive the ManageSubUserApiKey against specific id.
    /// </summary>
    /// <response code="200">ManageSubUserApiKey returns.</response>
    /// <response code="400">ManageSubUserApiKey not found.</response>
    /// <response code="500">Oops! Can't lookup your record right now.</response>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(Result<APIKeyPairDto>), 200)]
    [ProducesResponseType(typeof(IDictionary<string, string>), 400)]
    [ProducesResponseType(500)]
    [SwaggerHeader("tenant", "ManageSubUserApiKey", "View", "Input your tenant to access this API i.e. admin for test", "admin", true, true)]
    [MustHavePermission(PermissionConstants.SubUserAPIKeyPairs.View)]
    public async Task<IActionResult> GetAsync(Guid id)
    {
        var aPIKeyPair = await _service.GetAPIKeyPairAsync(id);
        return Ok(aPIKeyPair);
    }

    /// <summary>
    /// Create an ManageSubUserApiKey.
    /// </summary>
    /// <response code="200">ManageSubUserApiKey created.</response>
    /// <response code="400">ManageSubUserApiKey already exists.</response>
    /// <response code="500">Oops! Can't lookup your product right now.</response>
    [HttpPost]
    [ProducesResponseType(typeof(Result<Guid>), 200)]
    [ProducesResponseType(typeof(IDictionary<string, string>), 400)]
    [ProducesResponseType(500)]
    [SwaggerHeader("tenant", "ManageSubUserApiKey", "Create", "Input your tenant to access this API i.e. admin for test", "admin", true, true)]
    [MustHavePermission(PermissionConstants.SubUserAPIKeyPairs.Create)]
    public async Task<IActionResult> CreateAsync(CreateSubUserAPIKeyPairRequest request)
    {
        return Ok(await _service.CreateSubUserAPIKeyPairAsync(request));
    }

    /// <summary>
    /// update a specific ManageSubUserApiKey permissions not included by unique id.
    /// </summary>
    /// <response code="200">ManageSubUserApiKey updated.</response>
    /// <response code="404">ManageSubUserApiKey not found.</response>
    /// <response code="500">Oops! Can't lookup your product right now.</response>
    [HttpPut("userapikeyupdate/{id}")]
    [ProducesResponseType(typeof(Result<Guid>), 200)]
    [ProducesResponseType(404)]
    [ProducesResponseType(500)]
    [SwaggerHeader("tenant", "ManageSubUserApiKey", "Update", "Input your tenant to access this API i.e. admin for test", "admin", true, true)]
    [MustHavePermission(PermissionConstants.SubUserAPIKeyPairs.Update)]
    public async Task<IActionResult> UpdateAsync(UpdateSubUserAPIKeyPairRequest request, Guid id)
    {
        return Ok(await _service.UpdateSubUserAPIKeyPairAsync(request, id));
    }

    /// <summary>
    /// update a specific ManageSubUserApiKey permissions by unique id.
    /// </summary>
    /// <response code="200">ManageSubUserApiKey updated.</response>
    /// <response code="404">ManageSubUserApiKey not found.</response>
    /// <response code="500">Oops! Can't lookup your product right now.</response>
    [HttpPut("permissionsupdate/{id}")]
    [ProducesResponseType(typeof(Result<Guid>), 200)]
    [ProducesResponseType(404)]
    [ProducesResponseType(500)]
    [SwaggerHeader("tenant", "ManageSubUserApiKey", "Update", "Input your tenant to access this API i.e. admin for test", "admin", true, true)]
    [MustHavePermission(PermissionConstants.SubUserAPIKeyPairs.Update)]
    public async Task<IActionResult> UpdatePermissionAsync(UpdateSubUserAPIKeyPairPermissionRequest request, Guid id)
    {
        return Ok(await _service.UpdateSubUserAPIKeyPairPermissionsAsync(request, id));
    }

    /// <summary>
    /// Delete a specific ManageSubUserApiKey by unique id.
    /// </summary>
    /// <response code="200">ManageSubUserApiKey deleted.</response>
    /// <response code="404">ManageSubUserApiKey not found.</response>
    /// <response code="500">Oops! Can't lookup your product right now.</response>
    [HttpDelete("{id}")]
    [ProducesResponseType(typeof(Result<Guid>), 200)]
    [ProducesResponseType(404)]
    [ProducesResponseType(500)]
    [SwaggerHeader("tenant", "ManageSubUserApiKey", "Remove", "Input your tenant to access this API i.e. admin for test", "admin", true)]
    [MustHavePermission(PermissionConstants.SubUserAPIKeyPairs.Remove)]
    public async Task<IActionResult> DeleteAsync(Guid id)
    {
        var aPIKeyPairId = await _service.DeleteAPIKeyPairAsync(id);
        return Ok(aPIKeyPairId);
    }
}
