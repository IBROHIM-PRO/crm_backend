using CRMBanks.SharedKernel.Common.AbstractClasses;
using CRMBanks.SharedKernel.Common.Interfaces;

namespace CRMBanks.Core.Common;

public class EntityBaseWithDateCreated : EntityBase, IWithDateCreated
{
    public DateTime DateCreated { get; set; }
}