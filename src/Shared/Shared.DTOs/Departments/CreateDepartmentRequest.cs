using System.ComponentModel.DataAnnotations;

namespace MyReliableSite.Shared.DTOs.Departments;

public class CreateDepartmentRequest : IMustBeValid
{
    public Guid BrandId { get; set; }
    public string Name { get; set; }
    public string DeptNumber { get; set; }
    public bool DeptStatus { get; set; }
    public List<Guid> DepartmentAdmins { get; set; }
    public bool IsDefault { get; set; }
}

public class AssignDepartmentRequest : IMustBeValid
{
    [Required]
    public Guid DepartmentId { get; set; }
    [Required]
    public Guid UserId { get; set; }
}
