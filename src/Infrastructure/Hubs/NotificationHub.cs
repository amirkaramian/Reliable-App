using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using MyReliableSite.Application.Common.Interfaces;
using MyReliableSite.Infrastructure.Identity.Extensions;
using MyReliableSite.Shared.DTOs.Identity;

namespace MyReliableSite.Infrastructure.Hubs;

[Authorize]
public class NotificationHub : Hub, ITransientService
{
    private readonly ILogger<NotificationHub> _logger;
    private static List<OnlineUser> _users = new List<OnlineUser>();
    public NotificationHub(ILogger<NotificationHub> logger)
    {
        _logger = logger;
    }

    public override async Task OnConnectedAsync()
    {
        string tenant = Context.User.GetTenant();
        string name = Context.User.Identity?.Name;
        string userId = Context.User.GetUserId();
        if (string.IsNullOrEmpty(tenant))
        {
            throw new Exception();
        }

        _users.Add(new OnlineUser() { UserId = userId, UserNameEmail = name, ConnectionId = Context.ConnectionId });
        CurrentOnlineUser.OnlineUsers = _users;
        await Groups.AddToGroupAsync(Context.ConnectionId, $"GroupTenant-{tenant}");
        await base.OnConnectedAsync();
        await Clients.All.SendAsync("onlineUsers", _users);

        _logger.LogInformation($"A client connected to NotificationHub: {Context.ConnectionId}");
    }

    public override async Task OnDisconnectedAsync(Exception exception)
    {
        string tenant = Context.User.GetTenant();
        if (string.IsNullOrEmpty(tenant))
        {
            throw new Exception();
        }

        var thisUser = _users.FirstOrDefault(m => m.ConnectionId == Context.ConnectionId);
        if (thisUser != null)
        {
            _users.Remove(thisUser);

        }

        await Groups.RemoveFromGroupAsync(Context.ConnectionId, $"GroupTenant-{tenant}");
        await base.OnDisconnectedAsync(exception);
        await Clients.All.SendAsync("onlineUsers", _users);
        CurrentOnlineUser.OnlineUsers = _users;

        _logger.LogInformation($"A client disconnected from NotificationHub: {Context.ConnectionId}");
    }

}