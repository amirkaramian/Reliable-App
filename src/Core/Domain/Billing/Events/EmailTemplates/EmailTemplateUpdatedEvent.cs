using MyReliableSite.Domain.Common.Contracts;

namespace MyReliableSite.Domain.Billing.Events.EmailTemplates;

public class EmailTemplateUpdatedEvent : DomainEvent
{
    public EmailTemplateUpdatedEvent(EmailTemplate emailTemplate)
    {
        EmailTemplate = emailTemplate;
    }

    public EmailTemplate EmailTemplate { get; }
}