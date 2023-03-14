using MyReliableSite.Application.Common.Interfaces;
using MyReliableSite.Application.Wrapper;
using MyReliableSite.Domain.Categories;
using MyReliableSite.Shared.DTOs.Categories;

namespace MyReliableSite.Application.Categories.Interfaces;

public interface ICategoryService : ITransientService
{
    Task<PaginatedResult<CategoryDto>> SearchAsync(CategoryListFilter filter);

    Task<Result<Guid>> CreateCategoryAsync(CreateCategoryRequest request);

    Task<Result<Guid>> UpdateCategoryAsync(UpdateCategoryRequest request, Guid id);

    Task<Result<Guid>> DeleteCategoryAsync(Guid id);

    Task<Result<string>> GenerateRandomCategoryAsync(GenerateRandomCategoryRequest request);

    Task<Result<string>> DeleteRandomCategoryAsync();

    Task<Result<CategoryDto>> GetCategoryAsync(Guid id);
    Task<PaginatedResult<CategoryDto>> SearchParentsAsync(CategoryListFilter filter);
    Task<PaginatedResult<CategoryDto>> SearchChildrenCategoryAsync(CategoryListFilter filter, Guid id);
    Task<Result<List<Category>>> GetChildCategory(Guid id);
}
