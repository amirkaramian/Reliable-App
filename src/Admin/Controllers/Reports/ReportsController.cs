using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MyReliableSite.Application.Reports.Interfaces;
using MyReliableSite.Application.Wrapper;
using MyReliableSite.Domain.Constants;
using MyReliableSite.Infrastructure.Identity.Permissions;
using MyReliableSite.Infrastructure.Swagger;
using MyReliableSite.Shared.DTOs.Reports;
using OfficeOpenXml.Table;
using OfficeOpenXml;
using MyReliableSite.Application.Products.Interfaces;
using MyReliableSite.Domain.Products;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using MyReliableSite.Application.CreditManagement.Interfaces;
using MyReliableSite.Application.Settings.Interfaces;
using MyReliableSite.Application.Transactions.Interfaces;
using MyReliableSite.Application.Tickets.Interfaces;
using MyReliableSite.Application.Abstractions.Services.Identity;
using MyReliableSite.Application.ManageUserApiKey.Interfaces;
using MyReliableSite.Application.Auditing;
using MyReliableSite.Application.KnowledgeBase.Interfaces;
using MyReliableSite.Shared.DTOs.Notifications.Enums;
using OfficeOpenXml.FormulaParsing.Excel.Functions.DateTime;
using MyReliableSite.Shared.DTOs.Transaction;
using System.Linq;
using Newtonsoft.Json;
using System.Text;
using static MyReliableSite.Domain.Constants.PermissionConstants;

namespace MyReliableSite.Admin.API.Controllers.Reports;

public class ReportsController : BaseController
{
    private readonly IReportService _service;
    private readonly IProductService _productService;
    private readonly ICreditService _creditService;
    private readonly ISettingService _settingservice;
    private readonly ITransaction _transactionService;
    private readonly ITicketService _ticketservice;
    private readonly IUserService _userService;
    private readonly IAPIKeyPairService _apikeyservice;
    private readonly IAuditService _auditService;
    private readonly IArticleService _articleservice;
    public ReportsController(IReportService service, IProductService productService, ICreditService creditService, ISettingService settingservice, ITransaction transaction, ITicketService ticketService, IUserService userService, IAPIKeyPairService aPIKeyPairService, IAuditService auditService, IArticleService articleService)
    {
        _service = service;
        _productService = productService;
        _creditService = creditService;
        _settingservice = settingservice;
        _transactionService = transaction;
        _ticketservice = ticketService;
        _userService = userService;
        _auditService = auditService;
        _apikeyservice = aPIKeyPairService;
        _articleservice = articleService;
    }

    /// <summary>
    /// Retrive the Annual Income against a year.
    /// </summary>
    /// <response code="200">Annual Income returns.</response>
    /// <response code="500">Oops! Can't lookup your record right now.</response>
    [HttpGet("tickets/annualIncome/{year}")]
    [ProducesResponseType(typeof(Result<AnnualIncomeReportDto>), 200)]
    [ProducesResponseType(500)]
    [SwaggerHeader("tenant", "Reports", "View", "Input your tenant to access this API i.e. admin for test", "admin", true)]
    [MustHavePermission(PermissionConstants.Reports.View)]
    public async Task<IActionResult> GetAsync(int year)
    {
        var reports = await _service.AnnualIncomeReport(year);
        return Ok(reports);
    }

    /// <summary>
    /// Retrive the data for fields like by customer, by agent, by emails, by ticket priority against a date range.
    /// </summary>
    /// <response code="200">Annual Income returns.</response>
    /// <response code="500">Oops! Can't lookup your record right now.</response>
    [HttpGet("tickets/{reportField}/{startDate}/{endDate}")]
    [ProducesResponseType(typeof(Result<DateAndCountDto>), 200)]
    [ProducesResponseType(500)]
    [SwaggerHeader("tenant", "Reports", "View", "Input your tenant to access this API i.e. admin for test", "admin", true)]
    [MustHavePermission(PermissionConstants.Reports.View)]
    public async Task<IActionResult> GetAsync(ReportFields reportField, DateTime startDate, DateTime endDate)
    {
        var reports = await _service.TicketReportFields(reportField, startDate, endDate);
        return Ok(reports);
    }

    /// <summary>
    /// Retrive the Numbers of replies per tickets against a date range.
    /// </summary>
    /// <response code="200">Annual Income returns.</response>
    /// <response code="500">Oops! Can't lookup your record right now.</response>
    [HttpGet("tickets/repliesPerTickets/{startDate}/{endDate}")]
    [ProducesResponseType(typeof(Result<NoOfRepliesPerTicketsDto>), 200)]
    [ProducesResponseType(500)]
    [SwaggerHeader("tenant", "Reports", "View", "Input your tenant to access this API i.e. admin for test", "admin", true)]
    [MustHavePermission(PermissionConstants.Reports.View)]
    public async Task<IActionResult> GetAsync(DateTime startDate, DateTime endDate)
    {
        var reports = await _service.NoOfRepliesPerTickets(startDate, endDate);
        return Ok(reports);
    }

    /// <summary>
    /// Retrive the Tickets response time in seconds against a date range.
    /// </summary>
    /// <response code="200">Annual Income returns.</response>
    /// <response code="500">Oops! Can't lookup your record right now.</response>
    [HttpGet("tickets/responseTime/{startDate}/{endDate}")]
    [ProducesResponseType(typeof(Result<TicketResponseTimeDto>), 200)]
    [ProducesResponseType(500)]
    [SwaggerHeader("tenant", "Reports", "View", "Input your tenant to access this API i.e. admin for test", "admin", true)]
    [MustHavePermission(PermissionConstants.Reports.View)]
    public async Task<IActionResult> GetTicketResponseTimeAsync(DateTime startDate, DateTime endDate)
    {
        var reports = await _service.TicketsResponseTime(startDate, endDate);
        return Ok(reports);
    }

    /// <summary>
    /// Retrive the Data on Based of module input Export in Excel.
    /// </summary>
    /// <response code="200">Excel File returns.</response>
    /// <response code="500">No Data Found.</response>
    [HttpGet("tickets/ExportExcel/{userId}/{moduletype}/{startDate}/{endDate}")]
    [ProducesResponseType(typeof(Result<Product>), 200)]
    [ProducesResponseType(500)]
    [SwaggerHeader("tenant", "Reports", "View", "Input your tenant to access this API i.e. admin for test", "admin", true)]

    [MustHavePermission(PermissionConstants.Reports.View)]

    public async Task<IActionResult> ExportExcel(string moduletype, string userId, DateTime startDate, DateTime endDate)
    {
        try
        {
            var stream = new MemoryStream();

            string[] moduleType = moduletype.Split(',');

            string reportname = $"Exported-Data-{DateTime.Now.ToString("yyyyMMddHHmmssfff"):N}.xls";

            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

            using (var package = new ExcelPackage(stream))
            {
                if (moduleType.Contains("Products"))
                {
                    var products = await _productService.GetProductListAsync(userId, startDate, endDate);

                    foreach (var item in products.Data)
                    {
                        if (string.IsNullOrWhiteSpace(item.Name))
                            item.Name = string.Empty;
                        if (string.IsNullOrWhiteSpace(item.Description))
                            item.Description = string.Empty;
                        if (string.IsNullOrWhiteSpace(item.Thumbnail))
                            item.Thumbnail = string.Empty;
                        if (string.IsNullOrWhiteSpace(item.Notes))
                            item.Notes = string.Empty;
                        if (string.IsNullOrWhiteSpace(item.SuspendedReason))
                            item.SuspendedReason = string.Empty;
                        if (string.IsNullOrWhiteSpace(item.AdminAssigned))
                            item.AdminAssigned = string.Empty;
                        if (string.IsNullOrWhiteSpace(item.AssginedIPs))
                            item.AssginedIPs = string.Empty;
                        if (string.IsNullOrWhiteSpace(item.DedicatedIP))
                            item.DedicatedIP = string.Empty;
                        if (string.IsNullOrWhiteSpace(item.DomainName))
                            item.DomainName = string.Empty;
                        if (string.IsNullOrWhiteSpace(item.OldOrderId))
                            item.OldOrderId = string.Empty;
                        if (string.IsNullOrWhiteSpace(item.OldProductId))
                            item.OldProductId = string.Empty;
                        if (string.IsNullOrWhiteSpace(item.ServerId))
                            item.ServerId = string.Empty;
                    }

                    if (products.Data.Count > 0)
                    {
                        var worksheet = package.Workbook.Worksheets.Add("Products");

                        worksheet.Cells.LoadFromCollection(products.Data, true);

                    }
                }

                if (moduleType.Contains("Credits"))
                {
                    var credits = await _creditService.GetCreditListAsync(userId, startDate, endDate);

                    if (credits.Data.Count > 0)
                    {
                        var worksheet = package.Workbook.Worksheets.Add("Credits");
                        worksheet.Cells.LoadFromCollection(credits.Data, true);

                    }
                }

                if (moduleType.Contains("Billing"))
                {
                    var billing = await _settingservice.GetSettingListAsync(userId, startDate, endDate);
                    if (billing.Data.Count > 0)
                    {
                        var worksheet = package.Workbook.Worksheets.Add("Billings");
                        worksheet.Cells.LoadFromCollection(billing.Data, true);

                    }
                }

                if (moduleType.Contains("Transactions"))
                {
                    var billtransaction = await _transactionService.GetTransactionListAsync(userId, startDate, endDate);

                    foreach (var item in billtransaction.Data)
                    {
                        if (item.RefundRetainPercentage == null)
                            item.RefundRetainPercentage = 0;
                        if (item.TotalAfterRefundRetain == null)
                            item.TotalAfterRefundRetain = 0;
                    }

                    var newBillTransactions = billtransaction.Data.Select(x => new { x.Id, x.TransactionNo, x.ReferenceNo, x.ReferenceId, x.CreatedOn, x.LastModifiedOn, x.TransactionBy, x.TransactionType, x.ActionTakenBy, x.Total, x.Notes, x.TransactionByRole, x.TransactionStatus, x.RefundRetainPercentage, x.TotalAfterRefundRetain }).ToList();

                    if (newBillTransactions.Count > 0)
                    {
                        var worksheet = package.Workbook.Worksheets.Add("Transactions");
                        worksheet.Cells.LoadFromCollection(newBillTransactions, true);

                    }
                }

                if (moduleType.Contains("Tickets"))
                {
                    var ticket = await _ticketservice.GetTicketListAsync(userId, startDate, endDate);

                    foreach (var item in ticket.Data)
                    {
                        if (string.IsNullOrWhiteSpace(item.AgentUser))
                            item.AgentUser = string.Empty;
                        if (string.IsNullOrWhiteSpace(item.Duration))
                            item.Duration = string.Empty;
                        if (string.IsNullOrWhiteSpace(item.FollowUpComment))
                            item.FollowUpComment = string.Empty;
                        if (string.IsNullOrWhiteSpace(item.Group))
                            item.Group = string.Empty;
                        if (string.IsNullOrWhiteSpace(item.Notes))
                            item.Notes = string.Empty;
                        if (string.IsNullOrWhiteSpace(item.TransferComments))
                            item.TransferComments = string.Empty;
                    }

                    if (ticket.Data.Count > 0)
                    {
                        var worksheet = package.Workbook.Worksheets.Add("Tickets");
                        worksheet.Cells.LoadFromCollection(ticket.Data, true);

                    }
                }

                if (moduleType.Contains("SubUsers"))
                {
                    var subuser = await _userService.GetSubUserListAsync(userId, startDate, endDate);

                    if (subuser.Data.Count > 0)
                    {
                        var worksheet = package.Workbook.Worksheets.Add("SubUsers");
                        worksheet.Cells.LoadFromCollection(subuser.Data, true);

                    }
                }

                if (moduleType.Contains("APIKeys"))
                {
                    var aPIKeyPairs = await _apikeyservice.GetAPIKeyPairListAsync(userId, startDate, endDate);

                    if (aPIKeyPairs.Data.Count() > 0)
                    {
                        var worksheet = package.Workbook.Worksheets.Add("ApiKey");
                        worksheet.Cells.LoadFromCollection(aPIKeyPairs.Data, true);
                    }
                }

                if (moduleType.Contains("Logs"))
                {
                    var auditservice = await _auditService.GetAllAsync(userId, startDate, endDate);

                    foreach (var item in auditservice.Data)
                    {
                        if (string.IsNullOrWhiteSpace(item.OldValues))
                            item.OldValues = string.Empty;
                        if (string.IsNullOrWhiteSpace(item.NewValues))
                            item.NewValues = string.Empty;
                        if (string.IsNullOrWhiteSpace(item.AffectedColumns))
                            item.AffectedColumns = string.Empty;
                        if (string.IsNullOrWhiteSpace(item.FullName))
                            item.FullName = string.Empty;
                        if (string.IsNullOrWhiteSpace(item.Text))
                            item.Text = string.Empty;
                    }

                    if (auditservice.Data.Count() > 0)
                    {
                        var worksheet = package.Workbook.Worksheets.Add("Auditlog");
                        worksheet.Cells.LoadFromCollection(auditservice.Data, true);
                    }
                }

                if (moduleType.Contains("KnowledgeBase"))
                {
                    var knowledgeservice = await _articleservice.GetArticlesListAsync(userId, startDate, endDate);

                    if (knowledgeservice.Data.Count > 0)
                    {
                        var worksheet = package.Workbook.Worksheets.Add("Knowledge");
                        worksheet.Cells.LoadFromCollection(knowledgeservice.Data, true);
                    }
                }

                if (package.Workbook.Worksheets.Count > 0)
                    package.Save();
            }

            stream.Position = 0;

            if (stream.Capacity > 0)
                return File(stream, "application/vnd.ms-excel", reportname);
            else

                return Content("No Data Found");

        }
        catch (Exception)
        {

            throw;
        }
    }
}
