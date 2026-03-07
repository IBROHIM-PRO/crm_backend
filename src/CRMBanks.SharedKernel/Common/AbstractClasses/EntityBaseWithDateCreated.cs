using CRMBanks.SharedKernel.Common.Interfaces;

namespace CRMBanks.SharedKernel.Common.AbstractClasses;

public class EntityBaseWithDateCreated : EntityBase, IWithDateCreated
{
    public DateTime DateCreated { get; set; }
}