using Microsoft.AspNetCore.Mvc;
using MyReliableSite.Application.CronJobs.Interfaces;
using MyReliableSite.Domain.Constants;
using MyReliableSite.Infrastructure.Identity.Permissions;
using MyReliableSite.Infrastructure.Swagger;
using MyReliableSite.Shared.DTOs.CronJobs;

namespace MyReliableSite.Client.API.Controllers.CronJobs;

public class CronJobsController : BaseController
{
    private readonly ICronJobsService _service;

    public CronJobsController(ICronJobsService service)
    {
        _service = service;
    }

    [HttpPost]
    [SwaggerHeader("tenant,gen-api-key", "Articles", "Search", "Input your tenant to access this API i.e. admin for test", "admin", true, true)]
    [MustHavePermission(PermissionConstants.CronJobs.Register)]
    public async Task<IActionResult> CreateAsync(CreateCronJobsRequest request)
    {
        return Ok(await _service.CreateCronJobsAsync(request));
    }

    [HttpPut("{id}")]
    [SwaggerHeader("tenant,gen-api-key", "Articles", "Search", "Input your tenant to access this API i.e. admin for test", "admin", true, true)]
    [MustHavePermission(PermissionConstants.CronJobs.Update)]
    public async Task<IActionResult> UpdateAsync(UpdateCronJobsRequest request, Guid id)
    {
        return Ok(await _service.UpdateCronJobsAsync(request, id));
    }

    [HttpGet("{id}")]
    [SwaggerHeader("tenant,gen-api-key", "Articles", "Search", "Input your tenant to access this API i.e. admin for test", "admin", true, true)]
    [MustHavePermission(PermissionConstants.CronJobs.View)]
    public async Task<IActionResult> GetAsync(Guid id)
    {
        var result = await _service.GetCronJobsDetailsAsync(id);
        return Ok(result);
    }

    [HttpDelete("{id}")]
    [SwaggerHeader("tenant,gen-api-key", "Articles", "Search", "Input your tenant to access this API i.e. admin for test", "admin", true, true)]
    [MustHavePermission(PermissionConstants.CronJobs.Remove)]
    public async Task<IActionResult> DeleteAsync(Guid id)
    {
        var articleId = await _service.DeleteCronJobsAsync(id);
        return Ok(articleId);
    }
}