namespace MyReliableSite.Shared.DTOs.MFA;

public class UserAuthenticatorStatus
{
    public bool HasAuthenticator { get; set; }
    public bool Is2faEnabled { get; set; }
    public bool IsMachineRemembered { get; set; }
    public int RecoveryCodesLeft { get; set; }
    public bool HasAppAuthenticator { get; set; }
}
