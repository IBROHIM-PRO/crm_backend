using CRMBanks.SharedKernel.Common.AbstractClasses;

namespace CRMBanks.Core.Entities;

public class TypeCredit : EntityBase
{
    public string Name { get; set; } = string.Empty;
    public ICollection<Credit> Credits { get; set; } = new List<Credit>();
    public ICollection<TypeProduct> TypeProducts { get; set; } = new List<TypeProduct>();
}
