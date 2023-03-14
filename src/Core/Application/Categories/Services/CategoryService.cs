using Microsoft.Extensions.Localization;
using MyReliableSite.Application.Categories.Interfaces;
using MyReliableSite.Application.Common.Interfaces;
using MyReliableSite.Application.Exceptions;
using MyReliableSite.Application.Storage;
using MyReliableSite.Application.Wrapper;
using MyReliableSite.Domain.Categories;
using MyReliableSite.Domain.Common.Contracts;
using MyReliableSite.Domain.Dashboard;
using MyReliableSite.Shared.DTOs.Categories;
using System.Linq.Expressions;

namespace MyReliableSite.Application.Categories.Services;

public class CategoryService : ICategoryService
{
    private readonly IStringLocalizer<CategoryService> _localizer;
    private readonly IRepositoryAsync _repository;
    private readonly IJobService _jobService;
    private readonly IFileStorageService _fileStorageService;
    public CategoryService(IRepositoryAsync repository, IFileStorageService fileStorageService, IStringLocalizer<CategoryService> localizer, IJobService jobService)
    {
        _repository = repository;
        _localizer = localizer;
        _jobService = jobService;
        _fileStorageService = fileStorageService;
    }

    public async Task<Result<Guid>> CreateCategoryAsync(CreateCategoryRequest request)
    {
        bool categoryExists = await _repository.ExistsAsync<Category>(a => a.Name == request.Name && a.CategoryType == (CategoryType)request.CategoryType);
        if (categoryExists) throw new EntityAlreadyExistsException(string.Format(_localizer["category.alreadyexists"], request.Name));
        string filename = await _fileStorageService.UploadAsync<Category>(request.CategoryIcon, Domain.Common.FileType.Image);
        var category = new Category(request.Name, request.Description, request.ParentCategoryId, (CategoryType)request.CategoryType, filename);
        category.DomainEvents.Add(new StatsChangedEvent());
        var categoryId = await _repository.CreateAsync<Category>((Category)category);
        await _repository.SaveChangesAsync();
        return await Result<Guid>.SuccessAsync(categoryId);
    }

    public async Task<Result<Guid>> DeleteCategoryAsync(Guid id)
    {
        var categoryToDelete = await _repository.RemoveByIdAsync<Category>(id);
        categoryToDelete.DomainEvents.Add(new StatsChangedEvent());
        await _repository.SaveChangesAsync();
        return await Result<Guid>.SuccessAsync(id);
    }

    public async Task<Result<CategoryDto>> GetCategoryAsync(Guid id)
    {
        var category = await _repository.GetByIdAsync<Category, CategoryDto>(id);
        category.CategoryIcon = await _fileStorageService.ReturnBase64StringOfImageFileAsync(category.CategoryIcon);

        return await Result<CategoryDto>.SuccessAsync(category);
    }

    public async Task<PaginatedResult<CategoryDto>> SearchAsync(CategoryListFilter filter)
    {
        var categories = await _repository.GetSearchResultsAsync<Category, CategoryDto>(filter.PageNumber, filter.PageSize, filter.OrderBy, filter.OrderType, filter.AdvancedSearch, filter.Keyword, FilterByCategoryType((CategoryType)filter.CategoryType));
        foreach (var item in categories.Data)
        {
            item.CategoryIcon = await _fileStorageService.ReturnBase64StringOfImageFileAsync(item.CategoryIcon);
        }

        return categories;
    }

    public async Task<PaginatedResult<CategoryDto>> SearchParentsAsync(CategoryListFilter filter)
    {
        var categories = await _repository.GetSearchResultsAsync<Category, CategoryDto>(filter.PageNumber, filter.PageSize, filter.OrderBy, filter.OrderType, filter.AdvancedSearch, filter.Keyword, FilterParentByCategoryType((CategoryType)filter.CategoryType));
        foreach (var item in categories.Data)
        {
            item.CategoryIcon = await _fileStorageService.ReturnBase64StringOfImageFileAsync(item.CategoryIcon);
        }

        return categories;
    }

    public async Task<Result<List<Category>>> GetChildCategory(Guid id)
    {
        var categories = await _repository.GetListAsync<Category>(x => x.ParentCategoryId == id);
        return await Result<List<Category>>.SuccessAsync(categories);
    }

    public async Task<PaginatedResult<CategoryDto>> SearchChildrenCategoryAsync(CategoryListFilter filter, Guid id)
    {
        var categories = await _repository.GetSearchResultsAsync<Category, CategoryDto>(filter.PageNumber, filter.PageSize, filter.OrderBy, filter.OrderType, filter.AdvancedSearch, filter.Keyword, x => x.ParentCategoryId == id);
        foreach (var item in categories.Data)
        {
            item.CategoryIcon = await _fileStorageService.ReturnBase64StringOfImageFileAsync(item.CategoryIcon);
        }

        return categories;
    }

    public async Task<Result<Guid>> UpdateCategoryAsync(UpdateCategoryRequest request, Guid id)
    {
        var category = await _repository.GetByIdAsync<Category>(id);
        if (category == null) throw new EntityNotFoundException(string.Format(_localizer["category.notfound"], id));
        string filename = await _fileStorageService.UploadAsync<Category>(request.CategoryIcon, Domain.Common.FileType.Image);

        var updatedCategory = category.Update(request.Name, request.Description, request.ParentCategoryId, filename);
        updatedCategory.DomainEvents.Add(new StatsChangedEvent());
        await _repository.UpdateAsync<Category>(updatedCategory);
        await _repository.SaveChangesAsync();
        return await Result<Guid>.SuccessAsync(id);
    }

    public async Task<Result<string>> GenerateRandomCategoryAsync(GenerateRandomCategoryRequest request)
    {
        string jobId = _jobService.Enqueue<ICategoryGeneratorJob>(x => x.GenerateAsync(request.NSeed));
        return await Result<string>.SuccessAsync(jobId);
    }

    public async Task<Result<string>> DeleteRandomCategoryAsync()
    {
        string jobId = _jobService.Schedule<ICategoryGeneratorJob>(x => x.CleanAsync(), TimeSpan.FromSeconds(5));
        return await Result<string>.SuccessAsync(jobId);
    }

    private Expression<Func<Category, bool>> FilterByCategoryType(CategoryType categoryType)
    {
        return x => x.CategoryType.Equals(categoryType);
    }

    private Expression<Func<Category, bool>> FilterParentByCategoryType(CategoryType categoryType)
    {
        return x => x.CategoryType.Equals(categoryType) && (x.ParentCategoryId == null || x.ParentCategoryId == Guid.Empty);
    }
}
