namespace MyReliableSite.Shared.DTOs.Identity;

public class UserLoginHistoryDto : IDto
{
    public Guid Id { get; set; }
    public string UserId { get; set; }
    public string FullName { get; set; }
    public DateTime LoginTime { get; set; }
    public string IpAddress { get; set; }
    public string DeviceName { get; set; }
    public string Location { get; set; }
    public UserLoginStatus Status { get; set; }
}

public enum UserLoginStatus
{
    OK,
    ERR,
    WRN
}
