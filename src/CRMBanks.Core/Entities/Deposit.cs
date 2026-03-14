using CRMBanks.SharedKernel.Common.AbstractClasses;

namespace CRMBanks.Core.Entities;

public class Deposit : EntityProduction
{
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public int BankId { get; set; }
    public Bank? Bank { get; set; }
    public int TypeDepositId { get; set; }
    public TypeDeposit? TypeDeposit { get; set; }
    public double Foiz { get; set; }
    public int AzSum { get; set; }
    public int ToSum { get; set; }
    public string Document { get; set; } = string.Empty;
    public string TypeSumId { get; set; } = string.Empty;
    public TypeSum? TypeSum { get; set; }
    public int AzSana { get; set; }
    public int ToSana { get; set; }
    public string Infoprot { get; set; } = string.Empty;

    public ICollection<Product> Products { get; set; } = new List<Product>();
}
