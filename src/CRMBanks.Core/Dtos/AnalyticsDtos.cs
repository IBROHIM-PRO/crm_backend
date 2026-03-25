using CRMBanks.SharedKernel.Enums;

namespace CRMBanks.Core.Dtos;

// Worker Performance KPIs
public class WorkerPerformanceKpiDto
{
    public int WorkerId { get; set; }
    public string WorkerName { get; set; } = string.Empty;
    public string BankName { get; set; } = string.Empty;
    public string RegionName { get; set; } = string.Empty;
    
    // Core KPIs
    public int TotalProcessed { get; set; }
    public int ApprovedCount { get; set; }
    public int RejectedCount { get; set; }
    public int EscalatedCount { get; set; }
    public decimal ApprovalRate { get; set; }
    public decimal RejectionRate { get; set; }
    
    // Time Metrics
    public decimal AverageProcessingTimeHours { get; set; }
    public decimal AverageProcessingTimeMinutes { get; set; }
    public TimeSpan? FastestProcessingTime { get; set; }
    public TimeSpan? SlowestProcessingTime { get; set; }
    
    // Current Status
    public int PendingQueueCount { get; set; }
    public int ActionRequiredCount { get; set; }
    public DateTime? LastActivityAt { get; set; }
    
    // Performance Metrics
    public decimal ApplicationsPerHour { get; set; }
    public decimal ApplicationsPerDay { get; set; }
    public int StuckApplicationsCount { get; set; } // Applications stuck > 24 hours
}

// Real-time Activity Monitoring
public class WorkerActivityDto
{
    public int WorkerId { get; set; }
    public string WorkerName { get; set; } = string.Empty;
    public string CurrentActivity { get; set; } = string.Empty;
    public string ApplicationType { get; set; } = string.Empty; // "loan" or "deposit"
    public int? CurrentApplicationId { get; set; }
    public decimal? CurrentApplicationAmount { get; set; }
    public DateTime ActivityStartedAt { get; set; }
    public TimeSpan? CurrentActivityDuration { get; set; }
}

public class ActivityStreamDto
{
    public int Id { get; set; }
    public string Message { get; set; } = string.Empty;
    public string ActivityType { get; set; } = string.Empty; // "claim", "approve", "reject", "escalate"
    public int WorkerId { get; set; }
    public string WorkerName { get; set; } = string.Empty;
    public int? ApplicationId { get; set; }
    public decimal? ApplicationAmount { get; set; }
    public string ApplicationType { get; set; } = string.Empty;
    public DateTime Timestamp { get; set; }
    public string? Details { get; set; }
}

// Visual Analytics Data
public class WorkerProductivityChartDto
{
    public string WorkerName { get; set; } = string.Empty;
    public int TotalApplications { get; set; }
    public int ApprovedApplications { get; set; }
    public int RejectedApplications { get; set; }
    public decimal AverageProcessingTime { get; set; }
    public decimal ProductivityScore { get; set; } // Calculated score
}

public class TimeSeriesProductivityDto
{
    public DateTime Date { get; set; }
    public int TotalApplications { get; set; }
    public int ApprovedApplications { get; set; }
    public int RejectedApplications { get; set; }
    public decimal AverageProcessingTime { get; set; }
    public int ActiveWorkers { get; set; }
}

// Worker Leaderboard
public class WorkerLeaderboardDto
{
    public int Rank { get; set; }
    public int WorkerId { get; set; }
    public string WorkerName { get; set; } = string.Empty;
    public string BankName { get; set; } = string.Empty;
    public decimal ProductivityScore { get; set; }
    public int TotalProcessed { get; set; }
    public decimal AverageProcessingTimeHours { get; set; }
    public decimal ApprovalRate { get; set; }
    public int CurrentQueue { get; set; }
}

// Bottleneck Alerts
public class BottleneckAlertDto
{
    public int ApplicationId { get; set; }
    public string ApplicationType { get; set; } = string.Empty;
    public int WorkerId { get; set; }
    public string WorkerName { get; set; } = string.Empty;
    public DateTime ClaimedAt { get; set; }
    public TimeSpan StuckDuration { get; set; }
    public decimal ApplicationAmount { get; set; }
    public string ApplicantName { get; set; } = string.Empty;
    public ApplicationStatus CurrentStatus { get; set; }
    public string AlertSeverity { get; set; } = string.Empty; // "Low", "Medium", "High", "Critical"
}

// Analytics Filter Parameters
public class AnalyticsFilterDto
{
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public List<int>? BankIds { get; set; }
    public List<int>? RegionIds { get; set; }
    public List<int>? WorkerIds { get; set; }
    public string DateRangeType { get; set; } = string.Empty; // "Today", "ThisWeek", "ThisMonth", "Custom"
    public string ApplicationType { get; set; } = string.Empty; // "loan", "deposit", "all"
}

// Dashboard Summary
public class AnalyticsDashboardDto
{
    public List<WorkerPerformanceKpiDto> WorkerPerformance { get; set; } = new();
    public List<WorkerActivityDto> CurrentActivities { get; set; } = new();
    public List<ActivityStreamDto> RecentActivities { get; set; } = new();
    public List<WorkerProductivityChartDto> ProductivityChart { get; set; } = new();
    public List<TimeSeriesProductivityDto> TimeSeriesData { get; set; } = new();
    public List<WorkerLeaderboardDto> Leaderboard { get; set; } = new();
    public List<BottleneckAlertDto> BottleneckAlerts { get; set; } = new();
    
    // Summary Statistics
    public int TotalApplications { get; set; }
    public int PendingApplications { get; set; }
    public int UnderReviewApplications { get; set; }
    public decimal OverallApprovalRate { get; set; }
    public decimal AverageProcessingTimeHours { get; set; }
    public int ActiveWorkers { get; set; }
    public DateTime LastUpdated { get; set; }
}

// Processing Time Analysis
public class ProcessingTimeAnalysisDto
{
    public string ApplicationType { get; set; } = string.Empty;
    public decimal MinTimeHours { get; set; }
    public decimal MaxTimeHours { get; set; }
    public decimal AverageTimeHours { get; set; }
    public decimal MedianTimeHours { get; set; }
    public int TotalApplications { get; set; }
    public List<TimeDistributionDto> TimeDistribution { get; set; } = new();
}

public class TimeDistributionDto
{
    public string TimeRange { get; set; } = string.Empty; // "0-2 hours", "2-4 hours", etc.
    public int ApplicationCount { get; set; }
    public decimal Percentage { get; set; }
}
