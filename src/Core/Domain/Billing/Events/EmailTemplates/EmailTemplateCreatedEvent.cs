using MyReliableSite.Domain.Common.Contracts;

namespace MyReliableSite.Domain.Billing.Events.EmailTemplates;

public class EmailTemplateCreatedEvent : DomainEvent
{
    public EmailTemplateCreatedEvent(EmailTemplate emailTemplate)
    {
        EmailTemplate = emailTemplate;
    }

    public EmailTemplate EmailTemplate { get; }
}