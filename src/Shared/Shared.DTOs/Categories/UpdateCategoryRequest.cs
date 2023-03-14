using MyReliableSite.Shared.DTOs.Storage;

namespace MyReliableSite.Shared.DTOs.Categories;

public class UpdateCategoryRequest : IMustBeValid
{
    public string Name { get; set; }
    public string Description { get; set; }
    public Guid? ParentCategoryId { get; set; }
    public FileUploadRequest CategoryIcon { get; set; }
}
