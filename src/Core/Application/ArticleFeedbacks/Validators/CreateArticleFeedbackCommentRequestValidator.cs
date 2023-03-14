using FluentValidation;
using MyReliableSite.Application.Common.Validators;
using MyReliableSite.Shared.DTOs.ArticleFeedbacks;

namespace MyReliableSite.Application.Departments.Validators;

public class CreateArticleFeedbackCommentRequestValidator : CustomValidator<CreateArticleFeedbackCommentRequest>
{
    public CreateArticleFeedbackCommentRequestValidator()
    {
        RuleFor(p => p.ArticleFeedbackId).NotNull().NotEmpty();
        RuleFor(p => p.CommentText).NotNull().NotEmpty();
    }
}
