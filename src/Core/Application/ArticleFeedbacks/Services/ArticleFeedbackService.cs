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
using MyReliableSite.Shared.DTOs.KnowledgeBase;
using MyReliableSite.Domain.KnowledgeBase;

namespace MyReliableSite.Application.ArticleFeedbacks.Services;

public class ArticleFeedbackService : IArticleFeedbackService
{
    private readonly IStringLocalizer<ArticleFeedbackService> _localizer;
    private readonly IRepositoryAsync _repository;
    private readonly IUserService _userService;
    private readonly IFileStorageService _fileStorageService;
    private readonly INotificationService _notificationService;
    private readonly ICurrentUser _currentUser;

    public ArticleFeedbackService()
    {
    }

    public ArticleFeedbackService(IRepositoryAsync repository, IUserService userService, IFileStorageService fileStorageService, IStringLocalizer<ArticleFeedbackService> localizer, INotificationService notificationService, ICurrentUser currentUser)
    {
        _repository = repository;
        _userService = userService;
        _fileStorageService = fileStorageService;
        _localizer = localizer;
        _notificationService = notificationService;
        _currentUser = currentUser;
    }

    public async Task<Result<Guid>> CreateArticleFeedbackAsync(CreateArticleFeedbackRequest request)
    {
        var user = await _userService.GetAsync(_currentUser.GetUserId().ToString());
        string parentId = user.Data.ParentID;
        if(string.IsNullOrEmpty(parentId)) parentId = Guid.Empty.ToString();
        request.AssignedTo = parentId;
        var articleFeedback = new ArticleFeedback(request.Description, request.AssignedTo, request.ArticleFeedbackPriority, (ArticleFeedbackRelatedTo)request.ArticleFeedbackRelatedTo, request.ArticleFeedbackRelatedToId, (Domain.ArticleFeedbacks.ArticleFeedbackStatus)request.ArticleFeedbackStatus);
        articleFeedback.DomainEvents.Add(new ArticleFeedbackCreatedEvent(articleFeedback));
        var articleFeedbackId = await _repository.CreateAsync<ArticleFeedback>((ArticleFeedback)articleFeedback);
        await _repository.SaveChangesAsync();

        string message = $"Hello [[fullName]], new article feedback is added.";
        await _notificationService.SendMessageToUserAsync(articleFeedback.AssignedTo, new BasicNotification() { Message = message, Label = BasicNotification.LabelType.Information, NotificationType = NotificationType.ARTICLE_FEEDBACK_ADDED, TargetUserTypes = NotificationTargetUserTypes.Admins, NotifyModelId = articleFeedbackId, Data = new { ArticleFeedbackId = articleFeedbackId } });
        var userId = _currentUser.GetUserId();
        await _notificationService.SendMessageToUserAsync(userId.ToString(), new BasicNotification() { Message = message, });
        return await Result<Guid>.SuccessAsync(articleFeedbackId);
    }

    public async Task<Result<Guid>> DeleteArticleFeedbackAsync(Guid id)
    {
        var articleFeedback = await _repository.GetByIdAsync<ArticleFeedback>(id);
        if (articleFeedback == null) throw new EntityNotFoundException(string.Format(_localizer["ArticleFeedback.notfound"], id));
        var articleFeedbackToDelete = await _repository.RemoveByIdAsync<ArticleFeedback>(id);
        articleFeedbackToDelete.DomainEvents.Add(new ArticleFeedbackDeletedEvent(articleFeedbackToDelete));

        await _repository.SaveChangesAsync();

        // user assignment to default articleFeedback
        return await Result<Guid>.SuccessAsync(id);
    }

    public async Task<PaginatedResult<ArticleFeedbackDto>> SearchAsync(ArticleFeedbackListFilter filter)
    {
        var feedbacks = await _repository.GetSearchResultsAsync<ArticleFeedback, ArticleFeedbackDto>(filter.PageNumber, filter.PageSize, filter.OrderBy, filter.OrderType, filter.AdvancedSearch, filter.Keyword, m => m.ArticleFeedbackRelatedTo == (ArticleFeedbackRelatedTo)filter.ArticleFeedbackRelatedTo);
        foreach (var articleFeedback in feedbacks.Data)
        {
            articleFeedback.Article = await _repository.GetByIdAsync<Article, ArticleDto>(Guid.Parse(articleFeedback.ArticleFeedbackRelatedToId));

        }

        return feedbacks;
    }

    public async Task<Result<Guid>> UpdateArticleFeedbackAsync(UpdateArticleFeedbackRequest request, Guid id)
    {
        var articleFeedback = await _repository.GetByIdAsync<ArticleFeedback>(id);
        if (articleFeedback == null) throw new EntityNotFoundException(string.Format(_localizer["ArticleFeedback.notfound"], id));

        var updatedArticleFeedback = articleFeedback.Update(request.Description, request.AssignedTo, request.ArticleFeedbackPriority, (ArticleFeedbackRelatedTo)request.ArticleFeedbackRelatedTo, request.ArticleFeedbackRelatedToId, (Domain.ArticleFeedbacks.ArticleFeedbackStatus)request.ArticleFeedbackStatus, request.IsReviewed);
        updatedArticleFeedback.DomainEvents.Add(new ArticleFeedbackUpdatedEvent(updatedArticleFeedback));
        await _repository.UpdateAsync<ArticleFeedback>(updatedArticleFeedback);
        await _repository.SaveChangesAsync();

        return await Result<Guid>.SuccessAsync(id);
    }

    public async Task<Result<ArticleFeedbackDto>> GetArticleFeedbackAsync(Guid id)
    {

        var articleFeedback = await _repository.GetByIdAsync<ArticleFeedback, ArticleFeedbackDto>(id);
        articleFeedback.Article = await _repository.GetByIdAsync<Article, ArticleDto>(Guid.Parse(articleFeedback.ArticleFeedbackRelatedToId));

        var articleFeedbackComments = await _repository.GetListAsync<ArticleFeedbackComment>(m => m.ArticleFeedback.Id == id);
        if (articleFeedbackComments != null && articleFeedbackComments.Count() > 0)
        {
            articleFeedback.ArticleFeedbackComments = articleFeedbackComments.Adapt<List<ArticleFeedbackCommentDto>>();

            articleFeedback.ArticleFeedbackComments.OrderByDescending(m => m.CreatedOn);

            var userIds = articleFeedback.ArticleFeedbackComments.Select(c => c.UserId).ToList();

            userIds.Add(articleFeedback.CreatedBy.ToString());
            userIds.Add(articleFeedback.AssignedTo);

            var userDetails = await _userService.GetAllAsync(userIds);

            var assignedToUserObj = userDetails.Data.FirstOrDefault(m => m.Id == Guid.Parse(articleFeedback.AssignedTo));
            if (assignedToUserObj != null)
            {
                articleFeedback.AssignedToFullName = assignedToUserObj.FullName;
            }

            var createdByObj = userDetails.Data.FirstOrDefault(m => m.Id == articleFeedback.CreatedBy);
            if (createdByObj != null)
            {
                articleFeedback.CreatedByName = createdByObj.FullName;
            }

            foreach (var itemComment in articleFeedback.ArticleFeedbackComments)
            {
                var commenterObj = userDetails.Data.FirstOrDefault(m => m.Id == Guid.Parse(itemComment.UserId));
                if (commenterObj != null)
                {
                    itemComment.UserFullName = commenterObj.FullName;
                    if (!string.IsNullOrEmpty(commenterObj.ImageUrl))
                    {
                        itemComment.UserImagePath = await _fileStorageService.ReturnBase64StringOfImageFileAsync(commenterObj.ImageUrl);
                    }
                }

                var articleFeedbackCommentReplies = await _repository.GetListAsync<ArticleFeedbackCommentReply>(m => m.ArticleFeedbackComment.Id == itemComment.Id);
                if (articleFeedbackCommentReplies != null && articleFeedbackCommentReplies.Count() > 0)
                {
                    itemComment.ArticleFeedbackCommentReplies = articleFeedbackCommentReplies.Adapt<List<ArticleFeedbackCommentReplyDto>>();

                    foreach (var itemReply in itemComment.ArticleFeedbackCommentReplies)
                    {
                        if (Guid.Parse(itemReply.UserId) != Guid.Empty)
                        {
                            var itemReplyObj = await _userService.GetUserProfileAsync(itemReply.UserId);
                            if (itemReplyObj != null)
                            {
                                itemReply.UserFullName = itemReplyObj.Data.FullName;
                                if (!string.IsNullOrEmpty(itemReplyObj.Data.ImageUrl))
                                {
                                    itemReply.UserImagePath = await _fileStorageService.ReturnBase64StringOfImageFileAsync(itemReplyObj.Data.ImageUrl);
                                }
                            }
                        }
                    }

                    itemComment.ArticleFeedbackCommentReplies.OrderByDescending(m => m.CreatedOn);
                }
            }

        }

        return await Result<ArticleFeedbackDto>.SuccessAsync(articleFeedback);
    }

    public async Task<Result<List<ArticleFeedbackDto>>> GetArticleFeedbackAgainstArticleAsync(string ArtilceId)
    {
        var articleFeedbacks = await _repository.FindByConditionAsync<ArticleFeedback>(x => x.ArticleFeedbackRelatedToId == ArtilceId);
        var mappedArticles = articleFeedbacks.Adapt<List<ArticleFeedbackDto>>();
        var newList = new List<ArticleFeedbackDto>();
        foreach (var item in mappedArticles)
        {
            var newItem = await GetArticleFeedbackAsync(item.Id);
            newList.Add(newItem.Data);
        }

        return await Result<List<ArticleFeedbackDto>>.SuccessAsync(newList);
    }

    public async Task<Result<List<ArticleFeedbackDto>>> GetArticleFeedbackAgainstArticleClientAsync(string ArtilceId)
    {
        var articleFeedbacks = await _repository.FindByConditionAsync<ArticleFeedback>(x => x.ArticleFeedbackRelatedToId == ArtilceId && x.CreatedBy == _currentUser.GetUserId());
        var mappedArticles = articleFeedbacks.Adapt<List<ArticleFeedbackDto>>();
        var newList = new List<ArticleFeedbackDto>();
        foreach(var item in mappedArticles)
        {
            var newItem = await GetArticleFeedbackAsync(item.Id);
            newList.Add(newItem.Data);
        }

        return await Result<List<ArticleFeedbackDto>>.SuccessAsync(newList);
    }

    public async Task<Result<int>> PendingFeedbackCount()
    {
        int count = await _repository.GetCountAsync<ArticleFeedback>(x => !x.IsReviewed);
        return await Result<int>.SuccessAsync(count);
    }

}
