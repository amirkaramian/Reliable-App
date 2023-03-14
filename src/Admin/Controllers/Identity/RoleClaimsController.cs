using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MyReliableSite.Application.Abstractions.Services.Identity;
using MyReliableSite.Application.Wrapper;
using MyReliableSite.Domain.Constants;
using MyReliableSite.Infrastructure.Swagger;
using MyReliableSite.Shared.DTOs.Identity;

namespace MyReliableSite.Admin.API.Controllers.Identity;

[ApiController]
[Route("api/[controller]")]
public class RoleClaimsController : ControllerBase
{
    private readonly IRoleClaimsService _roleClaimService;

    public RoleClaimsController(IRoleClaimsService roleClaimService)
    {
        _roleClaimService = roleClaimService;
    }

    /// <summary>
    /// List of records with pagination &amp; filters.
    /// </summary>
    /// <response code="200">RoleClaims List returns.</response>
    /// <response code="400">RoleClaim not found.</response>
    /// <response code="500">Oops! Can't lookup your record right now.</response>
    [ProducesResponseType(typeof(PaginatedResult<List<RoleClaimResponse>>), 200)]
    [ProducesResponseType(typeof(IDictionary<string, string>), 400)]
    [ProducesResponseType(500)]
    [Authorize(Policy = PermissionConstants.RoleClaims.View)]
    [HttpGet]
    [SwaggerHeader("tenant", "Identity", "View", "Input your tenant to access this API i.e. admin for test", "admin", true, false, false)]
    public async Task<IActionResult> GetAllAsync()
    {
        var roleClaims = await _roleClaimService.GetAllAsync();
        return Ok(roleClaims);
    }

    /// <summary>
    /// retrive the RoleClaim against specific id.
    /// </summary>
    /// <response code="200">RoleClaim returns.</response>
    /// <response code="400">RoleClaim not found.</response>
    /// <response code="500">Oops! Can't lookup your record right now.</response>
    [ProducesResponseType(typeof(Result<List<RoleClaimResponse>>), 200)]
    [ProducesResponseType(typeof(IDictionary<string, string>), 400)]
    [ProducesResponseType(500)]
    [Authorize(Policy = PermissionConstants.RoleClaims.View)]
    [HttpGet("{roleId}")]
    [SwaggerHeader("tenant", "Identity", "View", "Input your tenant to access this API i.e. admin for test", "admin", true, false, false)]
    public async Task<IActionResult> GetAllByRoleIdAsync([FromRoute] string roleId)
    {
        var response = await _roleClaimService.GetAllByRoleIdAsync(roleId);
        return Ok(response);
    }

    /// <summary>
    /// Create an RoleClaim.
    /// </summary>
    /// <response code="200">RoleClaim created.</response>
    /// <response code="400">RoleClaim already exists.</response>
    /// <response code="500">Oops! Can't lookup your product right now.</response>
    [HttpPost]
    [ProducesResponseType(typeof(Result<Guid>), 200)]
    [ProducesResponseType(typeof(IDictionary<string, string>), 400)]
    [ProducesResponseType(500)]
    [Authorize(Policy = PermissionConstants.RoleClaims.Create)]
    [HttpPost]
    [SwaggerHeader("tenant", "Identity", "Create", "Input your tenant to access this API i.e. admin for test", "admin", true, false, false)]
    public async Task<IActionResult> PostAsync(RoleClaimRequest request)
    {
        var response = await _roleClaimService.SaveAsync(request);
        return Ok(response);
    }

    /// <summary>
    /// Delete a specific RoleClaim by unique id.
    /// </summary>
    /// <response code="200">RoleClaim deleted.</response>
    /// <response code="404">RoleClaim not found.</response>
    /// <response code="500">Oops! Can't lookup your product right now.</response>
    [HttpDelete("{id}")]
    [ProducesResponseType(typeof(Result<string>), 200)]
    [ProducesResponseType(404)]
    [ProducesResponseType(500)]
    [Authorize(Policy = PermissionConstants.RoleClaims.Delete)]
    [SwaggerHeader("tenant", "Identity", "Delete", "Input your tenant to access this API i.e. admin for test", "admin", true, false, false)]
    public async Task<IActionResult> DeleteAsync(int id)
    {
        var response = await _roleClaimService.DeleteAsync(id);
        return Ok(response);
    }
}