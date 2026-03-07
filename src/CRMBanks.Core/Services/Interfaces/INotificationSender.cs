using CRMBanks.Core.Dtos;

namespace CRMBanks.Core.Services.Interfaces;

public interface INotificationSender
{
    Task SendToUserAsync(int userId, NotificationDto notification);
}
