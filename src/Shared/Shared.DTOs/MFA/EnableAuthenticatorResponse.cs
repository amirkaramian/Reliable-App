using MyReliableSite.Shared.DTOs.Identity;

namespace MyReliableSite.Shared.DTOs.MFA;

public class EnableAuthenticatorResponse
{
    public string SharedKey { get; set; }
    public string AuthenticatorUri { get; set; }
    public string[] RecoveryCodes { get; set; }
    public string StatusMessage { get; set; }
    public TokenResponse tokenResponse { get; set; }
}
