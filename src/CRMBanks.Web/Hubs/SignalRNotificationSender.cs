using CRMBanks.Core.Dtos;
using CRMBanks.Core.Services.Interfaces;
using Microsoft.AspNetCore.SignalR;

namespace CRMBanks.Web.Hubs;

public class SignalRNotificationSender(IHubContext<NotificationHub> hubContext) : INotificationSender
{
    public async Task SendToUserAsync(int userId, NotificationDto notification)
    {
        await hubContext.Clients
            .Group($"user_{userId}")
            .SendAsync("ReceiveNotification", notification);
    }
}
