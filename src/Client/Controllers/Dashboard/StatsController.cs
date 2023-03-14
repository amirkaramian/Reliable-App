using Microsoft.AspNetCore.Mvc;
using MyReliableSite.Application.Dashboard;
using MyReliableSite.Application.Wrapper;
using MyReliableSite.Infrastructure.Swagger;
using MyReliableSite.Shared.DTOs;

namespace MyReliableSite.Client.API.Controllers.Dashboard;

public class StatsController : BaseController
{
    private readonly IStatsService _service;

    public StatsController(IStatsService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<IActionResult> GetAsync()
    {
        var stats = await _service.GetDataAsync();
        return Ok(stats);
    }
}