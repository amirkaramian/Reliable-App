using MyReliableSite.Domain.Common.Contracts;

namespace MyReliableSite.Domain.Departments.Events;

public class DepartmentCreatedEvent : DomainEvent
{
    public DepartmentCreatedEvent(Department department)
    {
        Department = department;
    }

    public Department Department { get; }
}
