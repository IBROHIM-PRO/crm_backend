namespace CRMBanks.SharedKernel.Common.Interfaces;

public interface IDeletable
{
    bool IsDeleted { get; set; }
    
    public DateTime? DateDeletedAt { get; set; }
}