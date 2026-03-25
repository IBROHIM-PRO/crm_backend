using CRMBanks.Core.Services.Interfaces;
using CRMBanks.Web.Hubs;
using Microsoft.AspNetCore.SignalR;

namespace CRMBanks.Web.Services;

public class AnalyticsNotificationService(IHubContext<AnalyticsHub> analyticsHub) : IAnalyticsNotificationService
{
    private readonly IHubContext<AnalyticsHub> _hubContext = analyticsHub;

    public async Task NotifyWorkerActivityAsync(int workerId, string activity, string applicationType, decimal? amount)
    {
        await _hubContext.Clients.Group("Bosses").SendAsync("WorkerActivityUpdate", new
        {
            WorkerId = workerId,
            Activity = activity,
            ApplicationType = applicationType,
            Amount = amount,
            Timestamp = DateTime.UtcNow
        });
    }

    public async Task NotifyApplicationStatusChangeAsync(int applicationId, string applicationType, int workerId, string newStatus, decimal? amount)
    {
        await _hubContext.Clients.Group("Bosses").SendAsync("ApplicationStatusUpdate", new
        {
            ApplicationId = applicationId,
            ApplicationType = applicationType,
            WorkerId = workerId,
            NewStatus = newStatus,
            Amount = amount,
            Timestamp = DateTime.UtcNow
        });
    }

    public async Task NotifyBottleneckAlertAsync(int applicationId, string applicationType, int workerId, TimeSpan stuckDuration)
    {
        await _hubContext.Clients.Group("Bosses").SendAsync("BottleneckAlert", new
        {
            ApplicationId = applicationId,
            ApplicationType = applicationType,
            WorkerId = workerId,
            StuckDurationHours = stuckDuration.TotalHours,
            Timestamp = DateTime.UtcNow
        });
    }

    public async Task NotifyPerformanceUpdateAsync(int workerId, int totalProcessed, decimal approvalRate, decimal avgProcessingTime)
    {
        await _hubContext.Clients.Group("Bosses").SendAsync("PerformanceUpdate", new
        {
            WorkerId = workerId,
            TotalProcessed = totalProcessed,
            ApprovalRate = approvalRate,
            AverageProcessingTime = avgProcessingTime,
            Timestamp = DateTime.UtcNow
        });
    }

    public async Task NotifyNewActivityAsync(string activityMessage, int workerId, string workerName, string applicationType, decimal? amount)
    {
        await _hubContext.Clients.Group("Bosses").SendAsync("NewActivity", new
        {
            Message = activityMessage,
            WorkerId = workerId,
            WorkerName = workerName,
            ApplicationType = applicationType,
            Amount = amount,
            Timestamp = DateTime.UtcNow
        });
    }

    public async Task NotifyDashboardRefreshAsync()
    {
        await _hubContext.Clients.Group("Bosses").SendAsync("DashboardRefresh", new
        {
            Timestamp = DateTime.UtcNow,
            Message = "Dashboard data updated"
        });
    }

    public async Task NotifyKpiUpdateAsync(int workerId, object kpiData)
    {
        await _hubContext.Clients.Group("Bosses").SendAsync("KpiUpdate", new
        {
            WorkerId = workerId,
            Data = kpiData,
            Timestamp = DateTime.UtcNow
        });
    }
}

public interface IAnalyticsNotificationService
{
    Task NotifyWorkerActivityAsync(int workerId, string activity, string applicationType, decimal? amount);
    Task NotifyApplicationStatusChangeAsync(int applicationId, string applicationType, int workerId, string newStatus, decimal? amount);
    Task NotifyBottleneckAlertAsync(int applicationId, string applicationType, int workerId, TimeSpan stuckDuration);
    Task NotifyPerformanceUpdateAsync(int workerId, int totalProcessed, decimal approvalRate, decimal avgProcessingTime);
    Task NotifyNewActivityAsync(string activityMessage, int workerId, string workerName, string applicationType, decimal? amount);
    Task NotifyDashboardRefreshAsync();
    Task NotifyKpiUpdateAsync(int workerId, object kpiData);
}
