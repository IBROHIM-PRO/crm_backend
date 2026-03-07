using System.ComponentModel.DataAnnotations;
using CRMBanks.SharedKernel.Common.AbstractClasses;

namespace CRMBanks.Core.Entities;

public class Region : EntityBase
{
    [Required(ErrorMessage = "Ном холи аст")]
    public string Name { get; set; } = string.Empty;

    public ICollection<User> Users { get; set; } = new List<User>();
    public ICollection<Request> Requests { get; set; } = new List<Request>();
}
