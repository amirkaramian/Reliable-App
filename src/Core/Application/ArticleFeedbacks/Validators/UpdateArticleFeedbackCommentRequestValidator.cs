using FluentValidation;
using MyReliableSite.Application.Common.Validators;
using MyReliableSite.Shared.DTOs.ArticleFeedbacks;

namespace MyReliableSite.Application.ArticleFeedbacks.Validators;

public class UpdateArticleFeedbackCommentRequestValidator : CustomValidator<UpdateArticleFeedbackCommentRequest>
{
    public UpdateArticleFeedbackCommentRequestValidator()
    {
        RuleFor(p => p.CommentText).NotNull().NotEmpty();
    }
}
