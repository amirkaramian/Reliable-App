using MyReliableSite.Domain.Common.Contracts;
using MyReliableSite.Domain.Contracts;

namespace MyReliableSite.Domain.MaintenanceMode;

public class MaintenanceMode : AuditableEntity, IMustHaveTenant
{
    public DateTime ExpirationDateTime { get; set; }
    public string Message { get; set; }
    public bool Status { get; set; }
    public string Tenant { get; set; }
    public string ByPassuserRoles { get; set; }
    public string ByPassUsers { get; set; }

    public MaintenanceMode()
    {
    }

    public MaintenanceMode(DateTime expirationDateTime, string message, bool status, string byPassuserRoles, string byPassUsers)
    {
        ExpirationDateTime = expirationDateTime;
        Message = message;
        Status = status;
        ByPassUsers = byPassUsers;
        ByPassuserRoles = byPassuserRoles;
    }

    public MaintenanceMode Update(DateTime expirationDateTime, string message, bool status, string byPassuserRoles, string byPassUsers)
    {
        if (status != Status) Status = status;
        if (expirationDateTime != ExpirationDateTime) ExpirationDateTime = expirationDateTime;
        if (!string.IsNullOrWhiteSpace(message) && !string.Equals(Message, message, StringComparison.InvariantCultureIgnoreCase)) Message = message;
        if (!string.IsNullOrWhiteSpace(byPassuserRoles) && !string.Equals(ByPassuserRoles, byPassuserRoles, StringComparison.InvariantCultureIgnoreCase)) ByPassuserRoles = byPassuserRoles;
        if (!string.IsNullOrWhiteSpace(byPassUsers) && !string.Equals(ByPassUsers, byPassUsers, StringComparison.InvariantCultureIgnoreCase)) ByPassUsers = byPassUsers;
        return this;
    }
}
