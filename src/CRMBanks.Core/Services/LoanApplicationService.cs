using CRMBanks.Core.Common;
using CRMBanks.Core.Dtos;
using CRMBanks.Core.Entities;
using CRMBanks.Core.Services.Interfaces;
using CRMBanks.SharedKernel.Enums;
using Microsoft.EntityFrameworkCore;

namespace CRMBanks.Core.Services;

public class LoanApplicationService(
    IRepository<LoanApplication> loanApplicationRepository,
    IRepository<LoanApplicationAction> actionRepository,
    IRepository<User> userRepository,
    IRepository<Bank> bankRepository,
    IRepository<Region> regionRepository) : ILoanApplicationService
{
    public async Task<PagedLoanApplicationDto> GetFilteredAsync(LoanApplicationFilterDto filter, int currentUserId, string userRole)
    {
        var query = loanApplicationRepository.GetQuery()
            .Include(l => l.User)
            .Include(l => l.Credit)
            .Include(l => l.Region)
            .Include(l => l.SelectedBanks)
            .Include(l => l.AssignedWorker)
            .Include(l => l.Actions)
                .ThenInclude(a => a.User)
            .AsQueryable();

        // Apply role-based filtering
        if (userRole == "worker")
        {
            var worker = await userRepository.GetQuery()
                .Include(u => u.Regions)
                .FirstOrDefaultAsync(u => u.Id == currentUserId);
            if (worker != null)
            {
                var workerRegionIds = worker.Regions.Select(r => r.Id).ToList();
                query = query.Where(l => 
                    workerRegionIds.Contains(l.RegionId) && 
                    l.RequestedAmount <= worker.MaxLoanAmount);
            }
        }
        else if (userRole == "client")
        {
            query = query.Where(l => l.UserId == currentUserId);
        }

        // Apply additional filters
        if (filter.RegionId.HasValue)
            query = query.Where(l => l.RegionId == filter.RegionId);

        if (filter.MaxAmount.HasValue)
            query = query.Where(l => l.RequestedAmount <= filter.MaxAmount);

        if (filter.Status.HasValue)
            query = query.Where(l => l.Status == filter.Status);

        if (filter.UserId.HasValue)
            query = query.Where(l => l.UserId == filter.UserId);

        var totalCount = await query.CountAsync();
        var items = await query
            .OrderByDescending(l => l.ApplicationDate)
            .Skip((filter.Page - 1) * filter.PageSize)
            .Take(filter.PageSize)
            .Select(l => new LoanApplicationDto
            {
                Id = l.Id,
                ApplicantName = l.ApplicantName,
                ApplicantPhone = l.ApplicantPhone,
                ApplicantEmail = l.ApplicantEmail,
                UserId = l.UserId,
                UserName = l.User.Name,
                CreditId = l.CreditId,
                CreditName = l.Credit.Name,
                RequestedAmount = l.RequestedAmount,
                RequestedTermMonths = l.RequestedTermMonths,
                RegionId = l.RegionId,
                RegionName = l.Region.Name,
                ApplicationPurpose = l.ApplicationPurpose,
                MonthlyIncome = l.MonthlyIncome,
                EmploymentStatus = l.EmploymentStatus,
                Status = l.Status,
                RejectionReason = l.RejectionReason,
                ApplicationDate = l.ApplicationDate,
                LastUpdatedDate = l.LastUpdatedDate,
                SendToAllBanks = l.SendToAllBanks,
                SelectedBankNames = l.SelectedBanks.Select(b => b.Name).ToList(),
                
                // Workflow fields
                AssignedWorkerId = l.AssignedWorkerId,
                AssignedWorkerName = l.AssignedWorker != null ? l.AssignedWorker.Name : null,
                ClaimedAt = l.Actions.FirstOrDefault(a => a.Action == "Claimed") != null ? 
                    l.Actions.FirstOrDefault(a => a.Action == "Claimed").CreatedAt : (DateTime?)null,
                
                Actions = l.Actions.Select(a => new LoanApplicationActionDto
                {
                    Id = a.Id,
                    LoanApplicationId = a.LoanApplicationId,
                    UserName = a.User.Name,
                    Action = a.Action,
                    Comments = a.Comments,
                    CreatedAt = a.CreatedAt
                }).ToList()
            })
            .ToListAsync();

        return new PagedLoanApplicationDto
        {
            Items = items,
            TotalCount = totalCount,
            Page = filter.Page,
            PageSize = filter.PageSize,
            TotalPages = (int)Math.Ceiling((double)totalCount / filter.PageSize)
        };
    }

    public async Task<LoanApplicationDto?> GetByIdAsync(int id)
    {
        var loanApplication = await loanApplicationRepository.GetQuery()
            .Include(l => l.User)
            .Include(l => l.Credit)
            .Include(l => l.Region)
            .Include(l => l.SelectedBanks)
            .Include(l => l.Actions)
                .ThenInclude(a => a.User)
            .FirstOrDefaultAsync(l => l.Id == id);

        if (loanApplication == null) return null;

        return new LoanApplicationDto
        {
            Id = loanApplication.Id,
            ApplicantName = loanApplication.ApplicantName,
            ApplicantPhone = loanApplication.ApplicantPhone,
            ApplicantEmail = loanApplication.ApplicantEmail,
            UserId = loanApplication.UserId,
            UserName = loanApplication.User.Name,
            CreditId = loanApplication.CreditId,
            CreditName = loanApplication.Credit.Name,
            RequestedAmount = loanApplication.RequestedAmount,
            RequestedTermMonths = loanApplication.RequestedTermMonths,
            RegionId = loanApplication.RegionId,
            RegionName = loanApplication.Region.Name,
            ApplicationPurpose = loanApplication.ApplicationPurpose,
            MonthlyIncome = loanApplication.MonthlyIncome,
            EmploymentStatus = loanApplication.EmploymentStatus,
            Status = loanApplication.Status,
            RejectionReason = loanApplication.RejectionReason,
            ApplicationDate = loanApplication.ApplicationDate,
            LastUpdatedDate = loanApplication.LastUpdatedDate,
            SendToAllBanks = loanApplication.SendToAllBanks,
            SelectedBankNames = loanApplication.SelectedBanks.Select(b => b.Name).ToList(),
            Actions = loanApplication.Actions.Select(a => new LoanApplicationActionDto
            {
                Id = a.Id,
                LoanApplicationId = a.LoanApplicationId,
                UserName = a.User.Name,
                Action = a.Action,
                Comments = a.Comments,
                CreatedAt = a.CreatedAt
            }).ToList()
        };
    }

    public async Task<LoanApplicationDto> CreateAsync(CreateLoanApplicationDto dto, int userId)
    {
        var loanApplication = new LoanApplication
        {
            ApplicantName = dto.ApplicantName,
            ApplicantPhone = dto.ApplicantPhone,
            ApplicantEmail = dto.ApplicantEmail,
            UserId = userId,
            CreditId = dto.CreditId,
            RequestedAmount = dto.RequestedAmount,
            RequestedTermMonths = dto.RequestedTermMonths,
            RegionId = dto.RegionId,
            ApplicationPurpose = dto.ApplicationPurpose,
            MonthlyIncome = dto.MonthlyIncome,
            EmploymentStatus = dto.EmploymentStatus,
            SendToAllBanks = dto.SendToAllBanks,
            Status = ApplicationStatus.Pending
        };

        // Handle bank selection
        if (dto.SendToAllBanks)
        {
            var allBanks = await bankRepository.GetAllAsync();
            loanApplication.SelectedBanks = allBanks.ToList();
        }
        else
        {
            var selectedBanks = await bankRepository.GetQuery()
                .Where(b => dto.SelectedBankIds.Contains(b.Id))
                .ToListAsync();
            loanApplication.SelectedBanks = selectedBanks;
        }

        await loanApplicationRepository.AddAsync(loanApplication);
        await loanApplicationRepository.SaveChangesAsync();

        // Add initial action
        await actionRepository.AddAsync(new LoanApplicationAction
        {
            LoanApplicationId = loanApplication.Id,
            UserId = userId,
            Action = "Created",
            Comments = "Loan application created"
        });
        await loanApplicationRepository.SaveChangesAsync();

        // Get the created application and send notifications
        var createdApplication = await GetByIdAsync(loanApplication.Id);
        if (createdApplication != null)
        {
            await NotifyEligibleWorkersAsync(createdApplication);
        }

        return createdApplication ?? throw new InvalidOperationException("Failed to retrieve created loan application");
    }

    private async Task NotifyEligibleWorkersAsync(LoanApplicationDto application)
    {
        // TODO: Send real-time notifications to eligible workers using the enhanced notification service
        // await notificationService.CreateForLoanApplicationAsync(application.Id);
        
        // For now, we'll handle notifications in the ApplicationWorkflowService
    }

    public async Task<bool> UpdateStatusAsync(int id, UpdateLoanApplicationStatusDto dto, int userId)
    {
        var loanApplication = await loanApplicationRepository.GetIdAsync(id);
        if (loanApplication == null) return false;

        loanApplication.Status = dto.Status;
        loanApplication.LastUpdatedDate = DateTime.UtcNow;

        if (dto.Status == ApplicationStatus.Rejected && !string.IsNullOrEmpty(dto.Comments))
        {
            loanApplication.RejectionReason = dto.Comments;
        }

        loanApplicationRepository.Update(loanApplication);
        await loanApplicationRepository.SaveChangesAsync();

        // Add action record
        await actionRepository.AddAsync(new LoanApplicationAction
        {
            LoanApplicationId = id,
            UserId = userId,
            Action = $"Status changed to {dto.Status}",
            Comments = dto.Comments
        });
        await loanApplicationRepository.SaveChangesAsync();

        return true;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var loanApplication = await loanApplicationRepository.GetIdAsync(id);
        if (loanApplication == null) return false;

        loanApplicationRepository.Remove(loanApplication);
        await loanApplicationRepository.SaveChangesAsync();
        return true;
    }

    public async Task<List<LoanApplicationDto>> GetAvailableForWorkerAsync(int workerId)
    {
        var worker = await userRepository.GetQuery()
            .Include(u => u.Regions)
            .FirstOrDefaultAsync(u => u.Id == workerId);

        if (worker == null) return new List<LoanApplicationDto>();

        var workerRegionIds = worker.Regions.Select(r => r.Id).ToList();

        var applications = await loanApplicationRepository.GetQuery()
            .Include(l => l.User)
            .Include(l => l.Credit)
            .Include(l => l.Region)
            .Include(l => l.SelectedBanks)
            .Where(l => 
                workerRegionIds.Contains(l.RegionId) && 
                l.RequestedAmount <= worker.MaxLoanAmount &&
                l.Status == ApplicationStatus.Pending)
            .Select(l => new LoanApplicationDto
            {
                Id = l.Id,
                ApplicantName = l.ApplicantName,
                ApplicantPhone = l.ApplicantPhone,
                ApplicantEmail = l.ApplicantEmail,
                UserId = l.UserId,
                UserName = l.User.Name,
                CreditId = l.CreditId,
                CreditName = l.Credit.Name,
                RequestedAmount = l.RequestedAmount,
                RequestedTermMonths = l.RequestedTermMonths,
                RegionId = l.RegionId,
                RegionName = l.Region.Name,
                ApplicationPurpose = l.ApplicationPurpose,
                MonthlyIncome = l.MonthlyIncome,
                EmploymentStatus = l.EmploymentStatus,
                Status = l.Status,
                ApplicationDate = l.ApplicationDate,
                SendToAllBanks = l.SendToAllBanks,
                SelectedBankNames = l.SelectedBanks.Select(b => b.Name).ToList()
            })
            .ToListAsync();

        return applications;
    }

    public async Task<List<LoanApplicationDto>> GetAllForBossAsync()
    {
        var applications = await loanApplicationRepository.GetQuery()
            .Include(l => l.User)
            .Include(l => l.Credit)
            .Include(l => l.Region)
            .Include(l => l.SelectedBanks)
            .OrderByDescending(l => l.ApplicationDate)
            .Select(l => new LoanApplicationDto
            {
                Id = l.Id,
                ApplicantName = l.ApplicantName,
                ApplicantPhone = l.ApplicantPhone,
                ApplicantEmail = l.ApplicantEmail,
                UserId = l.UserId,
                UserName = l.User.Name,
                CreditId = l.CreditId,
                CreditName = l.Credit.Name,
                RequestedAmount = l.RequestedAmount,
                RequestedTermMonths = l.RequestedTermMonths,
                RegionId = l.RegionId,
                RegionName = l.Region.Name,
                ApplicationPurpose = l.ApplicationPurpose,
                MonthlyIncome = l.MonthlyIncome,
                EmploymentStatus = l.EmploymentStatus,
                Status = l.Status,
                ApplicationDate = l.ApplicationDate,
                SendToAllBanks = l.SendToAllBanks,
                SelectedBankNames = l.SelectedBanks.Select(b => b.Name).ToList()
            })
            .ToListAsync();

        return applications;
    }
}
