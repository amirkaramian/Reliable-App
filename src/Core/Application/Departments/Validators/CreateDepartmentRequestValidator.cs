using FluentValidation;
using MyReliableSite.Application.Common.Validators;
using MyReliableSite.Shared.DTOs.Departments;

namespace MyReliableSite.Application.Departments.Validators;

public class CreateDepartmentRequestValidator : CustomValidator<CreateDepartmentRequest>
{
    public CreateDepartmentRequestValidator()
    {
        RuleFor(p => p.BrandId).NotNull().NotEmpty();
        RuleFor(p => p.DeptNumber).NotEmpty();
        RuleFor(p => p.Name).NotEmpty();
        RuleFor(p => p.DeptStatus).Must(x => x == true || x == false);
        RuleFor(p => p.IsDefault).Must(x => x == true || x == false);
    }
}

public class AssignDepartmentRequestValidator : CustomValidator<AssignDepartmentRequest>
{
    public AssignDepartmentRequestValidator()
    {
        RuleFor(p => p.DepartmentId).NotEmpty().NotNull();
        RuleFor(p => p.UserId).NotEmpty().NotNull();
    }
}