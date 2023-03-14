using Microsoft.AspNetCore.Mvc;
using MyReliableSite.Application.Departments.Interfaces;
using MyReliableSite.Application.Wrapper;
using MyReliableSite.Domain.Constants;
using MyReliableSite.Infrastructure.Identity.Permissions;
using MyReliableSite.Infrastructure.Swagger;
using MyReliableSite.Shared.DTOs.Departments;
using MyReliableSite.Shared.DTOs.Identity;
using Swashbuckle.AspNetCore.Annotations;

namespace MyReliableSite.Client.API.Controllers.Setting;
[Route("api/[controller]")]
[ApiController]
public class DepartmentsController : BaseController
{
    private readonly IDepartmentService _service;
    private readonly IConfiguration _config;
    public DepartmentsController(IDepartmentService service, IConfiguration config)
    {
        _service = service;
        _config = config;
    }

    /// <summary>
    /// List of records with pagination &amp; filters.
    /// </summary>
    /// <response code="200">Department List returns.</response>
    /// <response code="400">Department not found.</response>
    /// <response code="500">Oops! Can't lookup your record right now.</response>
    [HttpPost("search")]
    [ProducesResponseType(typeof(PaginatedResult<DepartmentDto>), 200)]
    [ProducesResponseType(typeof(IDictionary<string, string>), 400)]
    [ProducesResponseType(500)]
    [SwaggerHeader("tenant", "Setting", "Search", "Input your tenant to access this API i.e. client for test", "client", true)]
    [MustHavePermission(PermissionConstants.Departments.Search)]
    [SwaggerOperation(Summary = "Search Departments using available Filters.")]
    public async Task<IActionResult> SearchAsync(DepartmentListFilter filter)
    {
        var departments = await _service.SearchAsync(filter);
        return Ok(departments);
    }
}