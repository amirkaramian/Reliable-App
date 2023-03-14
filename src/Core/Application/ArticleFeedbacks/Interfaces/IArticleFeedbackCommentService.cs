using MyReliableSite.Application.Common.Interfaces;
using MyReliableSite.Application.Wrapper;
using MyReliableSite.Shared.DTOs.ArticleFeedbacks;

namespace MyReliableSite.Application.ArticleFeedbacks.Interfaces;

public interface IArticleFeedbackCommentService : ITransientService
{
    Task<PaginatedResult<ArticleFeedbackCommentDto>> SearchAsync(ArticleFeedbackCommentListFilter filter);

    Task<Result<Guid>> CreateArticleFeedbackCommentAsync(CreateArticleFeedbackCommentRequest request);

    Task<Result<Guid>> UpdateArticleFeedbackCommentAsync(UpdateArticleFeedbackCommentRequest request, Guid id);

    Task<Result<Guid>> DeleteArticleFeedbackCommentAsync(Guid id);
    Task<Result<ArticleFeedbackCommentDto>> GetArticleFeedbackCommentAsync(Guid id);
    Task<PaginatedResult<ArticleFeedbackCommentDto>> SearchByArticleAsync(ArticleFeedbackCommentListFilter filter, Guid articleFeedbackId);
}
