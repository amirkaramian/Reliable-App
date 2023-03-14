using FluentValidation;
using MyReliableSite.Application.Common.Validators;
using MyReliableSite.Application.Storage;
using MyReliableSite.Shared.DTOs.Orders;

namespace MyReliableSite.Application.Orders.Validators;
public class UpdateOrderRequestValidator : CustomValidator<UpdateOrderRequest>
{
    public UpdateOrderRequestValidator()
    {
        RuleFor(p => p.Status).IsInEnum();
    }
}
