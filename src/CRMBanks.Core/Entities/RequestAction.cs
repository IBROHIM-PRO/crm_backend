using CRMBanks.SharedKernel.Common.AbstractClasses;
using CRMBanks.SharedKernel.Enums;

namespace CRMBanks.Core.Entities;

public class RequestAction : EntityProduction
{
    public int RequestId { get; set; }
    public Request? Request { get; set; }
    public int UserId { get; set; }
    public User? Users { get; set; }
    public State State { get; set; }
    public string? Reason { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
