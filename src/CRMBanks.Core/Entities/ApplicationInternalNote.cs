using CRMBanks.SharedKernel.Common.AbstractClasses;

namespace CRMBanks.Core.Entities;

public class ApplicationInternalNote : EntityProduction
{
    public int ApplicationId { get; set; }
    public string ApplicationType { get; set; } = string.Empty; // "Loan" or "Deposit"
    
    public int WorkerId { get; set; }
    public User Worker { get; set; } = null!;
    
    public string Note { get; set; } = string.Empty;
    public bool IsPrivate { get; set; } = true; // Internal notes are private by default
    public string? NoteType { get; set; } // "Verification", "Risk Assessment", "Client Communication", etc.
    
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }
}

public class VerificationChecklist : EntityProduction
{
    public int ApplicationId { get; set; }
    public string ApplicationType { get; set; } = string.Empty; // "Loan" or "Deposit"
    
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public bool IsCompleted { get; set; }
    public bool IsMandatory { get; set; }
    
    public DateTime? CompletedAt { get; set; }
    public int? CompletedByWorkerId { get; set; }
    public User? CompletedByWorker { get; set; }
    
    public string? Comments { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }
}
