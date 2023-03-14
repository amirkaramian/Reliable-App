using FluentValidation;
using MyReliableSite.Application.Common.Validators;
using MyReliableSite.Shared.DTOs.Categories;

namespace MyReliableSite.Application.Categories.Validators;

public class UpdateCategoryRequestValidator : CustomValidator<UpdateCategoryRequest>
{
    public UpdateCategoryRequestValidator()
    {
        RuleFor(p => p.Name).MaximumLength(75).NotEmpty();
    }
}