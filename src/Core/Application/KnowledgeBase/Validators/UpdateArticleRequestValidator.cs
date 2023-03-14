using FluentValidation;
using MyReliableSite.Application.Common.Validators;
using MyReliableSite.Application.Storage;
using MyReliableSite.Shared.DTOs.KnowledgeBase;

namespace MyReliableSite.Application.KnowledgeBase.Validators;

public class UpdateArticleRequestValidator : CustomValidator<UpdateArticleRequest>
{
    public UpdateArticleRequestValidator()
    {
        RuleFor(p => p.Title).MaximumLength(75).NotEmpty();
        RuleFor(p => p.Visibility).Must(x => x == true || x == false);
        RuleFor(p => p.Image).SetValidator(new FileUploadRequestValidator());
        RuleFor(p => p.BodyText).NotEmpty().NotNull();
        RuleFor(p => p.Categories).NotEmpty().NotNull();
    }
}