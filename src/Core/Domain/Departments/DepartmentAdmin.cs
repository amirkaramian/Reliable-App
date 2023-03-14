using MyReliableSite.Domain.Common.Contracts;
using MyReliableSite.Domain.Contracts;

namespace MyReliableSite.Domain.Departments;

public class DepartmentAdmin : BaseEntity, IMustHaveTenant
{
    public string Tenant { get; set; }
    public Guid DepartmentId { get; set; }
    public Department Department { get; set; }

    public Guid AdminUserId { get; set; }
}