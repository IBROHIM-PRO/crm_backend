namespace CRMBanks.Core.Dtos;

public class CardDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public int ShelfLifeMonths { get; set; }
    public decimal PurchaseAmount { get; set; }
    public string Currency { get; set; } = "USD";
    public int TypeCardId { get; set; }
    public TypeCardDto? TypeCard { get; set; }
}
