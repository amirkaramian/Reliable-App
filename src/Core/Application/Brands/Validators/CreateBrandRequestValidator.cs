using FluentValidation;
using MyReliableSite.Application.Common.Validators;
using MyReliableSite.Application.Storage;
using MyReliableSite.Shared.DTOs.Brands;

namespace MyReliableSite.Application.Brands.Validators;

public class CreateBrandRequestValidator : CustomValidator<CreateBrandRequest>
{
    public CreateBrandRequestValidator()
    {
        RuleFor(p => p.CompanyName).MaximumLength(75).NotEmpty();
        RuleFor(p => p.Name).MaximumLength(75).NotEmpty();
        RuleFor(p => p.ClientAssigned).NotNull().NotEmpty();
        RuleFor(p => p.Image).SetValidator(new FileUploadRequestValidator());
        RuleFor(p => p.Status).Must(x => x == false || x == true);
    }
}