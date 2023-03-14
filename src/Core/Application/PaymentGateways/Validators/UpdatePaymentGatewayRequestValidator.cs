﻿using FluentValidation;

using MyReliableSite.Application.Common.Validators;
using MyReliableSite.Application.Storage;
using MyReliableSite.Shared.DTOs.PaymentGateways;

namespace MyReliableSite.Application.PaymentGateways.Validators;

public class UpdatePaymentGatewayRequestValidator : CustomValidator<UpdatePaymentGatewayRequest>
{
    public UpdatePaymentGatewayRequestValidator()
    {
        RuleFor(p => p.Name).MaximumLength(75).NotEmpty().NotNull();
        RuleFor(p => p.Status).Must(x => x == false || x == true);
        RuleFor(p => p.ApiKey).NotEmpty().NotNull().NotNull();
    }
}