using CRMBanks.Core.Common;
using CRMBanks.Core.Dtos;
using CRMBanks.Core.Entities;
using CRMBanks.Core.Services.Interfaces;
using CRMBanks.SharedKernel.Enums;
using Microsoft.EntityFrameworkCore;

namespace CRMBanks.Core.Services;

public class ApplicationWorkflowService(
    IRepository<LoanApplication> loanApplicationRepository,
    IRepository<DepositApplication> depositApplicationRepository,
    IRepository<User> userRepository,
    IRepository<ApplicationInternalNote> internalNoteRepository,
    IRepository<VerificationChecklist> verificationChecklistRepository,
    IRepository<LoanApplicationAction> loanActionRepository,
    IRepository<DepositApplicationAction> depositActionRepository) : IApplicationWorkflowService
{
    public async Task<ClaimResponseDto> ClaimApplicationAsync(int applicationId, int workerId, string applicationType, string? comments = null)
    {
        try
        {
            if (applicationType.ToLower() == "loan")
            {
                var application = await loanApplicationRepository.GetQuery()
                    .Include(l => l.AssignedWorker)
                    .FirstOrDefaultAsync(l => l.Id == applicationId);

                if (application == null)
                    return new ClaimResponseDto { Success = false, Message = "Application not found" };

                if (application.AssignedWorkerId.HasValue)
                    return new ClaimResponseDto 
                    { 
                        Success = false, 
                        Message = $"Application already claimed by {application.AssignedWorker?.Name}" 
                    };

                // Claim the application
                application.AssignedWorkerId = workerId;
                application.Status = ApplicationStatus.UnderReview;
                application.LastUpdatedDate = DateTime.UtcNow;

                await loanApplicationRepository.SaveChangesAsync();

                // Add action log
                await AddLoanApplicationActionAsync(applicationId, workerId, "Claimed", comments ?? "Application claimed for processing");

                // Notify other workers that this application is now locked
                await NotifyApplicationLockedAsync(applicationId, "loan", workerId);

                return new ClaimResponseDto 
                { 
                    Success = true, 
                    Message = "Application claimed successfully",
                    ClaimedBy = (await userRepository.GetIdAsync(workerId))?.Name,
                    ClaimedAt = DateTime.UtcNow
                };
            }
            else if (applicationType.ToLower() == "deposit")
            {
                var application = await depositApplicationRepository.GetQuery()
                    .Include(d => d.AssignedWorker)
                    .FirstOrDefaultAsync(d => d.Id == applicationId);

                if (application == null)
                    return new ClaimResponseDto { Success = false, Message = "Application not found" };

                if (application.AssignedWorkerId.HasValue)
                    return new ClaimResponseDto 
                    { 
                        Success = false, 
                        Message = $"Application already claimed by {application.AssignedWorker?.Name}" 
                    };

                // Claim the application
                application.AssignedWorkerId = workerId;
                application.Status = ApplicationStatus.UnderReview;
                application.LastUpdatedDate = DateTime.UtcNow;

                await depositApplicationRepository.SaveChangesAsync();

                // Add action log
                await AddDepositApplicationActionAsync(applicationId, workerId, "Claimed", comments ?? "Application claimed for processing");

                // Notify other workers that this application is now locked
                await NotifyApplicationLockedAsync(applicationId, "deposit", workerId);

                return new ClaimResponseDto 
                { 
                    Success = true, 
                    Message = "Application claimed successfully",
                    ClaimedBy = (await userRepository.GetIdAsync(workerId))?.Name,
                    ClaimedAt = DateTime.UtcNow
                };
            }

            return new ClaimResponseDto { Success = false, Message = "Invalid application type" };
        }
        catch (Exception ex)
        {
            return new ClaimResponseDto { Success = false, Message = $"Error claiming application: {ex.Message}" };
        }
    }

    public async Task<bool> ReleaseApplicationAsync(int applicationId, int workerId, string applicationType)
    {
        try
        {
            if (applicationType.ToLower() == "loan")
            {
                var application = await loanApplicationRepository.GetIdAsync(applicationId);
                if (application == null || application.AssignedWorkerId != workerId)
                    return false;

                application.AssignedWorkerId = null;
                application.Status = ApplicationStatus.Pending;
                application.LastUpdatedDate = DateTime.UtcNow;

                await loanApplicationRepository.SaveChangesAsync();
                await AddLoanApplicationActionAsync(applicationId, workerId, "Released", "Application released back to queue");
            }
            else if (applicationType.ToLower() == "deposit")
            {
                var application = await depositApplicationRepository.GetIdAsync(applicationId);
                if (application == null || application.AssignedWorkerId != workerId)
                    return false;

                application.AssignedWorkerId = null;
                application.Status = ApplicationStatus.Pending;
                application.LastUpdatedDate = DateTime.UtcNow;

                await depositApplicationRepository.SaveChangesAsync();
                await AddDepositApplicationActionAsync(applicationId, workerId, "Released", "Application released back to queue");
            }

            // Notify workers that this application is now available
            await NotifyApplicationReleasedAsync(applicationId, applicationType);
            return true;
        }
        catch
        {
            return false;
        }
    }

    public async Task<bool> UpdateApplicationStatusAsync(int applicationId, UpdateApplicationStatusDto dto, int workerId, string applicationType)
    {
        try
        {
            if (applicationType.ToLower() == "loan")
            {
                var application = await loanApplicationRepository.GetIdAsync(applicationId);
                if (application == null || application.AssignedWorkerId != workerId)
                    return false;

                var oldStatus = application.Status;
                application.Status = dto.Status;
                application.LastUpdatedDate = DateTime.UtcNow;

                if (dto.Status == ApplicationStatus.Rejected && !string.IsNullOrEmpty(dto.RejectionReason))
                {
                    application.RejectionReason = dto.RejectionReason;
                }

                await loanApplicationRepository.SaveChangesAsync();

                // Add history entry
                await AddLoanApplicationActionAsync(applicationId, workerId, dto.Status.ToString(), 
                    dto.Comments ?? $"Status changed from {oldStatus} to {dto.Status}");

                // Notify relevant parties
                await NotifyStatusChangedAsync(applicationId, "loan", dto.Status, workerId);
            }
            else if (applicationType.ToLower() == "deposit")
            {
                var application = await depositApplicationRepository.GetIdAsync(applicationId);
                if (application == null || application.AssignedWorkerId != workerId)
                    return false;

                var oldStatus = application.Status;
                application.Status = dto.Status;
                application.LastUpdatedDate = DateTime.UtcNow;

                if (dto.Status == ApplicationStatus.Rejected && !string.IsNullOrEmpty(dto.RejectionReason))
                {
                    application.RejectionReason = dto.RejectionReason;
                }

                await depositApplicationRepository.SaveChangesAsync();

                // Add history entry
                await AddDepositApplicationActionAsync(applicationId, workerId, dto.Status.ToString(), 
                    dto.Comments ?? $"Status changed from {oldStatus} to {dto.Status}");

                // Notify relevant parties
                await NotifyStatusChangedAsync(applicationId, "deposit", dto.Status, workerId);
            }

            return true;
        }
        catch
        {
            return false;
        }
    }

    public async Task<bool> EscalateApplicationAsync(EscalateApplicationDto dto, string applicationType)
    {
        try
        {
            if (applicationType.ToLower() == "loan")
            {
                var application = await loanApplicationRepository.GetIdAsync(dto.ApplicationId);
                if (application == null || application.AssignedWorkerId != dto.WorkerId)
                    return false;

                application.Status = ApplicationStatus.Escalated;
                application.LastUpdatedDate = DateTime.UtcNow;

                await loanApplicationRepository.SaveChangesAsync();

                await AddLoanApplicationActionAsync(dto.ApplicationId, dto.WorkerId, "Escalated", 
                    $"Escalated to boss: {dto.Reason}");

                // Notify boss about escalation
                await NotifyEscalatedToBossAsync(dto.ApplicationId, "loan", dto.Reason);
            }
            else if (applicationType.ToLower() == "deposit")
            {
                var application = await depositApplicationRepository.GetIdAsync(dto.ApplicationId);
                if (application == null || application.AssignedWorkerId != dto.WorkerId)
                    return false;

                application.Status = ApplicationStatus.Escalated;
                application.LastUpdatedDate = DateTime.UtcNow;

                await depositApplicationRepository.SaveChangesAsync();

                await AddDepositApplicationActionAsync(dto.ApplicationId, dto.WorkerId, "Escalated", 
                    $"Escalated to boss: {dto.Reason}");

                // Notify boss about escalation
                await NotifyEscalatedToBossAsync(dto.ApplicationId, "deposit", dto.Reason);
            }

            return true;
        }
        catch
        {
            return false;
        }
    }

    public async Task<bool> AddInternalNoteAsync(AddInternalNoteDto dto)
    {
        try
        {
            var note = new ApplicationInternalNote
            {
                ApplicationId = dto.ApplicationId,
                ApplicationType = dto.ApplicationId.ToString().StartsWith("1") ? "Loan" : "Deposit", // Simplified logic
                WorkerId = dto.WorkerId,
                Note = dto.Note,
                IsPrivate = dto.IsPrivate,
                NoteType = dto.NoteType ?? "General"
            };

            await internalNoteRepository.AddAsync(note);
            await internalNoteRepository.SaveChangesAsync();

            return true;
        }
        catch
        {
            return false;
        }
    }

    public async Task<List<InternalNoteDto>> GetInternalNotesAsync(int applicationId, string applicationType)
    {
        var notes = await internalNoteRepository.GetQuery()
            .Include(n => n.Worker)
            .Where(n => n.ApplicationId == applicationId && n.ApplicationType.ToLower() == applicationType.ToLower())
            .OrderByDescending(n => n.CreatedAt)
            .Select(n => new InternalNoteDto
            {
                Id = n.Id,
                ApplicationId = n.ApplicationId,
                Note = n.Note,
                WorkerName = n.Worker.Name,
                CreatedAt = n.CreatedAt,
                IsPrivate = n.IsPrivate,
                NoteType = n.NoteType
            })
            .ToListAsync();

        return notes;
    }

    public async Task<VerificationChecklistDto> GetVerificationChecklistAsync(int applicationId, string applicationType)
    {
        var checklistItems = await verificationChecklistRepository.GetQuery()
            .Include(vc => vc.CompletedByWorker)
            .Where(vc => vc.ApplicationId == applicationId && vc.ApplicationType.ToLower() == applicationType.ToLower())
            .OrderBy(vc => vc.Id)
            .Select(vc => new VerificationItemDto
            {
                Id = vc.Id,
                Title = vc.Title,
                Description = vc.Description,
                IsCompleted = vc.IsCompleted,
                IsMandatory = vc.IsMandatory,
                CompletedAt = vc.CompletedAt,
                CompletedBy = vc.CompletedByWorker != null ? vc.CompletedByWorker.Name : null,
                Comments = vc.Comments
            })
            .ToListAsync();

        return new VerificationChecklistDto
        {
            ApplicationId = applicationId,
            Checklist = checklistItems,
            AllMandatoryCompleted = checklistItems.Where(ci => ci.IsMandatory).All(ci => ci.IsCompleted),
            CompletedCount = checklistItems.Count(ci => ci.IsCompleted),
            TotalCount = checklistItems.Count,
            LastVerifiedAt = checklistItems.Where(ci => ci.IsCompleted).Max(ci => ci.CompletedAt),
            VerifiedBy = checklistItems.Where(ci => ci.IsCompleted).LastOrDefault()?.CompletedBy
        };
    }

    public async Task<bool> InitializeDefaultChecklistAsync(int applicationId, string applicationType)
    {
        try
        {
            var defaultItems = applicationType.ToLower() == "loan" 
                ? GetDefaultLoanChecklistItems()
                : GetDefaultDepositChecklistItems();

            foreach (var item in defaultItems)
            {
                var checklistItem = new VerificationChecklist
                {
                    ApplicationId = applicationId,
                    ApplicationType = applicationType,
                    Title = item.Title,
                    Description = item.Description,
                    IsMandatory = item.IsMandatory,
                    IsCompleted = false
                };

                await verificationChecklistRepository.AddAsync(checklistItem);
            }

            await verificationChecklistRepository.SaveChangesAsync();
            return true;
        }
        catch
        {
            return false;
        }
    }

    public async Task<List<ApplicationHistoryDto>> GetApplicationHistoryAsync(int applicationId, string applicationType)
    {
        if (applicationType.ToLower() == "loan")
        {
            return await loanActionRepository.GetQuery()
                .Include(la => la.User)
                .Where(la => la.LoanApplicationId == applicationId)
                .OrderByDescending(la => la.CreatedAt)
                .Select(la => new ApplicationHistoryDto
                {
                    Id = la.Id,
                    ApplicationId = la.LoanApplicationId,
                    Action = la.Action,
                    Description = la.Comments ?? la.Action,
                    PerformedBy = la.User.Name,
                    PerformedAt = la.CreatedAt,
                    Details = null
                })
                .ToListAsync();
        }
        else
        {
            return await depositActionRepository.GetQuery()
                .Include(da => da.User)
                .Where(da => da.DepositApplicationId == applicationId)
                .OrderByDescending(da => da.CreatedAt)
                .Select(da => new ApplicationHistoryDto
                {
                    Id = da.Id,
                    ApplicationId = da.DepositApplicationId,
                    Action = da.Action,
                    Description = da.Comments ?? da.Action,
                    PerformedBy = da.User.Name,
                    PerformedAt = da.CreatedAt,
                    Details = null
                })
                .ToListAsync();
        }
    }

    public async Task<EnhancedLoanApplicationDto?> GetEnhancedLoanApplicationAsync(int id, int currentUserId, string userRole)
    {
        var application = await loanApplicationRepository.GetQuery()
            .Include(l => l.AssignedWorker)
            .Include(l => l.Credit)
            .Include(l => l.Region)
            .Include(l => l.SelectedBanks)
            .Include(l => l.Actions)
                .ThenInclude(a => a.User)
            .FirstOrDefaultAsync(l => l.Id == id);

        if (application == null) return null;

        return new EnhancedLoanApplicationDto
        {
            Id = application.Id,
            ApplicantName = application.ApplicantName,
            RequestedAmount = application.RequestedAmount,
            Status = application.Status,
            ApplicationDate = application.ApplicationDate,
            LastUpdatedDate = application.LastUpdatedDate,
            AssignedWorkerId = application.AssignedWorkerId,
            AssignedWorkerName = application.AssignedWorker?.Name,
            ClaimedAt = application.Actions.FirstOrDefault(a => a.Action == "Claimed")?.CreatedAt,
            InternalNotes = await GetInternalNotesAsync(id, "loan"),
            History = await GetApplicationHistoryAsync(id, "loan"),
            VerificationChecklist = await GetVerificationChecklistAsync(id, "loan"),
            RegionName = application.Region.Name,
            CreditName = application.Credit.Name,
            SelectedBankNames = application.SelectedBanks.Select(b => b.Name).ToList()
        };
    }

    public async Task<EnhancedDepositApplicationDto?> GetEnhancedDepositApplicationAsync(int id, int currentUserId, string userRole)
    {
        var application = await depositApplicationRepository.GetQuery()
            .Include(d => d.AssignedWorker)
            .Include(d => d.Deposit)
            .Include(d => d.Region)
            .Include(d => d.SelectedBanks)
            .Include(d => d.Actions)
                .ThenInclude(a => a.User)
            .FirstOrDefaultAsync(d => d.Id == id);

        if (application == null) return null;

        return new EnhancedDepositApplicationDto
        {
            Id = application.Id,
            ApplicantName = application.ApplicantName,
            DepositAmount = application.DepositAmount,
            Status = application.Status,
            ApplicationDate = application.ApplicationDate,
            LastUpdatedDate = application.LastUpdatedDate,
            AssignedWorkerId = application.AssignedWorkerId,
            AssignedWorkerName = application.AssignedWorker?.Name,
            ClaimedAt = application.Actions.FirstOrDefault(a => a.Action == "Claimed")?.CreatedAt,
            InternalNotes = await GetInternalNotesAsync(id, "deposit"),
            History = await GetApplicationHistoryAsync(id, "deposit"),
            VerificationChecklist = await GetVerificationChecklistAsync(id, "deposit"),
            RegionName = application.Region.Name,
            DepositName = application.Deposit.Name,
            SelectedBankNames = application.SelectedBanks.Select(b => b.Name).ToList()
        };
    }

    // Private helper methods
    private async Task AddLoanApplicationActionAsync(int applicationId, int userId, string action, string comments)
    {
        var actionEntry = new LoanApplicationAction
        {
            LoanApplicationId = applicationId,
            UserId = userId,
            Action = action,
            Comments = comments,
            CreatedAt = DateTime.UtcNow
        };

        await loanActionRepository.AddAsync(actionEntry);
        await loanActionRepository.SaveChangesAsync();
    }

    private async Task AddDepositApplicationActionAsync(int applicationId, int userId, string action, string comments)
    {
        var actionEntry = new DepositApplicationAction
        {
            DepositApplicationId = applicationId,
            UserId = userId,
            Action = action,
            Comments = comments,
            CreatedAt = DateTime.UtcNow
        };

        await depositActionRepository.AddAsync(actionEntry);
        await depositActionRepository.SaveChangesAsync();
    }

    private async Task NotifyApplicationLockedAsync(int applicationId, string applicationType, int workerId)
    {
        // SignalR notification to other workers that this application is locked
        var worker = await userRepository.GetIdAsync(workerId);
        var message = $"Application {applicationId} ({applicationType}) claimed by {worker?.Name}";
        
        // This would send to all workers except the one who claimed it
        // Implementation depends on your SignalR setup
    }

    private async Task NotifyApplicationReleasedAsync(int applicationId, string applicationType)
    {
        // SignalR notification that this application is now available
        var message = $"Application {applicationId} ({applicationType}) is now available";
        
        // This would send to all eligible workers
        // Implementation depends on your SignalR setup
    }

    private async Task NotifyStatusChangedAsync(int applicationId, string applicationType, ApplicationStatus status, int workerId)
    {
        // SignalR notification about status change
        var message = $"Application {applicationId} ({applicationType}) status changed to {status}";
        
        // This would send to relevant parties
        // Implementation depends on your SignalR setup
    }

    private async Task NotifyEscalatedToBossAsync(int applicationId, string applicationType, string reason)
    {
        // SignalR notification to boss about escalation
        var message = $"Application {applicationId} ({applicationType}) escalated: {reason}";
        
        // This would send specifically to boss users
        // Implementation depends on your SignalR setup
    }

    private List<(string Title, string Description, bool IsMandatory)> GetDefaultLoanChecklistItems()
    {
        return new List<(string, string, bool)>
        {
            ("Identity Verified", "Client identity documents verified and match application details", true),
            ("Income Matches Request", "Client's declared income matches requested loan amount", true),
            ("No Existing Bad Debts", "Client has no history of bad debts or defaults", true),
            ("Employment Status Verified", "Client's employment status and stability confirmed", true),
            ("Regional Compliance", "Application complies with regional lending requirements", true),
            ("Credit History Check", "Client's credit history reviewed and acceptable", false),
            ("Collateral Assessment", "Collateral (if any) assessed and valued", false)
        };
    }

    private List<(string Title, string Description, bool IsMandatory)> GetDefaultDepositChecklistItems()
    {
        return new List<(string, string, bool)>
        {
            ("Identity Verified", "Client identity documents verified and match application details", true),
            ("Source of Funds", "Source of deposit funds verified and legitimate", true),
            ("Account Compliance", "Account opening requirements met", true),
            ("Regional Compliance", "Application complies with regional banking requirements", true),
            ("Risk Assessment", "Money laundering and terrorism financing risk assessed", false),
            ("Expected Activity", "Expected account activity level is reasonable", false)
        };
    }

    // Implementation of remaining interface methods
    public async Task<bool> UpdateInternalNoteAsync(int noteId, string noteContent, int workerId)
    {
        try
        {
            var note = await internalNoteRepository.GetQuery()
                .FirstOrDefaultAsync(n => n.Id == noteId && n.WorkerId == workerId);

            if (note == null) return false;

            note.Note = noteContent;
            note.UpdatedAt = DateTime.UtcNow;

            await internalNoteRepository.SaveChangesAsync();
            return true;
        }
        catch
        {
            return false;
        }
    }

    public async Task<bool> DeleteInternalNoteAsync(int noteId, int workerId)
    {
        try
        {
            var note = await internalNoteRepository.GetQuery()
                .FirstOrDefaultAsync(n => n.Id == noteId && n.WorkerId == workerId);

            if (note == null) return false;

            internalNoteRepository.Remove(note);
            await internalNoteRepository.SaveChangesAsync();
            return true;
        }
        catch
        {
            return false;
        }
    }

    public async Task<bool> UpdateVerificationItemAsync(int checklistItemId, bool isCompleted, int workerId, string? comments = null)
    {
        try
        {
            var item = await verificationChecklistRepository.GetIdAsync(checklistItemId);
            if (item == null) return false;

            item.IsCompleted = isCompleted;
            item.CompletedByWorkerId = isCompleted ? workerId : null;
            item.CompletedAt = isCompleted ? DateTime.UtcNow : null;
            item.Comments = comments;
            item.UpdatedAt = DateTime.UtcNow;

            await verificationChecklistRepository.SaveChangesAsync();
            return true;
        }
        catch
        {
            return false;
        }
    }

    public async Task<bool> AddHistoryEntryAsync(int applicationId, string applicationType, string action, string description, int performedBy, string? details = null)
    {
        if (applicationType.ToLower() == "loan")
        {
            await AddLoanApplicationActionAsync(applicationId, performedBy, action, description);
        }
        else
        {
            await AddDepositApplicationActionAsync(applicationId, performedBy, action, description);
        }

        return true;
    }
}
