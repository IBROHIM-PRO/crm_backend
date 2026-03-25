using Microsoft.AspNetCore.SignalR;

namespace CRMBanks.Web.Hubs;

public class AnalyticsHub : Hub
{
    private readonly ILogger<AnalyticsHub> _logger;
    private static readonly Dictionary<string, List<string>> _connectedBosses = new();

    public AnalyticsHub(ILogger<AnalyticsHub> logger)
    {
        _logger = logger;
    }

    public async Task JoinBossGroup()
    {
        var userId = Context.UserIdentifier;
        if (userId == null) return;

        await Groups.AddToGroupAsync(Context.ConnectionId, "Bosses");
        
        if (!_connectedBosses.ContainsKey(userId))
        {
            _connectedBosses[userId] = new List<string>();
        }
        _connectedBosses[userId].Add(Context.ConnectionId);

        _logger.LogInformation($"Boss {userId} joined analytics group");
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        var userId = Context.UserIdentifier;
        if (userId != null && _connectedBosses.ContainsKey(userId))
        {
            _connectedBosses[userId].Remove(Context.ConnectionId);
            if (_connectedBosses[userId].Count == 0)
            {
                _connectedBosses.Remove(userId);
            }
        }

        await base.OnDisconnectedAsync(exception);
    }

    // Real-time analytics events
    public async Task NotifyWorkerActivity(int workerId, string activity, string applicationType, decimal? amount)
    {
        await Clients.Group("Bosses").SendAsync("WorkerActivityUpdate", new
        {
            WorkerId = workerId,
            Activity = activity,
            ApplicationType = applicationType,
            Amount = amount,
            Timestamp = DateTime.UtcNow
        });
    }

    public async Task NotifyApplicationStatusChange(int applicationId, string applicationType, int workerId, string newStatus, decimal? amount)
    {
        await Clients.Group("Bosses").SendAsync("ApplicationStatusUpdate", new
        {
            ApplicationId = applicationId,
            ApplicationType = applicationType,
            WorkerId = workerId,
            NewStatus = newStatus,
            Amount = amount,
            Timestamp = DateTime.UtcNow
        });
    }

    public async Task NotifyBottleneckAlert(int applicationId, string applicationType, int workerId, TimeSpan stuckDuration)
    {
        await Clients.Group("Bosses").SendAsync("BottleneckAlert", new
        {
            ApplicationId = applicationId,
            ApplicationType = applicationType,
            WorkerId = workerId,
            StuckDurationHours = stuckDuration.TotalHours,
            Timestamp = DateTime.UtcNow
        });
    }

    public async Task NotifyPerformanceUpdate(int workerId, int totalProcessed, decimal approvalRate, decimal avgProcessingTime)
    {
        await Clients.Group("Bosses").SendAsync("PerformanceUpdate", new
        {
            WorkerId = workerId,
            TotalProcessed = totalProcessed,
            ApprovalRate = approvalRate,
            AverageProcessingTime = avgProcessingTime,
            Timestamp = DateTime.UtcNow
        });
    }

    public async Task NotifyNewActivity(string activityMessage, int workerId, string workerName, string applicationType, decimal? amount)
    {
        await Clients.Group("Bosses").SendAsync("NewActivity", new
        {
            Message = activityMessage,
            WorkerId = workerId,
            WorkerName = workerName,
            ApplicationType = applicationType,
            Amount = amount,
            Timestamp = DateTime.UtcNow
        });
    }
}
