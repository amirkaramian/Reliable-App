using FluentValidation;
using MyReliableSite.Application.Common.Validators;
using MyReliableSite.Shared.DTOs.ArticleFeedbacks;

namespace MyReliableSite.Application.ArticleFeedbacks.Validators;

public class UpdateArticleFeedbackRequestValidator : CustomValidator<UpdateArticleFeedbackRequest>
{
    public UpdateArticleFeedbackRequestValidator()
    {
        RuleFor(p => p.Description).NotNull().NotEmpty();
        RuleFor(p => p.ArticleFeedbackRelatedTo).IsInEnum();
        RuleFor(p => p.ArticleFeedbackPriority).IsInEnum();
        RuleFor(p => p.ArticleFeedbackStatus).IsInEnum();
        RuleFor(p => p.ArticleFeedbackRelatedToId).NotNull().NotEmpty();
    }
}
