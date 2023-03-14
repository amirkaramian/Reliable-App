using MyReliableSite.Shared.DTOs.General.Requests;
using MyReliableSite.Shared.DTOs.Identity;
using MyReliableSite.Shared.DTOs.SmtpConfigurations;

namespace MyReliableSite.Application.Common.Interfaces;

public interface IMailService : ITransientService
{
    Task SendAsync(MailRequest request);
    Task SendAsync(MailRequest request, SmtpConfigurationDto smtpConfig);
    Task SendEmailViaSMTPTemplate(List<UserDetailsDto> clients, Shared.DTOs.EmailTemplates.EmailTemplateType emailTemplateType, string subject, Guid? orderId = null, string url = null, string orderStatus = null);
}