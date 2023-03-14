using MyReliableSite.Application.Common.Interfaces;
using MyReliableSite.Application.Wrapper;
using MyReliableSite.Domain.KnowledgeBase;
using MyReliableSite.Shared.DTOs.KnowledgeBase;
using MyReliableSite.Shared.DTOs.Transaction;

namespace MyReliableSite.Application.KnowledgeBase.Interfaces;

public interface IArticleService : ITransientService
{
    Task<Result<ArticleDetailsDto>> GetArticleDetailsAsync(Guid id);

    Task<PaginatedResult<ArticleDto>> SearchAsync(ArticleListFilter filter);

    Task<Result<Guid>> CreateArticleAsync(CreateArticleRequest request);

    Task<Result<Guid>> UpdateArticleAsync(UpdateArticleRequest request, Guid id);

    Task<Result<Guid>> DeleteArticleAsync(Guid id);
    Task<Result<int>> GetUserSubmissionCount();
    Task<PaginatedResult<ArticleDto>> SearchSubmissionsAsync(ArticleListFilter filter);
    Task<Result<Guid>> SubmitArticleAsync(CreateArticleRequest request);

    Task<Result<List<ArticleEXL>>> GetArticlesListAsync(string userId, DateTime startDate, DateTime endDate);
    Task<PaginatedResult<ArticleDto>> SearchUserSubmissionsAsync(ArticleListFilter filter);
    Task<Result<Guid>> ApproveUserSubmissionAsync(Guid id);
}