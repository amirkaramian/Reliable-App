using MyReliableSite.Application.Common.Interfaces;
using MyReliableSite.Application.Wrapper;
using MyReliableSite.Shared.DTOs.ArticleFeedbacks;

namespace MyReliableSite.Application.ArticleFeedbacks.Interfaces;

public interface IArticleFeedbackService : ITransientService
{
    Task<PaginatedResult<ArticleFeedbackDto>> SearchAsync(ArticleFeedbackListFilter filter);

    Task<Result<Guid>> CreateArticleFeedbackAsync(CreateArticleFeedbackRequest request);

    Task<Result<Guid>> UpdateArticleFeedbackAsync(UpdateArticleFeedbackRequest request, Guid id);

    Task<Result<Guid>> DeleteArticleFeedbackAsync(Guid id);
    Task<Result<ArticleFeedbackDto>> GetArticleFeedbackAsync(Guid id);
    Task<Result<List<ArticleFeedbackDto>>> GetArticleFeedbackAgainstArticleAsync(string ArtilceId);
    Task<Result<int>> PendingFeedbackCount();
    Task<Result<List<ArticleFeedbackDto>>> GetArticleFeedbackAgainstArticleClientAsync(string ArtilceId);
}
