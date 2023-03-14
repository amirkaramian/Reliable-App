using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MimeKit;
using MyReliableSite.Application.Abstractions.Services.Identity;
using MyReliableSite.Application.Common.Interfaces;
using MyReliableSite.Application.Settings;
using MyReliableSite.Application.SmtpConfigurations.Interfaces;
using MyReliableSite.Shared.DTOs.General.Requests;
using MyReliableSite.Shared.DTOs.Identity;
using MyReliableSite.Shared.DTOs.Orders;
using MyReliableSite.Shared.DTOs.SmtpConfigurations;

namespace MyReliableSite.Infrastructure.Common.Services;

public class SmtpMailService : IMailService
{
    private readonly MailSettings _settings;
    private readonly ILogger<SmtpMailService> _logger;
    private readonly IEmailTemplateService _emailTemplateService;
    private readonly ISmtpConfigurationService _smtpConfigurationService;

    public SmtpMailService(IOptions<MailSettings> settings, ILogger<SmtpMailService> logger, IEmailTemplateService emailTemplateService, ISmtpConfigurationService smtpConfigurationService)
    {
        _settings = settings.Value;
        _logger = logger;
        _emailTemplateService = emailTemplateService;
        _smtpConfigurationService = smtpConfigurationService;
    }

    public async Task SendAsync(MailRequest request)
    {
        int emailsSent = 0;
        try
        {
            int amountLimitToSend = request.AmountLimit is > 0 ? request.AmountLimit.Value : request.To.Count;
            var email = new MimeMessage
            {
                Sender = new MailboxAddress(_settings.DisplayName, request.From ?? _settings.From),
                Subject = request.Subject,
                Body = new BodyBuilder
                {
                    HtmlBody = request.Body
                }.ToMessageBody()
            };

            using var smtp = new SmtpClient();
            await smtp.ConnectAsync(_settings.Host, _settings.Port, SecureSocketOptions.StartTls);
            await smtp.AuthenticateAsync(_settings.UserName, _settings.Password);

            do
            {
                email.To.AddRange(GetAddresses(request.To, emailsSent, amountLimitToSend));
                await smtp.SendAsync(email);
                emailsSent += amountLimitToSend;

                if (request.MinutesToWait is null or < 0)
                    continue;

                await Task.Delay((int)TimeSpan.FromMinutes(request.MinutesToWait.Value).TotalMilliseconds);
            }
            while (emailsSent < request.To.Count);

            await smtp.DisconnectAsync(true);
        }
        catch (System.Exception ex)
        {
            _logger.LogError(ex.Message, ex);
        }
    }

    public async Task SendAsync(MailRequest request, SmtpConfigurationDto smtpConfig)
    {
        int emailsSent = 0;
        try
        {
            int amountLimitToSend = request.AmountLimit is > 0 ? request.AmountLimit.Value : request.To.Count;
            var email = new MimeMessage
            {
                Sender = new MailboxAddress(_settings.DisplayName, request.From ?? _settings.From),
                Subject = request.Subject,
                Body = new BodyBuilder
                {
                    HtmlBody = request.Body
                }.ToMessageBody()
            };

            using var smtp = new SmtpClient();
            await smtp.ConnectAsync(smtpConfig.Host, smtpConfig.Port, smtpConfig.HttpsProtocol ? SecureSocketOptions.StartTls : SecureSocketOptions.Auto);

            await smtp.AuthenticateAsync(smtpConfig.Username, smtpConfig.Password);

            do
            {
                email.To.AddRange(GetAddresses(request.To, emailsSent, amountLimitToSend));
                await smtp.SendAsync(email);
                emailsSent += amountLimitToSend;

                if (request.MinutesToWait is null or < 0)
                    continue;

                await Task.Delay((int)TimeSpan.FromMinutes(request.MinutesToWait.Value).TotalMilliseconds);
            }
            while (emailsSent < request.To.Count);

            await smtp.DisconnectAsync(true);
        }
        catch (System.Exception ex)
        {
            _logger.LogError(ex.Message, ex);
        }
    }

    private static IEnumerable<MailboxAddress> GetAddresses(IEnumerable<string> receivers, int toSkip, int toTake)
    {
        return receivers.Where(r => !string.IsNullOrWhiteSpace(r))
                        .Skip(toSkip)
                        .Take(toTake)
                        .Select(MailboxAddress.Parse);
    }

    public async Task SendEmailViaSMTPTemplate(List<UserDetailsDto> clients, Shared.DTOs.EmailTemplates.EmailTemplateType emailTemplateType, string subject, Guid? orderId = null, string url = null, string orderStatus = null)
    {
        foreach (var client in clients)
        {
            var smtpConfigurationDto = new SmtpConfigurationDto();
            var emailTemplate = await _emailTemplateService.GetTemplateByTypeAsync(emailTemplateType);
            string body = emailTemplate.Data.Body;

            if (emailTemplate.Data.Body.Contains("[fullName]"))
             body = emailTemplate.Data.Body.Replace("[fullName]", client?.FullName ?? client.FirstName + client.LastName);
            if (!string.IsNullOrEmpty(client.BrandId))
            {
                var result = await _smtpConfigurationService.GetByBrandAsync(Guid.Parse(client.BrandId));
                if (result?.Data != null && result?.Data.Id != Guid.Empty)
                    smtpConfigurationDto = result?.Data;
            }

            if (!string.IsNullOrWhiteSpace(url))
            {
                if (emailTemplateType == Shared.DTOs.EmailTemplates.EmailTemplateType.OrderCreated ||
                    emailTemplateType == Shared.DTOs.EmailTemplates.EmailTemplateType.OrderAssignment)
                {
                    if (body.Contains("[orderlink]"))
                        body = body.Replace("[orderlink]", url);
                }

                if (emailTemplateType == Shared.DTOs.EmailTemplates.EmailTemplateType.TicketCreated || emailTemplateType == Shared.DTOs.EmailTemplates.EmailTemplateType.TicketAssignment)
                {
                    if (body.Contains("[tickelink]"))
                        body = body.Replace("[tickelink]", url);
                }

                if (emailTemplateType == Shared.DTOs.EmailTemplates.EmailTemplateType.Orders)
                {
                    if (body.Contains("[invoicelink]"))
                        body = body.Replace("[invoicelink]", url);
                    if (body.Contains("[status]"))
                        body = body.Replace("[status]", orderStatus);

                }

                if (emailTemplateType == Shared.DTOs.EmailTemplates.EmailTemplateType.ProductCreated ||
                    emailTemplateType == Shared.DTOs.EmailTemplates.EmailTemplateType.OrderAssignment)
                {
                    if (body.Contains("[productlink]"))
                        body = body.Replace("[productlink]", url);
                }
            }

            if (smtpConfigurationDto == null || smtpConfigurationDto.Id == Guid.Empty)
            {
                await SendAsync(new MailRequest
                {
                    Body = body,
                    Subject = subject,
                    To = new List<string> { client.Email},
                });
            }
            else
            {
                await SendAsync(
                    new MailRequest
                    {
                        Body = body,
                        From = smtpConfigurationDto.FromEmail,
                        Subject = subject,
                        To = new List<string> { client.Email },
                    },
                    smtpConfigurationDto);
            }
        }
    }
}