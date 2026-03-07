using CRMBanks.SharedKernel.Common.AbstractClasses;

namespace CRMBanks.Core.Entities;

public class Product : EntityBase
{
    public int? CreditId { get; set; }
    public Credit? Credit { get; set; }
    public int? DepositId { get; set; }
    public Deposit? Deposit { get; set; }
    public int? CardId { get; set; }
    public Card? Card { get; set; }

    public ICollection<Request> Requests { get; set; } = new List<Request>();
}
