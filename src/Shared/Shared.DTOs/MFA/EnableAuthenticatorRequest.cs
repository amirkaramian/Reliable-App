namespace MyReliableSite.Shared.DTOs.MFA;

public class EnableAuthenticatorRequest
{
    public string UserId { get; set; }

    public string Code { get; set; }
    public bool isRemember { get; set; }
}

public class EnableDisableAuthenticatorRequest
{
    public string UserId { get; set; }

    public bool Flag { get; set; }
}
