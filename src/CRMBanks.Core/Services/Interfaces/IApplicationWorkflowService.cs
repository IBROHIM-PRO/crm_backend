using CRMBanks.Core.Dtos;

namespace CRMBanks.Core.Services.Interfaces;

public interface IApplicationWorkflowService
{
    // Claim & Lock Mechanism
    Task<ClaimResponseDto> ClaimApplicationAsync(int applicationId, int workerId, string applicationType, string? comments = null);
    Task<bool> ReleaseApplicationAsync(int applicationId, int workerId, string applicationType);
    
    // Status Management
    Task<bool> UpdateApplicationStatusAsync(int applicationId, UpdateApplicationStatusDto dto, int workerId, string applicationType);
    Task<bool> EscalateApplicationAsync(EscalateApplicationDto dto, string applicationType);
    
    // Internal Notes
    Task<bool> AddInternalNoteAsync(AddInternalNoteDto dto);
    Task<List<InternalNoteDto>> GetInternalNotesAsync(int applicationId, string applicationType);
    Task<bool> UpdateInternalNoteAsync(int noteId, string noteContent, int workerId);
    Task<bool> DeleteInternalNoteAsync(int noteId, int workerId);
    
    // Verification Checklist
    Task<VerificationChecklistDto> GetVerificationChecklistAsync(int applicationId, string applicationType);
    Task<bool> UpdateVerificationItemAsync(int checklistItemId, bool isCompleted, int workerId, string? comments = null);
    Task<bool> InitializeDefaultChecklistAsync(int applicationId, string applicationType);
    
    // Audit Trail / History
    Task<List<ApplicationHistoryDto>> GetApplicationHistoryAsync(int applicationId, string applicationType);
    Task<bool> AddHistoryEntryAsync(int applicationId, string applicationType, string action, string description, int performedBy, string? details = null);
    
    // Enhanced Application Retrieval
    Task<EnhancedLoanApplicationDto?> GetEnhancedLoanApplicationAsync(int id, int currentUserId, string userRole);
    Task<EnhancedDepositApplicationDto?> GetEnhancedDepositApplicationAsync(int id, int currentUserId, string userRole);
}
