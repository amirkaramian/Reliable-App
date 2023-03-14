using Microsoft.AspNetCore.Mvc;
using MyReliableSite.Application.Departments.Interfaces;
using MyReliableSite.Application.Wrapper;
using MyReliableSite.Domain.Constants;
using MyReliableSite.Infrastructure.Identity.Permissions;
using MyReliableSite.Infrastructure.Swagger;
using MyReliableSite.Shared.DTOs.Departments;
using MyReliableSite.Shared.DTOs.Identity;
using Swashbuckle.AspNetCore.Annotations;

namespace MyReliableSite.Admin.API.Controllers.Setting;
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
    [SwaggerHeader("tenant", "Setting", "Search", "Input your tenant to access this API i.e. admin for test", "admin", true)]
    [MustHavePermission(PermissionConstants.Departments.Search)]
    [SwaggerOperation(Summary = "Search Departments using available Filters.")]
    public async Task<IActionResult> SearchAsync(DepartmentListFilter filter)
    {
        var departments = await _service.SearchAsync(filter);
        return Ok(departments);
    }

    /// <summary>
    /// retrive the Department against specific id.
    /// </summary>
    /// <response code="200">Department returns.</response>
    /// <response code="400">Department not found.</response>
    /// <response code="500">Oops! Can't lookup your record right now.</response>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(Result<DepartmentDto>), 200)]
    [ProducesResponseType(typeof(IDictionary<string, string>), 400)]
    [ProducesResponseType(500)]
    [SwaggerHeader("tenant", "Setting", "View", "Input your tenant to access this API i.e. admin for test", "admin", true)]
    [MustHavePermission(PermissionConstants.Departments.View)]
    public async Task<IActionResult> GetAsync(Guid id)
    {
        var department = await _service.GetDepartmentAsync(id);
        return Ok(department);
    }

    /// <summary>
    /// retrive the Department against specific id.
    /// </summary>
    /// <response code="200">Department returns.</response>
    /// <response code="400">Department not found.</response>
    /// <response code="500">Oops! Can't lookup your record right now.</response>
    [HttpGet("getdepartmentusers/{id}")]
    [ProducesResponseType(typeof(Result<UserDetailsDto>), 200)]
    [ProducesResponseType(typeof(IDictionary<string, string>), 400)]
    [ProducesResponseType(500)]
    [SwaggerHeader("tenant", "Setting", "View", "Input your tenant to access this API i.e. admin for test", "admin", true)]
    [MustHavePermission(PermissionConstants.Departments.View)]
    public async Task<IActionResult> GetDepartmentUsersAsync(Guid id)
    {
        var users = await _service.GetDepartmentUsersAsync(id);
        return Ok(users);
    }

    /// <summary>
    /// retrive the Department against specific id.
    /// </summary>
    /// <response code="200">Department returns.</response>
    /// <response code="400">Department not found.</response>
    /// <response code="500">Oops! Can't lookup your record right now.</response>
    [HttpGet("getUserDepartments/{id}")]
    [ProducesResponseType(typeof(Result<List<DepartmentAdminAssignStatusDto>>), 200)]
    [ProducesResponseType(typeof(IDictionary<string, string>), 400)]
    [ProducesResponseType(500)]
    [SwaggerHeader("tenant", "Setting", "View", "Input your tenant to access this API i.e. admin for test", "admin", true)]
    [MustHavePermission(PermissionConstants.Departments.View)]
    public async Task<IActionResult> GetUserDepartmentsAsync(Guid id)
    {
        var department = await _service.GetDepartmentByUserIdAsync(id);
        return Ok(department);
    }

    /// <summary>
    /// Create an Department.
    /// </summary>
    /// <response code="200">Department created.</response>
    /// <response code="400">Department already exists.</response>
    /// <response code="500">Oops! Can't lookup your product right now.</response>
    [HttpPost]
    [ProducesResponseType(typeof(Result<Guid>), 200)]
    [ProducesResponseType(typeof(IDictionary<string, string>), 400)]
    [ProducesResponseType(500)]
    [SwaggerHeader("tenant", "Setting", "Create", "Input your tenant to access this API i.e. admin for test", "admin", true)]
    [MustHavePermission(PermissionConstants.Departments.Create)]
    public async Task<IActionResult> CreateAsync(CreateDepartmentRequest request)
    {
        return Ok(await _service.CreateDepartmentAsync(request));
    }

    /// <summary>
    /// Assign a Department to a user.
    /// </summary>
    /// <response code="200">Department assigned.</response>
    /// <response code="400">Department already exists.</response>
    /// <response code="500">Oops! Can't lookup your product right now.</response>
    [HttpPost("AssignDepartmentAsync")]
    [ProducesResponseType(typeof(Result<Guid>), 200)]
    [ProducesResponseType(typeof(IDictionary<string, string>), 400)]
    [ProducesResponseType(500)]
    [SwaggerHeader("tenant", "Setting", "Update", "Input your tenant to access this API i.e. admin for test", "admin", true)]
    [MustHavePermission(PermissionConstants.Departments.Update)]
    public async Task<IActionResult> AssignDepartmentAsync(AssignDepartmentRequest request)
    {
        return Ok(await _service.AssignDepartmentAsync(request));
    }

    /// <summary>
    /// UnAssign a Department to a user.
    /// </summary>
    /// <response code="200">Department assigned.</response>
    /// <response code="400">Department already exists.</response>
    /// <response code="500">Oops! Can't lookup your product right now.</response>
    [HttpPost("UnAssignDepartmentAsync")]
    [ProducesResponseType(typeof(Result<Guid>), 200)]
    [ProducesResponseType(typeof(IDictionary<string, string>), 400)]
    [ProducesResponseType(500)]
    [SwaggerHeader("tenant", "Setting", "Update", "Input your tenant to access this API i.e. admin for test", "admin", true)]
    [MustHavePermission(PermissionConstants.Departments.Update)]
    public async Task<IActionResult> UnAssignDepartmentAsync(AssignDepartmentRequest request)
    {
        return Ok(await _service.UnAssignDepartmentAsync(request));
    }

    /// <summary>
    /// update a specific Department by unique id.
    /// </summary>
    /// <response code="200">Department updated.</response>
    /// <response code="404">Department not found.</response>
    /// <response code="500">Oops! Can't lookup your product right now.</response>
    [HttpPut("{id}")]
    [ProducesResponseType(typeof(Result<Guid>), 200)]
    [ProducesResponseType(404)]
    [ProducesResponseType(500)]
    [SwaggerHeader("tenant", "Setting", "Update", "Input your tenant to access this API i.e. admin for test", "admin", true)]
    [MustHavePermission(PermissionConstants.Departments.Update)]
    public async Task<IActionResult> UpdateAsync(UpdateDepartmentRequest request, Guid id)
    {
        return Ok(await _service.UpdateDepartmentAsync(request, id));
    }

    /// <summary>
    /// Delete a specific Department by unique id.
    /// </summary>
    /// <response code="200">Department deleted.</response>
    /// <response code="404">Department not found.</response>
    /// <response code="500">Oops! Can't lookup your product right now.</response>
    [HttpDelete("{id}")]
    [ProducesResponseType(typeof(Result<Guid>), 200)]
    [ProducesResponseType(404)]
    [ProducesResponseType(500)]
    [SwaggerHeader("tenant", "Setting", "Remove", "Input your tenant to access this API i.e. admin for test", "admin", true)]
    [MustHavePermission(PermissionConstants.Departments.Remove)]
    public async Task<IActionResult> DeleteAsync(Guid id)
    {
        var departmentId = await _service.DeleteDepartmentAsync(id);
        return Ok(departmentId);
    }
}