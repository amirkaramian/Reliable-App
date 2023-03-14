using Microsoft.AspNetCore.Mvc;
using MyReliableSite.Application.CronJobs.Interfaces;
using MyReliableSite.Application.Wrapper;
using MyReliableSite.Domain.Constants;
using MyReliableSite.Infrastructure.Identity.Permissions;
using MyReliableSite.Infrastructure.Swagger;
using MyReliableSite.Shared.DTOs.CronJobs;

namespace MyReliableSite.Admin.API.Controllers.CronJobs;

public class CronJobsController : BaseController
{
    private readonly ICronJobsService _service;

    public CronJobsController(ICronJobsService service)
    {
        _service = service;
    }

    /// <summary>
    /// Create an CronJob.
    /// </summary>
    /// <response code="200">CronJob created.</response>
    /// <response code="400">CronJob already exists.</response>
    /// <response code="500">Oops! Can't lookup your product right now.</response>
    [HttpPost]
    [ProducesResponseType(typeof(Result<Guid>), 200)]
    [ProducesResponseType(typeof(IDictionary<string, string>), 400)]
    [ProducesResponseType(500)]
    [SwaggerHeader("tenant", "CronJobs", "Create", "Input your tenant to access this API i.e. admin for test", "", true)]
    [MustHavePermission(PermissionConstants.CronJobs.Register)]
    public async Task<IActionResult> CreateAsync(CreateCronJobsRequest request)
    {
        return Ok(await _service.CreateCronJobsAsync(request));
    }

    /// <summary>
    /// update a specific CronJob by unique id.
    /// </summary>
    /// <response code="200">CronJob updated.</response>
    /// <response code="404">CronJob not found.</response>
    /// <response code="500">Oops! Can't lookup your product right now.</response>
    [HttpPut("{id}")]
    [ProducesResponseType(typeof(Result<Guid>), 200)]
    [ProducesResponseType(404)]
    [ProducesResponseType(500)]
    [SwaggerHeader("tenant", "CronJobs", "Update", "Input your tenant to access this API i.e. admin for test", "", true)]
    [MustHavePermission(PermissionConstants.CronJobs.Update)]
    public async Task<IActionResult> UpdateAsync(UpdateCronJobsRequest request, Guid id)
    {
        return Ok(await _service.UpdateCronJobsAsync(request, id));
    }

    /// <summary>
    /// retrive the CronJob against specific id.
    /// </summary>
    /// <response code="200">CronJob returns.</response>
    /// <response code="400">CronJob not found.</response>
    /// <response code="500">Oops! Can't lookup your record right now.</response>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(Result<CronJobsDetailsDto>), 200)]
    [ProducesResponseType(typeof(IDictionary<string, string>), 400)]
    [ProducesResponseType(500)]
    [SwaggerHeader("tenant", "CronJobs", "Search", "Input your tenant to access this API i.e. admin for test", "admin", true)]
    [MustHavePermission(PermissionConstants.CronJobs.View)]
    public async Task<IActionResult> GetAsync(Guid id)
    {
        var result = await _service.GetCronJobsDetailsAsync(id);
        return Ok(result);
    }

    /// <summary>
    /// Delete a specific CronJob by unique id.
    /// </summary>
    /// <response code="200">CronJob deleted.</response>
    /// <response code="404">CronJob not found.</response>
    /// <response code="500">Oops! Can't lookup your product right now.</response>
    [HttpDelete("{id}")]
    [ProducesResponseType(typeof(Result<Guid>), 200)]
    [ProducesResponseType(404)]
    [ProducesResponseType(500)]
    [SwaggerHeader("tenant", "CronJobs", "Remove", "Input your tenant to access this API i.e. admin for test", "", true)]
    [MustHavePermission(PermissionConstants.CronJobs.Remove)]
    public async Task<IActionResult> DeleteAsync(Guid id)
    {
        var cronJobId = await _service.DeleteCronJobsAsync(id);
        return Ok(cronJobId);
    }
}
