using CRMBanks.SharedKernel.Enums;

namespace CRMBanks.Core.Dtos;

public class NotificationDto
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public int RequestId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public NotificationStatus Status { get; set; } = NotificationStatus.Sent;
    public string StatusColor => Status switch
    {
        NotificationStatus.Sent => "#9E9E9E",
        NotificationStatus.Seen => "#2196F3",
        NotificationStatus.Dismissed => "#4CAF50",
        _ => "#9E9E9E"
    };
    public DateTime CreatedAt { get; set; }
}

public class UpdateStateWithReasonDto
{
    public string State { get; set; } = string.Empty;
    public string? Reason { get; set; }
}
