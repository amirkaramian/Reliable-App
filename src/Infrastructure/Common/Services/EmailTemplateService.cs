using Mapster;

using Microsoft.Extensions.Localization;
using MyReliableSite.Application.Abstractions.Services.Identity;
using MyReliableSite.Application.Common.Interfaces;
using MyReliableSite.Application.Exceptions;
using MyReliableSite.Application.Wrapper;
using MyReliableSite.Domain.Billing;
using MyReliableSite.Domain.Billing.Events.EmailTemplates;
using MyReliableSite.Domain.Common.Contracts;
using MyReliableSite.Domain.Dashboard;
using MyReliableSite.Domain.Products;
using MyReliableSite.Infrastructure.Identity.Models;
using MyReliableSite.Shared.DTOs.EmailTemplates;
using MyReliableSite.Shared.DTOs.Identity;
using System.Text;

namespace MyReliableSite.Infrastructure.Common.Services;

public class EmailTemplateService : IEmailTemplateService
{
    private readonly IRepositoryAsync _repository;
    private readonly IStringLocalizer<EmailTemplateService> _localizer;
    private readonly Dictionary<EmailTemplateType, string> _emailVariables = new Dictionary<EmailTemplateType, string>()
            {
                { EmailTemplateType.EmailConfirmation, "[fullName], [company], [address],[userName],[email],[emailVerificationUri]" },
                { EmailTemplateType.EmailOTP, "[fullName], [company], [address],[otpcode],[userName]" },
                { EmailTemplateType.General, "[userName],[email],[company],[address],[fullname]" },
                { EmailTemplateType.Invoice, "[fullName], [company], [address],[invoicelink]" },
                { EmailTemplateType.Orders, "[fullName], [company], [address],[orderlink]" },
                { EmailTemplateType.ProductCancellation, "[fullName], [company], [address],[productlink]" },
                { EmailTemplateType.ProductStatusUpdated, "[fullName], [company], [address],[productlink]" },
                { EmailTemplateType.ResetPassword, "[fullName], [company], [address],[userName],[resetPasswordUri]" },
                { EmailTemplateType.TicketAssignment, "[fullName], [company], [address],[ticketlink]" },
                { EmailTemplateType.TicketCreated, "[fullName], [company], [address],[ticketlink]" },
                { EmailTemplateType.TicketUpdated, "[fullName], [company], [address],[ticketlink]" }
            };

    public EmailTemplateService(IStringLocalizer<EmailTemplateService> localizer, IRepositoryAsync repository)
    {
        _localizer = localizer ?? throw new ArgumentNullException(nameof(localizer));
        _repository = repository ?? throw new ArgumentNullException(nameof(repository));
    }

    public Result<Dictionary<EmailTemplateType, string>> GetTemplateVariables()
    {
        return Result<Dictionary<EmailTemplateType, string>>.Success(_emailVariables);
    }

    public async Task<EmailTemplateDto> GenerateEmailConfirmationMail(UserDetailsDto user, string emailVerificationUri)
    {
        var confirmEmailTemplate = await _repository.FirstByConditionAsync<EmailTemplate>(m => m.EmailTemplateType == EmailTemplateType.EmailConfirmation && m.IsSystem);
        var emailDTo = confirmEmailTemplate.Adapt<EmailTemplateDto>();
        if (emailDTo == null)
        {
            await using var fs = new FileStream(Path.Combine(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Email Templates"), "email-confirmation.html"), FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
            using var sr = new StreamReader(fs, Encoding.Default);
            string mailText = await sr.ReadToEndAsync();
            sr.Close();

            emailDTo.Subject = "Confirm Registration";
            emailDTo.Body = string.IsNullOrEmpty(mailText)
                ? string.Format(_localizer["Please confirm your account by <a href='{0}'>clicking here</a>."], emailVerificationUri)
                : mailText.Replace("[userName]", user.UserName).Replace("[email]", user.Email).Replace("[emailVerificationUri]", emailVerificationUri);
        }
        else
        {
            emailDTo.Body = emailDTo.Body
                .Replace("[fullName]", user.FullName)
                .Replace("[company]", user.CompanyName)
                .Replace("[address]", user.Address1)
                .Replace("[userName]", user.UserName)
                .Replace("[email]", user.Email)
                .Replace("[emailVerificationUri]", emailVerificationUri);
        }

        return emailDTo;
    }

    public async Task<Result<EmailTemplateDto>> GetTemplateByTypeAsync(EmailTemplateType emailTemplateType)
    {
        var emailTemplateTypeDto = await _repository.FirstByConditionAsync<EmailTemplate>(m => m.EmailTemplateType == emailTemplateType && m.IsSystem);
        return await Result<EmailTemplateDto>.SuccessAsync(emailTemplateTypeDto.Adapt<EmailTemplateDto>());
    }

    public async Task<EmailTemplateDto> GenerateEmailOTP(UserDetailsDto user, string OTP)
    {
        var confirmEmailTemplate = await _repository.FirstByConditionAsync<EmailTemplate>(m => m.EmailTemplateType == EmailTemplateType.EmailOTP && m.IsSystem);
        var emailDTo = confirmEmailTemplate.Adapt<EmailTemplateDto>();
        if (emailDTo == null)
        {
            string filename = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Email Templates/email-otp.html");
            byte[] result;

            using (FileStream sourceStream = File.Open(filename, FileMode.Open))
            {
                result = new byte[sourceStream.Length];
                await sourceStream.ReadAsync(result, 0, (int)sourceStream.Length);
            }

            string mailText = System.Text.Encoding.ASCII.GetString(result);
            emailDTo.Body = string.IsNullOrEmpty(mailText) ? string.Format(_localizer["Please confirm your OTP [0] to login."], OTP)
                : mailText.Replace("[userName]", user.UserName).Replace("[otpcode]", OTP);
            emailDTo.Subject = "OTP";
        }
        else
        {
            emailDTo.Body = emailDTo.Body
                .Replace("[fullName]", user.FullName)
                .Replace("[company]", user.CompanyName)
                .Replace("[address]", user.Address1)
                .Replace("[userName]", user.UserName)
                .Replace("[otpcode]", OTP);
        }

        return emailDTo;
    }

    public async Task<EmailTemplateDto> GenerateEmailForgetPassword(UserDetailsDto user, string token, string passwordResetUrl)
    {
        var confirmEmailTemplate = await _repository.FirstByConditionAsync<EmailTemplate>(m => m.EmailTemplateType == EmailTemplateType.ResetPassword && m.IsSystem);
        var emailDTo = confirmEmailTemplate.Adapt<EmailTemplateDto>();
        if (emailDTo == null)
        {
            string filename = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Email Templates/reset-password.html");
            byte[] result;

            using (FileStream sourceStream = File.Open(filename, FileMode.Open))
            {
                result = new byte[sourceStream.Length];
                await sourceStream.ReadAsync(result, 0, (int)sourceStream.Length);
            }

            emailDTo.Subject = "Reset Password";
            string mailText = System.Text.Encoding.ASCII.GetString(result);
            emailDTo.Body = string.IsNullOrEmpty(mailText) ? string.Format(_localizer["Your Password Reset Token is '{1}' Or You can reset your password by <a href='{0}'>clicking here</a>."], passwordResetUrl, token)
                : mailText.Replace("[userName]", user.UserName).Replace("[resetPasswordUri]", passwordResetUrl);
        }
        else
        {
            emailDTo.Body = emailDTo.Body
                .Replace("[fullName]", user.FullName)
                .Replace("[token]", token)
                .Replace("[company]", user.CompanyName)
                .Replace("[address]", user.Address1)
                .Replace("[userName]", user.UserName)
                .Replace("[resetPasswordUri]", passwordResetUrl);
        }

        return emailDTo;
    }

    public async Task<Result<EmailTemplateDto>> GetAsync(Guid id)
    {
        var toReturn = await _repository.GetByIdAsync<EmailTemplate>(id);

        if (toReturn == null)
            throw new EntityNotFoundException(string.Format(_localizer["emailtemplate.notfound"], id));

        return await Result<EmailTemplateDto>.SuccessAsync(toReturn.Adapt<EmailTemplateDto>());
    }

    public async Task<PaginatedResult<EmailTemplateDto>> SearchAsync(EmailTemplateListFilter filter)
    {
        return await _repository.GetSearchResultsAsync<EmailTemplate, EmailTemplateDto>(filter.PageNumber, filter.PageSize, filter.OrderBy, filter.OrderType, new Filters<EmailTemplate>(), filter.AdvancedSearch, filter.Keyword);
    }

    public async Task<Result<EmailTemplateDto>> CreateAsync(CreateEmailTemplateRequest request)
    {
        if (await _repository.ExistsAsync<EmailTemplate>(a => a.Subject.Equals(request.Subject)))
            throw new EntityAlreadyExistsException(string.Format(_localizer["emailtemplate.alreadyexists"], request.Subject));

        var toAdd = new EmailTemplate(request.CreatedBy, request.Subject, request.Body, request.Variables, request.JsonBody, request.Status, request.SmtpConfigurationId, request.IsSystem, request.EmailTemplateType);

        toAdd.DomainEvents.Add(new EmailTemplateCreatedEvent(toAdd));
        toAdd.DomainEvents.Add(new StatsChangedEvent());

        _ = await _repository.CreateAsync(toAdd);
        _ = await _repository.SaveChangesAsync();
        return await Result<EmailTemplateDto>.SuccessAsync(toAdd.Adapt<EmailTemplateDto>());
    }

    public async Task<Result<EmailTemplateDto>> UpdateAsync(Guid id, UpdateEmailTemplateRequest request)
    {
        var toUpdate = await _repository.GetByIdAsync<EmailTemplate>(id);

        if (toUpdate == null)
            throw new EntityNotFoundException(string.Format(_localizer["emailtemplate.notfound"], id));

        var updatedEmailTemplate = toUpdate.Update(request.Subject, request.Body, request.Variables, request.JsonBody, request.Status, request.SmtpConfigurationId, request.IsSystem, request.EmailTemplateType);

        toUpdate.DomainEvents.Add(new EmailTemplateUpdatedEvent(toUpdate));
        toUpdate.DomainEvents.Add(new StatsChangedEvent());

        await _repository.UpdateAsync(updatedEmailTemplate);
        _ = await _repository.SaveChangesAsync();
        return await Result<EmailTemplateDto>.SuccessAsync(updatedEmailTemplate.Adapt<EmailTemplateDto>());
    }

    public async Task<Result<bool>> DeleteAsync(Guid id)
    {
        var toDelete = await _repository.GetByIdAsync<EmailTemplate>(id);
        if (toDelete.IsSystem)
            throw new InvalidOperationException(string.Format(_localizer["emailtemplate.systemtemplatecannotbedeleted"], id));

        await _repository.RemoveByIdAsync<EmailTemplate>(id);

        toDelete.DomainEvents.Add(new EmailTemplateDeletedEvent(toDelete));
        toDelete.DomainEvents.Add(new StatsChangedEvent());
        return await Result<bool>.SuccessAsync(await _repository.SaveChangesAsync() > 0);
    }

    public async Task<EmailTemplateDto> GenerateEmailBodyForProductStatusUpdate(Product product, UserDetailsDto user)
    {
        var confirmEmailTemplate = await _repository.FirstByConditionAsync<EmailTemplate>(m => m.EmailTemplateType == EmailTemplateType.ProductStatusUpdated && m.IsSystem);
        var emailDTo = confirmEmailTemplate.Adapt<EmailTemplateDto>();
        if (emailDTo != null)
        {
            // string origin = await GetDomainApiUriAsync(user, origin);

            emailDTo.Body = emailDTo.Body
                .Replace("[fullName]", user.FullName)
                .Replace("[company]", user.CompanyName)
                .Replace("[address]", user.Address1)
                .Replace("[productlink]", "https://myreliablesite.m2mbeta.com/admin/dashboard/billing/products-services/list/show");
        }

        return emailDTo;
    }

    public async Task<EmailTemplateDto> GenerateEmailBodyForProductCancellation(Product product, UserDetailsDto user, string token)
    {
        var confirmEmailTemplate = await _repository.FirstByConditionAsync<EmailTemplate>(m => m.EmailTemplateType == EmailTemplateType.ProductCancellation && m.IsSystem);
        var emailDTo = confirmEmailTemplate.Adapt<EmailTemplateDto>();
        if (emailDTo != null)
        {
            emailDTo.Body = emailDTo.Body
                .Replace("[fullName]", user.FullName)
                .Replace("[company]", user.CompanyName)
                .Replace("[address]", user.Address1)
                .Replace("Open", "Cancel")
                .Replace("Cancelled", "Cancellation")
                .Replace("is cancelled", "is about to be cancelled")
                .Replace("view products related to you", "confirm cancellation")
                .Replace("[productlink]", "https://client.myreliablesite.m2mbeta.com/client/dashboard/products/cancellation/confirm/" + product.Id.ToString() + $"?token={token}");
        }

        return emailDTo;
    }

}