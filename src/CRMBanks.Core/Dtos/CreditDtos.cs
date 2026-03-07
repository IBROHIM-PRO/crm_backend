namespace CRMBanks.Core.Dtos;

public class CreditDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public int BankId { get; set; }
    public BankDto? Bank { get; set; }
    public int TypeCreditId { get; set; }
    public TypeCreditDto? TypeCredit { get; set; }
    public double Foiz { get; set; }
    public int AzSum { get; set; }
    public int ToSum { get; set; }
    public string Document { get; set; } = string.Empty;
    public string TypeSumId { get; set; } = string.Empty;
    public int AzSana { get; set; }
    public int ToSana { get; set; }
    public string Infoprot { get; set; } = string.Empty;
}
