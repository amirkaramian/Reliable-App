using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Options;
using MyReliableSite.Application.Abstractions.Services.Identity;
using MyReliableSite.Application.Common.Interfaces;
using MyReliableSite.Application.MassEmails.Interfaces;
using MyReliableSite.Application.Settings;
using MyReliableSite.Domain.Billing;
using MyReliableSite.Shared.DTOs.General.Requests;
using MyReliableSite.Shared.DTOs.MassEmail;
using MyReliableSite.Shared.DTOs.SmtpConfigurations;

namespace MyReliableSite.Application.MassEmails.Services;

public class MassEmailService : IMassEmailService
{
    private readonly IJobService _jobService;
    private readonly IMailService _mailService;
    private readonly MailSettings _mailSettings;
    private readonly IStringLocalizer<MassEmailService> _localizer;
    private readonly INotificationService _notificationService;
    private readonly IRepositoryAsync _repo;
    private readonly IUserService _userService;

    public MassEmailService(
        IJobService jobService,
        IMailService mailService,
        IOptions<MailSettings> mailSettings,
        IStringLocalizer<MassEmailService> localizer,
        INotificationService notificationService,
        IRepositoryAsync repo,
        IUserService userService)
    {
        _jobService = jobService;
        _mailService = mailService;
        _mailSettings = mailSettings.Value;
        _localizer = localizer;
        _notificationService = notificationService;
        _repo = repo;
        _userService = userService;
    }

    public async Task<bool> SendEmailAsync(MassEmailSendRequest request)
    {
        var smtp = await _repo.GetByIdAsync<SmtpConfiguration, SmtpConfigurationDto>(request.SmtpConfigId);

        var userDetails = await _userService.GetAllAsync(request.ClientIds.Select(x => x.ToString()));

        int index = 0;
        int intervalInSeconds = 1;
        do
        {
            var users = userDetails.Data.Skip(index).Take(request.NumberOfEmails).ToList();

            foreach (var item in users)
            {
                var mailRequest = new MailRequest
                {
                    From = smtp.FromEmail,
                    To = new List<string> { item.Email },
                    Body = request.EmailBody.Replace("[[fullName]]", item.FullName).Replace("[[company]]", item.CompanyName).Replace("[[address]]", item.Address1),
                    Subject = _localizer["Update | Mass Email"]
                };

                _jobService.Schedule(() => _mailService.SendAsync(mailRequest, smtp), TimeSpan.FromSeconds(intervalInSeconds));

                index++;
            }

            intervalInSeconds += request.IntervalInSeconds;
        }
        while (index < userDetails.Data.Count);

        return true;
    }
}
