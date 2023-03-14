using Microsoft.AspNetCore.Mvc;
using MyReliableSite.Application.KnowledgeBase.Interfaces;
using MyReliableSite.Application.Wrapper;
using MyReliableSite.Domain.Constants;
using MyReliableSite.Infrastructure.Identity.Permissions;
using MyReliableSite.Infrastructure.Swagger;
using MyReliableSite.Shared.DTOs.KnowledgeBase;

namespace MyReliableSite.Client.API.Controllers.Catalog;

public class ArticlesController : BaseController
{
    private readonly IArticleService _service;

    public ArticlesController(IArticleService service)
    {
        _service = service;
    }

    /// <summary>
    /// retrive the article against specific id.
    /// </summary>
    /// <response code="200">Article returns.</response>
    /// <response code="400">Article not found.</response>
    /// <response code="500">Oops! Can't lookup your record right now.</response>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(Result<ArticleDto>), 200)]
    [ProducesResponseType(typeof(IDictionary<string, string>), 400)]
    [ProducesResponseType(500)]
    [SwaggerHeader("tenant,gen-api-key", "Articles", "View", "Input your tenant to access this API i.e. client for test", "admin", true, true)]
    [MustHavePermission(PermissionConstants.Articles.View)]
    public async Task<IActionResult> GetAsync(Guid id)
    {
        var article = await _service.GetArticleDetailsAsync(id);
        return Ok(article);
    }

    /// <summary>
    /// List of records with pagination &amp; filters.
    /// </summary>
    /// <response code="200">Articles List returns.</response>
    /// <response code="400">Article not found.</response>
    /// <response code="500">Oops! Can't lookup your record right now.</response>
    [HttpPost("search")]
    [ProducesResponseType(typeof(PaginatedResult<ArticleDto>), 200)]
    [ProducesResponseType(typeof(IDictionary<string, string>), 400)]
    [ProducesResponseType(500)]
    [SwaggerHeader("tenant,gen-api-key", "Articles", "Search", "Input your tenant to access this API i.e. admin for test", "client", true, true)]
    [MustHavePermission(PermissionConstants.Articles.Search)]
    public async Task<IActionResult> SearchAsync(ArticleListFilter filter)
    {
        var articles = await _service.SearchAsync(filter);
        return Ok(articles);
    }

    /// <summary>
    /// List of records without pagination &amp; filters.
    /// </summary>
    /// <response code="200">Articles List returns.</response>
    /// <response code="400">Article not found.</response>
    /// <response code="500">Oops! Can't lookup your record right now.</response>
    [HttpPost("usersubmissions/currentuser")]
    [ProducesResponseType(typeof(PaginatedResult<ArticleDto>), 200)]
    [ProducesResponseType(typeof(IDictionary<string, string>), 400)]
    [ProducesResponseType(500)]
    [SwaggerHeader("tenant", "Articles", "Search", "Input your tenant to access this API i.e. admin for test", "admin", true)]
    [MustHavePermission(PermissionConstants.Articles.Search)]
    public async Task<IActionResult> UserSubmissionsAsync(ArticleListFilter filter)
    {
        var articles = await _service.SearchUserSubmissionsAsync(filter);
        return Ok(articles);
    }

    /// <summary>
    /// Create an Article.
    /// </summary>
    /// <response code="200">Article created.</response>
    /// <response code="400">Article already exists.</response>
    /// <response code="500">Oops! Can't lookup your product right now.</response>
    [HttpPost]
    [ProducesResponseType(typeof(Result<Guid>), 200)]
    [ProducesResponseType(typeof(IDictionary<string, string>), 400)]
    [ProducesResponseType(500)]
    [SwaggerHeader("tenant,gen-api-key", "Articles", "Create", "Input your tenant to access this API i.e. admin for test", "admin", true, true)]
    [MustHavePermission(PermissionConstants.Articles.Create)]
    public async Task<IActionResult> SubmitAsync(CreateArticleRequest request)
    {
        return Ok(await _service.SubmitArticleAsync(request));
    }

    /// <summary>
    /// update a specific article by unique id.
    /// </summary>
    /// <response code="200">Article updated.</response>
    /// <response code="404">Article not found.</response>
    /// <response code="500">Oops! Can't lookup your product right now.</response>
    [HttpPut("{id}")]
    [ProducesResponseType(typeof(Result<Guid>), 200)]
    [ProducesResponseType(404)]
    [ProducesResponseType(500)]
    [SwaggerHeader("tenant,gen-api-key", "Articles", "Update", "Input your tenant to access this API i.e. admin for test", "admin", true, true)]
    [MustHavePermission(PermissionConstants.Articles.Update)]
    public async Task<IActionResult> UpdateAsync(UpdateArticleRequest request, Guid id)
    {
        return Ok(await _service.UpdateArticleAsync(request, id));
    }

    /// <summary>
    /// Delete a specific article by unique id.
    /// </summary>
    /// <response code="200">Article deleted.</response>
    /// <response code="404">Article not found.</response>
    /// <response code="500">Oops! Can't lookup your product right now.</response>
    [HttpDelete("{id}")]
    [ProducesResponseType(typeof(Result<Guid>), 200)]
    [ProducesResponseType(404)]
    [ProducesResponseType(500)]
    [SwaggerHeader("tenant,gen-api-key", "Articles", "Remove", "Input your tenant to access this API i.e. admin for test", "admin", true, true)]
    [MustHavePermission(PermissionConstants.Articles.Remove)]
    public async Task<IActionResult> DeleteAsync(Guid id)
    {
        var articleId = await _service.DeleteArticleAsync(id);
        return Ok(articleId);
    }
}
