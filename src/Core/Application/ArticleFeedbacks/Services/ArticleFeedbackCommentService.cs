using Microsoft.Extensions.Localization;
using MyReliableSite.Application.ArticleFeedbacks.Interfaces;
using MyReliableSite.Application.Common.Interfaces;
using MyReliableSite.Application.Exceptions;
using MyReliableSite.Application.Wrapper;
using MyReliableSite.Domain.ArticleFeedbacks;
using MyReliableSite.Domain.Dashboard;
using MyReliableSite.Shared.DTOs.ArticleFeedbacks;
using MyReliableSite.Application.Specifications;
using Mapster;
using MyReliableSite.Application.Abstractions.Services.Identity;
using MyReliableSite.Application.Storage;
using MyReliableSite.Domain.ArticleFeedbacks.Events;
using MyReliableSite.Shared.DTOs.Notifications;
using MyReliableSite.Shared.DTOs.Notifications.Enums;

namespace MyReliableSite.Application.ArticleFeedbacks.Services;

public class ArticleFeedbackCommentService : IArticleFeedbackCommentService
{
    private readonly IStringLocalizer<ArticleFeedbackCommentService> _localizer;
    private readonly IRepositoryAsync _repository;
    private readonly IUserService _userService;
    private readonly IFileStorageService _fileStorageService;
    private readonly ICurrentUser _user;
    private readonly INotificationService _notificationService;

    public ArticleFeedbackCommentService()
    {
    }

    public ArticleFeedbackCommentService(IRepositoryAsync repository, IUserService userService, IFileStorageService fileStorageService, ICurrentUser user, IStringLocalizer<ArticleFeedbackCommentService> localizer, INotificationService notificationService)
    {
        _repository = repository;
        _userService = userService;
        _fileStorageService = fileStorageService;
        _user = user;
        _localizer = localizer;
        _notificationService = notificationService;
    }

    public async Task<Result<Guid>> CreateArticleFeedbackCommentAsync(CreateArticleFeedbackCommentRequest request)
    {
        var articleFeedback = await _repository.GetByIdAsync<ArticleFeedback>(request.ArticleFeedbackId);
        if (articleFeedback == null) throw new EntityNotFoundException(string.Format(_localizer["ArticleFeedback.notfound"], request.ArticleFeedbackId));
        string userId = _user.GetUserId().ToString();
        var articleFeedbackComment = new ArticleFeedbackComment(request.CommentText, userId);
        articleFeedbackComment.ArticleFeedback = articleFeedback;
        articleFeedbackComment.DomainEvents.Add(new ArticleFeedbackCommentCreatedEvent(articleFeedbackComment));
        var articleFeedbackCommentId = await _repository.CreateAsync<ArticleFeedbackComment>((ArticleFeedbackComment)articleFeedbackComment);
        await _repository.SaveChangesAsync();
        await _repository.ClearCacheAsync<ArticleFeedback>(articleFeedback);

        string message = $"Hello [[fullName]], a new comment for article feedback is added.";
        await _notificationService.SendMessageToUserAsync(articleFeedback.AssignedTo, new BasicNotification() { Message = message, Label = BasicNotification.LabelType.Information, NotificationType = NotificationType.ARTICLE_FEEDBACK_COMMENT_ADDED, TargetUserTypes = NotificationTargetUserTypes.Admins, NotifyModelId = request.ArticleFeedbackId, Data = new { ArticleFeedbackId = request.ArticleFeedbackId, ArticleFeedbackCommentId = articleFeedbackCommentId } });

        return await Result<Guid>.SuccessAsync(articleFeedbackCommentId);
    }

    public async Task<Result<Guid>> DeleteArticleFeedbackCommentAsync(Guid id)
    {
        var articleFeedbackComment = await _repository.GetByIdAsync<ArticleFeedbackComment>(id);
        if (articleFeedbackComment == null) throw new EntityNotFoundException(string.Format(_localizer["ArticleFeedbackComment.notfound"], id));
        var articleFeedbackToDelete = await _repository.RemoveByIdAsync<ArticleFeedbackComment>(id);
        articleFeedbackToDelete.DomainEvents.Add(new ArticleFeedbackCommentDeletedEvent(articleFeedbackToDelete));

        await _repository.SaveChangesAsync();

        var articleFeedback = await _repository.GetByIdAsync<ArticleFeedback>(articleFeedbackComment.ArticleFeedback.Id);
        await _repository.ClearCacheAsync<ArticleFeedback>(articleFeedback);

        // user assignment to default articleFeedbackComment
        return await Result<Guid>.SuccessAsync(id);
    }

    public async Task<PaginatedResult<ArticleFeedbackCommentDto>> SearchAsync(ArticleFeedbackCommentListFilter filter)
    {
        return await _repository.GetSearchResultsAsync<ArticleFeedbackComment, ArticleFeedbackCommentDto>(filter.PageNumber, filter.PageSize, filter.OrderBy, filter.OrderType, filter.AdvancedSearch, filter.Keyword);
    }

    public async Task<Result<Guid>> UpdateArticleFeedbackCommentAsync(UpdateArticleFeedbackCommentRequest request, Guid id)
    {
        var articleFeedbackComment = await _repository.GetByIdAsync<ArticleFeedbackComment>(id);
        if (articleFeedbackComment == null) throw new EntityNotFoundException(string.Format(_localizer["ArticleFeedback.notfound"], id));
        var updatedArticleFeedback = articleFeedbackComment.Update(request.CommentText);
        updatedArticleFeedback.DomainEvents.Add(new ArticleFeedbackCommentUpdatedEvent(updatedArticleFeedback));
        await _repository.UpdateAsync<ArticleFeedbackComment>(updatedArticleFeedback);

        await _repository.SaveChangesAsync();
        var articleFeedback = await _repository.GetByIdAsync<ArticleFeedback>(articleFeedbackComment.ArticleFeedback.Id);
        await _repository.ClearCacheAsync<ArticleFeedback>(articleFeedback);

        return await Result<Guid>.SuccessAsync(id);
    }

    public async Task<Result<ArticleFeedbackCommentDto>> GetArticleFeedbackCommentAsync(Guid id)
    {
        var spec = new BaseSpecification<ArticleFeedbackComment>();
        spec.Includes.Add(a => a.ArticleFeedbackCommentReplies);
        var articleFeedbackComment = await _repository.GetByIdAsync<ArticleFeedbackComment, ArticleFeedbackCommentDto>(id, spec);

        var userIds = articleFeedbackComment.ArticleFeedbackCommentReplies.Select(c => c.UserId).ToList();

        var userDetails = await _userService.GetAllAsync(userIds);

        var assignedToUserObj = userDetails.Data.FirstOrDefault(m => m.Id == Guid.Parse(articleFeedbackComment.UserId));
        if (assignedToUserObj != null)
        {
            articleFeedbackComment.UserFullName = assignedToUserObj.FullName;
            if (!string.IsNullOrEmpty(assignedToUserObj.ImageUrl))
            {
                articleFeedbackComment.UserImagePath = await _fileStorageService.ReturnBase64StringOfImageFileAsync(assignedToUserObj.ImageUrl);
            }
        }

        foreach (var itemReply in articleFeedbackComment.ArticleFeedbackCommentReplies)
        {
            var itemReplyObj = userDetails.Data.FirstOrDefault(m => m.Id == Guid.Parse(itemReply.UserId));
            if (itemReplyObj != null)
            {
                itemReply.UserFullName = itemReplyObj.FullName;
                if (!string.IsNullOrEmpty(itemReplyObj.ImageUrl))
                {
                    itemReply.UserImagePath = await _fileStorageService.ReturnBase64StringOfImageFileAsync(itemReplyObj.ImageUrl);
                }
            }
        }

        return await Result<ArticleFeedbackCommentDto>.SuccessAsync(articleFeedbackComment);
    }

    public async Task<PaginatedResult<ArticleFeedbackCommentDto>> SearchByArticleAsync(ArticleFeedbackCommentListFilter filter, Guid articleFeedbackId)
    {
        return await _repository.GetSearchResultsAsync<ArticleFeedbackComment, ArticleFeedbackCommentDto>(filter.PageNumber, filter.PageSize, filter.OrderBy, filter.OrderType, filter.AdvancedSearch, filter.Keyword, x => x.ArticleFeedback.Id == articleFeedbackId);
    }
}
