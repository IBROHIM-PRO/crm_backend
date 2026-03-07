using CRMBanks.SharedKernel.Common.AbstractClasses;

namespace CRMBanks.Core.Entities;

public class TypeSum : EntityBase
{
    public string Name { get; set; } = string.Empty;
    public ICollection<Credit> Credits { get; set; } = new List<Credit>();
    public ICollection<Deposit> Deposits { get; set; } = new List<Deposit>();
}
