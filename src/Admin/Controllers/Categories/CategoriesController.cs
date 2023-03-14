using Microsoft.AspNetCore.Mvc;
using MyReliableSite.Application.Categories.Interfaces;
using MyReliableSite.Application.Wrapper;
using MyReliableSite.Domain.Constants;
using MyReliableSite.Infrastructure.Identity.Permissions;
using MyReliableSite.Infrastructure.Swagger;
using MyReliableSite.Shared.DTOs.Categories;
using Swashbuckle.AspNetCore.Annotations;

namespace MyReliableSite.Admin.API.Controllers.Categories;

public class CategoriesController : BaseController
{
    private readonly ICategoryService _service;

    public CategoriesController(ICategoryService service)
    {
        _service = service;
    }

    /// <summary>
    /// List of records with pagination &amp; filters.
    /// </summary>
    /// <response code="200">Categories List returns.</response>
    /// <response code="400">Category not found.</response>
    /// <response code="500">Oops! Can't lookup your record right now.</response>
    [HttpPost("search")]
    [ProducesResponseType(typeof(PaginatedResult<CategoryDto>), 200)]
    [ProducesResponseType(typeof(IDictionary<string, string>), 400)]
    [ProducesResponseType(500)]
    [SwaggerHeader("tenant", "Categories", "Search", "Input your tenant to access this API i.e. admin for test", "admin", true)]
    [MustHavePermission(PermissionConstants.Articles.Search)]
    [SwaggerOperation(Summary = "Search Categories using available Filters.")]
    public async Task<IActionResult> SearchAsync(CategoryListFilter filter)
    {
        var categories = await _service.SearchAsync(filter);
        return Ok(categories);
    }

    /// <summary>
    /// retrive the Category against specific id.
    /// </summary>
    /// <response code="200">Category returns.</response>
    /// <response code="400">Category not found.</response>
    /// <response code="500">Oops! Can't lookup your record right now.</response>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(Result<CategoryDto>), 200)]
    [ProducesResponseType(typeof(IDictionary<string, string>), 400)]
    [ProducesResponseType(500)]
    [SwaggerHeader("tenant", "Categories", "Search", "Input your tenant to access this API i.e. admin for test", "admin", true)]
    [MustHavePermission(PermissionConstants.Articles.View)]
    public async Task<IActionResult> GetAsync(Guid id)
    {
        return Ok(await _service.GetCategoryAsync(id));
    }

    /// <summary>
    /// Create an Category.
    /// </summary>
    /// <response code="200">Category created.</response>
    /// <response code="400">Category already exists.</response>
    /// <response code="500">Oops! Can't lookup your product right now.</response>
    [HttpPost]
    [ProducesResponseType(typeof(Result<Guid>), 200)]
    [ProducesResponseType(typeof(IDictionary<string, string>), 400)]
    [ProducesResponseType(500)]
    [SwaggerHeader("tenant", "Categories", "Create", "Input your tenant to access this API i.e. admin for test", "", true)]
    [MustHavePermission(PermissionConstants.Articles.Create)]
    public async Task<IActionResult> CreateAsync(CreateCategoryRequest request)
    {
        return Ok(await _service.CreateCategoryAsync(request));
    }

    /// <summary>
    /// update a specific Category by unique id.
    /// </summary>
    /// <response code="200">Category updated.</response>
    /// <response code="404">Category not found.</response>
    /// <response code="500">Oops! Can't lookup your product right now.</response>
    [HttpPut("{id}")]
    [ProducesResponseType(typeof(Result<Guid>), 200)]
    [ProducesResponseType(404)]
    [ProducesResponseType(500)]
    [SwaggerHeader("tenant", "Categories", "Update", "Input your tenant to access this API i.e. admin for test", "", true)]
    [MustHavePermission(PermissionConstants.Articles.Update)]
    public async Task<IActionResult> UpdateAsync(UpdateCategoryRequest request, Guid id)
    {
        return Ok(await _service.UpdateCategoryAsync(request, id));
    }

    /// <summary>
    /// Delete a specific Category by unique id.
    /// </summary>
    /// <response code="200">Category deleted.</response>
    /// <response code="404">Category not found.</response>
    /// <response code="500">Oops! Can't lookup your product right now.</response>
    [HttpDelete("{id}")]
    [ProducesResponseType(typeof(Result<Guid>), 200)]
    [ProducesResponseType(404)]
    [ProducesResponseType(500)]
    [SwaggerHeader("tenant", "Categories", "Remove", "Input your tenant to access this API i.e. admin for test", "", true)]
    [MustHavePermission(PermissionConstants.Articles.Remove)]
    public async Task<IActionResult> DeleteAsync(Guid id)
    {
        var categoryId = await _service.DeleteCategoryAsync(id);
        return Ok(categoryId);
    }

}
