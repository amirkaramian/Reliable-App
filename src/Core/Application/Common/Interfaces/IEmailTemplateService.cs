using MyReliableSite.Application.Wrapper;
using MyReliableSite.Domain.Products;
using MyReliableSite.Shared.DTOs.EmailTemplates;
using MyReliableSite.Shared.DTOs.Filters;
using MyReliableSite.Shared.DTOs.Identity;

namespace MyReliableSite.Application.Common.Interfaces;

public interface IEmailTemplateService : ITransientService
{
    Task<EmailTemplateDto> GenerateEmailConfirmationMail(UserDetailsDto user, string emailVerificationUri);
    Task<EmailTemplateDto> GenerateEmailForgetPassword(UserDetailsDto user, string token, string passwordResetUrl);

    Task<EmailTemplateDto> GenerateEmailOTP(UserDetailsDto user, string OTP);

    Task<Result<EmailTemplateDto>> GetTemplateByTypeAsync(EmailTemplateType emailTemplateType);

    Task<Result<EmailTemplateDto>> GetAsync(Guid id);

    Task<PaginatedResult<EmailTemplateDto>> SearchAsync(EmailTemplateListFilter filter);

    Task<Result<EmailTemplateDto>> CreateAsync(CreateEmailTemplateRequest request);

    Task<Result<EmailTemplateDto>> UpdateAsync(Guid id, UpdateEmailTemplateRequest request);

    Task<Result<bool>> DeleteAsync(Guid id);
    Task<EmailTemplateDto> GenerateEmailBodyForProductStatusUpdate(Product product, UserDetailsDto user);

    Result<Dictionary<EmailTemplateType, string>> GetTemplateVariables();
    Task<EmailTemplateDto> GenerateEmailBodyForProductCancellation(Product product, UserDetailsDto user, string token);
}