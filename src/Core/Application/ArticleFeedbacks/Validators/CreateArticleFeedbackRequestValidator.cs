using FluentValidation;
using MyReliableSite.Application.Common.Validators;
using MyReliableSite.Domain.ArticleFeedbacks;
using MyReliableSite.Shared.DTOs.ArticleFeedbacks;

namespace MyReliableSite.Application.Departments.Validators;

public class CreateArticleFeedbackRequestValidator : CustomValidator<CreateArticleFeedbackRequest>
{
    public CreateArticleFeedbackRequestValidator()
    {
        RuleFor(p => p.Description).NotNull().NotEmpty();
        RuleFor(p => p.ArticleFeedbackRelatedTo).IsInEnum();
        RuleFor(p => p.ArticleFeedbackPriority).IsInEnum();
        RuleFor(p => p.ArticleFeedbackStatus).IsInEnum();

        RuleFor(p => p.ArticleFeedbackRelatedToId).NotNull().NotEmpty();
    }
}
