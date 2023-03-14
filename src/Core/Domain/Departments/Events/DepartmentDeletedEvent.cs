using MyReliableSite.Domain.Common.Contracts;

namespace MyReliableSite.Domain.Departments.Events;

public class DepartmentDeletedEvent : DomainEvent
{
    public DepartmentDeletedEvent(Department department)
    {
        Department = department;
    }

    public Department Department { get; }
}
