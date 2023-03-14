using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MyReliableSite.Application.Abstractions.Services.Identity;
using MyReliableSite.Application.Auditing;
using MyReliableSite.Application.CreditManagement.Interfaces;
using MyReliableSite.Application.KnowledgeBase.Interfaces;
using MyReliableSite.Application.ManageUserApiKey.Interfaces;
using MyReliableSite.Application.Products.Interfaces;
using MyReliableSite.Application.Reports.Interfaces;
using MyReliableSite.Application.Settings.Interfaces;
using MyReliableSite.Application.Tickets.Interfaces;
using MyReliableSite.Application.Transactions.Interfaces;
using MyReliableSite.Application.Transactions.Services;
using MyReliableSite.Application.Wrapper;
using MyReliableSite.Domain.Constants;
using MyReliableSite.Domain.Products;
using MyReliableSite.Infrastructure.Identity.Permissions;
using MyReliableSite.Infrastructure.Swagger;
using OfficeOpenXml;
using System.ComponentModel;

namespace MyReliableSite.Client.API.Controllers.Reports;
public class ReportsController : BaseController
{

    private readonly IProductService _productService;
    private readonly ICreditService _creditService;
    private readonly ISettingService _settingservice;
    private readonly ITransaction _transactionService;
    private readonly ITicketService _ticketservice;
    private readonly IUserService _userService;
    private readonly IAPIKeyPairService _apikeyservice;
    private readonly IAuditService _auditService;
    private readonly IArticleService _articleservice;
    public ReportsController(IProductService productService, ICreditService creditService, ISettingService settingservice, ITransaction transaction, ITicketService ticketService, IUserService userService, IAPIKeyPairService aPIKeyPairService, IAuditService auditService, IArticleService articleService)
    {
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
    /// Retrive the Data on Based of module input Export in Excel.
    /// </summary>
    /// <response code="200">Excel File returns.</response>
    /// <response code="500">No Data Found.</response>
    [HttpGet("ExportExcel/{userId}/{moduletype}/{startDate}/{endDate}")]
    [ProducesResponseType(typeof(Result<Product>), 200)]
    [ProducesResponseType(404)]
    [ProducesResponseType(500)]
    [SwaggerHeader("tenant", "Reports", "View", "Input your tenant to access this API i.e. client", "client", true)]
    [MustHavePermission(PermissionConstants.Reports.View)]

    public async Task<IActionResult> ExportExcel(string moduletype, string userId, DateTime startDate, DateTime endDate)
    {
        try
        {
            var stream = new MemoryStream();

            string[] moduleType = moduletype.Split(',');

            string reportname = $"Exported-Data-{DateTime.Now.ToString("yyyyMMddHHmmssfff"):N}.xls";

            ExcelPackage.LicenseContext = OfficeOpenXml.LicenseContext.NonCommercial;

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

                // return File(package.GetAsByteArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", reportname);

                // var fileContent = File(package.GetAsByteArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", reportname);

                // return new JsonResult(fileContent);

                // return Ok(fileContent);
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
