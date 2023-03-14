using Microsoft.AspNetCore.Mvc;
using MyReliableSite.Application.ManageUserApiKey.Interfaces;
using MyReliableSite.Application.Wrapper;
using MyReliableSite.Domain.Constants;
using MyReliableSite.Infrastructure.Identity.Permissions;
using MyReliableSite.Infrastructure.Swagger;
using MyReliableSite.Shared.DTOs.ManageUserApiKey;
using Swashbuckle.AspNetCore.Annotations;

namespace MyReliableSite.Admin.API.Controllers.ManageUserApiKey;
[Route("api/[controller]")]
[ApiController]
public class ManageUserApiKeyController : BaseController
{
    private readonly IAPIKeyPairService _service;
    private readonly IConfiguration _config;
    public ManageUserApiKeyController(IAPIKeyPairService service, IConfiguration config)
    {
        _service = service;
        _config = config;
    }

    /// <summary>
    /// List of records with pagination &amp; filters.
    /// </summary>
    /// <response code="200">ManageUserApiKey List returns.</response>
    /// <response code="400">ManageUserApiKey not found.</response>
    /// <response code="500">Oops! Can't lookup your record right now.</response>
    [HttpPost("search")]
    [ProducesResponseType(typeof(PaginatedResult<APIKeyPairDto>), 200)]
    [ProducesResponseType(typeof(IDictionary<string, string>), 400)]
    [ProducesResponseType(500)]
    [SwaggerHeader("tenant", "ManageUserApiKey", "Search", "Input your tenant to access this API i.e. admin for test", "admin", true)]
    [MustHavePermission(PermissionConstants.APIKeyPairs.Search)]
    [SwaggerOperation(Summary = "Search APIKeyPairs using available Filters.")]
    public async Task<IActionResult> SearchAsync(APIKeyPairListFilter filter)
    {
        var aPIKeyPairs = await _service.SearchAsync(filter);
        return Ok(aPIKeyPairs);
    }

    /// <summary>
    /// retrive the ManageUserApiKey against specific id.
    /// </summary>
    /// <response code="200">ManageUserApiKey returns.</response>
    /// <response code="400">ManageUserApiKey not found.</response>
    /// <response code="500">Oops! Can't lookup your record right now.</response>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(Result<APIKeyPairDto>), 200)]
    [ProducesResponseType(typeof(IDictionary<string, string>), 400)]
    [ProducesResponseType(500)]
    [SwaggerHeader("tenant", "ManageUserApiKey", "Search", "Input your tenant to access this API i.e. admin for test", "admin", true)]
    [MustHavePermission(PermissionConstants.APIKeyPairs.View)]
    public async Task<IActionResult> GetAsync(Guid id)
    {
        var aPIKeyPair = await _service.GetAPIKeyPairAsync(id);
        return Ok(aPIKeyPair);
    }

    /// <summary>
    /// Create an ManageUserApiKey.
    /// </summary>
    /// <response code="200">ManageUserApiKey created.</response>
    /// <response code="400">ManageUserApiKey already exists.</response>
    /// <response code="500">Oops! Can't lookup your product right now.</response>
    [HttpPost]
    [ProducesResponseType(typeof(Result<Guid>), 200)]
    [ProducesResponseType(typeof(IDictionary<string, string>), 400)]
    [ProducesResponseType(500)]
    [SwaggerHeader("tenant", "ManageUserApiKey", "Search", "Input your tenant to access this API i.e. admin for test", "admin", true)]
    [MustHavePermission(PermissionConstants.APIKeyPairs.Create)]
    public async Task<IActionResult> CreateAsync(CreateAPIKeyPairRequest request)
    {
        return Ok(await _service.CreateAPIKeyPairAsync(request));
    }

    /// <summary>
    /// update a specific ManageUserApiKey permissions not included by unique id.
    /// </summary>
    /// <response code="200">ManageUserApiKey updated.</response>
    /// <response code="404">ManageUserApiKey not found.</response>
    /// <response code="500">Oops! Can't lookup your product right now.</response>
    [HttpPut("userapikeyupdate/{id}")]
    [ProducesResponseType(typeof(Result<Guid>), 200)]
    [ProducesResponseType(404)]
    [ProducesResponseType(500)]
    [SwaggerHeader("tenant", "ManageUserApiKey", "Search", "Input your tenant to access this API i.e. admin for test", "admin", true)]
    [MustHavePermission(PermissionConstants.APIKeyPairs.Update)]
    public async Task<IActionResult> UpdateAsync(UpdateAPIKeyPairRequest request, Guid id)
    {
        return Ok(await _service.UpdateAPIKeyPairAsync(request, id));
    }

    /// <summary>
    /// update a specific ManageUserApiKey permissions by unique id.
    /// </summary>
    /// <response code="200">ManageUserApiKey updated.</response>
    /// <response code="404">ManageUserApiKey not found.</response>
    /// <response code="500">Oops! Can't lookup your product right now.</response>
    [HttpPut("permissionsupdate/{id}")]
    [ProducesResponseType(typeof(Result<Guid>), 200)]
    [ProducesResponseType(404)]
    [ProducesResponseType(500)]
    [SwaggerHeader("tenant", "ManageUserApiKey", "Search", "Input your tenant to access this API i.e. admin for test", "admin", true)]
    [MustHavePermission(PermissionConstants.APIKeyPairs.Update)]
    public async Task<IActionResult> UpdatePermissionAsync(UpdateAPIKeyPairPermissionRequest request, Guid id)
    {
        return Ok(await _service.UpdateAPIKeyPairPermissionsAsync(request, id));
    }

    /// <summary>
    /// Delete a specific ManageUserApiKey by unique id.
    /// </summary>
    /// <response code="200">ManageUserApiKey deleted.</response>
    /// <response code="404">ManageUserApiKey not found.</response>
    /// <response code="500">Oops! Can't lookup your product right now.</response>
    [HttpDelete("{id}")]
    [ProducesResponseType(typeof(Result<Guid>), 200)]
    [ProducesResponseType(404)]
    [ProducesResponseType(500)]
    [SwaggerHeader("tenant", "ManageUserApiKey", "Search", "Input your tenant to access this API i.e. admin for test", "admin", true)]
    [MustHavePermission(PermissionConstants.APIKeyPairs.Remove)]
    public async Task<IActionResult> DeleteAsync(Guid id)
    {
        var aPIKeyPairId = await _service.DeleteAPIKeyPairAsync(id);
        return Ok(aPIKeyPairId);
    }
}