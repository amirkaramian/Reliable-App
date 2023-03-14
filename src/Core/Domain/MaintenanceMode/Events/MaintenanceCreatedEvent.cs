using MyReliableSite.Domain.Common.Contracts;

namespace MyReliableSite.Domain.MaintenanceMode.Events;

public class MaintenanceCreatedEvent : DomainEvent
{
    public MaintenanceCreatedEvent(MaintenanceMode maintenanceMode)
    {
        MaintenanceMode = maintenanceMode;
    }

    public MaintenanceMode MaintenanceMode { get; }
}