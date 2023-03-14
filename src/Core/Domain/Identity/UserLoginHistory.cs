using MyReliableSite.Domain.Common.Contracts;
using MyReliableSite.Domain.Contracts;

namespace MyReliableSite.Domain.Identity;

public class UserLoginHistory : AuditableEntity
{
    public string UserId { get; set; }
    public DateTime LoginTime { get; set; }
    public string IpAddress { get; set; }
    public string DeviceName { get; set; }
    public string Location { get; set; }
    public UserLoginStatus Status { get; set; }
    public UserLoginHistory(string userId, DateTime loginTime, string ipAddress, string deviceName, string location, UserLoginStatus status)
    {
        UserId = userId;
        LoginTime = loginTime;
        IpAddress = ipAddress;
        DeviceName = deviceName;
        Location = location;
        Status = status;
    }
}

public enum UserLoginStatus
{
    OK,
    ERR,
    WRN
}