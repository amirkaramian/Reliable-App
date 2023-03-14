using System.ComponentModel;

namespace MyReliableSite.Domain.Constants;

public class PermissionConstants
{

    [DisplayName("WHMCS")]
    [Description("WHMCS Permissions")]
    public static class WHMCS
    {
        public const string View = "Permissions.WHMCS.View";
        public const string Search = "Permissions.WHMCS.Search";
        public const string Update = "Permissions.WHMCS.Update";
        public const string Remove = "Permissions.WHMCS.Remove";
        public const string Create = "Permissions.WHMCS.Create";
    }

    [DisplayName("ArticleFeedbacks")]
    [Description("ArticleFeedbacks Permissions")]
    public static class ArticleFeedbacks
    {
        public const string View = "Permissions.ArticleFeedbacks.View";
        public const string Search = "Permissions.ArticleFeedbacks.Search";
        public const string Update = "Permissions.ArticleFeedbacks.Update";
        public const string Remove = "Permissions.ArticleFeedbacks.Remove";
        public const string Create = "Permissions.ArticleFeedbacks.Create";
    }

    [DisplayName("Tickets")]
    [Description("Tickets Permissions")]
    public static class Tickets
    {
        public const string View = "Permissions.Tickets.View";
        public const string Search = "Permissions.Tickets.Search";
        public const string Update = "Permissions.Tickets.Update";
        public const string Remove = "Permissions.Tickets.Remove";
        public const string Create = "Permissions.Tickets.Create";
    }

    [DisplayName("PaymentGateways")]
    [Description("PaymentGateways Permissions")]
    public static class PaymentGateways
    {
        public const string View = "Permissions.PaymentGateways.View";
        public const string Search = "Permissions.PaymentGateways.Search";
        public const string Update = "Permissions.PaymentGateways.Update";
        public const string Remove = "Permissions.PaymentGateways.Remove";
        public const string Create = "Permissions.PaymentGateways.Create";
    }

    [DisplayName("Departments")]
    [Description("Department Permissions")]
    public static class Departments
    {
        public const string View = "Permissions.Departments.View";
        public const string Search = "Permissions.Departments.Search";
        public const string Update = "Permissions.Departments.Update";
        public const string Remove = "Permissions.Departments.Remove";
        public const string Create = "Permissions.Departments.Create";
    }

    [DisplayName("UserLoginHistory")]
    [Description("User Login History Permissions")]
    public static class UserLoginHistory
    {
        public const string View = "Permissions.UserLoginHistory.View";
        public const string Search = "Permissions.UserLoginHistory.Search";
        public const string Create = "Permissions.UserLoginHistory.Create";
    }

    [DisplayName("Modules")]
    [Description("Module Management Permissions")]
    public static class ModuleManagements
    {
        public const string View = "Permissions.ModuleManagements.View";
        public const string Search = "Permissions.ModuleManagements.Search";
        public const string Update = "Permissions.ModuleManagements.Update";
        public const string Remove = "Permissions.ModuleManagements.Remove";
        public const string Create = "Permissions.ModuleManagements.Create";
    }

    [DisplayName("AdminGroups")]
    [Description("AdminGroups Permissions")]
    public static class AdminGroups
    {
        public const string View = "Permissions.AdminGroups.View";
        public const string Search = "Permissions.AdminGroups.Search";
        public const string Update = "Permissions.AdminGroups.Update";
        public const string Remove = "Permissions.AdminGroups.Remove";
        public const string Create = "Permissions.AdminGroups.Create";
    }

    [DisplayName("APIKeyPairs")]
    [Description("APIKeyPair Permissions")]
    public static class APIKeyPairs
    {
        public const string View = "Permissions.APIKeyPairs.View";
        public const string Search = "Permissions.APIKeyPairs.Search";
        public const string Update = "Permissions.APIKeyPairs.Update";
        public const string Remove = "Permissions.APIKeyPairs.Remove";
        public const string Create = "Permissions.APIKeyPairs.Create";
    }

    [DisplayName("SubUserAPIKeyPairs")]
    [Description("SubUserAPIKeyPairs Permissions")]
    public static class SubUserAPIKeyPairs
    {
        public const string View = "Permissions.SubUserAPIKeyPairs.View";
        public const string Search = "Permissions.SubUserAPIKeyPairs.Search";
        public const string Update = "Permissions.SubUserAPIKeyPairs.Update";
        public const string Remove = "Permissions.SubUserAPIKeyPairs.Remove";
        public const string Create = "Permissions.SubUserAPIKeyPairs.Create";
    }

    [DisplayName("Identity")]
    [Description("Identity Permissions")]
    public static class Identity
    {
        public const string Register = "Permissions.Identity.Register";
    }

    [DisplayName("Roles")]
    [Description("Roles Permissions")]
    public static class Roles
    {
        public const string View = "Permissions.Roles.View";
        public const string ListAll = "Permissions.Roles.ViewAll";
        public const string Register = "Permissions.Roles.Register";
        public const string Update = "Permissions.Roles.Update";
        public const string Remove = "Permissions.Roles.Remove";
    }

    [DisplayName("Users")]
    [Description("Users Permissions")]
    public static class Users
    {
        public const string View = "Permissions.Users.View";
        public const string ListAll = "Permissions.Users.ViewAll";
        public const string Register = "Permissions.Users.Register";
        public const string Update = "Permissions.Users.Update";
        public const string Remove = "Permissions.Users.Remove";
    }

    [DisplayName("SubUsers")]
    [Description("SubUsers Permissions")]
    public static class SubUsers
    {
        public const string View = "Permissions.SubUsers.View";
        public const string ListAll = "Permissions.SubUsers.ViewAll";
        public const string Create = "Permissions.SubUsers.Create";
        public const string Update = "Permissions.SubUsers.Update";
        public const string Remove = "Permissions.SubUsers.Remove";
    }

    [DisplayName("Articles")]
    [Description("Articles Permissions")]
    public static class Articles
    {
        public const string View = "Permissions.Articles.View";
        public const string Search = "Permissions.Articles.Search";
        public const string Create = "Permissions.Articles.Create";
        public const string Update = "Permissions.Articles.Update";
        public const string Remove = "Permissions.Articles.Remove";
    }

    [DisplayName("Settings")]
    [Description("Settings Permissions")]
    public static class Settings
    {
        public const string View = "Permissions.Settings.View";
        public const string Search = "Permissions.Settings.Search";
        public const string Register = "Permissions.Settings.Register";
        public const string Update = "Permissions.Settings.Update";
        public const string Remove = "Permissions.Settings.Remove";
    }

    [DisplayName("Admin")]
    [Description("Admin Permissions")]
    public static class Admin
    {
        public const string Maintenance = "Permissions.Admin.Maintenance";
        public const string AdminLoginAsClient = "Permissions.Admin.AdminLoginAsClient";
        public const string ClientLoginAsAdmin = "Permissions.Admin.ClientLoginAsAdmin";
    }

    [DisplayName("CronJobs")]
    [Description("CronJobs Permissions")]
    public static class CronJobs
    {
        public const string View = "Permissions.CronJobs.View";
        public const string Search = "Permissions.CronJobs.Search";
        public const string Register = "Permissions.CronJobs.Register";
        public const string Update = "Permissions.CronJobs.Update";
        public const string Remove = "Permissions.CronJobs.Remove";
    }

    [DisplayName("WebHooks")]
    [Description("WebHooks Permissions")]
    public static class WebHooks
    {
        public const string View = "Permissions.WebHooks.View";
        public const string Search = "Permissions.WebHooks.Search";
        public const string Create = "Permissions.WebHooks.Create";
        public const string Update = "Permissions.WebHooks.Update";
        public const string Remove = "Permissions.WebHooks.Remove";
    }

    [DisplayName("Categories")]
    [Description("Categories Permissions")]
    public static class Categories
    {
        public const string View = "Permissions.Categories.View";
        public const string Search = "Permissions.Categories.Search";
        public const string Register = "Permissions.Categories.Register";
        public const string Update = "Permissions.Categories.Update";
        public const string Remove = "Permissions.Categories.Remove";
        public const string Generate = "Permissions.Categories.Generate";
        public const string Clean = "Permissions.Categories.Clean";
    }

    [DisplayName("Role Claims")]
    [Description("Role Claims Permissions")]
    public static class RoleClaims
    {
        public const string View = "Permissions.RoleClaims.View";
        public const string Create = "Permissions.RoleClaims.Create";
        public const string Edit = "Permissions.RoleClaims.Edit";
        public const string Delete = "Permissions.RoleClaims.Delete";
        public const string Search = "Permissions.RoleClaims.Search";
    }

    [DisplayName("Brands")]
    [Description("Brands Permissions")]
    public static class Brands
    {
        public const string Search = "Permissions.Brands.Search";
        public const string Create = "Permissions.Brands.Create";
        public const string Read = "Permissions.Brands.Read";
        public const string Update = "Permissions.Brands.Update";
        public const string Delete = "Permissions.Brands.Delete";
    }

    [DisplayName("SMTP Configurations")]
    [Description("SMTP Configurations Permissions")]
    public static class SmtpConfigurations
    {
        public const string Search = "Permissions.SmtpConfigurations.Search";
        public const string ReadAll = "Permissions.SmtpConfigurations.ReadAll";
        public const string Create = "Permissions.SmtpConfigurations.Create";
        public const string Read = "Permissions.SmtpConfigurations.Read";
        public const string Update = "Permissions.SmtpConfigurations.Update";
        public const string Delete = "Permissions.SmtpConfigurations.Delete";
    }

    [DisplayName("Email templates")]
    [Description("Email templates Permissions")]
    public static class EmailTemplates
    {
        public const string Search = "Permissions.EmailTemplates.Search";
        public const string Create = "Permissions.EmailTemplates.Create";
        public const string Read = "Permissions.EmailTemplates.Read";
        public const string Update = "Permissions.EmailTemplates.Update";
        public const string Delete = "Permissions.EmailTemplates.Delete";
    }

    [DisplayName("Notifications")]
    [Description("Notifications Permissions")]
    public static class Notifications
    {
        public const string Search = "Permissions.Notifications.Search";
        public const string ReadAll = "Permissions.Notifications.ReadAll";
        public const string Create = "Permissions.Notifications.Create";
        public const string Read = "Permissions.Notifications.Read";
        public const string Update = "Permissions.Notifications.Update";
        public const string Delete = "Permissions.Notifications.Delete";
    }

    [DisplayName("Notification templates")]
    [Description("Notification templates Permissions")]
    public static class NotificationTemplates
    {
        public const string Search = "Permissions.NotificationTemplates.Search";
        public const string Create = "Permissions.NotificationTemplates.Create";
        public const string Read = "Permissions.NotificationTemplates.Read";
        public const string Update = "Permissions.NotificationTemplates.Update";
        public const string Delete = "Permissions.NotificationTemplates.Delete";
    }

    [DisplayName("Template variables")]
    [Description("Template variables Permissions")]
    public static class TemplateVariables
    {
        public const string Search = "Permissions.TemplateVariables.Search";
        public const string Create = "Permissions.TemplateVariables.Create";
        public const string Read = "Permissions.TemplateVariables.Read";
        public const string Update = "Permissions.TemplateVariables.Update";
        public const string Delete = "Permissions.TemplateVariables.Delete";
    }

    [DisplayName("Products")]
    [Description("Product Permissions")]
    public static class Products
    {
        public const string View = "Permissions.Products.View";
        public const string Search = "Permissions.Products.Search";
        public const string Update = "Permissions.Products.Update";
        public const string Remove = "Permissions.Products.Remove";
        public const string Create = "Permissions.Products.Create";
    }

    [DisplayName("Orders")]
    [Description("Orders Permissions")]
    public static class Orders
    {
        public const string View = "Permissions.Orders.View";
        public const string Search = "Permissions.Orders.Search";
        public const string Update = "Permissions.Orders.Update";
        public const string Remove = "Permissions.Orders.Remove";
        public const string Create = "Permissions.Orders.Create";
    }

    [DisplayName("Bills")]
    [Description("Bills Permissions")]
    public static class Bills
    {
        public const string View = "Permissions.Bills.View";
        public const string Search = "Permissions.Bills.Search";
        public const string Update = "Permissions.Bills.Update";
        public const string Remove = "Permissions.Bills.Remove";
        public const string Create = "Permissions.Bills.Create";
    }

    [DisplayName("Billing")]
    [Description("Billing Permissions")]
    public static class Billing
    {
        public const string View = "Permissions.Billing.View";
        public const string Search = "Permissions.Billing.Search";
        public const string Update = "Permissions.Billing.Update";
        public const string Remove = "Permissions.Billing.Remove";
        public const string Create = "Permissions.Billing.Create";
    }

    [DisplayName("Invoices")]
    [Description("Invoices Permissions")]
    public static class Invoices
    {
        public const string View = "Permissions.Invoices.View";
        public const string Search = "Permissions.Invoices.Search";
        public const string Update = "Permissions.Invoices.Update";
        public const string Remove = "Permissions.Invoices.Remove";
        public const string Create = "Permissions.Invoices.Create";
    }

    [DisplayName("Transactions")]
    [Description("Transactions Permissions")]
    public static class Transactions
    {
        public const string View = "Permissions.Transactions.View";
        public const string Search = "Permissions.Transactions.Search";
        public const string Update = "Permissions.Transactions.Update";
        public const string Remove = "Permissions.Transactions.Remove";
        public const string Create = "Permissions.Transactions.Create";
    }

    [DisplayName("Reports")]
    [Description("Reports Permissions")]
    public static class Reports
    {
        public const string View = "Permissions.Transactions.View";
    }

    [DisplayName("Audit logs")]
    [Description("Audit Logs")]
    public static class AuditLogs
    {
        public const string Search = "Permissions.Transactions.Search";
        public const string Read = "Permissions.Transactions.Read";
    }

    [DisplayName("Credits")]
    [Description("Credits Permissions")]
    public static class Credits
    {
        public const string View = "Permissions.Credits.View";
        public const string Search = "Permissions.Credits.Search";
        public const string Update = "Permissions.Credits.Update";
        public const string Remove = "Permissions.Credits.Remove";
        public const string Create = "Permissions.Credits.Create";
    }

    [DisplayName("Refunds")]
    [Description("Refunds Permissions")]
    public static class Refunds
    {
        public const string View = "Permissions.Refunds.View";
        public const string Search = "Permissions.Refunds.Search";
        public const string Update = "Permissions.Refunds.Update";
        public const string Remove = "Permissions.Refunds.Remove";
        public const string Create = "Permissions.Refunds.Create";
    }
}
