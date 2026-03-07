using CRMBanks.SharedKernel.Enums;

namespace CRMBanks.Core.Dtos;

public class RequestDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public int? ProductId { get; set; }
    public int Sum { get; set; }
    public int Srok { get; set; }
    public int? RegionId { get; set; }
    public string? RegionName { get; set; }
    public int? TypeProductId { get; set; }
    public List<int> BankIds { get; set; } = new();
    public State State { get; set; }
}
