using MyReliableSite.Domain.Billing;
using MyReliableSite.Domain.Common;
using MyReliableSite.Domain.Common.Contracts;
using MyReliableSite.Domain.Contracts;
using MyReliableSite.Domain.Tickets;

namespace MyReliableSite.Domain.Departments;

public class Department : AuditableEntity, IMustHaveTenant
{
    public string Name { get; set; }
    public string DeptNumber { get; set; }
    public bool DeptStatus { get; set; }
    public bool IsDefault { get; set; }
    public string Tenant { get; set; }
    public Guid BrandId { get; set; }
    public List<DepartmentAdmin> DepartmentAdmins { get; set; }
    public virtual ICollection<Ticket> Tickets { get; set; }
    public virtual Brand Brand { get; set; }

    public Department()
    {
    }

    public Department(string name, string deptNumber, bool deptStatus, bool isDefault, Guid brandId)
    {
        Name = name;
        DeptNumber = deptNumber;
        DeptStatus = deptStatus;
        IsDefault = isDefault;
        BrandId = brandId;
    }

    public Department Update(string name, string deptNumber, bool deptStatus, bool isDefault, Guid brandId)
    {
        if (name != null && !Name.NullToString().Equals(name)) Name = name;
        if (deptNumber != null && !DeptNumber.NullToString().Equals(deptNumber)) DeptNumber = deptNumber;
        if (deptStatus != DeptStatus) DeptStatus = deptStatus;
        if (isDefault != IsDefault) IsDefault = isDefault;
        if (brandId != BrandId) BrandId = brandId;
        return this;
    }
}
