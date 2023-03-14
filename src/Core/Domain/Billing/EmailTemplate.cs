using MyReliableSite.Domain.Common.Contracts;
using MyReliableSite.Domain.Contracts;
using MyReliableSite.Shared.DTOs.EmailTemplates;

namespace MyReliableSite.Domain.Billing;

public class EmailTemplate : AuditableEntity, IMustHaveTenant
{
    public new Guid? CreatedBy { get; set; }
    public string Subject { get; set; }
    public string Body { get; set; }
    public string JsonBody { get; set; }
    public string Variables { get; set; }
    public string Tenant { get; set; }
    public bool Status { get; set; }
    public Guid SmtpConfigurationId { get; set; }
    public bool IsSystem { get; set; }
    public EmailTemplateType EmailTemplateType { get; set; }

    public EmailTemplate()
    {
    }

    public EmailTemplate(Guid? createdBy, string subject, string body, string variables, string jsonBody, bool status, in Guid smtpConfigurationId, bool isSystem, EmailTemplateType emailTemplateType)
    {
        CreatedBy = createdBy;
        Subject = subject;
        Body = body;
        Status = status;
        SmtpConfigurationId = smtpConfigurationId;
        IsSystem = isSystem;
        EmailTemplateType = emailTemplateType;
        Variables = variables;
        JsonBody = jsonBody;
    }

    public EmailTemplate Update(string subject, string body, string variables, string jsonBody, bool status, in Guid smtpConfigurationId, bool isSystem, EmailTemplateType emailTemplateType)
    {
        if (smtpConfigurationId != Guid.Empty && !string.Equals(SmtpConfigurationId.ToString(), smtpConfigurationId.ToString(), StringComparison.CurrentCultureIgnoreCase)) SmtpConfigurationId = smtpConfigurationId;

        if (status != Status) Status = status;
        if (isSystem != IsSystem) IsSystem = isSystem;
        if (emailTemplateType != EmailTemplateType) EmailTemplateType = emailTemplateType;
        if (!string.IsNullOrWhiteSpace(subject) && !string.Equals(Subject, subject, StringComparison.InvariantCultureIgnoreCase)) Subject = subject;
        if (!string.IsNullOrWhiteSpace(body) && !string.Equals(Body, body, StringComparison.InvariantCultureIgnoreCase)) Body = body;
        if (!string.IsNullOrWhiteSpace(variables) && !string.Equals(Variables, variables, StringComparison.InvariantCultureIgnoreCase)) Variables = variables;
        if (!string.IsNullOrWhiteSpace(jsonBody) && !string.Equals(JsonBody, jsonBody, StringComparison.InvariantCultureIgnoreCase)) JsonBody = jsonBody;

        return this;
    }
}