using Microsoft.AspNetCore.Mvc;
using MyReliableSite.Application.Categories.Interfaces;
using MyReliableSite.Domain.Constants;
using MyReliableSite.Infrastructure.Identity.Permissions;
using MyReliableSite.Infrastructure.Swagger;
using MyReliableSite.Shared.DTOs.Categories;
using Swashbuckle.AspNetCore.Annotations;

namespace MyReliableSite.Client.API.Controllers.Catalog;

public class CategoriesController : BaseController
{
    private readonly ICategoryService _service;

    public CategoriesController(ICategoryService service)
    {
        _service = service;
    }

    [HttpPost("search")]
    [SwaggerHeader("tenant,gen-api-key", "Articles", "Search", "Input your tenant to access this API i.e. admin for test", "admin", true, true)]
    [MustHavePermission(PermissionConstants.Categories.Search)]
    [SwaggerOperation(Summary = "Search Categories using available Filters.")]
    public async Task<IActionResult> SearchAsync(CategoryListFilter filter)
    {
        var categories = await _service.SearchAsync(filter);
        return Ok(categories);
    }

    [HttpPost("parents/search")]
    [SwaggerHeader("tenant,gen-api-key", "Articles", "Search", "Input your tenant to access this API i.e. admin for test", "admin", true, true)]
    [MustHavePermission(PermissionConstants.Categories.Search)]
    [SwaggerOperation(Summary = "Search Parent Categories using available Filters.")]
    public async Task<IActionResult> SearchParentsAsync(CategoryListFilter filter)
    {
        var categories = await _service.SearchParentsAsync(filter);
        return Ok(categories);
    }

    [HttpPost("parents/{id}/childcatergories")]
    [SwaggerHeader("tenant,gen-api-key", "Articles", "Search", "Input your tenant to access this API i.e. admin for test", "admin", true, true)]
    [MustHavePermission(PermissionConstants.Categories.Search)]
    [SwaggerOperation(Summary = "Search Child Categories using available Filters.")]
    public async Task<IActionResult> SearchChildCategoriesAsync(CategoryListFilter filter, Guid id)
    {
        var categories = await _service.SearchChildrenCategoryAsync(filter, id);
        return Ok(categories);
    }

    [HttpPost]
    [SwaggerHeader("tenant,gen-api-key", "Articles", "Search", "Input your tenant to access this API i.e. admin for test", "admin", true, true)]
    [MustHavePermission(PermissionConstants.Categories.Register)]
    public async Task<IActionResult> CreateAsync(CreateCategoryRequest request)
    {
        return Ok(await _service.CreateCategoryAsync(request));
    }

    [HttpPut("{id}")]
    [SwaggerHeader("tenant,gen-api-key", "Articles", "Search", "Input your tenant to access this API i.e. admin for test", "admin", true, true)]
    [MustHavePermission(PermissionConstants.Categories.Update)]
    public async Task<IActionResult> UpdateAsync(UpdateCategoryRequest request, Guid id)
    {
        return Ok(await _service.UpdateCategoryAsync(request, id));
    }

    [HttpDelete("{id}")]
    [SwaggerHeader("tenant,gen-api-key", "Articles", "Search", "Input your tenant to access this API i.e. admin for test", "admin", true, true)]
    [MustHavePermission(PermissionConstants.Categories.Remove)]
    public async Task<IActionResult> DeleteAsync(Guid id)
    {
        var categoryId = await _service.DeleteCategoryAsync(id);
        return Ok(categoryId);
    }
}