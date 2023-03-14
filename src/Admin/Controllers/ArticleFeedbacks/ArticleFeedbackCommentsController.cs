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
public class ArticleFeedbackCommentsController : BaseController
{
    private readonly IArticleFeedbackCommentService _service;
    private readonly IConfiguration _config;
    public ArticleFeedbackCommentsController(IArticleFeedbackCommentService service, IConfiguration config)
    {
        _service = service;
        _config = config;
    }

    [HttpPost("search")]
    [ProducesResponseType(typeof(PaginatedResult<ArticleFeedbackCommentDto>), 200)]
    [ProducesResponseType(typeof(IDictionary<string, string>), 400)]
    [ProducesResponseType(500)]
    [SwaggerHeader("tenant", "ArticleFeedbacks", "Search", "Input your tenant to access this API i.e. admin for test", "", true)]
    [MustHavePermission(PermissionConstants.ArticleFeedbacks.Search)]
    [SwaggerOperation(Summary = "Search ArticleFeedbackComments using available Filters.")]
    public async Task<IActionResult> SearchAsync(ArticleFeedbackCommentListFilter filter)
    {
        var departments = await _service.SearchAsync(filter);
        return Ok(departments);
    }

    /// <summary>
    /// retrive the article against specific id.
    /// </summary>
    /// <response code="200">Article feedback returns.</response>
    /// <response code="400">Article feedback not found.</response>
    /// <response code="500">Oops! Can't lookup your record right now.</response>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(Result<ArticleFeedbackCommentDto>), 200)]
    [ProducesResponseType(typeof(IDictionary<string, string>), 400)]
    [ProducesResponseType(500)]
    [SwaggerHeader("tenant", "ArticleFeedbacks", "View", "Input your tenant to access this API i.e. admin for test", "", true)]
    [MustHavePermission(PermissionConstants.ArticleFeedbacks.View)]
    public async Task<IActionResult> GetAsync(Guid id)
    {
        var department = await _service.GetArticleFeedbackCommentAsync(id);
        return Ok(department);
    }

    /// <summary>
    /// Create an Article.
    /// </summary>
    /// <response code="200">Article feedback created.</response>
    /// <response code="400">Article feedback already exists.</response>
    /// <response code="500">Oops! Can't lookup your product right now.</response>
    [HttpPost]
    [ProducesResponseType(typeof(Result<Guid>), 200)]
    [ProducesResponseType(typeof(IDictionary<string, string>), 400)]
    [ProducesResponseType(500)]
    [SwaggerHeader("tenant", "ArticleFeedbacks", "Search", "Input your tenant to access this API i.e. admin for test", "", true)]
    [MustHavePermission(PermissionConstants.ArticleFeedbacks.Create)]
    public async Task<IActionResult> CreateAsync(CreateArticleFeedbackCommentRequest request)
    {
        return Ok(await _service.CreateArticleFeedbackCommentAsync(request));
    }

    /// <summary>
    /// update a specific article feedback by unique id.
    /// </summary>
    /// <response code="200">Article feedback updated.</response>
    /// <response code="404">Article feedback not found.</response>
    /// <response code="500">Oops! Can't lookup your product right now.</response>
    [HttpPut("{id}")]
    [ProducesResponseType(typeof(Result<Guid>), 200)]
    [ProducesResponseType(404)]
    [ProducesResponseType(500)]
    [SwaggerHeader("tenant", "ArticleFeedbacks", "Update", "Input your tenant to access this API i.e. admin for test", "", true)]
    [MustHavePermission(PermissionConstants.ArticleFeedbacks.Update)]
    public async Task<IActionResult> UpdateAsync(UpdateArticleFeedbackCommentRequest request, Guid id)
    {
        return Ok(await _service.UpdateArticleFeedbackCommentAsync(request, id));
    }

    /// <summary>
    /// Delete a specific article feedback by unique id.
    /// </summary>
    /// <response code="200">Article feedback deleted.</response>
    /// <response code="404">Article feedback not found.</response>
    /// <response code="500">Oops! Can't lookup your record right now.</response>
    [HttpDelete("{id}")]
    [ProducesResponseType(typeof(Result<Guid>), 200)]
    [ProducesResponseType(404)]
    [ProducesResponseType(500)]
    [SwaggerHeader("tenant", "ArticleFeedbacks", "Remove", "Input your tenant to access this API i.e. admin for test", "", true)]
    [MustHavePermission(PermissionConstants.ArticleFeedbacks.Remove)]
    public async Task<IActionResult> DeleteAsync(Guid id)
    {
        var departmentId = await _service.DeleteArticleFeedbackCommentAsync(id);
        return Ok(departmentId);
    }
}