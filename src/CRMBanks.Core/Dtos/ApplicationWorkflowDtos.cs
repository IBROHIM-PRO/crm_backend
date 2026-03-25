using CRMBanks.SharedKernel.Enums;

namespace CRMBanks.Core.Dtos;

// Claim & Lock Mechanism
public class ClaimApplicationDto
{
    public int ApplicationId { get; set; }
    public int WorkerId { get; set; }
    public string? Comments { get; set; }
}

public class ClaimResponseDto
{
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;
    public string? ClaimedBy { get; set; }
    public DateTime? ClaimedAt { get; set; }
}

// Comprehensive Status Updates
public class UpdateApplicationStatusDto
{
    public ApplicationStatus Status { get; set; }
    public string? Comments { get; set; }
    public string? RejectionReason { get; set; }
    public bool RequiresClientAction { get; set; }
    public string? ClientActionMessage { get; set; }
}

// Escalation to Boss
public class EscalateApplicationDto
{
    public int ApplicationId { get; set; }
    public int WorkerId { get; set; }
    public string Reason { get; set; } = string.Empty;
    public string? Comments { get; set; }
    public int? EscalatedToBossId { get; set; }
}

// Internal Notes
public class AddInternalNoteDto
{
    public int ApplicationId { get; set; }
    public int WorkerId { get; set; }
    public string Note { get; set; } = string.Empty;
    public bool IsPrivate { get; set; } = true; // Internal notes are private by default
    public string? NoteType { get; set; } // "Verification", "Risk Assessment", "Client Communication", etc.
}

public class InternalNoteDto
{
    public int Id { get; set; }
    public int ApplicationId { get; set; }
    public string Note { get; set; } = string.Empty;
    public string WorkerName { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public bool IsPrivate { get; set; }
    public string? NoteType { get; set; }
}

// Audit Trail / History Log
public class ApplicationHistoryDto
{
    public int Id { get; set; }
    public int ApplicationId { get; set; }
    public string Action { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string PerformedBy { get; set; } = string.Empty;
    public DateTime PerformedAt { get; set; }
    public string? Details { get; set; }
}

// Quick Verification Dashboard
public class VerificationChecklistDto
{
    public int ApplicationId { get; set; }
    public List<VerificationItemDto> Checklist { get; set; } = new();
    public bool AllMandatoryCompleted { get; set; }
    public int CompletedCount { get; set; }
    public int TotalCount { get; set; }
    public DateTime? LastVerifiedAt { get; set; }
    public string? VerifiedBy { get; set; }
}

public class VerificationItemDto
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public bool IsCompleted { get; set; }
    public bool IsMandatory { get; set; }
    public DateTime? CompletedAt { get; set; }
    public string? CompletedBy { get; set; }
    public string? Comments { get; set; }
}

// Enhanced Application DTOs with workflow information
public class EnhancedLoanApplicationDto
{
    public int Id { get; set; }
    public string ApplicantName { get; set; } = string.Empty;
    public decimal RequestedAmount { get; set; }
    public ApplicationStatus Status { get; set; }
    public DateTime ApplicationDate { get; set; }
    public DateTime? LastUpdatedDate { get; set; }
    
    // Assignment & Lock Information
    public int? AssignedWorkerId { get; set; }
    public string? AssignedWorkerName { get; set; }
    public DateTime? ClaimedAt { get; set; }
    public bool IsLocked => AssignedWorkerId.HasValue;
    
    // Workflow Information
    public List<InternalNoteDto> InternalNotes { get; set; } = new();
    public List<ApplicationHistoryDto> History { get; set; } = new();
    public VerificationChecklistDto? VerificationChecklist { get; set; }
    
    // Original fields
    public string RegionName { get; set; } = string.Empty;
    public string CreditName { get; set; } = string.Empty;
    public List<string> SelectedBankNames { get; set; } = new();
}

public class EnhancedDepositApplicationDto
{
    public int Id { get; set; }
    public string ApplicantName { get; set; } = string.Empty;
    public decimal DepositAmount { get; set; }
    public ApplicationStatus Status { get; set; }
    public DateTime ApplicationDate { get; set; }
    public DateTime? LastUpdatedDate { get; set; }
    
    // Assignment & Lock Information
    public int? AssignedWorkerId { get; set; }
    public string? AssignedWorkerName { get; set; }
    public DateTime? ClaimedAt { get; set; }
    public bool IsLocked => AssignedWorkerId.HasValue;
    
    // Workflow Information
    public List<InternalNoteDto> InternalNotes { get; set; } = new();
    public List<ApplicationHistoryDto> History { get; set; } = new();
    public VerificationChecklistDto? VerificationChecklist { get; set; }
    
    // Original fields
    public string RegionName { get; set; } = string.Empty;
    public string DepositName { get; set; } = string.Empty;
    public List<string> SelectedBankNames { get; set; } = new();
}
