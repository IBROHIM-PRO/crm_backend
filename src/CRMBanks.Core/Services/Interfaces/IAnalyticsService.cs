using CRMBanks.Core.Dtos;

namespace CRMBanks.Core.Services.Interfaces;

public interface IAnalyticsService
{
    // Worker Performance KPIs
    Task<List<WorkerPerformanceKpiDto>> GetWorkerPerformanceKpisAsync(AnalyticsFilterDto filter);
    Task<WorkerPerformanceKpiDto?> GetWorkerPerformanceAsync(int workerId, AnalyticsFilterDto filter);
    
    // Real-time Activity Monitoring
    Task<List<WorkerActivityDto>> GetCurrentWorkerActivitiesAsync();
    Task<List<ActivityStreamDto>> GetRecentActivitiesAsync(int limit = 50);
    Task<List<ActivityStreamDto>> GetActivityStreamAsync(AnalyticsFilterDto filter, int limit = 100);
    
    // Visual Analytics
    Task<List<WorkerProductivityChartDto>> GetProductivityChartDataAsync(AnalyticsFilterDto filter);
    Task<List<TimeSeriesProductivityDto>> GetTimeSeriesProductivityAsync(AnalyticsFilterDto filter, string groupBy = "day");
    Task<ProcessingTimeAnalysisDto> GetProcessingTimeAnalysisAsync(AnalyticsFilterDto filter);
    
    // Worker Leaderboard
    Task<List<WorkerLeaderboardDto>> GetWorkerLeaderboardAsync(AnalyticsFilterDto filter, int top = 10);
    Task<List<WorkerLeaderboardDto>> GetTopPerformersAsync(AnalyticsFilterDto filter, int count = 3);
    
    // Bottleneck Detection
    Task<List<BottleneckAlertDto>> GetBottleneckAlertsAsync(AnalyticsFilterDto filter);
    Task<List<BottleneckAlertDto>> GetCriticalBottlenecksAsync(); // > 24 hours
    
    // Dashboard Summary
    Task<AnalyticsDashboardDto> GetDashboardSummaryAsync(AnalyticsFilterDto filter);
    
    // Summary Statistics
    Task<int> GetTotalApplicationsAsync(AnalyticsFilterDto filter);
    Task<decimal> GetOverallApprovalRateAsync(AnalyticsFilterDto filter);
    Task<decimal> GetAverageProcessingTimeAsync(AnalyticsFilterDto filter);
    Task<int> GetActiveWorkersCountAsync();
    
    // Performance Metrics
    Task<List<WorkerPerformanceKpiDto>> GetWorkersByPerformanceAsync(string sortBy, int limit = 20);
    Task<List<BottleneckAlertDto>> GetStuckApplicationsAsync(int hoursThreshold = 24);
}
