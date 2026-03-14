using CRMBanks.SharedKernel.Common.AbstractClasses;

namespace CRMBanks.Core.Entities;

public class TypeCard : EntityBase
{
    public string Name { get; set; } = string.Empty;
    
    public ICollection<Card> Cards { get; set; } = new List<Card>();
}
