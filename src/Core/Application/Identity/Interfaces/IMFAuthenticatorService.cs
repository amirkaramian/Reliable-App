using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MyReliableSite.Application.Common.Interfaces;
using MyReliableSite.Application.Wrapper;
using MyReliableSite.Shared.DTOs.MFA;

namespace MyReliableSite.Application.Identity.Interfaces;

public interface IMFAuthenticatorService : ITransientService
{
    Task<EnableAuthenticatorResponse> GETqRCodeUriForAuthenticatorApp(EnableAuthenticatorRequest enableAuthenticatorRequest);
    Task<EnableAuthenticatorResponse> ValidateMFACodeAndAddApp(EnableAuthenticatorRequest enableAuthenticatorRequest, bool flgAdmin, string iPaddress);
    Task<UserAuthenticatorStatus> GetCurrentStatusOfTwoFactorAuthentication(EnableAuthenticatorRequest enableAuthenticatorRequest);
    Task<bool> RemoveTwoFactorAuthentication(EnableAuthenticatorRequest enableAuthenticatorRequest);
    Task<Result<string>> EnableDisable2fa(EnableDisableAuthenticatorRequest enableAuthenticatorRequest);
    Task<UserAuthenticatorStatus> ResetAuthenticator(EnableAuthenticatorRequest enableAuthenticatorRequest);
    Task<string[]> ResetGenerateRecoveryCodes(EnableAuthenticatorRequest enableAuthenticatorRequest);
    Task<Result<string>> GenerateOTPEmail(EnableAuthenticatorRequest enableAuthenticatorRequest);
    Task<Result<string>> Validate2FAEnable2FAByEmail(EnableAuthenticatorRequest enableAuthenticatorRequest);
}
