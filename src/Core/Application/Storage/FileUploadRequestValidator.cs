using FluentValidation;
using MyReliableSite.Application.Common.Validators;
using MyReliableSite.Shared.DTOs.Storage;

namespace MyReliableSite.Application.Storage;

public class FileUploadRequestValidator : CustomValidator<FileUploadRequest>
{
    public FileUploadRequestValidator()
    {
        RuleFor(p => p.Name).MaximumLength(75).NotEmpty().WithMessage("Image Name cannot be empty!");
        RuleFor(p => p.Extension).MaximumLength(5).NotEmpty().WithMessage("Image Extension cannot be empty!");
        RuleFor(p => p.Data).NotEmpty().WithMessage("Image Data cannot be empty!");
    }
}