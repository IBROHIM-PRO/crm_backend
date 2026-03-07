using System.ComponentModel.DataAnnotations;
using CRMBanks.SharedKernel.Common.AbstractClasses;

namespace CRMBanks.Core.Entities;

public class User : EntityBase
{
    [Required(ErrorMessage = "Ном холи аст")]
    public string Name { get; set; } = string.Empty;

    public string Phone { get; set; } = string.Empty;

    public int BankId { get; set; }
    public Bank? Bank { get; set; }

    public int RoleId { get; set; }
    public Role? Role { get; set; }

    public int AzSum { get; set; } = 0;

    public int ToSum { get; set; } = 0;

    public string Email { get; set; }

    public string Password { get; set; }
    
    public string Token { get; set; }

    public virtual List<Region> Regions { get; set; } = new List<Region>();

    public ICollection<RequestAction> RequestActions { get; set; } = new List<RequestAction>();
    public ICollection<Auth2F> Auth2Fs { get; set; } = new List<Auth2F>();
}
