using CRMBanks.Core.Common;
using CRMBanks.Core.Dtos;
using CRMBanks.Core.Entities;
using CRMBanks.Core.Services.Interfaces;
using CRMBanks.SharedKernel.Enums;
using Microsoft.EntityFrameworkCore;

namespace CRMBanks.Core.Services;

public class NotificationService(
    IRepository<Notification> notificationRepository,
    IRepository<Request> requestRepository,
    IRepository<User> userRepository,
    INotificationSender notificationSender) : INotificationService
{
    public async Task<IEnumerable<NotificationDto>> GetByUserIdAsync(int userId)
    {
        return await notificationRepository.GetQuery()
            .Where(n => n.UserId == userId)
            .OrderByDescending(n => n.CreatedAt)
            .Select(n => new NotificationDto
            {
                Id = n.Id,
                UserId = n.UserId,
                RequestId = n.RequestId,
                Title = n.Title,
                Message = n.Message,
                Status = n.Status,
                CreatedAt = n.CreatedAt
            })
            .ToListAsync();
    }

    public async Task<int> GetUnseenCountAsync(int userId)
    {
        return await notificationRepository.GetQuery()
            .CountAsync(n => n.UserId == userId && n.Status == NotificationStatus.Sent);
    }

    public async Task<bool> UpdateStatusAsync(int notificationId, NotificationStatus status)
    {
        var notification = await notificationRepository.GetIdAsync(notificationId);
        if (notification == null) return false;

        notification.Status = status;
        notificationRepository.Update(notification);
        return true;
    }

    public async Task<bool> MarkAllSeenAsync(int userId)
    {
        var notifications = await notificationRepository.GetQuery()
            .Where(n => n.UserId == userId && n.Status == NotificationStatus.Sent).ToListAsync();

        if (!notifications.Any()) return false;

        foreach (var n in notifications)
            n.Status = NotificationStatus.Seen;

        notificationRepository.UpdateRange(notifications);
        return true;
    }

    public async Task CreateForBankWorkersAsync(int requestId)
    {
        var request = await requestRepository.GetQuery()
            .Include(r => r.Banks)
            .Include(r => r.Region)
            .FirstOrDefaultAsync(r => r.Id == requestId);

        if (request == null) return;

        var bankIds = request.Banks?.Select(b => b.Id).ToList() ?? new List<int>();
        if (!bankIds.Any()) return;

        var query = userRepository.GetQuery()
            .Include(u => u.Regions)
            .Where(u => bankIds.Contains(u.BankId));

        if (request.RegionId.HasValue)
        {
            query = query.Where(u => u.Regions.Any(r => r.Id == request.RegionId.Value));
        }
        query = query.Where(u =>
            (u.AzSum == 0 && u.ToSum == 0) ||
            (u.AzSum <= request.Sum && u.ToSum >= request.Sum));

        var workers = await query.ToListAsync();

        var notifications = new List<Notification>();
        foreach (var worker in workers)
        {
            notifications.Add(new Notification
            {
                UserId = worker.Id,
                RequestId = request.Id,
                Title = "Дархости нав",
                Message = $"Дархости нав аз {request.Name} ба маблағи {request.Sum} сомонӣ расид.",
                Status = NotificationStatus.Sent,
                CreatedAt = DateTime.UtcNow
            });
        }

        if (!notifications.Any()) return;

        await notificationRepository.AddRangeAsync(notifications);

        foreach (var notification in notifications)
        {
            var dto = new NotificationDto
            {
                Id = notification.Id,
                UserId = notification.UserId,
                RequestId = notification.RequestId,
                Title = notification.Title,
                Message = notification.Message,
                Status = notification.Status,
                CreatedAt = notification.CreatedAt
            };
            await notificationSender.SendToUserAsync(notification.UserId, dto);
        }
    }
}
