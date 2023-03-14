using MyReliableSite.Domain.Common.Contracts;

namespace MyReliableSite.Domain.Billing.Events.SmtpConfigurations;

public class SmtpConfigurationCreatedEvent : DomainEvent
{
    public SmtpConfigurationCreatedEvent(SmtpConfiguration smtpConfiguration)
    {
        SmtpConfiguration = smtpConfiguration;
    }

    public SmtpConfiguration SmtpConfiguration { get; }
}