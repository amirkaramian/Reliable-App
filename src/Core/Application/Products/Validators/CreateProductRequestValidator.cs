using FluentValidation;
using MyReliableSite.Application.Common.Validators;
using MyReliableSite.Application.Storage;
using MyReliableSite.Shared.DTOs.Products;

namespace MyReliableSite.Application.Products.Validators;

public class CreateProductRequestValidator : CustomValidator<CreateProductRequest>
{
    public CreateProductRequestValidator()
    {
        RuleFor(p => p.Name).MaximumLength(75).NotEmpty();
        RuleFor(p => p.Description).NotEmpty();
        RuleFor(p => p.Thumbnail).SetValidator(new FileUploadRequestValidator());
        RuleFor(p => p.ProductLineItems).NotEmpty().NotNull();
        RuleFor(p => p.Status).IsInEnum();
        RuleFor(p => p.PaymentType).IsInEnum();
    }
}
