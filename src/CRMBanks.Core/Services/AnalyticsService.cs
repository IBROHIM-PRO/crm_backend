using CRMBanks.Core.Common;
using CRMBanks.Core.Dtos;
using CRMBanks.Core.Entities;
using CRMBanks.Core.Services.Interfaces;
using CRMBanks.SharedKernel.Enums;
using Microsoft.EntityFrameworkCore;

namespace CRMBanks.Core.Services;

public class AnalyticsService(
    IRepository<LoanApplication> loanApplicationRepository,
    IRepository<DepositApplication> depositApplicationRepository,
    IRepository<LoanApplicationAction> loanActionRepository,
    IRepository<DepositApplicationAction> depositActionRepository,
    IRepository<User> userRepository,
    IRepository<Bank> bankRepository,
    IRepository<Region> regionRepository) : IAnalyticsService
{
    public async Task<List<WorkerPerformanceKpiDto>> GetWorkerPerformanceKpisAsync(AnalyticsFilterDto filter)
    {
        var dateFilter = ApplyDateFilter(filter);
        
        // Get loan application performance
        var loanPerformance = await GetLoanWorkerPerformanceAsync(dateFilter, filter);
        
        // Get deposit application performance
        var depositPerformance = await GetDepositWorkerPerformanceAsync(dateFilter, filter);
        
        // Combine and aggregate results
        var combinedPerformance = CombineWorkerPerformance(loanPerformance, depositPerformance);
        
        return combinedPerformance.OrderByDescending(w => w.TotalProcessed).ToList();
    }

    public async Task<WorkerPerformanceKpiDto?> GetWorkerPerformanceAsync(int workerId, AnalyticsFilterDto filter)
    {
        var allPerformance = await GetWorkerPerformanceKpisAsync(filter);
        return allPerformance.FirstOrDefault(w => w.WorkerId == workerId);
    }

    public async Task<List<WorkerActivityDto>> GetCurrentWorkerActivitiesAsync()
    {
        // Get currently claimed loan applications
        var loanActivities = await loanApplicationRepository.GetQuery()
            .Include(l => l.AssignedWorker)
            .Include(l => l.Credit)
            .Where(l => l.AssignedWorkerId.HasValue && l.Status == ApplicationStatus.UnderReview)
            .Select(l => new WorkerActivityDto
            {
                WorkerId = l.AssignedWorkerId!.Value,
                WorkerName = l.AssignedWorker.Name,
                CurrentActivity = "Processing Loan Application",
                ApplicationType = "loan",
                CurrentApplicationId = l.Id,
                CurrentApplicationAmount = l.RequestedAmount,
                ActivityStartedAt = l.Actions
                    .Where(a => a.Action == "Claimed")
                    .OrderByDescending(a => a.CreatedAt)
                    .FirstOrDefault()!.CreatedAt,
                CurrentActivityDuration = DateTime.UtcNow - l.Actions
                    .Where(a => a.Action == "Claimed")
                    .OrderByDescending(a => a.CreatedAt)
                    .FirstOrDefault()!.CreatedAt
            })
            .ToListAsync();

        // Get currently claimed deposit applications
        var depositActivities = await depositApplicationRepository.GetQuery()
            .Include(d => d.AssignedWorker)
            .Include(d => d.Deposit)
            .Where(d => d.AssignedWorkerId.HasValue && d.Status == ApplicationStatus.UnderReview)
            .Select(d => new WorkerActivityDto
            {
                WorkerId = d.AssignedWorkerId!.Value,
                WorkerName = d.AssignedWorker.Name,
                CurrentActivity = "Processing Deposit Application",
                ApplicationType = "deposit",
                CurrentApplicationId = d.Id,
                CurrentApplicationAmount = d.DepositAmount,
                ActivityStartedAt = d.Actions
                    .Where(a => a.Action == "Claimed")
                    .OrderByDescending(a => a.CreatedAt)
                    .FirstOrDefault()!.CreatedAt,
                CurrentActivityDuration = DateTime.UtcNow - d.Actions
                    .Where(a => a.Action == "Claimed")
                    .OrderByDescending(a => a.CreatedAt)
                    .FirstOrDefault()!.CreatedAt
            })
            .ToListAsync();

        return loanActivities.Concat(depositActivities).ToList();
    }

    public async Task<List<ActivityStreamDto>> GetRecentActivitiesAsync(int limit = 50)
    {
        var loanActivities = await loanActionRepository.GetQuery()
            .Include(a => a.User)
            .Include(a => a.LoanApplication)
            .OrderByDescending(a => a.CreatedAt)
            .Take(limit)
            .Select(a => new ActivityStreamDto
            {
                Id = a.Id,
                Message = GenerateActivityMessage(a.Action, a.User.Name, "loan", a.LoanApplication.RequestedAmount),
                ActivityType = a.Action.ToLower(),
                WorkerId = a.UserId,
                WorkerName = a.User.Name,
                ApplicationId = a.LoanApplicationId,
                ApplicationAmount = a.LoanApplication.RequestedAmount,
                ApplicationType = "loan",
                Timestamp = a.CreatedAt,
                Details = a.Comments
            })
            .ToListAsync();

        var depositActivities = await depositActionRepository.GetQuery()
            .Include(a => a.User)
            .Include(a => a.DepositApplication)
            .OrderByDescending(a => a.CreatedAt)
            .Take(limit)
            .Select(a => new ActivityStreamDto
            {
                Id = a.Id,
                Message = GenerateActivityMessage(a.Action, a.User.Name, "deposit", a.DepositApplication.DepositAmount),
                ActivityType = a.Action.ToLower(),
                WorkerId = a.UserId,
                WorkerName = a.User.Name,
                ApplicationId = a.DepositApplicationId,
                ApplicationAmount = a.DepositApplication.DepositAmount,
                ApplicationType = "deposit",
                Timestamp = a.CreatedAt,
                Details = a.Comments
            })
            .ToListAsync();

        return loanActivities.Concat(depositActivities)
            .OrderByDescending(a => a.Timestamp)
            .Take(limit)
            .ToList();
    }

    public async Task<List<ActivityStreamDto>> GetActivityStreamAsync(AnalyticsFilterDto filter, int limit = 100)
    {
        var dateFilter = ApplyDateFilter(filter);
        
        var query = from la in loanActionRepository.GetQuery()
                    .Include(a => a.User)
                    .Include(a => a.LoanApplication)
                    .Where(a => a.CreatedAt >= dateFilter.Item1 && a.CreatedAt <= dateFilter.Item2)
                    select new ActivityStreamDto
                    {
                        Id = la.Id,
                        Message = GenerateActivityMessage(la.Action, la.User.Name, "loan", la.LoanApplication.RequestedAmount),
                        ActivityType = la.Action.ToLower(),
                        WorkerId = la.UserId,
                        WorkerName = la.User.Name,
                        ApplicationId = la.LoanApplicationId,
                        ApplicationAmount = la.LoanApplication.RequestedAmount,
                        ApplicationType = "loan",
                        Timestamp = la.CreatedAt,
                        Details = la.Comments
                    };

        var depositQuery = from da in depositActionRepository.GetQuery()
                          .Include(a => a.User)
                          .Include(a => a.DepositApplication)
                          .Where(a => a.CreatedAt >= dateFilter.Item1 && a.CreatedAt <= dateFilter.Item2)
                          select new ActivityStreamDto
                          {
                              Id = da.Id,
                              Message = GenerateActivityMessage(da.Action, da.User.Name, "deposit", da.DepositApplication.DepositAmount),
                              ActivityType = da.Action.ToLower(),
                              WorkerId = da.UserId,
                              WorkerName = da.User.Name,
                              ApplicationId = da.DepositApplicationId,
                              ApplicationAmount = da.DepositApplication.DepositAmount,
                              ApplicationType = "deposit",
                              Timestamp = da.CreatedAt,
                              Details = da.Comments
                          };

        var combinedQuery = query.Concat(depositQuery);
        
        return await combinedQuery
            .OrderByDescending(a => a.Timestamp)
            .Take(limit)
            .ToListAsync();
    }

    public async Task<List<WorkerProductivityChartDto>> GetProductivityChartDataAsync(AnalyticsFilterDto filter)
    {
        var performance = await GetWorkerPerformanceKpisAsync(filter);
        
        return performance.Select(p => new WorkerProductivityChartDto
        {
            WorkerName = p.WorkerName,
            TotalApplications = p.TotalProcessed,
            ApprovedApplications = p.ApprovedCount,
            RejectedApplications = p.RejectedCount,
            AverageProcessingTime = p.AverageProcessingTimeHours,
            ProductivityScore = CalculateProductivityScore(p)
        }).OrderByDescending(p => p.ProductivityScore).ToList();
    }

    public async Task<List<TimeSeriesProductivityDto>> GetTimeSeriesProductivityAsync(AnalyticsFilterDto filter, string groupBy = "day")
    {
        var dateFilter = ApplyDateFilter(filter);
        
        var loanData = await GetLoanTimeSeriesData(dateFilter, groupBy);
        var depositData = await GetDepositTimeSeriesData(dateFilter, groupBy);
        
        return CombineTimeSeriesData(loanData, depositData)
            .OrderByDescending(t => t.Date)
            .ToList();
    }

    public async Task<ProcessingTimeAnalysisDto> GetProcessingTimeAnalysisAsync(AnalyticsFilterDto filter)
    {
        var dateFilter = ApplyDateFilter(filter);
        
        var loanAnalysis = await GetLoanProcessingTimeAnalysis(dateFilter);
        var depositAnalysis = await GetDepositProcessingTimeAnalysis(dateFilter);
        
        return CombineProcessingTimeAnalysis(loanAnalysis, depositAnalysis);
    }

    public async Task<List<WorkerLeaderboardDto>> GetWorkerLeaderboardAsync(AnalyticsFilterDto filter, int top = 10)
    {
        var performance = await GetWorkerPerformanceKpisAsync(filter);
        
        var leaderboard = performance
            .Select((p, index) => new WorkerLeaderboardDto
            {
                Rank = index + 1,
                WorkerId = p.WorkerId,
                WorkerName = p.WorkerName,
                BankName = p.BankName,
                ProductivityScore = CalculateProductivityScore(p),
                TotalProcessed = p.TotalProcessed,
                AverageProcessingTimeHours = p.AverageProcessingTimeHours,
                ApprovalRate = p.ApprovalRate,
                CurrentQueue = p.PendingQueueCount
            })
            .OrderByDescending(l => l.ProductivityScore)
            .Take(top)
            .ToList();
        
        // Update ranks
        for (int i = 0; i < leaderboard.Count; i++)
        {
            leaderboard[i].Rank = i + 1;
        }
        
        return leaderboard;
    }

    public async Task<List<WorkerLeaderboardDto>> GetTopPerformersAsync(AnalyticsFilterDto filter, int count = 3)
    {
        return await GetWorkerLeaderboardAsync(filter, count);
    }

    public async Task<List<BottleneckAlertDto>> GetBottleneckAlertsAsync(AnalyticsFilterDto filter)
    {
        var dateFilter = ApplyDateFilter(filter);
        
        // Get stuck loan applications (> 24 hours in UnderReview)
        var loanBottlenecks = await loanApplicationRepository.GetQuery()
            .Include(l => l.AssignedWorker)
            .Include(l => l.Region)
            .Where(l => l.Status == ApplicationStatus.UnderReview && 
                       l.AssignedWorkerId.HasValue &&
                       l.Actions.Any(a => a.Action == "Claimed") &&
                       (DateTime.UtcNow - l.Actions.Where(a => a.Action == "Claimed")
                           .OrderByDescending(a => a.CreatedAt)
                           .First().CreatedAt).TotalHours > 24)
            .Select(l => new BottleneckAlertDto
            {
                ApplicationId = l.Id,
                ApplicationType = "loan",
                WorkerId = l.AssignedWorkerId!.Value,
                WorkerName = l.AssignedWorker.Name,
                ClaimedAt = l.Actions.Where(a => a.Action == "Claimed")
                    .OrderByDescending(a => a.CreatedAt)
                    .First().CreatedAt,
                StuckDuration = DateTime.UtcNow - l.Actions.Where(a => a.Action == "Claimed")
                    .OrderByDescending(a => a.CreatedAt)
                    .First().CreatedAt,
                ApplicationAmount = l.RequestedAmount,
                ApplicantName = l.ApplicantName,
                CurrentStatus = l.Status,
                AlertSeverity = CalculateAlertSeverity(DateTime.UtcNow - l.Actions.Where(a => a.Action == "Claimed")
                    .OrderByDescending(a => a.CreatedAt)
                    .First().CreatedAt)
            })
            .ToListAsync();

        // Get stuck deposit applications
        var depositBottlenecks = await depositApplicationRepository.GetQuery()
            .Include(d => d.AssignedWorker)
            .Include(d => d.Region)
            .Where(d => d.Status == ApplicationStatus.UnderReview && 
                       d.AssignedWorkerId.HasValue &&
                       d.Actions.Any(a => a.Action == "Claimed") &&
                       (DateTime.UtcNow - d.Actions.Where(a => a.Action == "Claimed")
                           .OrderByDescending(a => a.CreatedAt)
                           .First().CreatedAt).TotalHours > 24)
            .Select(d => new BottleneckAlertDto
            {
                ApplicationId = d.Id,
                ApplicationType = "deposit",
                WorkerId = d.AssignedWorkerId!.Value,
                WorkerName = d.AssignedWorker.Name,
                ClaimedAt = d.Actions.Where(a => a.Action == "Claimed")
                    .OrderByDescending(a => a.CreatedAt)
                    .First().CreatedAt,
                StuckDuration = DateTime.UtcNow - d.Actions.Where(a => a.Action == "Claimed")
                    .OrderByDescending(a => a.CreatedAt)
                    .First().CreatedAt,
                ApplicationAmount = d.DepositAmount,
                ApplicantName = d.ApplicantName,
                CurrentStatus = d.Status,
                AlertSeverity = CalculateAlertSeverity(DateTime.UtcNow - d.Actions.Where(a => a.Action == "Claimed")
                    .OrderByDescending(a => a.CreatedAt)
                    .First().CreatedAt)
            })
            .ToListAsync();

        return loanBottlenecks.Concat(depositBottlenecks)
            .OrderByDescending(b => b.StuckDuration.TotalHours)
            .ToList();
    }

    public async Task<List<BottleneckAlertDto>> GetCriticalBottlenecksAsync()
    {
        var filter = new AnalyticsFilterDto { DateRangeType = "ThisWeek" };
        return await GetBottleneckAlertsAsync(filter);
    }

    public async Task<AnalyticsDashboardDto> GetDashboardSummaryAsync(AnalyticsFilterDto filter)
    {
        var performance = await GetWorkerPerformanceKpisAsync(filter);
        var currentActivities = await GetCurrentWorkerActivitiesAsync();
        var recentActivities = await GetRecentActivitiesAsync(20);
        var productivityChart = await GetProductivityChartDataAsync(filter);
        var timeSeriesData = await GetTimeSeriesProductivityAsync(filter);
        var leaderboard = await GetWorkerLeaderboardAsync(filter, 10);
        var bottleneckAlerts = await GetBottleneckAlertsAsync(filter);

        return new AnalyticsDashboardDto
        {
            WorkerPerformance = performance,
            CurrentActivities = currentActivities,
            RecentActivities = recentActivities,
            ProductivityChart = productivityChart,
            TimeSeriesData = timeSeriesData,
            Leaderboard = leaderboard,
            BottleneckAlerts = bottleneckAlerts,
            TotalApplications = await GetTotalApplicationsAsync(filter),
            PendingApplications = performance.Sum(p => p.PendingQueueCount),
            UnderReviewApplications = currentActivities.Count,
            OverallApprovalRate = await GetOverallApprovalRateAsync(filter),
            AverageProcessingTimeHours = await GetAverageProcessingTimeAsync(filter),
            ActiveWorkers = currentActivities.Count,
            LastUpdated = DateTime.UtcNow
        };
    }

    public async Task<int> GetTotalApplicationsAsync(AnalyticsFilterDto filter)
    {
        var dateFilter = ApplyDateFilter(filter);
        
        var loanCount = await loanApplicationRepository.GetQuery()
            .Where(l => l.ApplicationDate >= dateFilter.Item1 && l.ApplicationDate <= dateFilter.Item2)
            .CountAsync();
            
        var depositCount = await depositApplicationRepository.GetQuery()
            .Where(d => d.ApplicationDate >= dateFilter.Item1 && d.ApplicationDate <= dateFilter.Item2)
            .CountAsync();
            
        return loanCount + depositCount;
    }

    public async Task<decimal> GetOverallApprovalRateAsync(AnalyticsFilterDto filter)
    {
        var dateFilter = ApplyDateFilter(filter);
        
        var loanApproved = await loanApplicationRepository.GetQuery()
            .Where(l => l.ApplicationDate >= dateFilter.Item1 && l.ApplicationDate <= dateFilter.Item2 && 
                       l.Status == ApplicationStatus.Approved)
            .CountAsync();
            
        var loanTotal = await loanApplicationRepository.GetQuery()
            .Where(l => l.ApplicationDate >= dateFilter.Item1 && l.ApplicationDate <= dateFilter.Item2 && 
                       (l.Status == ApplicationStatus.Approved || l.Status == ApplicationStatus.Rejected))
            .CountAsync();
            
        var depositApproved = await depositApplicationRepository.GetQuery()
            .Where(d => d.ApplicationDate >= dateFilter.Item1 && d.ApplicationDate <= dateFilter.Item2 && 
                       d.Status == ApplicationStatus.Approved)
            .CountAsync();
            
        var depositTotal = await depositApplicationRepository.GetQuery()
            .Where(d => d.ApplicationDate >= dateFilter.Item1 && d.ApplicationDate <= dateFilter.Item2 && 
                       (d.Status == ApplicationStatus.Approved || d.Status == ApplicationStatus.Rejected))
            .CountAsync();
            
        var totalApproved = loanApproved + depositApproved;
        var totalDecided = loanTotal + depositTotal;
        
        return totalDecided > 0 ? (decimal)totalApproved / totalDecided * 100 : 0;
    }

    public async Task<decimal> GetAverageProcessingTimeAsync(AnalyticsFilterDto filter)
    {
        var performance = await GetWorkerPerformanceKpisAsync(filter);
        
        if (!performance.Any()) return 0;
        
        return performance.Average(p => p.AverageProcessingTimeHours);
    }

    public async Task<int> GetActiveWorkersCountAsync()
    {
        var activities = await GetCurrentWorkerActivitiesAsync();
        return activities.DistinctBy(a => a.WorkerId).Count();
    }

    public async Task<List<WorkerPerformanceKpiDto>> GetWorkersByPerformanceAsync(string sortBy, int limit = 20)
    {
        var performance = await GetWorkerPerformanceKpisAsync(new AnalyticsFilterDto());
        
        return sortBy.ToLower() switch
        {
            "processed" => performance.OrderByDescending(p => p.TotalProcessed).Take(limit).ToList(),
            "approvalrate" => performance.OrderByDescending(p => p.ApprovalRate).Take(limit).ToList(),
            "processingtime" => performance.OrderBy(p => p.AverageProcessingTimeHours).Take(limit).ToList(),
            "productivity" => performance.OrderByDescending(p => CalculateProductivityScore(p)).Take(limit).ToList(),
            _ => performance.Take(limit).ToList()
        };
    }

    public async Task<List<BottleneckAlertDto>> GetStuckApplicationsAsync(int hoursThreshold = 24)
    {
        var filter = new AnalyticsFilterDto { DateRangeType = "ThisMonth" };
        var bottlenecks = await GetBottleneckAlertsAsync(filter);
        
        return bottlenecks
            .Where(b => b.StuckDuration.TotalHours >= hoursThreshold)
            .OrderByDescending(b => b.StuckDuration.TotalHours)
            .ToList();
    }

    // Private helper methods
    private (DateTime, DateTime) ApplyDateFilter(AnalyticsFilterDto filter)
    {
        var now = DateTime.UtcNow;
        var startDate = filter.StartDate ?? now.AddDays(-30);
        var endDate = filter.EndDate ?? now;

        return filter.DateRangeType.ToLower() switch
        {
            "today" => (now.Date, now.Date.AddDays(1).AddTicks(-1)),
            "thisweek" => (now.AddDays(-(int)now.DayOfWeek), now),
            "thismonth" => (new DateTime(now.Year, now.Month, 1), now),
            "custom" => (startDate, endDate),
            _ => (startDate, endDate)
        };
    }

    private async Task<List<WorkerPerformanceKpiDto>> GetLoanWorkerPerformanceAsync((DateTime, DateTime) dateFilter, AnalyticsFilterDto filter)
    {
        return await loanApplicationRepository.GetQuery()
            .Include(l => l.AssignedWorker)
            .Include(l => l.SelectedBanks)
            .Include(l => l.Region)
            .Include(l => l.Actions)
            .Where(l => l.AssignedWorkerId.HasValue)
            .Where(l => l.ApplicationDate >= dateFilter.Item1 && l.ApplicationDate <= dateFilter.Item2)
            .GroupBy(l => new { l.AssignedWorkerId, WorkerName = l.AssignedWorker.Name, PrimaryBankName = l.SelectedBanks.FirstOrDefault() != null ? l.SelectedBanks.FirstOrDefault().Name : "Unknown", RegionName = l.Region.Name })
            .Select(g => new WorkerPerformanceKpiDto
            {
                WorkerId = g.Key.AssignedWorkerId!.Value,
                WorkerName = g.Key.WorkerName,
                BankName = g.Key.PrimaryBankName,
                RegionName = g.Key.RegionName,
                TotalProcessed = g.Count(),
                ApprovedCount = g.Count(l => l.Status == ApplicationStatus.Approved),
                RejectedCount = g.Count(l => l.Status == ApplicationStatus.Rejected),
                EscalatedCount = g.Count(l => l.Status == ApplicationStatus.Escalated),
                ApprovalRate = g.Count(l => l.Status == ApplicationStatus.Approved) > 0 ? 
                    (decimal)g.Count(l => l.Status == ApplicationStatus.Approved) / g.Count(l => l.Status == ApplicationStatus.Approved || l.Status == ApplicationStatus.Rejected) * 100 : 0,
                RejectionRate = g.Count(l => l.Status == ApplicationStatus.Rejected) > 0 ? 
                    (decimal)g.Count(l => l.Status == ApplicationStatus.Rejected) / g.Count(l => l.Status == ApplicationStatus.Approved || l.Status == ApplicationStatus.Rejected) * 100 : 0,
                PendingQueueCount = g.Count(l => l.Status == ApplicationStatus.UnderReview),
                ActionRequiredCount = g.Count(l => l.Status == ApplicationStatus.ActionRequired),
                LastActivityAt = g.Max(l => l.LastUpdatedDate)
            })
            .ToListAsync();
    }

    private async Task<List<WorkerPerformanceKpiDto>> GetDepositWorkerPerformanceAsync((DateTime, DateTime) dateFilter, AnalyticsFilterDto filter)
    {
        return await depositApplicationRepository.GetQuery()
            .Include(d => d.AssignedWorker)
            .Include(d => d.SelectedBanks)
            .Include(d => d.Region)
            .Include(d => d.Actions)
            .Where(d => d.AssignedWorkerId.HasValue)
            .Where(d => d.ApplicationDate >= dateFilter.Item1 && d.ApplicationDate <= dateFilter.Item2)
            .GroupBy(d => new { d.AssignedWorkerId, WorkerName = d.AssignedWorker.Name, PrimaryBankName = d.SelectedBanks.FirstOrDefault() != null ? d.SelectedBanks.FirstOrDefault().Name : "Unknown", RegionName = d.Region.Name })
            .Select(g => new WorkerPerformanceKpiDto
            {
                WorkerId = g.Key.AssignedWorkerId!.Value,
                WorkerName = g.Key.WorkerName,
                BankName = g.Key.PrimaryBankName,
                RegionName = g.Key.RegionName,
                TotalProcessed = g.Count(),
                ApprovedCount = g.Count(d => d.Status == ApplicationStatus.Approved),
                RejectedCount = g.Count(d => d.Status == ApplicationStatus.Rejected),
                EscalatedCount = g.Count(d => d.Status == ApplicationStatus.Escalated),
                ApprovalRate = g.Count(d => d.Status == ApplicationStatus.Approved) > 0 ? 
                    (decimal)g.Count(d => d.Status == ApplicationStatus.Approved) / g.Count(d => d.Status == ApplicationStatus.Approved || d.Status == ApplicationStatus.Rejected) * 100 : 0,
                RejectionRate = g.Count(d => d.Status == ApplicationStatus.Rejected) > 0 ? 
                    (decimal)g.Count(d => d.Status == ApplicationStatus.Rejected) / g.Count(d => d.Status == ApplicationStatus.Approved || d.Status == ApplicationStatus.Rejected) * 100 : 0,
                PendingQueueCount = g.Count(d => d.Status == ApplicationStatus.UnderReview),
                ActionRequiredCount = g.Count(d => d.Status == ApplicationStatus.ActionRequired),
                LastActivityAt = g.Max(d => d.LastUpdatedDate)
            })
            .ToListAsync();
    }

    private List<WorkerPerformanceKpiDto> CombineWorkerPerformance(
        List<WorkerPerformanceKpiDto> loanPerformance, 
        List<WorkerPerformanceKpiDto> depositPerformance)
    {
        var combined = new Dictionary<int, WorkerPerformanceKpiDto>();
        
        // Add loan performance
        foreach (var loan in loanPerformance)
        {
            combined[loan.WorkerId] = loan;
        }
        
        // Add/merge deposit performance
        foreach (var deposit in depositPerformance)
        {
            if (combined.ContainsKey(deposit.WorkerId))
            {
                var existing = combined[deposit.WorkerId];
                existing.TotalProcessed += deposit.TotalProcessed;
                existing.ApprovedCount += deposit.ApprovedCount;
                existing.RejectedCount += deposit.RejectedCount;
                existing.EscalatedCount += deposit.EscalatedCount;
                existing.PendingQueueCount += deposit.PendingQueueCount;
                existing.ActionRequiredCount += deposit.ActionRequiredCount;
                
                // Recalculate rates
                var totalDecided = existing.ApprovedCount + existing.RejectedCount;
                if (totalDecided > 0)
                {
                    existing.ApprovalRate = (decimal)existing.ApprovedCount / totalDecided * 100;
                    existing.RejectionRate = (decimal)existing.RejectedCount / totalDecided * 100;
                }
                
                if (deposit.LastActivityAt > existing.LastActivityAt)
                {
                    existing.LastActivityAt = deposit.LastActivityAt;
                }
            }
            else
            {
                combined[deposit.WorkerId] = deposit;
            }
        }
        
        return combined.Values.ToList();
    }

    private string GenerateActivityMessage(string action, string workerName, string applicationType, decimal amount)
    {
        return action.ToLower() switch
        {
            "claimed" => $"{workerName} claimed a new {applicationType} application for {amount:C}",
            "approved" => $"{workerName} approved a {applicationType} application for {amount:C}",
            "rejected" => $"{workerName} rejected a {applicationType} application for {amount:C}",
            "escalated" => $"{workerName} escalated a {applicationType} application for {amount:C}",
            "released" => $"{workerName} released a {applicationType} application back to queue",
            _ => $"{workerName} performed action '{action}' on {applicationType} application for {amount:C}"
        };
    }

    private decimal CalculateProductivityScore(WorkerPerformanceKpiDto performance)
    {
        // Weighted score calculation
        var volumeScore = performance.TotalProcessed * 10;
        var approvalScore = performance.ApprovalRate * 2;
        var speedScore = performance.AverageProcessingTimeHours > 0 ? (100 / performance.AverageProcessingTimeHours) : 100;
        var queuePenalty = performance.PendingQueueCount * 5;
        
        return volumeScore + approvalScore + speedScore - queuePenalty;
    }

    private string CalculateAlertSeverity(TimeSpan stuckDuration)
    {
        return stuckDuration.TotalHours switch
        {
            < 48 => "Low",
            < 72 => "Medium", 
            < 120 => "High",
            _ => "Critical"
        };
    }

    private async Task<List<TimeSeriesProductivityDto>> GetLoanTimeSeriesData((DateTime, DateTime) dateFilter, string groupBy)
    {
        // Implementation for loan time series data
        return new List<TimeSeriesProductivityDto>();
    }

    private async Task<List<TimeSeriesProductivityDto>> GetDepositTimeSeriesData((DateTime, DateTime) dateFilter, string groupBy)
    {
        // Implementation for deposit time series data
        return new List<TimeSeriesProductivityDto>();
    }

    private List<TimeSeriesProductivityDto> CombineTimeSeriesData(
        List<TimeSeriesProductivityDto> loanData, 
        List<TimeSeriesProductivityDto> depositData)
    {
        // Implementation for combining time series data
        return new List<TimeSeriesProductivityDto>();
    }

    private async Task<ProcessingTimeAnalysisDto> GetLoanProcessingTimeAnalysis((DateTime, DateTime) dateFilter)
    {
        // Implementation for loan processing time analysis
        return new ProcessingTimeAnalysisDto { ApplicationType = "loan" };
    }

    private async Task<ProcessingTimeAnalysisDto> GetDepositProcessingTimeAnalysis((DateTime, DateTime) dateFilter)
    {
        // Implementation for deposit processing time analysis
        return new ProcessingTimeAnalysisDto { ApplicationType = "deposit" };
    }

    private ProcessingTimeAnalysisDto CombineProcessingTimeAnalysis(
        ProcessingTimeAnalysisDto loanAnalysis, 
        ProcessingTimeAnalysisDto depositAnalysis)
    {
        // Implementation for combining processing time analysis
        return new ProcessingTimeAnalysisDto { ApplicationType = "combined" };
    }
}
