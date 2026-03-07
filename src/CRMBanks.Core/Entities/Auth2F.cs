using CRMBanks.SharedKernel.Common.AbstractClasses;

namespace CRMBanks.Core.Entities;

public class Auth2F : EntityBase
{
    public int UserId { get; set; }
    public User? Users { get; set; }
    public int Code { get; set; }
    public DateTimeOffset DateTimeSendCode { get; set; }
    public bool IsEnabled { get; set; }
}
