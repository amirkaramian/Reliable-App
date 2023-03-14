namespace MyReliableSite.Shared.DTOs.EmailTemplates;

public class EmailTemplateDto : IDto
{
    public Guid Id { get; set; }
    public Guid? CreatedBy { get; set; }
    public string Subject { get; set; }
    public string Body { get; set; }
    public string JsonBody { get; set; }
    public string Variables { get; set; }
    public string Tenant { get; set; }
    public bool Status { get; set; }
    public Guid SmtpConfigurationId { get; set; }
    public DateTime CreatedOn { get; set; }
    public bool IsSystem { get; set; }
    public EmailTemplateType EmailTemplateType { get; set; }
}

public enum EmailTemplateType
{
    General,
    EmailConfirmation,
    EmailOTP,
    ProductCancellation,
    ResetPassword,
    TicketUpdated,
    TicketCreated,
    TicketAssignment,
    Orders,
    Invoice,
    ProductStatusUpdated,
    OrderCreated,
    OrderAssignment,
    ProductCreated,
    ProductAssignment,
}
