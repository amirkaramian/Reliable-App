using FluentValidation;
using MyReliableSite.Application.Common.Validators;
using MyReliableSite.Application.Storage;
using MyReliableSite.Shared.DTOs.Orders;

namespace MyReliableSite.Application.Orders.Validators;
public class CreateOrderTemplateRequestValidator : CustomValidator<CreateOrderTemplateRequest>
{
    public CreateOrderTemplateRequestValidator()
    {
        RuleFor(p => p.ProductName).MaximumLength(75).NotEmpty();
        RuleFor(p => p.ProductDescription).NotEmpty();
        RuleFor(p => p.IsActive).Must(x => x == true || x == false);
        RuleFor(p => p.Name).MaximumLength(75).NotEmpty();
        RuleFor(p => p.Description).NotEmpty();
        RuleFor(p => p.Thumbnail).SetValidator(new FileUploadRequestValidator());
        RuleFor(p => p.OrderTemplateLineItems).NotEmpty().NotNull();
        RuleFor(p => p.PaymentType).IsInEnum();
    }
}
