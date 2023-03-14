namespace MyReliableSite.Shared.DTOs.Departments;

public class UpdateDepartmentRequest : IMustBeValid
{
    public Guid BrandId { get; set; }
    public string Name { get; set; }
    public string DeptNumber { get; set; }
    public bool DeptStatus { get; set; }
    public bool IsDefault { get; set; }
    public List<Guid> DepartmentAdmins { get; set; }

}