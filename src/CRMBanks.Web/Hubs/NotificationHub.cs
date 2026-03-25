using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace CRMBanks.Web.Hubs;

[Authorize]
public class NotificationHub : Hub
{
    public override async Task OnConnectedAsync()
    {
        var userId = Context.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (!string.IsNullOrEmpty(userId))
            await Groups.AddToGroupAsync(Context.ConnectionId, $"user_{userId}");
        await base.OnConnectedAsync();
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        var userId = Context.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (!string.IsNullOrEmpty(userId))
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, $"user_{userId}");
        await base.OnDisconnectedAsync(exception);
    }

    // Method for workers to join their role-specific group
    public async Task JoinWorkerGroup()
    {
        var userId = Context.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        var role = Context.User?.FindFirst(ClaimTypes.Role)?.Value;
        
        if (!string.IsNullOrEmpty(userId) && !string.IsNullOrEmpty(role) && role.ToLower() == "worker")
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, "workers");
        }
    }

    // Method for bosses to join their role-specific group
    public async Task JoinBossGroup()
    {
        var userId = Context.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        var role = Context.User?.FindFirst(ClaimTypes.Role)?.Value;
        
        if (!string.IsNullOrEmpty(userId) && !string.IsNullOrEmpty(role) && role.ToLower() == "boss")
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, "bosses");
        }
    }
}
