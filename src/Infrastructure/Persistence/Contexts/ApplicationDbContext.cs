using Microsoft.EntityFrameworkCore;
using MyReliableSite.Application.Abstractions.Services.Identity;
using MyReliableSite.Application.Common.Interfaces;
using MyReliableSite.Application.Multitenancy;
using MyReliableSite.Domain.Categories;
using MyReliableSite.Domain.Billing;
using MyReliableSite.Domain.Common.Contracts;
using System.Data;
using MyReliableSite.Domain.ManageModule;
using MyReliableSite.Domain.ManageUserApiKey;
using Microsoft.AspNetCore.Http;
using MyReliableSite.Domain.WebHooksDomain;
using MyReliableSite.Domain.Identity;
using MyReliableSite.Domain.MaintenanceMode;
using MyReliableSite.Domain.Departments;
using MyReliableSite.Domain.Products;
using MyReliableSite.Domain.Tickets;
using MyReliableSite.Domain.KnowledgeBase;
using MyReliableSite.Domain.ArticleFeedbacks;
using MyReliableSite.Infrastructure.Identity.Models;
using MyReliableSite.Domain.Scripting;

namespace MyReliableSite.Infrastructure.Persistence.Contexts;

public class ApplicationDbContext : BaseDbContext
{
    private readonly IEventService _eventService;
    private readonly ISerializerService _serializer;
    public IDbConnection Connection => Database.GetDbConnection();
    private readonly ICurrentUser _currentUserService;
    private readonly ITenantService _tenantService;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public ApplicationDbContext(DbContextOptions options, ITenantService tenantService, ICurrentUser currentUserService, ISerializerService serializer, IEventService eventService, IHttpContextAccessor httpContextAccessor)
    : base(options, tenantService, currentUserService, serializer)
    {
        _tenantService = tenantService;
        _currentUserService = currentUserService;
        _serializer = serializer;
        _eventService = eventService;
        _httpContextAccessor = httpContextAccessor;
    }

    public DbSet<Article> Articles { get; set; }
    public DbSet<ArticleCategory> ArticleCategories { get; set; }
    public DbSet<BrandArticle> BrandArticles { get; set; }
    public DbSet<ArticleFeedback> ArticleFeedbacks { get; set; }
    public DbSet<ArticleFeedbackCommentReply> ArticleFeedbackCommentReplies { get; set; }
    public DbSet<ArticleFeedbackComment> ArticleFeedbackComments { get; set; }
    public DbSet<Category> Categories { get; set; }
    public DbSet<Order> Orders { get; set; }
    public DbSet<OrderTemplate> OrderTemplates { get; set; }
    public DbSet<Bill> Bills { get; set; }
    public DbSet<OrderProductLineItem> OrderProductLineItems { get; set; }
    public DbSet<OrderTemplateLineItem> OrderTemplateLineItems { get; set; }

    public DbSet<Comment> Comments { get; set; }
    public DbSet<Credit> Credit { get; set; }
    public DbSet<Feedback> Feedbacks { get; set; }
    public DbSet<FraudAlert> FraudAlerts { get; set; }
    public DbSet<MaintenanceMode> MaintenanceModes { get; set; }
    public DbSet<PaymentGateway> PaymentGateways { get; set; }
    public DbSet<RecurringPayment> RecurringPayments { get; set; }
    public DbSet<Refund> Refunds { get; set; }
    public DbSet<Review> Reviews { get; set; }
    public DbSet<Ticket> Tickets { get; set; }
    public DbSet<TicketHistory> TicketHistories { get; set; }
    public DbSet<TicketCommentHistory> TicketCommentHistories { get; set; }
    public DbSet<TicketCommentReplyHistory> TicketCommentReplyHistories { get; set; }
    public DbSet<TicketCommentReply> TicketCommentReplies { get; set; }
    public DbSet<TicketComment> TicketComments { get; set; }
    public DbSet<Transaction> Transactions { get; set; }
    public DbSet<UploadFile> UploadFiles { get; set; }
    public DbSet<Setting> Settings { get; set; }
    public DbSet<BillingSetting> BillingSettings { get; set; }
    public DbSet<SupportSetting> SupportSettings { get; set; }
    public DbSet<UserAppSetting> UserAppSettings { get; set; }
    public DbSet<CronJobs> CronJobs { get; set; }
    public DbSet<APIKeyPair> APIKeyPairs { get; set; }
    public DbSet<UserApiKeyModule> UserApiKeyModules { get; set; }
    public DbSet<Module> Modules { get; set; }
    public DbSet<UserModule> UserModules { get; set; }
    public DbSet<Brand> Brands { get; set; }
    public DbSet<EmailTemplate> EmailTemplates { get; set; }
    public DbSet<Notification> Notifications { get; set; }
    public DbSet<NotificationTemplate> NotificationTemplates { get; set; }
    public DbSet<SmtpConfiguration> SmtpConfigurations { get; set; }
    public DbSet<BrandSmtpConfiguration> BrandSmtpConfigurations { get; set; }
    public DbSet<TemplateVariable> TemplateVariables { get; set; }
    public DbSet<WebHook> WebHooks { get; set; }

    public DbSet<WebHookRecord> WebHooksHistory { get; set; }
    public DbSet<WebHookHeader> WebHookHeaders { get; set; }
    public DbSet<AdminGroup> AdminGroups { get; set; }
    public DbSet<AdminGroupModule> AdminGroupModules { get; set; }
    public DbSet<UserLoginHistory> UserLoginHistories { get; set; }
    public DbSet<Department> Departments { get; set; }
    public DbSet<DepartmentAdmin> DepartmentAdmins { get; set; }
    public DbSet<Product> Products { get; set; }
    public DbSet<UserRestrictedIp> UserRestrictedIps { get; set; }
    public DbSet<ProductDepartments> ProductDepartments { get; set; }
    public DbSet<ProductLineItems> ProductLineItems { get; set; }
    public DbSet<ServerFile> ServerFiles { get; set; }
    public DbSet<Hook> Hooks { get; set; }
    public DbSet<AutomationModule> AutomationModules { get; set; }
    public DbSet<ServerHook> ServerHooks { get; set; }
    /*public DbSet<WHMSCTransaction> WHMSCTransactions { get; set; }
    public DbSet<WHMSCInvoice> WHMSCInvoices { get; set; }
    public DbSet<WHMSCDomain> WHMSCDomains { get; set; }
    public DbSet<WHMSCClient> WHMSCClients { get; set; }
    public DbSet<WHMSCService> WHMSCServices { get; set; }*/

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = new CancellationToken())
    {
        var currentUserId = _currentUserService.GetUserId();
        foreach (var entry in ChangeTracker.Entries<IAuditableEntity>().ToList())
        {
            switch (entry.State)
            {
                case EntityState.Added:
                    entry.Entity.CreatedBy = currentUserId;
                    entry.Entity.LastModifiedBy = currentUserId;
                    break;

                case EntityState.Modified:
                    entry.Entity.LastModifiedOn = DateTime.UtcNow;
                    entry.Entity.LastModifiedBy = currentUserId;
                    break;

                case EntityState.Deleted:
                    if (entry.Entity is ISoftDelete softDelete)
                    {
                        softDelete.DeletedBy = currentUserId;
                        softDelete.DeletedOn = DateTime.UtcNow;
                        entry.State = EntityState.Modified;
                    }

                    break;
            }

            if (_httpContextAccessor.HttpContext != null && _httpContextAccessor.HttpContext.Request.Headers.TryGetValue("AdminAsClient", out var adminAsClient))
            {
                var adminUser = await Users.FindAsync(adminAsClient);
                if (adminUser == null)
                {
                    throw new Exception("AdminAsClient Header has not valid value");
                }
                else
                {
                    entry.Entity.AdminAsClient = Guid.Parse(adminUser.Id);
                }
            }
        }

        int results = await base.SaveChangesAsync(cancellationToken);
        if (_eventService == null) return results;
        var entitiesWithEvents = ChangeTracker.Entries<BaseEntity>()
                                                .Select(e => e.Entity)
                                                .Where(e => e.DomainEvents.Count > 0)
                                                .ToArray();

        foreach (var entity in entitiesWithEvents)
        {
            var events = entity.DomainEvents.ToArray();
            entity.DomainEvents.Clear();
            foreach (var @event in events)
            {
                await _eventService.PublishAsync(@event);
            }
        }

        return results;
    }
}
