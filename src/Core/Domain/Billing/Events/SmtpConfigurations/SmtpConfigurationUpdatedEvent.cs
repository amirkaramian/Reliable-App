using MyReliableSite.Domain.Common.Contracts;

namespace MyReliableSite.Domain.Billing.Events.SmtpConfigurations;

public class SmtpConfigurationUpdatedEvent : DomainEvent
{
    public SmtpConfigurationUpdatedEvent(SmtpConfiguration smtpConfiguration)
    {
        SmtpConfiguration = smtpConfiguration;
    }

    public SmtpConfiguration SmtpConfiguration { get; }
}