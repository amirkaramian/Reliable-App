using MyReliableSite.Shared.DTOs.Filters;

namespace MyReliableSite.Shared.DTOs.Categories;

public class CategoryListFilter : PaginationFilter
{
    public CategoryTypeRequest CategoryType { get; set; }
}
