using CRMBanks.SharedKernel.Common.AbstractClasses;
using CRMBanks.SharedKernel.Enums;

namespace CRMBanks.Core.Entities;

public class DepositApplication : EntityProduction
{
    public string ApplicantName { get; set; } = string.Empty;
    public string ApplicantPhone { get; set; } = string.Empty;
    public string ApplicantEmail { get; set; } = string.Empty;
    
    public int DepositId { get; set; }
    public Deposit Deposit { get; set; } = null!;
    
    public decimal DepositAmount { get; set; }
    public int DepositTermMonths { get; set; }
    
    public int RegionId { get; set; }
    public Region Region { get; set; } = null!;
    
    public ApplicationStatus Status { get; set; } = ApplicationStatus.Pending;
    public string? RejectionReason { get; set; }
    
    public DateTime ApplicationDate { get; set; } = DateTime.UtcNow;
    public DateTime? LastUpdatedDate { get; set; }
    
    // Worker assignment for claim & lock mechanism
    public int? AssignedWorkerId { get; set; }
    public User? AssignedWorker { get; set; }
    
    // For multi-bank selection
    public bool SendToAllBanks { get; set; } = false;
    public virtual ICollection<Bank> SelectedBanks { get; set; } = new List<Bank>();
    
    public ICollection<DepositApplicationAction> Actions { get; set; } = new List<DepositApplicationAction>();
}

public class DepositApplicationAction : EntityBase
{
    public int DepositApplicationId { get; set; }
    public DepositApplication DepositApplication { get; set; } = null!;
    
    public int UserId { get; set; }
    public User User { get; set; } = null!;
    
    public string Action { get; set; } = string.Empty;
    public string? Comments { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
