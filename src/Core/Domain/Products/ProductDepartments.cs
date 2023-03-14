using MyReliableSite.Domain.Billing;
using MyReliableSite.Domain.Common.Contracts;
using MyReliableSite.Domain.Contracts;
using MyReliableSite.Domain.Departments;

namespace MyReliableSite.Domain.Products;
public class ProductDepartments : AuditableEntity, IMustHaveTenant
{
    public Guid ProductId { get; set; }
    public virtual Product Product { get; set; }
    public Guid DepartmentId { get; set; }
    public virtual Department Department { get; set; }
    public string Tenant { get; set; }

    public ProductDepartments()
    {

    }

    public ProductDepartments(Guid departmentId)
    {
        DepartmentId = departmentId;
    }

    public ProductDepartments(Guid departmentId, Guid productId)
    {
        DepartmentId = departmentId;
        ProductId = productId;
    }

    public ProductDepartments Update(Guid departmentId)
    {
        if (DepartmentId != Guid.Empty && !string.Equals(DepartmentId.ToString(), departmentId.ToString(), StringComparison.CurrentCultureIgnoreCase)) DepartmentId = departmentId;
        return this;
    }
}

public class OrderTemplateDepartments : AuditableEntity, IMustHaveTenant
{
    public Guid OrderTemplateId { get; set; }
    public virtual OrderTemplate OrderTemplate { get; set; }
    public Guid DepartmentId { get; set; }
    public virtual Department Department { get; set; }
    public string Tenant { get; set; }

    public OrderTemplateDepartments()
    {

    }

    public OrderTemplateDepartments(Guid departmentId)
    {
        DepartmentId = departmentId;
    }

    public OrderTemplateDepartments(Guid departmentId, Guid orderTemplateId)
    {
        DepartmentId = departmentId;
        OrderTemplateId = orderTemplateId;
    }

    public OrderTemplateDepartments Update(Guid departmentId)
    {
        if (DepartmentId != Guid.Empty && !string.Equals(DepartmentId.ToString(), departmentId.ToString(), StringComparison.CurrentCultureIgnoreCase)) DepartmentId = departmentId;
        return this;
    }
}

