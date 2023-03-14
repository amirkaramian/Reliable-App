using MyReliableSite.Domain.Common.Contracts;

namespace MyReliableSite.Domain.Departments.Events;

public class DepartmentUpdatedEvent : DomainEvent
{
    public DepartmentUpdatedEvent(Department department)
    {
        Department = department;
    }

    public Department Department { get; }
}
