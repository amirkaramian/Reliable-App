using Microsoft.AspNetCore.Mvc;
using MyReliableSite.Application.ManageUserApiKey.Interfaces;
using MyReliableSite.Application.Wrapper;
using MyReliableSite.Domain.Constants;
using MyReliableSite.Infrastructure.Identity.Permissions;
using MyReliableSite.Infrastructure.Swagger;
using MyReliableSite.Shared.DTOs.ManageUserApiKey;
using Swashbuckle.AspNetCore.Annotations;

namespace MyReliableSite.Client.API.Controllers.ManageUserApiKey;

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
    [SwaggerHeader("tenant", "ManageUserApiKey", "Search", "Input your tenant to access this API i.e. admin for test", "admin", true, true)]
    [MustHavePermission(PermissionConstants.APIKeyPairs.Search)]
    [SwaggerOperation(Summary = "Search APIKeyPairs using available Filters.")]
    public async Task<IActionResult> SearchAsync(APIKeyPairListFilter filter)
    {
        var aPIKeyPairs = await _service.SearchAsync(filter);
        return Ok(aPIKeyPairs);
    }
}
