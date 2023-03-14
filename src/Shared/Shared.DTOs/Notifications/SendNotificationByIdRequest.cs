using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyReliableSite.Shared.DTOs.Notifications;
public class SendNotificationByIdRequest : IMustBeValid
{
    public Guid NotificationId { get; set; }
    public List<string> ToUserIds { get; set; } = new List<string>();
}
