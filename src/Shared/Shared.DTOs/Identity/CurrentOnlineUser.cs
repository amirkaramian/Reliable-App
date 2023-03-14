using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyReliableSite.Shared.DTOs.Identity;

public class CurrentOnlineUser
{
    public static List<OnlineUser> OnlineUsers = new List<OnlineUser>();
}

public class OnlineUser
{
    public string UserId { get; set; }
    public string UserNameEmail { get; set; }
    public string ConnectionId { get; set; }
}
