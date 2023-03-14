using FluentValidation;

using MyReliableSite.Application.Common.Validators;
using MyReliableSite.Application.Storage;
using MyReliableSite.Shared.DTOs.Brands;

namespace MyReliableSite.Application.Brands.Validators;

public class UpdateBrandRequestValidator : CustomValidator<UpdateBrandRequest>
{
    public UpdateBrandRequestValidator()
    {
        RuleFor(p => p.CompanyName).MaximumLength(75).NotEmpty();
        RuleFor(p => p.Status).Must(x => x == false || x == true);
        RuleFor(p => p.ClientAssigned).NotEmpty().NotNull();
        RuleFor(p => p.Name).MaximumLength(75).NotEmpty();
        RuleFor(p => p.Image).SetValidator(new FileUploadRequestValidator());
    }
}