using FluentValidation;
using MyReliableSite.Application.Common.Validators;
using MyReliableSite.Shared.DTOs.Orders;

namespace MyReliableSite.Application.Orders.Validators;
public class CreateOrderRequestValidator : CustomValidator<CreateOrderRequest>
{
    public CreateOrderRequestValidator()
    {
        RuleFor(p => p.Products).NotEmpty().NotNull();
        RuleFor(p => p.Tenant).NotEmpty().NotNull();
        RuleFor(p => p.OrderForClientId).NotEmpty().NotNull();

    }
}
