namespace MyReliableSite.Shared.DTOs.ReCaptcha;

public class ReCaptchaResponse
{
    public bool Success { get; set; }

    public float Score { get; set; }

    public string Action { get; set; }

    // timestamp of the challenge load (ISO format yyyy-MM-dd'T'HH:mm:ssZZ)
    public DateTime ChallengeTs { get; set; }

    // the hostname of the site where the reCAPTCHA was solved
    public string HostName { get; set; }

    public string[] ErrorCodes { get; set; }
}
