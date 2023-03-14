using Microsoft.Extensions.Localization;
using MyReliableSite.Application.ArticleFeedbacks.Interfaces;
using MyReliableSite.Application.Common.Interfaces;
using MyReliableSite.Application.Exceptions;
using MyReliableSite.Application.Wrapper;
using MyReliableSite.Domain.ArticleFeedbacks;
using MyReliableSite.Shared.DTOs.ArticleFeedbacks;
using MyReliableSite.Application.Specifications;
using MyReliableSite.Application.Abstractions.Services.Identity;
using MyReliableSite.Domain.ArticleFeedbacks.Events;
using MyReliableSite.Shared.DTOs.Notifications.Enums;
using MyReliableSite.Shared.DTOs.Notifications;

namespace MyReliableSite.Application.ArticleFeedbacks.Services;

public class ArticleFeedbackCommentReplyService : IArticleFeedbackCommentReplyService
{
    private readonly IStringLocalizer<ArticleFeedbackCommentReplyService> _localizer;
    private readonly IRepositoryAsync _repository;

    private readonly ICurrentUser _user;
    private readonly INotificationService _notificationService;

    public ArticleFeedbackCommentReplyService()
    {
    }

    public ArticleFeedbackCommentReplyService(IRepositoryAsync repository, ICurrentUser user, IStringLocalizer<ArticleFeedbackCommentReplyService> localizer, INotificationService notificationService)
    {
        _repository = repository;
        _user = user;
        _localizer = localizer;
        _notificationService = notificationService;
    }

    public async Task<Result<Guid>> CreateArticleFeedbackCommentReplyAsync(CreateArticleFeedbackCommentReplyRequest request)
    {
        var spec = new BaseSpecification<ArticleFeedbackComment>();
        spec.Includes.Add(a => a.ArticleFeedback);
        var articleFeedbackComment = await _repository.GetByIdAsync<ArticleFeedbackComment>(request.ArticleFeedbackCommentId, spec);
        if (articleFeedbackComment == null) throw new EntityNotFoundException(string.Format(_localizer["ArticleFeedbackComment.notfound"], request.ArticleFeedbackCommentId));
        if (request.ArticleFeedbackCommentParentReplyId != null)
        {
            var articleFeedbackCommentReplyCheck = await _repository.GetByIdAsync<ArticleFeedbackCommentReply>(request.ArticleFeedbackCommentParentReplyId.Value);
            if (articleFeedbackCommentReplyCheck == null) throw new EntityNotFoundException(string.Format(_localizer["ArticleFeedbackCommentReply.notfound"], request.ArticleFeedbackCommentParentReplyId));
        }

        string userId = _user.GetUserId().ToString();
        var articleFeedbackCommentReply = new ArticleFeedbackCommentReply(request.CommentText, userId, request.ArticleFeedbackCommentParentReplyId);
        articleFeedbackCommentReply.ArticleFeedbackComment = articleFeedbackComment;
        articleFeedbackCommentReply.DomainEvents.Add(new ArticleFeedbackCommentReplyCreatedEvent(articleFeedbackCommentReply));
        var articleFeedbackId = await _repository.CreateAsync<ArticleFeedbackCommentReply>((ArticleFeedbackCommentReply)articleFeedbackCommentReply);

        await _repository.SaveChangesAsync();
        var articleFeedback = await _repository.GetByIdAsync<ArticleFeedback>(articleFeedbackComment.ArticleFeedback.Id);
        await _repository.ClearCacheAsync<ArticleFeedback>(articleFeedback);

        string message = $"Hello [[fullName]], a new comment for article feedback is added.";
        await _notificationService.SendMessageToUserAsync(articleFeedback.AssignedTo, new BasicNotification() { Message = message, Label = BasicNotification.LabelType.Information, NotificationType = NotificationType.ARTICLE_FEEDBACK_COMMENT_ADDED, TargetUserTypes = NotificationTargetUserTypes.Admins, NotifyModelId = articleFeedback.Id, Data = new { ArticleFeedbackId = articleFeedback.Id, ArticleFeedbackCommentReplyId = articleFeedbackId, ArticleFeedbackCommentId = request.ArticleFeedbackCommentId, ArticleFeedbackCommentParentReplyId = request.ArticleFeedbackCommentParentReplyId } });

        return await Result<Guid>.SuccessAsync(articleFeedbackId);
    }

    public async Task<Result<Guid>> DeleteArticleFeedbackCommentReplyAsync(Guid id)
    {
        var articleFeedbackCommentReply = await _repository.GetByIdAsync<ArticleFeedbackCommentReply>(id);
        if (articleFeedbackCommentReply == null) throw new EntityNotFoundException(string.Format(_localizer["ArticleFeedbackCommentReply.notfound"], id));
        var articleFeedbackToDelete = await _repository.RemoveByIdAsync<ArticleFeedbackCommentReply>(id);
        articleFeedbackToDelete.DomainEvents.Add(new ArticleFeedbackCommentReplyDeletedEvent(articleFeedbackToDelete));

        await _repository.SaveChangesAsync();
        var articleFeedback = await _repository.GetByIdAsync<ArticleFeedback>(articleFeedbackCommentReply.ArticleFeedbackComment.ArticleFeedback.Id);
        await _repository.ClearCacheAsync<ArticleFeedback>(articleFeedback);

        // user assignment to default articleFeedback
        return await Result<Guid>.SuccessAsync(id);
    }

    public async Task<Result<Guid>> UpdateArticleFeedbackCommentReplyAsync(UpdateArticleFeedbackCommentReplyRequest request, Guid id)
    {
        var articleFeedbackCommentReply = await _repository.GetByIdAsync<ArticleFeedbackCommentReply>(id);
        if (articleFeedbackCommentReply == null) throw new EntityNotFoundException(string.Format(_localizer["ArticleFeedbackCommentReply.notfound"], id));
        var updatedArticleFeedbackCommentReply = articleFeedbackCommentReply.Update(request.CommentText, request.ArticleFeedbackCommentParentReplyId);
        updatedArticleFeedbackCommentReply.DomainEvents.Add(new ArticleFeedbackCommentReplyUpdatedEvent(updatedArticleFeedbackCommentReply));
        await _repository.UpdateAsync<ArticleFeedbackCommentReply>(updatedArticleFeedbackCommentReply);

        await _repository.SaveChangesAsync();
        var articleFeedback = await _repository.GetByIdAsync<ArticleFeedback>(articleFeedbackCommentReply.ArticleFeedbackComment.ArticleFeedback.Id);
        await _repository.ClearCacheAsync<ArticleFeedback>(articleFeedback);

        return await Result<Guid>.SuccessAsync(id);
    }

    public async Task<Result<ArticleFeedbackCommentReplyDto>> GetArticleFeedbackCommentReplyAsync(Guid id)
    {
        var spec = new BaseSpecification<ArticleFeedbackCommentReply>();
        spec.Includes.Add(a => a.ArticleFeedbackCommentChildReplies);
        var articleFeedback = await _repository.GetByIdAsync<ArticleFeedbackCommentReply, ArticleFeedbackCommentReplyDto>(id, spec);
        return await Result<ArticleFeedbackCommentReplyDto>.SuccessAsync(articleFeedback);
    }
}
