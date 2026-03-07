using CRMBanks.SharedKernel.Common.AbstractClasses;

namespace CRMBanks.Core.Entities;

public class TypeDeposit : EntityBase
{
    public string Name { get; set; } = string.Empty;
    public ICollection<TypeProduct> TypeProducts { get; set; } = new List<TypeProduct>();
    public ICollection<Deposit> Deposits { get; set; } = new List<Deposit>();
}
