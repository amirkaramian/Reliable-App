namespace MyReliableSite.Shared.DTOs.Identity;

public class CreateUserLoginHistoryRequest : IMustBeValid
{
    public string UserId { get; set; }
    public DateTime LoginTime { get; set; }
    public string IpAddress { get; set; }
    public string DeviceName { get; set; }
    public string Location { get; set; }
    public UserLoginStatus Status { get; set; }
}