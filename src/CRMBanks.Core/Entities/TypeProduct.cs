using CRMBanks.SharedKernel.Common.AbstractClasses;

namespace CRMBanks.Core.Entities;

public class TypeProduct : EntityBase
{
    public int? TypeCreditId { get; set; }
    public TypeCredit? TypeCredit { get; set; }
    public int? TypeDepositId { get; set; }
    public TypeDeposit? TypeDeposit { get; set; }

    public ICollection<Request> Requests { get; set; } = new List<Request>();
}
