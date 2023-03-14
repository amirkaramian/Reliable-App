using Microsoft.AspNetCore.Mvc;
using MyReliableSite.Application.ArticleFeedbacks.Interfaces;
using MyReliableSite.Application.Wrapper;
using MyReliableSite.Domain.Constants;
using MyReliableSite.Infrastructure.Identity.Permissions;
using MyReliableSite.Infrastructure.Swagger;
using MyReliableSite.Shared.DTOs.ArticleFeedbacks;
using Swashbuckle.AspNetCore.Annotations;

namespace MyReliableSite.Admin.API.Controllers.ArticleFeedbacks;
[Route("api/[controller]")]
[ApiController]
public class ArticleFeedbacksController : BaseController
{
    private readonly IArticleFeedbackService _service;
    public ArticleFeedbacksController(IArticleFeedbackService service)
    {
        _service = service;
    }

    /// <summary>
    /// List of records with pagination &amp; filters.
    /// </summary>
    /// <response code="200">Articles Feedback List returns.</response>
    /// <response code="400">Article Feedback not found.</response>
    /// <response code="500">Oops! Can't lookup your record right now.</response>
    [HttpPost("search")]
    [ProducesResponseType(typeof(PaginatedResult<ArticleFeedbackDto>), 200)]
    [ProducesResponseType(typeof(IDictionary<string, string>), 400)]
    [ProducesResponseType(500)]
    [SwaggerHeader("tenant", "ArticleFeedbacks", "Search", "Input your tenant to access this API i.e. admin for test", "admin", true)]
    [MustHavePermission(PermissionConstants.ArticleFeedbacks.Search)]
    [SwaggerOperation(Summary = "Search ArticleFeedbacks using available Filters.")]
    public async Task<IActionResult> SearchAsync(ArticleFeedbackListFilter filter)
    {
        var articles = await _service.SearchAsync(filter);
        return Ok(articles);
    }

    /// <summary>
    /// retrive the article feedback against specific id.
    /// </summary>
    /// <response code="200">Article Feedback returns.</response>
    /// <response code="400">Article Feedback not found.</response>
    /// <response code="500">Oops! Can't lookup your record right now.</response>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(Result<ArticleFeedbackDto>), 200)]
    [ProducesResponseType(typeof(IDictionary<string, string>), 400)]
    [ProducesResponseType(500)]
    [SwaggerHeader("tenant", "ArticleFeedbacks", "View", "Input your tenant to access this API i.e. admin for test", "admin", true)]
    [MustHavePermission(PermissionConstants.ArticleFeedbacks.View)]
    public async Task<IActionResult> GetAsync(Guid id)
    {
        var article = await _service.GetArticleFeedbackAsync(id);
        return Ok(article);
    }

    /// <summary>
    /// retrive the amount pending article feedback.
    /// </summary>
    /// <response code="200">Article Feedback returns.</response>
    /// <response code="400">Article Feedback not found.</response>
    /// <response code="500">Oops! Can't lookup your record right now.</response>
    [HttpGet("pending/count")]
    [ProducesResponseType(typeof(Result<ArticleFeedbackDto>), 200)]
    [ProducesResponseType(typeof(IDictionary<string, string>), 400)]
    [ProducesResponseType(500)]
    [SwaggerHeader("tenant", "ArticleFeedbacks", "View", "Input your tenant to access this API i.e. admin for test", "admin", true)]
    [MustHavePermission(PermissionConstants.ArticleFeedbacks.View)]
    public async Task<IActionResult> GetPendingFeedbacksCount()
    {
       var count = await _service.PendingFeedbackCount();
       return Ok(count);
    }

    /// <summary>
    /// retrive the article feedback against specific article id.
    /// </summary>
    /// <response code="200">Article Feedback returns.</response>
    /// <response code="400">Article Feedback not found.</response>
    /// <response code="500">Oops! Can't lookup your record right now.</response>
    [HttpGet("getarticlefeedbackagainstarticle/{id}")]
    [ProducesResponseType(typeof(Result<List<ArticleFeedbackDto>>), 200)]
    [ProducesResponseType(typeof(IDictionary<string, string>), 400)]
    [ProducesResponseType(500)]
    [SwaggerHeader("tenant", "ArticleFeedbacks", "View", "Input your tenant to access this API i.e. admin for test", "admin", true)]
    [MustHavePermission(PermissionConstants.ArticleFeedbacks.View)]
    public async Task<IActionResult> GetArticleFeedbackAsync(string id)
    {
        var article = await _service.GetArticleFeedbackAgainstArticleAsync(id);
        return Ok(article);
    }

    /// <summary>
    /// Create an Article.
    /// </summary>
    /// <response code="200">Article Feedback created.</response>
    /// <response code="400">Article Feedback already exists.</response>
    /// <response code="500">Oops! Can't lookup your product right now.</response>
    [HttpPost]
    [ProducesResponseType(typeof(Result<Guid>), 200)]
    [ProducesResponseType(typeof(IDictionary<string, string>), 400)]
    [ProducesResponseType(500)]
    [SwaggerHeader("tenant", "ArticleFeedbacks", "Create", "Input your tenant to access this API i.e. admin for test", "admin", true)]
    [MustHavePermission(PermissionConstants.ArticleFeedbacks.Create)]
    public async Task<IActionResult> CreateAsync(CreateArticleFeedbackRequest request)
    {
        return Ok(await _service.CreateArticleFeedbackAsync(request));
    }

    /// <summary>
    /// update a specific article Feedback by unique id.
    /// </summary>
    /// <response code="200">Article Feedback updated.</response>
    /// <response code="404">Article Feedback not found.</response>
    /// <response code="500">Oops! Can't lookup your product right now.</response>
    [HttpPut("{id}")]
    [ProducesResponseType(typeof(Result<Guid>), 200)]
    [ProducesResponseType(404)]
    [ProducesResponseType(500)]
    [SwaggerHeader("tenant", "ArticleFeedbacks", "Update", "Input your tenant to access this API i.e. admin for test", "admin", true)]
    [MustHavePermission(PermissionConstants.ArticleFeedbacks.Update)]
    public async Task<IActionResult> UpdateAsync(UpdateArticleFeedbackRequest request, Guid id)
    {
        return Ok(await _service.UpdateArticleFeedbackAsync(request, id));
    }

    /// <summary>
    /// Delete a specific article Feedback by unique id.
    /// </summary>
    /// <response code="200">Article Feedback deleted.</response>
    /// <response code="404">Article Feedback not found.</response>
    /// <response code="500">Oops! Can't lookup your product right now.</response>
    [HttpDelete("{id}")]
    [ProducesResponseType(typeof(Result<Guid>), 200)]
    [ProducesResponseType(404)]
    [ProducesResponseType(500)]
    [SwaggerHeader("tenant", "ArticleFeedbacks", "Search", "Input your tenant to access this API i.e. admin for test", "admin", true)]
    [MustHavePermission(PermissionConstants.ArticleFeedbacks.Remove)]
    public async Task<IActionResult> DeleteAsync(Guid id)
    {
        var articleId = await _service.DeleteArticleFeedbackAsync(id);
        return Ok(articleId);
    }
}