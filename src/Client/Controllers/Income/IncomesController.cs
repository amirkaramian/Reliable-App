using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MyReliableSite.Application.Incomes.Interfaces;
using MyReliableSite.Domain.Constants;
using MyReliableSite.Infrastructure.Identity.Permissions;
using MyReliableSite.Infrastructure.Swagger;
using MyReliableSite.Shared.DTOs.Transaction;
using Swashbuckle.AspNetCore.Annotations;

namespace MyReliableSite.Client.API.Controllers.Income;
public class IncomesController : BaseController
{
    private readonly IIncomeService _incomeService;

    public IncomesController(IIncomeService incomeService)
    {
        _incomeService = incomeService;
    }

    /// <summary>
    /// List of Income Overview records.
    /// </summary>
    /// <response code="200">Incomes Overview returns.</response>
    /// <response code="500">Oops! Can't lookup your record right now.</response>
    [HttpGet("IncomeOverview")]
    [ProducesResponseType(500)]
    [SwaggerHeader("tenant", "Incomes", "View", "Input your tenant to access this API i.e. client", "client", true)]
    [MustHavePermission(PermissionConstants.Transactions.View)]
    [SwaggerOperation(Summary = "Get Income Overview data.")]
    public async Task<IActionResult> IncomeOverview()
    {
        var incomes = await _incomeService.GetIncomeOverview();

        return Ok(incomes);
    }

    /// <summary>
    /// Income Forecast records.
    /// </summary>
    /// <response code="200">Incomes Forecast returns.</response>
    /// <response code="500">Oops! Can't lookup your record right now.</response>
    [HttpGet("IncomeForecast")]
    [ProducesResponseType(500)]
    [SwaggerHeader("tenant", "Incomes", "View", "Input your tenant to access this API i.e. client", "client", true)]
    [MustHavePermission(PermissionConstants.Transactions.View)]
    [SwaggerOperation(Summary = "Get Income Forecast data.")]
    public async Task<IActionResult> IncomeForecast()
    {
        var incomes = await _incomeService.GetIncomeForecast();

        return Ok(incomes);
    }

    /// <summary>
    /// Income Transaction Overview records.
    /// </summary>
    /// <response code="200">Incomes Transaction Overview returns.</response>
    /// <response code="500">Oops! Can't lookup your record right now.</response>
    [HttpPost("IncomeTransactionOverview/{transactionType}")]
    [ProducesResponseType(500)]
    [SwaggerHeader("tenant", "Incomes", "View", "Input your tenant to access this API i.e. client", "client", true)]
    [MustHavePermission(PermissionConstants.Transactions.View)]
    [SwaggerOperation(Summary = "Get Income Overview transactions data.")]
    public async Task<IActionResult> IncomeTransactionOverview(TransactionListFilter filter, string transactionType)
    {
        var incomes = await _incomeService.GetIncomeTransactionOverviewAsync(filter, transactionType);

        return Ok(incomes);
    }
}
