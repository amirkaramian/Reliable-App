using MyReliableSite.Domain.Common.Contracts;

namespace MyReliableSite.Domain.Billing.Events.SmtpConfigurations;

public class SmtpConfigurationDeletedEvent : DomainEvent
{
    public SmtpConfigurationDeletedEvent(SmtpConfiguration smtpConfiguration)
    {
        SmtpConfiguration = smtpConfiguration;
    }

    public SmtpConfiguration SmtpConfiguration { get; }
}