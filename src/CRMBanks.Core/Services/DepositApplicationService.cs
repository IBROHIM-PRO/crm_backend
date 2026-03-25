using CRMBanks.Core.Common;
using CRMBanks.Core.Dtos;
using CRMBanks.Core.Entities;
using CRMBanks.Core.Services.Interfaces;
using CRMBanks.SharedKernel.Enums;
using Microsoft.EntityFrameworkCore;

namespace CRMBanks.Core.Services;

public class DepositApplicationService(
    IRepository<DepositApplication> depositApplicationRepository,
    IRepository<DepositApplicationAction> actionRepository,
    IRepository<User> userRepository,
    IRepository<Bank> bankRepository,
    IRepository<Deposit> depositRepository) : IDepositApplicationService
{
    public async Task<PagedDepositApplicationDto> GetFilteredAsync(DepositApplicationFilterDto filter, int currentUserId, string userRole)
    {
        var query = depositApplicationRepository.GetQuery()
            .Include(d => d.Deposit)
            .Include(d => d.Region)
            .Include(d => d.SelectedBanks)
            .Include(d => d.Actions)
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
                query = query.Where(d => 
                    workerRegionIds.Contains(d.RegionId) && 
                    d.DepositAmount <= worker.MaxLoanAmount);
            }
        }
        else if (userRole == "client")
        {
            var user = await userRepository.GetIdAsync(currentUserId);
            if (user != null)
            {
                query = query.Where(d => d.ApplicantEmail.Contains(user.Email));
            }
        }

        // Apply additional filters
        if (filter.RegionId.HasValue)
            query = query.Where(d => d.RegionId == filter.RegionId);

        if (filter.MaxAmount.HasValue)
            query = query.Where(d => d.DepositAmount <= filter.MaxAmount);

        if (filter.Status.HasValue)
            query = query.Where(d => d.Status == filter.Status);

        if (filter.UserId.HasValue)
            query = query.Where(d => d.Id == filter.UserId);

        var totalCount = await query.CountAsync();
        var items = await query
            .OrderByDescending(d => d.ApplicationDate)
            .Skip((filter.Page - 1) * filter.PageSize)
            .Take(filter.PageSize)
            .Select(d => new DepositApplicationDto
            {
                Id = d.Id,
                ApplicantName = d.ApplicantName,
                ApplicantPhone = d.ApplicantPhone,
                ApplicantEmail = d.ApplicantEmail,
                DepositId = d.DepositId,
                DepositName = d.Deposit.Name,
                DepositAmount = d.DepositAmount,
                DepositTermMonths = d.DepositTermMonths,
                RegionId = d.RegionId,
                RegionName = d.Region.Name,
                Status = d.Status,
                RejectionReason = d.RejectionReason,
                ApplicationDate = d.ApplicationDate,
                LastUpdatedDate = d.LastUpdatedDate,
                SendToAllBanks = d.SendToAllBanks,
                SelectedBankNames = d.SelectedBanks.Select(b => b.Name).ToList(),
                Actions = d.Actions.Select(a => new DepositApplicationActionDto
                {
                    Id = a.Id,
                    DepositApplicationId = a.DepositApplicationId,
                    UserName = a.User.Name,
                    Action = a.Action,
                    Comments = a.Comments,
                    CreatedAt = a.CreatedAt
                }).ToList()
            })
            .ToListAsync();

        return new PagedDepositApplicationDto
        {
            Items = items,
            TotalCount = totalCount,
            Page = filter.Page,
            PageSize = filter.PageSize,
            TotalPages = (int)Math.Ceiling((double)totalCount / filter.PageSize)
        };
    }

    public async Task<DepositApplicationDto?> GetByIdAsync(int id)
    {
        var depositApplication = await depositApplicationRepository.GetQuery()
            .Include(d => d.Deposit)
            .Include(d => d.Region)
            .Include(d => d.SelectedBanks)
            .Include(d => d.Actions)
                .ThenInclude(a => a.User)
            .FirstOrDefaultAsync(d => d.Id == id);

        if (depositApplication == null) return null;

        return new DepositApplicationDto
        {
            Id = depositApplication.Id,
            ApplicantName = depositApplication.ApplicantName,
            ApplicantPhone = depositApplication.ApplicantPhone,
            ApplicantEmail = depositApplication.ApplicantEmail,
            DepositId = depositApplication.DepositId,
            DepositName = depositApplication.Deposit.Name,
            DepositAmount = depositApplication.DepositAmount,
            DepositTermMonths = depositApplication.DepositTermMonths,
            RegionId = depositApplication.RegionId,
            RegionName = depositApplication.Region.Name,
            Status = depositApplication.Status,
            RejectionReason = depositApplication.RejectionReason,
            ApplicationDate = depositApplication.ApplicationDate,
            LastUpdatedDate = depositApplication.LastUpdatedDate,
            SendToAllBanks = depositApplication.SendToAllBanks,
            SelectedBankNames = depositApplication.SelectedBanks.Select(b => b.Name).ToList(),
            Actions = depositApplication.Actions.Select(a => new DepositApplicationActionDto
            {
                Id = a.Id,
                DepositApplicationId = a.DepositApplicationId,
                UserName = a.User.Name,
                Action = a.Action,
                Comments = a.Comments,
                CreatedAt = a.CreatedAt
            }).ToList()
        };
    }

    public async Task<DepositApplicationDto> CreateAsync(CreateDepositApplicationDto dto, int userId)
    {
        var depositApplication = new DepositApplication
        {
            ApplicantName = dto.ApplicantName,
            ApplicantPhone = dto.ApplicantPhone,
            ApplicantEmail = dto.ApplicantEmail,
            DepositId = dto.DepositId,
            DepositAmount = dto.DepositAmount,
            DepositTermMonths = dto.DepositTermMonths,
            RegionId = dto.RegionId,
            SendToAllBanks = dto.SendToAllBanks,
            Status = ApplicationStatus.Pending
        };

        // Handle bank selection
        if (dto.SendToAllBanks)
        {
            var allBanks = await bankRepository.GetAllAsync();
            depositApplication.SelectedBanks = allBanks.ToList();
        }
        else
        {
            var selectedBanks = await bankRepository.GetQuery()
                .Where(b => dto.SelectedBankIds.Contains(b.Id))
                .ToListAsync();
            depositApplication.SelectedBanks = selectedBanks;
        }

        await depositApplicationRepository.AddAsync(depositApplication);
        await depositApplicationRepository.SaveChangesAsync();

        // Add initial action
        await actionRepository.AddAsync(new DepositApplicationAction
        {
            DepositApplicationId = depositApplication.Id,
            UserId = userId,
            Action = "Created",
            Comments = "Deposit application created"
        });
        await depositApplicationRepository.SaveChangesAsync();

        // Get the created application and send notifications
        var createdApplication = await GetByIdAsync(depositApplication.Id);
        if (createdApplication != null)
        {
            await NotifyEligibleWorkersAsync(createdApplication);
        }

        return createdApplication ?? throw new InvalidOperationException("Failed to retrieve created deposit application");
    }

    private async Task NotifyEligibleWorkersAsync(DepositApplicationDto application)
    {
        // TODO: Send real-time notifications to eligible workers
        // await realTimeNotificationService.NotifyEligibleWorkersForNewDepositApplicationAsync(application);
    }

    public async Task<bool> UpdateStatusAsync(int id, UpdateDepositApplicationStatusDto dto, int userId)
    {
        var depositApplication = await depositApplicationRepository.GetIdAsync(id);
        if (depositApplication == null) return false;

        depositApplication.Status = dto.Status;
        depositApplication.LastUpdatedDate = DateTime.UtcNow;

        if (dto.Status == ApplicationStatus.Rejected && !string.IsNullOrEmpty(dto.Comments))
        {
            depositApplication.RejectionReason = dto.Comments;
        }

        depositApplicationRepository.Update(depositApplication);
        await depositApplicationRepository.SaveChangesAsync();

        // Add action record
        await actionRepository.AddAsync(new DepositApplicationAction
        {
            DepositApplicationId = id,
            UserId = userId,
            Action = $"Status changed to {dto.Status}",
            Comments = dto.Comments
        });
        await depositApplicationRepository.SaveChangesAsync();

        return true;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var depositApplication = await depositApplicationRepository.GetIdAsync(id);
        if (depositApplication == null) return false;

        depositApplicationRepository.Remove(depositApplication);
        await depositApplicationRepository.SaveChangesAsync();
        return true;
    }

    public async Task<List<DepositApplicationDto>> GetAvailableForWorkerAsync(int workerId)
    {
        var worker = await userRepository.GetQuery()
            .Include(u => u.Regions)
            .FirstOrDefaultAsync(u => u.Id == workerId);

        if (worker == null) return new List<DepositApplicationDto>();

        var workerRegionIds = worker.Regions.Select(r => r.Id).ToList();

        var applications = await depositApplicationRepository.GetQuery()
            .Include(d => d.Deposit)
            .Include(d => d.Region)
            .Include(d => d.SelectedBanks)
            .Where(d => 
                workerRegionIds.Contains(d.RegionId) && 
                d.DepositAmount <= worker.MaxLoanAmount &&
                d.Status == ApplicationStatus.Pending)
            .Select(d => new DepositApplicationDto
            {
                Id = d.Id,
                ApplicantName = d.ApplicantName,
                ApplicantPhone = d.ApplicantPhone,
                ApplicantEmail = d.ApplicantEmail,
                DepositId = d.DepositId,
                DepositName = d.Deposit.Name,
                DepositAmount = d.DepositAmount,
                DepositTermMonths = d.DepositTermMonths,
                RegionId = d.RegionId,
                RegionName = d.Region.Name,
                Status = d.Status,
                ApplicationDate = d.ApplicationDate,
                SendToAllBanks = d.SendToAllBanks,
                SelectedBankNames = d.SelectedBanks.Select(b => b.Name).ToList()
            })
            .ToListAsync();

        return applications;
    }

    public async Task<List<DepositApplicationDto>> GetAllForBossAsync()
    {
        var applications = await depositApplicationRepository.GetQuery()
            .Include(d => d.Deposit)
            .Include(d => d.Region)
            .Include(d => d.SelectedBanks)
            .OrderByDescending(d => d.ApplicationDate)
            .Select(d => new DepositApplicationDto
            {
                Id = d.Id,
                ApplicantName = d.ApplicantName,
                ApplicantPhone = d.ApplicantPhone,
                ApplicantEmail = d.ApplicantEmail,
                DepositId = d.DepositId,
                DepositName = d.Deposit.Name,
                DepositAmount = d.DepositAmount,
                DepositTermMonths = d.DepositTermMonths,
                RegionId = d.RegionId,
                RegionName = d.Region.Name,
                Status = d.Status,
                ApplicationDate = d.ApplicationDate,
                SendToAllBanks = d.SendToAllBanks,
                SelectedBankNames = d.SelectedBanks.Select(b => b.Name).ToList()
            })
            .ToListAsync();

        return applications;
    }
}
