namespace MyReliableSite.Shared.DTOs.Categories;

public class CategoryDto : IDto
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public string CategoryIcon { get; set; }
    public Guid ParentCategoryId { get; set; }
    public CategoryTypeRequest CategoryType { get; set; }
    public DateTime CreatedOn { get; set; }
}
