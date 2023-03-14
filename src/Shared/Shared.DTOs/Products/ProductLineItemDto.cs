using MyReliableSite.Shared.DTOs.Categories;
using MyReliableSite.Shared.DTOs.Departments;

namespace MyReliableSite.Shared.DTOs.Products;
public class ProductLineItemDto : IDto
{
    public Guid Id { get; set; }
    public string LineItem { get; set; }
    public decimal Price { get; set; }
    public DateTime? CreatedOn { get; set; }
    public DateTime? LastModifiedOn { get; set; }
    public PriceType PriceType { get; set; }
    public DateTime? DeletedOn { get; set; }
    public Guid? DeletedBy { get; set; }
}

public class OrderTemplateLineItemDto : IDto
{
    public Guid Id { get; set; }
    public string LineItem { get; set; }
    public decimal Price { get; set; }
    public PriceType PriceType { get; set; }
    public DateTime? CreatedOn { get; set; }
    public DateTime? LastModifiedOn { get; set; }
    public DateTime? DeletedOn { get; set; }
    public Guid? DeletedBy { get; set; }
}

public class OrderTemplateCategoryDto : IDto
{
    public Guid Id { get; set; }
    public Guid OrderTemplateId { get; set; }
    public Guid CategoryId { get; set; }
    public CategoryDto Category { get; set; }

}

public class OrderTemplateDepartmentDto : IDto
{
    public Guid Id { get; set; }

    public Guid OrderTemplateId { get; set; }
    public Guid DepartmentId { get; set; }
    public DepartmentDto Department { get; set; }
}