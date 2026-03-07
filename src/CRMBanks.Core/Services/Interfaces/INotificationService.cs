using CRMBanks.Core.Dtos;
using CRMBanks.SharedKernel.Enums;

namespace CRMBanks.Core.Services.Interfaces;

public interface INotificationService
{
    Task<IEnumerable<NotificationDto>> GetByUserIdAsync(int userId);
    Task<int> GetUnseenCountAsync(int userId);
    Task<bool> UpdateStatusAsync(int notificationId, NotificationStatus status);
    Task<bool> MarkAllSeenAsync(int userId);
    Task CreateForBankWorkersAsync(int requestId);
}
