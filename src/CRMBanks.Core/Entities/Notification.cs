using CRMBanks.SharedKernel.Common.AbstractClasses;
using CRMBanks.SharedKernel.Enums;

namespace CRMBanks.Core.Entities;

public class Notification : EntityProduction
{
    public int UserId { get; set; }
    public User? User { get; set; }

    public int RequestId { get; set; }
    public Request? Request { get; set; }

    public string Title { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;

    public NotificationStatus Status { get; set; } = NotificationStatus.Sent;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
