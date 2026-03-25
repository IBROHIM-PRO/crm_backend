using CRMBanks.SharedKernel.Common.AbstractClasses;
using CRMBanks.SharedKernel.Enums;

namespace CRMBanks.Core.Entities;

public class LoanApplication : EntityProduction
{
    public string ApplicantName { get; set; } = string.Empty;
    public string ApplicantPhone { get; set; } = string.Empty;
    public string ApplicantEmail { get; set; } = string.Empty;
    
    public int UserId { get; set; }
    public User User { get; set; } = null!;
    
    public int CreditId { get; set; }
    public Credit Credit { get; set; } = null!;
    
    public decimal RequestedAmount { get; set; }
    public int RequestedTermMonths { get; set; }
    
    public int RegionId { get; set; }
    public Region Region { get; set; } = null!;
    
    public string ApplicationPurpose { get; set; } = string.Empty;
    public string MonthlyIncome { get; set; } = string.Empty;
    public string EmploymentStatus { get; set; } = string.Empty;
    
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
    
    public ICollection<LoanApplicationAction> Actions { get; set; } = new List<LoanApplicationAction>();
}
