using MyReliableSite.Domain.Common.Contracts;

namespace MyReliableSite.Domain.Billing.Events.EmailTemplates;

public class EmailTemplateDeletedEvent : DomainEvent
{
    public EmailTemplateDeletedEvent(EmailTemplate emailTemplate)
    {
        EmailTemplate = emailTemplate;
    }

    public EmailTemplate EmailTemplate { get; }
}