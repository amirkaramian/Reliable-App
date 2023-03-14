using Microsoft.AspNetCore.Mvc;

using MyReliableSite.Application.Brands.Interfaces;
using MyReliableSite.Application.Wrapper;
using MyReliableSite.Domain.Constants;
using MyReliableSite.Infrastructure.Identity.Permissions;
using MyReliableSite.Infrastructure.Swagger;
using MyReliableSite.Shared.DTOs.Brands;
using MyReliableSite.Shared.DTOs.Identity;

namespace MyReliableSite.Admin.API.Controllers.Brands;

public class BrandsController : BaseController
{
    private readonly IBrandService _brandService;

    public BrandsController(IBrandService brandService)
    {
        _brandService = brandService ?? throw new ArgumentNullException(nameof(brandService));
    }

    /// <summary>
    /// retrive the Brand against specific id.
    /// </summary>
    /// <response code="200">Brand returns.</response>
    /// <response code="400">Brand not found.</response>
    /// <response code="500">Oops! Can't lookup your record right now.</response>
    [ProducesResponseType(typeof(Result<BrandDto>), 200)]
    [ProducesResponseType(typeof(IDictionary<string, string>), 400)]
    [ProducesResponseType(500)]
    [HttpGet("{id:guid}")]
    [MustHavePermission(PermissionConstants.Brands.Read)]
    [SwaggerHeader("tenant", "Brands", "Read", "Input your tenant to access this API i.e. admin for test", "admin", true)]
    public async Task<IActionResult> GetAsync(Guid id) => Ok(await _brandService.GetAsync(id));

    /// <summary>
    /// retrive the Department against specific id.
    /// </summary>
    /// <response code="200">Department returns.</response>
    /// <response code="400">Department not found.</response>
    /// <response code="500">Oops! Can't lookup your record right now.</response>
    [HttpGet("getbrandsusers/{id}")]
    [ProducesResponseType(typeof(Result<UserDetailsDto>), 200)]
    [ProducesResponseType(typeof(IDictionary<string, string>), 400)]
    [ProducesResponseType(500)]
    [SwaggerHeader("tenant", "Setting", "View", "Input your tenant to access this API i.e. admin for test", "admin", true)]
    [MustHavePermission(PermissionConstants.Departments.View)]
    public async Task<IActionResult> GetBrandUsersAsync(Guid id)
    {
        var users = await _brandService.GetBrandUsersAsync(id);
        return Ok(users);
    }

    /// <summary>
    /// List of records with pagination &amp; filters.
    /// </summary>
    /// <response code="200">Brands List returns.</response>
    /// <response code="400">Brand not found.</response>
    /// <response code="500">Oops! Can't lookup your record right now.</response>
    [HttpPost("search")]
    [ProducesResponseType(typeof(PaginatedResult<BrandDto>), 200)]
    [ProducesResponseType(typeof(IDictionary<string, string>), 400)]
    [ProducesResponseType(500)]
    [MustHavePermission(PermissionConstants.Brands.Search)]
    [SwaggerHeader("tenant", "Brands", "Search", "Input your tenant to access this API i.e. admin for test", "admin", true)]
    public async Task<IActionResult> SearchAsync(BrandListFilter filter)
    {
        return Ok(await _brandService.SearchAsync(filter));
    }

    /// <summary>
    /// Create an Brand.
    /// </summary>
    /// <response code="200">Brand created.</response>
    /// <response code="400">Brand already exists.</response>
    /// <response code="500">Oops! Can't lookup your product right now.</response>
    [HttpPost]
    [ProducesResponseType(typeof(Result<BrandDto>), 200)]
    [ProducesResponseType(typeof(IDictionary<string, string>), 400)]
    [ProducesResponseType(500)]
    [MustHavePermission(PermissionConstants.Brands.Create)]
    [SwaggerHeader("tenant", "Brands", "Create", "Input your tenant to access this API i.e. admin for test", "admin", true)]
    public async Task<IActionResult> CreateAsync(CreateBrandRequest request)
    {
        return Ok(await _brandService.CreateAsync(request));
    }

    /// <summary>
    /// update a specific Brand by unique id.
    /// </summary>
    /// <response code="200">Brand updated.</response>
    /// <response code="404">Brand not found.</response>
    /// <response code="500">Oops! Can't lookup your product right now.</response>
    [ProducesResponseType(typeof(Result<BrandDto>), 200)]
    [ProducesResponseType(404)]
    [ProducesResponseType(500)]
    [HttpPut("{id:guid}")]
    [MustHavePermission(PermissionConstants.Brands.Update)]
    [SwaggerHeader("tenant", "Brands", "Update", "Input your tenant to access this API i.e. admin for test", "admin", true)]
    public async Task<IActionResult> UpdateAsync(UpdateBrandRequest request, Guid id)
    {
        return Ok(await _brandService.UpdateAsync(id, request));
    }

    /// <summary>
    /// Delete a specific Brand by unique id.
    /// </summary>
    /// <response code="200">Brand deleted.</response>
    /// <response code="404">Brand not found.</response>
    /// <response code="500">Oops! Can't lookup your product right now.</response>
    [ProducesResponseType(typeof(Result<bool>), 200)]
    [ProducesResponseType(404)]
    [ProducesResponseType(500)]
    [HttpDelete("{id:guid}")]
    [MustHavePermission(PermissionConstants.Brands.Delete)]
    [SwaggerHeader("tenant", "Brands", "Delete", "Input your tenant to access this API i.e. admin for test", "admin", true)]
    public async Task<IActionResult> DeleteAsync(Guid id) => Ok(await _brandService.DeleteAsync(id));
}
