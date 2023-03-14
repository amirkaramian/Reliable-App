using MyReliableSite.Domain.Common.Contracts;

namespace MyReliableSite.Domain.Billing.Events;

public class CronJobsCreatedEvent : DomainEvent
{
    public CronJobsCreatedEvent(CronJobs cronJobs)
    {
        CronJobs = cronJobs;
    }

    public CronJobs CronJobs { get; }
}