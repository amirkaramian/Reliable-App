using MyReliableSite.Application.Common.Interfaces;
using MyReliableSite.Application.Wrapper;
using MyReliableSite.Shared.DTOs.ArticleFeedbacks;

namespace MyReliableSite.Application.ArticleFeedbacks.Interfaces;

public interface IArticleFeedbackCommentReplyService : ITransientService
{
    Task<Result<Guid>> CreateArticleFeedbackCommentReplyAsync(CreateArticleFeedbackCommentReplyRequest request);

    Task<Result<Guid>> UpdateArticleFeedbackCommentReplyAsync(UpdateArticleFeedbackCommentReplyRequest request, Guid id);

    Task<Result<Guid>> DeleteArticleFeedbackCommentReplyAsync(Guid id);
    Task<Result<ArticleFeedbackCommentReplyDto>> GetArticleFeedbackCommentReplyAsync(Guid id);
}
