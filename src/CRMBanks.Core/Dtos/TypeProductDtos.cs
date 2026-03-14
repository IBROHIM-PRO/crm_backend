namespace CRMBanks.Core.Dtos;

public class TypeProductDto
{
    public int Id { get; set; }
    public int? TypeCreditId { get; set; }
    public int? TypeDepositId { get; set; }
    public TypeCreditDto? TypeCredit { get; set; }
    public TypeDepositDto? TypeDeposit { get; set; }
}
