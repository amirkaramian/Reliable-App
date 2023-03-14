using FluentValidation;
using MyReliableSite.Application.Common.Validators;
using MyReliableSite.Shared.DTOs.Maintenance;

namespace MyReliableSite.Application.Maintenance;

public class MaintenanceRequestValidator : CustomValidator<MaintenanceRequest>
{
    public MaintenanceRequestValidator()
    {
        RuleFor(p => p.Message).NotNull().NotEmpty();
        RuleFor(p => p.ExpirationDateTime).NotNull().NotEmpty();
    }
}
