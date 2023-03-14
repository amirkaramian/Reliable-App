namespace MyReliableSite.Shared.DTOs.Departments;

public class DepartmentDto : IDto
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string DeptNumber { get; set; }
    public bool DeptStatus { get; set; }
    public bool IsDefault { get; set; }
    public Guid BrandId { get; set; }
    public List<Guid> DepartmentAdminsList { get; set; }

}

public class DepartmentAdminDto : IDto
{
    public Guid DepartmentId { get; set; }

    public Guid AdminUserId { get; set; }
}

public class DepartmentAdminAssignStatusDto : IDto
{
    public string DepartmentName { get; set; }
    public Guid DepartmentId { get; set; }

    public bool isAssign { get; set; }
}