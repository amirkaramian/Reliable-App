using FluentValidation;
using MyReliableSite.Application.Common.Validators;
using MyReliableSite.Shared.DTOs.ArticleFeedbacks;

namespace MyReliableSite.Application.Departments.Validators;

public class CreateArticleFeedbackCommentReplyRequestValidator : CustomValidator<CreateArticleFeedbackCommentReplyRequest>
{
    public CreateArticleFeedbackCommentReplyRequestValidator()
    {
        RuleFor(p => p.CommentText).NotNull().NotEmpty();
        RuleFor(p => p.ArticleFeedbackCommentId).NotNull().NotEmpty();
    }
}
