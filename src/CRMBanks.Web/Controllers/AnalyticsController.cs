using System.Security.Claims;
using CRMBanks.Core.Dtos;
using CRMBanks.Core.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CRMBanks.Web.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "boss,admin")]
public class AnalyticsController(IAnalyticsService analyticsService) : ControllerBase
{
    private int GetCurrentUserId()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        return int.Parse(userIdClaim ?? "0");
    }

    private string GetUserRole()
    {
        return User.FindFirst(ClaimTypes.Role)?.Value?.ToLower() ?? "";
    }

    // Worker Performance KPIs
    [HttpGet("worker-performance")]
    public async Task<IActionResult> GetWorkerPerformance([FromQuery] AnalyticsFilterDto filter)
    {
        var performance = await analyticsService.GetWorkerPerformanceKpisAsync(filter);
        return Ok(performance);
    }

    [HttpGet("worker-performance/{workerId:int}")]
    public async Task<IActionResult> GetWorkerPerformance(int workerId, [FromQuery] AnalyticsFilterDto filter)
    {
        var performance = await analyticsService.GetWorkerPerformanceAsync(workerId, filter);
        return performance == null ? NotFound() : Ok(performance);
    }

    // Real-time Activity Monitoring
    [HttpGet("current-activities")]
    public async Task<IActionResult> GetCurrentWorkerActivities()
    {
        var activities = await analyticsService.GetCurrentWorkerActivitiesAsync();
        return Ok(activities);
    }

    [HttpGet("recent-activities")]
    public async Task<IActionResult> GetRecentActivities([FromQuery] int limit = 50)
    {
        var activities = await analyticsService.GetRecentActivitiesAsync(limit);
        return Ok(activities);
    }

    [HttpGet("activity-stream")]
    public async Task<IActionResult> GetActivityStream([FromQuery] AnalyticsFilterDto filter, [FromQuery] int limit = 100)
    {
        var stream = await analyticsService.GetActivityStreamAsync(filter, limit);
        return Ok(stream);
    }

    // Visual Analytics
    [HttpGet("productivity-chart")]
    public async Task<IActionResult> GetProductivityChart([FromQuery] AnalyticsFilterDto filter)
    {
        var chart = await analyticsService.GetProductivityChartDataAsync(filter);
        return Ok(chart);
    }

    [HttpGet("time-series")]
    public async Task<IActionResult> GetTimeSeriesProductivity([FromQuery] AnalyticsFilterDto filter, [FromQuery] string groupBy = "day")
    {
        var timeSeries = await analyticsService.GetTimeSeriesProductivityAsync(filter, groupBy);
        return Ok(timeSeries);
    }

    [HttpGet("processing-time-analysis")]
    public async Task<IActionResult> GetProcessingTimeAnalysis([FromQuery] AnalyticsFilterDto filter)
    {
        var analysis = await analyticsService.GetProcessingTimeAnalysisAsync(filter);
        return Ok(analysis);
    }

    // Worker Leaderboard
    [HttpGet("leaderboard")]
    public async Task<IActionResult> GetWorkerLeaderboard([FromQuery] AnalyticsFilterDto filter, [FromQuery] int top = 10)
    {
        var leaderboard = await analyticsService.GetWorkerLeaderboardAsync(filter, top);
        return Ok(leaderboard);
    }

    [HttpGet("top-performers")]
    public async Task<IActionResult> GetTopPerformers([FromQuery] AnalyticsFilterDto filter, [FromQuery] int count = 3)
    {
        var topPerformers = await analyticsService.GetTopPerformersAsync(filter, count);
        return Ok(topPerformers);
    }

    // Bottleneck Detection
    [HttpGet("bottlenecks")]
    public async Task<IActionResult> GetBottleneckAlerts([FromQuery] AnalyticsFilterDto filter)
    {
        var bottlenecks = await analyticsService.GetBottleneckAlertsAsync(filter);
        return Ok(bottlenecks);
    }

    [HttpGet("critical-bottlenecks")]
    public async Task<IActionResult> GetCriticalBottlenecks()
    {
        var criticalBottlenecks = await analyticsService.GetCriticalBottlenecksAsync();
        return Ok(criticalBottlenecks);
    }

    [HttpGet("stuck-applications")]
    public async Task<IActionResult> GetStuckApplications([FromQuery] int hoursThreshold = 24)
    {
        var stuckApps = await analyticsService.GetStuckApplicationsAsync(hoursThreshold);
        return Ok(stuckApps);
    }

    // Dashboard Summary
    [HttpGet("dashboard")]
    public async Task<IActionResult> GetDashboardSummary([FromQuery] AnalyticsFilterDto filter)
    {
        var dashboard = await analyticsService.GetDashboardSummaryAsync(filter);
        return Ok(dashboard);
    }

    // Summary Statistics
    [HttpGet("summary")]
    public async Task<IActionResult> GetSummaryStatistics([FromQuery] AnalyticsFilterDto filter)
    {
        var summary = new
        {
            TotalApplications = await analyticsService.GetTotalApplicationsAsync(filter),
            OverallApprovalRate = await analyticsService.GetOverallApprovalRateAsync(filter),
            AverageProcessingTime = await analyticsService.GetAverageProcessingTimeAsync(filter),
            ActiveWorkersCount = await analyticsService.GetActiveWorkersCountAsync()
        };
        return Ok(summary);
    }

    // Performance Metrics
    [HttpGet("workers-by-performance")]
    public async Task<IActionResult> GetWorkersByPerformance([FromQuery] string sortBy = "processed", [FromQuery] int limit = 20)
    {
        var workers = await analyticsService.GetWorkersByPerformanceAsync(sortBy, limit);
        return Ok(workers);
    }

    // Quick Stats for Dashboard
    [HttpGet("quick-stats")]
    public async Task<IActionResult> GetQuickStats()
    {
        var todayFilter = new AnalyticsFilterDto { DateRangeType = "Today" };
        var weekFilter = new AnalyticsFilterDto { DateRangeType = "ThisWeek" };
        var monthFilter = new AnalyticsFilterDto { DateRangeType = "ThisMonth" };

        var stats = new
        {
            Today = new
            {
                TotalApplications = await analyticsService.GetTotalApplicationsAsync(todayFilter),
                ApprovalRate = await analyticsService.GetOverallApprovalRateAsync(todayFilter),
                AverageProcessingTime = await analyticsService.GetAverageProcessingTimeAsync(todayFilter),
                ActiveWorkers = await analyticsService.GetActiveWorkersCountAsync()
            },
            ThisWeek = new
            {
                TotalApplications = await analyticsService.GetTotalApplicationsAsync(weekFilter),
                ApprovalRate = await analyticsService.GetOverallApprovalRateAsync(weekFilter),
                AverageProcessingTime = await analyticsService.GetAverageProcessingTimeAsync(weekFilter),
                ActiveWorkers = await analyticsService.GetActiveWorkersCountAsync()
            },
            ThisMonth = new
            {
                TotalApplications = await analyticsService.GetTotalApplicationsAsync(monthFilter),
                ApprovalRate = await analyticsService.GetOverallApprovalRateAsync(monthFilter),
                AverageProcessingTime = await analyticsService.GetAverageProcessingTimeAsync(monthFilter),
                ActiveWorkers = await analyticsService.GetActiveWorkersCountAsync()
            },
            CriticalBottlenecks = await analyticsService.GetCriticalBottlenecksAsync(),
            TopPerformers = await analyticsService.GetTopPerformersAsync(monthFilter, 3)
        };

        return Ok(stats);
    }

    // Export functionality (for future implementation)
    [HttpGet("export/performance")]
    public async Task<IActionResult> ExportPerformanceData([FromQuery] AnalyticsFilterDto filter, [FromQuery] string format = "csv")
    {
        // TODO: Implement CSV/Excel export functionality
        return Ok(new { message = "Export functionality coming soon", format });
    }

    // Analytics configuration (for future implementation)
    [HttpPost("config/alerts")]
    public async Task<IActionResult> ConfigureAlerts([FromBody] object alertConfig)
    {
        // TODO: Implement alert configuration
        return Ok(new { message = "Alert configuration coming soon" });
    }
}
