using Mapster;
using Microsoft.Extensions.Localization;
using MyReliableSite.Application.KnowledgeBase.Interfaces;
using MyReliableSite.Application.Common.Interfaces;
using MyReliableSite.Application.Categories.Interfaces;
using MyReliableSite.Application.Exceptions;
using MyReliableSite.Application.Specifications;
using MyReliableSite.Application.Storage;
using MyReliableSite.Application.Wrapper;
using MyReliableSite.Domain.KnowledgeBase;
using MyReliableSite.Domain.KnowledgeBase.Events;
using MyReliableSite.Domain.Common;
using MyReliableSite.Domain.Common.Contracts;
using MyReliableSite.Domain.Dashboard;
using MyReliableSite.Shared.DTOs.KnowledgeBase;
using MyReliableSite.Domain.Categories;
using MyReliableSite.Domain.Billing;
using MyReliableSite.Shared.DTOs.Transaction;
using MyReliableSite.Shared.DTOs.Auditing;
using MyReliableSite.Application.Abstractions.Services.Identity;

namespace MyReliableSite.Application.KnowledgeBase.Services;

public class ArticleService : IArticleService
{
    private readonly IStringLocalizer<ArticleService> _localizer;
    private readonly IFileStorageService _file;
    private readonly IRepositoryAsync _repository;
    private readonly IJobService _jobService;
    private readonly ICategoryService _categoryService;
    private readonly ICurrentUser _currentUser;

    public ArticleService(IRepositoryAsync repository, IStringLocalizer<ArticleService> localizer, IFileStorageService file, IJobService jobService, ICategoryService categoryService, ICurrentUser currentUser)
    {
        _repository = repository;
        _localizer = localizer;
        _file = file;
        _jobService = jobService;
        _categoryService = categoryService;
        _currentUser = currentUser;
    }

    public async Task<Result<Guid>> CreateArticleAsync(CreateArticleRequest request)
    {
        bool articleExists = await _repository.ExistsAsync<Article>(a => a.Title == request.Title);
        if (articleExists) throw new EntityAlreadyExistsException(string.Format(_localizer["article.alreadyexists"], request.Title));
        var file = _file.PrepareFile(request.Image);
        string articleImagePath = string.Empty;
        if(file != null)
        {
            var uploadedFile = await _repository.CreateAsync(file);
            articleImagePath = uploadedFile.ToString();
        }

        var article = new Article(request.Title, request.BodyText, request.Visibility, articleImagePath, request.ArticleStatus);

        foreach (Guid id in request.Categories)
        {
            var category = await _repository.GetByIdAsync<Category>(id);
            if (category != null)
            {
                article.ArticleCategories.Add(new ArticleCategory() { Category = category, Article = article });
            }
        }

        var brands = await _repository.GetListAsync<Brand>(m => request.BrandIds.Contains(m.Id));
        foreach (var brandId in request.BrandIds)
        {
            var existingBrand = await _repository.GetByIdAsync<Brand>(brandId);

            if (existingBrand == null)
                throw new EntityNotFoundException(string.Format(_localizer["brand.notfound"], brandId));

            article.BrandArticles.Add(new BrandArticle() { Brand = existingBrand, BrandId = existingBrand.Id, Article = article, Articleid = article.Id });
        }

        // Add Domain Events to be raised after the commit
        article.DomainEvents.Add(new ArticleCreatedEvent(article));
        article.DomainEvents.Add(new StatsChangedEvent());

        var articleId = await _repository.CreateAsync(article);
        await _repository.SaveChangesAsync();
        return await Result<Guid>.SuccessAsync(articleId);
    }

    public async Task<Result<Guid>> SubmitArticleAsync(CreateArticleRequest request)
    {
        request.Visibility = false;
        request.ArticleStatus = "submitted";
        bool articleExists = await _repository.ExistsAsync<Article>(a => a.Title == request.Title);
        if (articleExists) throw new EntityAlreadyExistsException(string.Format(_localizer["article.alreadyexists"], request.Title));

        var file = _file.PrepareFile(request.Image);
        string articleImagePath = string.Empty;
        if (file != null)
        {
            var uploadedFile = await _repository.CreateAsync(file);
            articleImagePath = uploadedFile.ToString();
        }

        var article = new Article(request.Title, request.BodyText, request.Visibility, articleImagePath, request.ArticleStatus);

        foreach (Guid id in request.Categories)
        {
            var category = await _repository.GetByIdAsync<Category>(id);
            if (category != null)
            {
                article.ArticleCategories.Add(new ArticleCategory() { Category = category, Article = article });
            }
        }

        var brands = await _repository.GetListAsync<Brand>(m => request.BrandIds.Contains(m.Id));
        foreach (var brandId in request.BrandIds)
        {
            var existingBrand = await _repository.GetByIdAsync<Brand>(brandId);

            if (existingBrand == null)
                throw new EntityNotFoundException(string.Format(_localizer["brand.notfound"], brandId));

            article.BrandArticles.Add(new BrandArticle() { Brand = existingBrand, BrandId = existingBrand.Id, Article = article, Articleid = article.Id });
        }

        // Add Domain Events to be raised after the commit
        article.DomainEvents.Add(new ArticleCreatedEvent(article));
        article.DomainEvents.Add(new StatsChangedEvent());

        var articleId = await _repository.CreateAsync(article);
        await _repository.SaveChangesAsync();
        return await Result<Guid>.SuccessAsync(articleId);
    }

    public async Task<Result<Guid>> UpdateArticleAsync(UpdateArticleRequest request, Guid id)
    {
        var spec = new BaseSpecification<Article>();
        spec.Includes.Add(m => m.ArticleCategories);
        var article = await _repository.GetByIdAsync<Article>(id, spec);
        if (article == null) throw new EntityNotFoundException(string.Format(_localizer["article.notfound"], id));
        string articleImagePath = null;
        var file = _file.PrepareFile(request.Image);
        if(file != null)
        {
            var uploadedFile = await _repository.CreateAsync(file);
            articleImagePath = uploadedFile.ToString();
        }

        var updatedArticle = article.Update(request.Title, request.BodyText, request.Visibility, articleImagePath, request.ArticleStatus);

        if(request.Categories?.Count > 0)
        {
            await _repository.ClearAsync<ArticleCategory>(x => x.ArticleId == id);
        }

        // Update Product Categories
        foreach (var articleCategoryId in request.Categories)
        {
                var newcategory = await _repository.GetByIdAsync<Category>(articleCategoryId);
                if (newcategory != null)
                {
                    await _repository.CreateAsync(new ArticleCategory() { Category = newcategory, Article = article });
                    updatedArticle.LastModifiedOn = DateTime.UtcNow;
                }
        }

        // Add Domain Events to be raised after the commit
        updatedArticle.DomainEvents.Add(new ArticleUpdatedEvent(updatedArticle));
        updatedArticle.DomainEvents.Add(new StatsChangedEvent());

        await _repository.UpdateAsync(updatedArticle);
        await _repository.SaveChangesAsync();

        if (updatedArticle.BrandArticles != null && updatedArticle.BrandArticles.Count > 0)
        {
            await _repository.ClearAsync<BrandArticle>(m => m.Articleid == id);
        }

        foreach (var item in request.BrandIds)
        {
            var brand = await _repository.GetByIdAsync<Brand>(item);

            if (brand == null)
                throw new EntityNotFoundException(string.Format(_localizer["brand.notfound"], brand.Id));

            if (updatedArticle.BrandArticles == null)
            {
                updatedArticle.BrandArticles = new List<BrandArticle>();
            }

            updatedArticle.BrandArticles.Add(new BrandArticle()
            {
                Article = updatedArticle,
                Articleid = updatedArticle.Id,
                Brand = brand,
                BrandId = item
            });
        }

        await _repository.CreateRangeAsync<BrandArticle>(updatedArticle.BrandArticles);

        return await Result<Guid>.SuccessAsync(id);
    }

    public async Task<Result<Guid>> DeleteArticleAsync(Guid id)
    {
        var articleToDelete = await _repository.RemoveByIdAsync<Article>(id);
        articleToDelete.DomainEvents.Add(new ArticleDeletedEvent(articleToDelete));
        articleToDelete.DomainEvents.Add(new StatsChangedEvent());
        await _repository.SaveChangesAsync();
        return await Result<Guid>.SuccessAsync(id);
    }

    public async Task<Result<ArticleDetailsDto>> GetArticleDetailsAsync(Guid id)
    {
        var spec = new BaseSpecification<Article>();
        spec.IncludeStrings.Add("ArticleCategories.Category");
        spec.IncludeStrings.Add("BrandArticles.Brand");
        var article = await _repository.GetByIdAsync<Article, ArticleDetailsDto>(id, spec);
        if (!string.IsNullOrEmpty(article.ImagePath))
        {
            var image = await _repository.GetByIdAsync<ServerFile>(GetGuid(article.ImagePath));
            article.Base64Image = _file.PrepareDataForImage(image);
        }

        foreach (var item in article.ArticleCategories)
        {

        }

        _jobService.Enqueue(() => AddViewsAsync(id));
        return await Result<ArticleDetailsDto>.SuccessAsync(article);
    }

    public async Task<PaginatedResult<ArticleDto>> SearchAsync(ArticleListFilter filter)
    {
        var filters = new Filters<Article>();

        filters.Add(filter.BrandId.HasValue, x => x.BrandArticles.Any(m => m.BrandId.Equals(filter.BrandId.Value)));
        filters.Add(_currentUser.GetTenant().ToLower().Equals("client"), x => x.Visibility);
        if (filter.CategoryId.HasValue)
        {
            var categories = await _categoryService.GetChildCategory(filter.CategoryId.Value);
            var categoryIds = categories.Data.ConvertAll(c => c.Id);
            categoryIds.Add(filter.CategoryId.Value);
            filters.Add(true, x => x.ArticleCategories.Select(c => c.CategoryId).Any(g => categoryIds.Contains(g)));
        }

        filters.Add(filter.StartDate.HasValue, x => x.CreatedOn >= filter.StartDate);
        filters.Add(filter.EndDate.HasValue, x => x.CreatedOn <= filter.EndDate);
        var spec = new BaseSpecification<Article>();
        spec.IncludeStrings.Add("ArticleCategories.Category");
        spec.IncludeStrings.Add("BrandArticles.Brand");
        var articles = await _repository.GetSearchResultsAsync<Article, ArticleDto>(filter.PageNumber, filter.PageSize, filter.OrderBy, filter.OrderType, filters, filter.AdvancedSearch, filter.Keyword, spec);

        foreach (var article in articles.Data)
        {
            if (!string.IsNullOrEmpty(article.ImagePath))
            {
                var image = await _repository.GetByIdAsync<ServerFile>(GetGuid(article.ImagePath));
                article.Base64Image = _file.PrepareDataForImage(image);
            }
        }

        return articles;
    }

    public async Task<PaginatedResult<ArticleDto>> SearchSubmissionsAsync(ArticleListFilter filter)
    {
        var filters = new Filters<Article>();

        filters.Add(filter.BrandId.HasValue, x => x.BrandArticles.Any(m => m.BrandId.Equals(filter.BrandId.Value)));

        filters.Add(filter.CategoryId.HasValue, x => x.ArticleCategories.Any(m => m.CategoryId.Equals(filter.CategoryId.Value)));

        filters.Add(filter.StartDate.HasValue, x => x.CreatedOn >= filter.StartDate);
        filters.Add(filter.EndDate.HasValue, x => x.CreatedOn <= filter.EndDate);
        filters.Add(true, x => x.ArticleStatus == "submitted");
        var spec = new BaseSpecification<Article>();
        spec.IncludeStrings.Add("ArticleCategories.Category");
        spec.IncludeStrings.Add("BrandArticles.Brand");
        var articles = await _repository.GetSearchResultsAsync<Article, ArticleDto>(filter.PageNumber, filter.PageSize, filter.OrderBy, filter.OrderType, filters, filter.AdvancedSearch, filter.Keyword, spec);

        foreach (var article in articles.Data)
        {
            if (!string.IsNullOrEmpty(article.ImagePath))
            {
                var image = await _repository.GetByIdAsync<ServerFile>(GetGuid(article.ImagePath));
                article.Base64Image = _file.PrepareDataForImage(image);
            }
        }

        return articles;
    }

    public async Task<PaginatedResult<ArticleDto>> SearchUserSubmissionsAsync(ArticleListFilter filter)
    {
        var filters = new Filters<Article>();

        filters.Add(filter.BrandId.HasValue, x => x.BrandArticles.Any(m => m.BrandId.Equals(filter.BrandId.Value)));

        filters.Add(filter.CategoryId.HasValue, x => x.ArticleCategories.Any(m => m.CategoryId.Equals(filter.CategoryId.Value)));

        filters.Add(filter.StartDate.HasValue, x => x.CreatedOn >= filter.StartDate);
        filters.Add(filter.EndDate.HasValue, x => x.CreatedOn <= filter.EndDate);
        filters.Add(true, x => x.ArticleStatus == "submitted");
        filters.Add(true, x => x.CreatedBy.Equals(_currentUser.GetUserId()));
        var spec = new BaseSpecification<Article>();
        spec.IncludeStrings.Add("ArticleCategories.Category");
        spec.IncludeStrings.Add("BrandArticles.Brand");
        var articles = await _repository.GetSearchResultsAsync<Article, ArticleDto>(filter.PageNumber, filter.PageSize, filter.OrderBy, filter.OrderType, filters, filter.AdvancedSearch, filter.Keyword, spec);

        foreach (var article in articles.Data)
        {
            if (!string.IsNullOrEmpty(article.ImagePath))
            {
                var image = await _repository.GetByIdAsync<ServerFile>(GetGuid(article.ImagePath));
                article.Base64Image = _file.PrepareDataForImage(image);
            }
        }

        return articles;
    }

    public async Task<Result<int>> GetUserSubmissionCount()
    {
        int count = await _repository.GetCountAsync<Article>(x => x.ArticleStatus == "submitted");
        return await Result<int>.SuccessAsync(count);
    }

    public async Task<Result<List<ArticleEXL>>> GetArticlesListAsync(string userId, DateTime startDate, DateTime endDate)
    {

        var articles = await _repository.QueryWithDtoAsync<ArticleEXL>($@"SELECT A.*
                                                                                                        FROM Articles A
                                                                                                        WHERE ((CONVERT(date, [A].[CreatedOn]) >= '{startDate}') AND (CONVERT(date, [A].[CreatedOn]) <= '{endDate}')) and DeletedOn is null and CreatedBy = '{userId}' ORDER BY A.CreatedOn ASC");
        return await Result<List<ArticleEXL>>.SuccessAsync(articles.ToList());
    }

    public async Task<Result<Guid>> ApproveUserSubmissionAsync(Guid id)
    {
        var article = await _repository.GetByIdAsync<Article>(id);
        if (article == null) throw new EntityNotFoundException(string.Format(_localizer["article.notfound"], id));
        article.Visibility = true;
        article.ArticleStatus = "publish";
        await _repository.UpdateAsync(article);
        await _repository.SaveChangesAsync();
        article.DomainEvents.Add(new ArticleUpdatedEvent(article));
        article.DomainEvents.Add(new StatsChangedEvent());
        return await Result<Guid>.SuccessAsync(id);
    }

    public async Task<Result<Guid>> AddViewsAsync(Guid id)
    {
        var article = await _repository.GetByIdAsync<Article>(id);
        article.Views++;
        await _repository.UpdateAsync(article);
        await _repository.SaveChangesAsync();
        return await Result<Guid>.SuccessAsync(id);
    }

    private Guid GetGuid(string id)
    {
        Guid guid = Guid.Empty;
        if (!string.IsNullOrEmpty(id)) Guid.TryParse(id, out guid);
        return guid;
    }
}
