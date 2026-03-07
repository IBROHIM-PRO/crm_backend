using CRMBanks.SharedKernel.Common.AbstractClasses;
using CRMBanks.SharedKernel.Enums;

namespace CRMBanks.Core.Entities;

public class Request : EntityBase
{
    public string Name { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public int? ProductId { get; set; }
    public Product? Product { get; set; }
    public int? BankId { get; set; }
    public virtual ICollection<Bank>? Banks { get; set; }
    public int Sum { get; set; }
    public int Srok { get; set; }
    public int? RegionId { get; set; }
    public Region? Region { get; set; }
    public int? TypeProductId { get; set; }
    public TypeProduct? TypeProductinfo { get; set; }
    public State State { get; set; }

    public ICollection<RequestAction> RequestActions { get; set; } = new List<RequestAction>();
}
