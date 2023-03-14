using MyReliableSite.Application.Common.Interfaces;
using MyReliableSite.Shared.DTOs.MassEmail;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyReliableSite.Application.MassEmails.Interfaces;

public interface IMassEmailService : ITransientService
{
    Task<bool> SendEmailAsync(MassEmailSendRequest request);
}
