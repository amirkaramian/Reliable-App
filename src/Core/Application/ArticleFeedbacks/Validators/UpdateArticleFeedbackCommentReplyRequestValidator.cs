using FluentValidation;
using MyReliableSite.Application.Common.Validators;
using MyReliableSite.Shared.DTOs.ArticleFeedbacks;

namespace MyReliableSite.Application.ArticleFeedbacks.Validators;

public class UpdateArticleFeedbackCommentReplyRequestValidator : CustomValidator<UpdateArticleFeedbackCommentReplyRequest>
{
    public UpdateArticleFeedbackCommentReplyRequestValidator()
    {
        RuleFor(p => p.CommentText).NotNull().NotEmpty();
    }
}
