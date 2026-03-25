using CRMBanks.Core.Common;
using CRMBanks.Core.Dtos;
using CRMBanks.Core.Entities;
using CRMBanks.Core.Services.Interfaces;
using CRMBanks.SharedKernel.Enums;
using Microsoft.EntityFrameworkCore;

namespace CRMBanks.Core.Services;

public class StatisticsService(
    IRepository<LoanApplication> loanApplicationRepository,
    IRepository<LoanApplicationAction> actionRepository,
    IRepository<User> userRepository,
    IRepository<Region> regionRepository) : IStatisticsService
{
    public async Task<GlobalStatisticsDto> GetGlobalStatisticsAsync()
    {
        var applications = await loanApplicationRepository.GetAllAsync();
        var totalApplications = applications.Count;
        var pendingApplications = applications.Count(a => a.Status == ApplicationStatus.Pending);
        var approvedApplications = applications.Count(a => a.Status == ApplicationStatus.Approved);
        var rejectedApplications = applications.Count(a => a.Status == ApplicationStatus.Rejected);
        var totalRequestedAmount = applications.Sum(a => a.RequestedAmount);
        var averageLoanAmount = totalApplications > 0 ? totalRequestedAmount / totalApplications : 0;

        // Get worker performances
        var workers = await userRepository.GetQuery()
            .Include(u => u.Regions)
            .Where(u => u.Role != null && u.Role.Name == "worker")
            .ToListAsync();

        var workerPerformances = new List<WorkerPerformanceDto>();
        foreach (var worker in workers)
        {
            var workerRegionIds = worker.Regions.Select(r => r.Id).ToList();
            var workerApplications = applications.Where(a => 
                workerRegionIds.Contains(a.RegionId) && 
                a.RequestedAmount <= worker.MaxLoanAmount).ToList();

            var processedApplications = workerApplications.Where(a => 
                a.Status != ApplicationStatus.Pending).ToList();

            var workerActions = await actionRepository.GetQuery()
                .Include(a => a.LoanApplication)
                .Where(a => a.UserId == worker.Id)
                .ToListAsync();

            var approvedCount = processedApplications.Count(a => a.Status == ApplicationStatus.Approved);
            var rejectedCount = processedApplications.Count(a => a.Status == ApplicationStatus.Rejected);

            workerPerformances.Add(new WorkerPerformanceDto
            {
                WorkerId = worker.Id,
                WorkerName = worker.Name,
                ApplicationsProcessed = processedApplications.Count,
                ApplicationsApproved = approvedCount,
                ApplicationsRejected = rejectedCount,
                TotalAmountProcessed = processedApplications.Sum(a => a.RequestedAmount),
                AverageProcessingTimeHours = CalculateAverageProcessingTime(processedApplications)
            });
        }

        // Get region statistics
        var regions = await regionRepository.GetAllAsync();
        var regionStatistics = regions.Select(region => new RegionStatisticsDto
        {
            RegionId = region.Id,
            RegionName = region.Name,
            ApplicationCount = applications.Count(a => a.RegionId == region.Id),
            TotalRequestedAmount = applications.Where(a => a.RegionId == region.Id).Sum(a => a.RequestedAmount),
            ApprovalCount = applications.Count(a => a.RegionId == region.Id && a.Status == ApplicationStatus.Approved),
            ApprovalRate = applications.Any(a => a.RegionId == region.Id) 
                ? (decimal)applications.Count(a => a.RegionId == region.Id && a.Status == ApplicationStatus.Approved) 
                  / applications.Count(a => a.RegionId == region.Id) * 100
                : 0
        }).ToList();

        // Get monthly statistics for the last 12 months
        var monthlyStatistics = new List<MonthlyStatisticsDto>();
        var currentDate = DateTime.UtcNow;
        for (int i = 11; i >= 0; i--)
        {
            var monthDate = currentDate.AddMonths(-i);
            var monthApplications = applications.Where(a => 
                a.ApplicationDate.Year == monthDate.Year && 
                a.ApplicationDate.Month == monthDate.Month).ToList();

            monthlyStatistics.Add(new MonthlyStatisticsDto
            {
                Year = monthDate.Year,
                Month = monthDate.Month,
                MonthName = monthDate.ToString("MMM yyyy"),
                ApplicationCount = monthApplications.Count,
                TotalAmount = monthApplications.Sum(a => a.RequestedAmount),
                ApprovalCount = monthApplications.Count(a => a.Status == ApplicationStatus.Approved)
            });
        }

        return new GlobalStatisticsDto
        {
            TotalApplications = totalApplications,
            PendingApplications = pendingApplications,
            ApprovedApplications = approvedApplications,
            RejectedApplications = rejectedApplications,
            TotalRequestedAmount = totalRequestedAmount,
            AverageLoanAmount = averageLoanAmount,
            WorkerPerformances = workerPerformances,
            RegionStatistics = regionStatistics,
            MonthlyStatistics = monthlyStatistics
        };
    }

    public async Task<PersonalStatisticsDto> GetPersonalStatisticsAsync(int userId)
    {
        var user = await userRepository.GetQuery()
            .Include(u => u.Regions)
            .FirstOrDefaultAsync(u => u.Id == userId);

        if (user == null) return new PersonalStatisticsDto();

        var applications = await loanApplicationRepository.GetAllAsync();
        var userRegionIds = user.Regions.Select(r => r.Id).ToList();
        var userApplications = applications.Where(a => 
            userRegionIds.Contains(a.RegionId) && 
            a.RequestedAmount <= user.MaxLoanAmount).ToList();

        var processedApplications = userApplications.Where(a => 
            a.Status != ApplicationStatus.Pending).ToList();

        var approvedCount = processedApplications.Count(a => a.Status == ApplicationStatus.Approved);
        var rejectedCount = processedApplications.Count(a => a.Status == ApplicationStatus.Rejected);
        var pendingCount = userApplications.Count(a => a.Status == ApplicationStatus.Pending);

        // Get monthly performance for the last 12 months
        var monthlyPerformance = new List<MonthlyPerformanceDto>();
        var currentDate = DateTime.UtcNow;
        for (int i = 11; i >= 0; i--)
        {
            var monthDate = currentDate.AddMonths(-i);
            var monthApplications = processedApplications.Where(a => 
                a.LastUpdatedDate.HasValue &&
                a.LastUpdatedDate.Value.Year == monthDate.Year && 
                a.LastUpdatedDate.Value.Month == monthDate.Month).ToList();

            monthlyPerformance.Add(new MonthlyPerformanceDto
            {
                Year = monthDate.Year,
                Month = monthDate.Month,
                MonthName = monthDate.ToString("MMM yyyy"),
                ApplicationsProcessed = monthApplications.Count,
                AmountProcessed = monthApplications.Sum(a => a.RequestedAmount)
            });
        }

        // Get region performance
        var userRegions = user.Regions.ToList();
        var regionPerformance = userRegions.Select(region => new RegionPerformanceDto
        {
            RegionId = region.Id,
            RegionName = region.Name,
            ApplicationsProcessed = userApplications.Count(a => a.RegionId == region.Id),
            AmountProcessed = userApplications.Where(a => a.RegionId == region.Id).Sum(a => a.RequestedAmount)
        }).ToList();

        return new PersonalStatisticsDto
        {
            TotalApplicationsProcessed = processedApplications.Count,
            ApplicationsApproved = approvedCount,
            ApplicationsRejected = rejectedCount,
            ApplicationsPending = pendingCount,
            TotalAmountProcessed = processedApplications.Sum(a => a.RequestedAmount),
            AverageProcessingTimeHours = CalculateAverageProcessingTime(processedApplications),
            MonthlyPerformance = monthlyPerformance,
            RegionPerformance = regionPerformance
        };
    }

    private decimal CalculateAverageProcessingTime(List<LoanApplication> applications)
    {
        if (!applications.Any()) return 0;

        var processingTimes = applications
            .Where(a => a.LastUpdatedDate.HasValue)
            .Select(a => (a.LastUpdatedDate!.Value - a.ApplicationDate).TotalHours)
            .ToList();

        return processingTimes.Any() ? (decimal)processingTimes.Average() : 0;
    }
}
