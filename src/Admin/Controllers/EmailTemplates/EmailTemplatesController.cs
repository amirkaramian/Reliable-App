using Microsoft.AspNetCore.Mvc;
using MyReliableSite.Application.Common.Interfaces;
using MyReliableSite.Application.Wrapper;
using MyReliableSite.Domain.Constants;
using MyReliableSite.Infrastructure.Identity.Permissions;
using MyReliableSite.Infrastructure.Swagger;
using MyReliableSite.Shared.DTOs.EmailTemplates;

namespace MyReliableSite.Admin.API.Controllers.EmailTemplates;

public class EmailTemplatesController : BaseController
{
    private readonly IEmailTemplateService _emailTemplateService;

    public EmailTemplatesController(IEmailTemplateService emailTemplateService)
    {
        _emailTemplateService = emailTemplateService ?? throw new ArgumentNullException(nameof(emailTemplateService));
    }

    /// <summary>
    /// retrive the EmailTemplate against specific id.
    /// </summary>
    /// <response code="200">EmailTemplate returns.</response>
    /// <response code="400">EmailTemplate not found.</response>
    /// <response code="500">Oops! Can't lookup your record right now.</response>
    [ProducesResponseType(typeof(Result<EmailTemplateDto>), 200)]
    [ProducesResponseType(typeof(IDictionary<string, string>), 400)]
    [ProducesResponseType(500)]
    [HttpGet("{id:guid}")]
    [MustHavePermission(PermissionConstants.EmailTemplates.Read)]
    [SwaggerHeader("tenant", "EmailTemplates", "Read", "Input your tenant to access this API i.e. admin for test", "admin", true)]
    public async Task<IActionResult> GetAsync(Guid id) => Ok(await _emailTemplateService.GetAsync(id));

    /// <summary>
    /// retrive the EmailTemplate against specific id.
    /// </summary>
    /// <response code="200">EmailTemplate returns.</response>
    /// <response code="400">EmailTemplate not found.</response>
    /// <response code="500">Oops! Can't lookup your record right now.</response>
    [ProducesResponseType(typeof(Result<Dictionary<EmailTemplateType, string>>), 200)]
    [ProducesResponseType(typeof(IDictionary<string, string>), 400)]
    [ProducesResponseType(500)]
    [HttpGet("getEmailTemplateVariables")]
    [MustHavePermission(PermissionConstants.EmailTemplates.Read)]
    [SwaggerHeader("tenant", "EmailTemplates", "Read", "Input your tenant to access this API i.e. admin for test", "admin", true)]
    public IActionResult GetvariablesAsync()
    {
        return Ok(_emailTemplateService.GetTemplateVariables());
    }

    /// <summary>
    /// List of records with pagination &amp; filters.
    /// </summary>
    /// <response code="200">EmailTemplates List returns.</response>
    /// <response code="500">Oops! Can't lookup your record right now.</response>
    [ProducesResponseType(typeof(PaginatedResult<EmailTemplateDto>), 200)]
    [ProducesResponseType(500)]
    [HttpPost("search")]
    [MustHavePermission(PermissionConstants.EmailTemplates.Search)]
    [SwaggerHeader("tenant", "EmailTemplates", "Search", "Input your tenant to access this API i.e. admin for test", "admin", true)]
    public async Task<IActionResult> SearchAsync(EmailTemplateListFilter filter)
    {
        return Ok(await _emailTemplateService.SearchAsync(filter));
    }

    /// <summary>
    /// Create an EmailTemplate.
    /// </summary>
    /// <response code="200">EmailTemplate created.</response>
    /// <response code="400">EmailTemplate already exists.</response>
    /// <response code="500">Oops! Can't lookup your product right now.</response>
    [HttpPost]
    [ProducesResponseType(typeof(Result<EmailTemplateDto>), 200)]
    [ProducesResponseType(typeof(IDictionary<string, string>), 400)]
    [ProducesResponseType(500)]
    [HttpPost]
    [MustHavePermission(PermissionConstants.EmailTemplates.Create)]
    [SwaggerHeader("tenant", "EmailTemplates", "Create", "Input your tenant to access this API i.e. admin for test", "admin", true)]
    public async Task<IActionResult> CreateAsync(CreateEmailTemplateRequest request)
    {
        return Ok(await _emailTemplateService.CreateAsync(request));
    }

    /// <summary>
    /// update a specific EmailTemplate by unique id.
    /// </summary>
    /// <response code="200">EmailTemplate updated.</response>
    /// <response code="404">EmailTemplate not found.</response>
    /// <response code="500">Oops! Can't lookup your product right now.</response>
    [ProducesResponseType(typeof(Result<EmailTemplateDto>), 200)]
    [ProducesResponseType(404)]
    [ProducesResponseType(500)]
    [HttpPut("{id:guid}")]
    [MustHavePermission(PermissionConstants.EmailTemplates.Update)]
    [SwaggerHeader("tenant", "EmailTemplates", "Update", "Input your tenant to access this API i.e. admin for test", "admin", true)]
    public async Task<IActionResult> UpdateAsync(UpdateEmailTemplateRequest request, Guid id)
    {
        return Ok(await _emailTemplateService.UpdateAsync(id, request));
    }

    /// <summary>
    /// Delete a specific EmailTemplate by unique id.
    /// </summary>
    /// <response code="200">EmailTemplate deleted.</response>
    /// <response code="404">EmailTemplate not found.</response>
    /// <response code="500">Oops! Can't lookup your product right now.</response>
    [ProducesResponseType(typeof(Result<bool>), 200)]
    [ProducesResponseType(404)]
    [ProducesResponseType(500)]
    [SwaggerHeader("tenant", "EmailTemplates", "Remove", "Input your tenant to access this API i.e. admin for test", "", true)]
    [HttpDelete("{id:guid}")]
    [MustHavePermission(PermissionConstants.EmailTemplates.Delete)]
    public async Task<IActionResult> DeleteAsync(Guid id) => Ok(await _emailTemplateService.DeleteAsync(id));
}
