using Microsoft.AspNetCore.Mvc;
using MyReliableSite.Application.ArticleFeedbacks.Interfaces;
using MyReliableSite.Application.Wrapper;
using MyReliableSite.Domain.Constants;
using MyReliableSite.Infrastructure.Identity.Permissions;
using MyReliableSite.Infrastructure.Swagger;
using MyReliableSite.Shared.DTOs.ArticleFeedbacks;

namespace MyReliableSite.Client.API.Controllers.ArticleFeedbacks;
[Route("api/[controller]")]
[ApiController]
public class ArticleFeedbackCommentRepliesController : BaseController
{
    private readonly IArticleFeedbackCommentReplyService _service;
    public ArticleFeedbackCommentRepliesController(IArticleFeedbackCommentReplyService service)
    {
        _service = service;
    }

    /// <summary>
    /// retrive the article feedback against specific id.
    /// </summary>
    /// <response code="200">Article feedback returns.</response>
    /// <response code="400">Article feedback not found.</response>
    /// <response code="500">Oops! Can't lookup your article feedback right now.</response>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(Result<ArticleFeedbackCommentReplyDto>), 200)]
    [ProducesResponseType(typeof(IDictionary<string, string>), 400)]
    [ProducesResponseType(500)]
    [SwaggerHeader("tenant", "ArticleFeedbacks", "View", "Input your tenant to access this API i.e. admin for test", "", true)]
    [MustHavePermission(PermissionConstants.ArticleFeedbacks.View)]
    public async Task<IActionResult> GetAsync(Guid id)
    {
        var department = await _service.GetArticleFeedbackCommentReplyAsync(id);
        return Ok(department);
    }

    /// <summary>
    /// Create an Article feedback.
    /// </summary>
    /// <response code="200">Article feedback created.</response>
    /// <response code="400">Article feedback already exists.</response>
    /// <response code="500">Oops! Can't lookup your article feedback right now.</response>
    [HttpPost]
    [ProducesResponseType(typeof(Result<Guid>), 200)]
    [ProducesResponseType(typeof(IDictionary<string, string>), 400)]
    [ProducesResponseType(500)]
    [HttpPost]
    [SwaggerHeader("tenant", "ArticleFeedbacks", "Create", "Input your tenant to access this API i.e. admin for test", "", true)]
    [MustHavePermission(PermissionConstants.ArticleFeedbacks.Create)]
    public async Task<IActionResult> CreateAsync(CreateArticleFeedbackCommentReplyRequest request)
    {
        return Ok(await _service.CreateArticleFeedbackCommentReplyAsync(request));
    }

    /// <summary>
    /// update a specific article by unique id.
    /// </summary>
    /// <response code="200">Article feedback updated.</response>
    /// <response code="404">Article feedback not found.</response>
    /// <response code="500">Oops! Can't lookup your product right now.</response>
    [HttpPut("{id}")]
    [SwaggerHeader("tenant", "ArticleFeedbacks", "Update", "Input your tenant to access this API i.e. admin for test", "", true)]
    [MustHavePermission(PermissionConstants.ArticleFeedbacks.Update)]
    public async Task<IActionResult> UpdateAsync(UpdateArticleFeedbackCommentReplyRequest request, Guid id)
    {
        return Ok(await _service.UpdateArticleFeedbackCommentReplyAsync(request, id));
    }

    /// <summary>
    /// Delete a specific article by unique id.
    /// </summary>
    /// <response code="200">Article ArticleFeedbacks deleted.</response>
    /// <response code="404">Article ArticleFeedbacks not found.</response>
    /// <response code="500">Oops! Can't lookup your ArticleFeedbacks right now.</response>
    [HttpDelete("{id}")]
    [SwaggerHeader("tenant", "ArticleFeedbacks", "Remove", "Input your tenant to access this API i.e. admin for test", "", true)]
    [MustHavePermission(PermissionConstants.ArticleFeedbacks.Remove)]
    public async Task<IActionResult> DeleteAsync(Guid id)
    {
        var departmentId = await _service.DeleteArticleFeedbackCommentReplyAsync(id);
        return Ok(departmentId);
    }
}