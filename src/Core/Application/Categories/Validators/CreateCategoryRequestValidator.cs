using FluentValidation;
using MyReliableSite.Application.Common.Validators;
using MyReliableSite.Shared.DTOs.Categories;

namespace MyReliableSite.Application.Categories.Validators;

public class CreateCategoryRequestValidator : CustomValidator<CreateCategoryRequest>
{
    public CreateCategoryRequestValidator()
    {
        RuleFor(p => p.Name).MaximumLength(75).NotEmpty();
        RuleFor(p => p.CategoryType).IsInEnum();
    }
}
