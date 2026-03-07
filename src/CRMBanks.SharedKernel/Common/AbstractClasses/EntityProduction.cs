using CRMBanks.SharedKernel.Common.Interfaces;

namespace CRMBanks.SharedKernel.Common.AbstractClasses;

public abstract class EntityProduction : EntityBaseWithDateCreated, IDeletable, IWithDateUpdated
{
    public DateTime? DateUpdated { get; set; }
    public bool IsDeleted { get; set; }
    
    public DateTime? DateDeletedAt { get; set; }
}