using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyReliableSite.Shared.DTOs.Maintenance;

public class CreateMaintenanceRequest : IMustBeValid
{
    public DateTime ExpirationDateTime { get; set; }
    public string Message { get; set; }
    public bool Status { get; set; }
    public string ByPassuserRoles { get; set; }
    public string ByPassUsers { get; set; }
}

public class MaintenanceRequest : IMustBeValid
{
    public DateTime ExpirationDateTime { get; set; }
    public string Message { get; set; }
    public bool Status { get; set; }
    public string[] ByPassuserRoles { get; set; }
    public string[] ByPassUsers { get; set; }
}
