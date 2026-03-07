using CRMBanks.SharedKernel.Common.AbstractClasses;

namespace CRMBanks.Core.Entities;

public class Card : EntityBase
{
    public string Name { get; set; } = string.Empty;
    public ICollection<Product> Products { get; set; } = new List<Product>();
}
