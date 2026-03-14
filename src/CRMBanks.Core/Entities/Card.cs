using CRMBanks.SharedKernel.Common.AbstractClasses;

namespace CRMBanks.Core.Entities;

public class Card : EntityProduction
{
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public int ShelfLifeMonths { get; set; }
    public decimal PurchaseAmount { get; set; }
    public string Currency { get; set; } = "USD";
    public TypeCard TypeCard { get; set; }
    public int TypeCardId { get; set; }
    public ICollection<Product> Products { get; set; } = new List<Product>();
}
