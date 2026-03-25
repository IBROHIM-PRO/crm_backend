namespace CRMBanks.Core.Dtos;

public class GlobalStatisticsDto
{
    public int TotalApplications { get; set; }
    public int PendingApplications { get; set; }
    public int ApprovedApplications { get; set; }
    public int RejectedApplications { get; set; }
    public decimal TotalRequestedAmount { get; set; }
    public decimal AverageLoanAmount { get; set; }
    public decimal ApprovalRate { get; set; }
    public decimal AverageProcessingTime { get; set; }
    public decimal ApplicationGrowth { get; set; }
    public decimal AmountGrowth { get; set; }
    public List<WorkerPerformanceDto> WorkerPerformances { get; set; } = new();
    public List<RegionStatisticsDto> RegionStatistics { get; set; } = new();
    public List<MonthlyStatisticsDto> MonthlyStatistics { get; set; } = new();
}

public class WorkerPerformanceDto
{
    public int WorkerId { get; set; }
    public string WorkerName { get; set; } = string.Empty;
    public int ApplicationsProcessed { get; set; }
    public int ApplicationsApproved { get; set; }
    public int ApplicationsRejected { get; set; }
    public decimal AverageProcessingTimeHours { get; set; }
    public decimal TotalAmountProcessed { get; set; }
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string BankName { get; set; } = string.Empty;
    public int TotalApplications { get; set; }
    public decimal ApprovalRate { get; set; }
    public decimal TotalAmount { get; set; }
}

public class PersonalStatisticsDto
{
    public int TotalApplications { get; set; }
    public int ApprovedApplications { get; set; }
    public int RejectedApplications { get; set; }
    public int PendingApplications { get; set; }
    public decimal TotalAmount { get; set; }
    public decimal ApprovalRate { get; set; }
    public decimal AverageProcessingTime { get; set; }
    public int TotalApplicationsProcessed { get; set; }
    public int ApplicationsApproved { get; set; }
    public int ApplicationsRejected { get; set; }
    public int ApplicationsPending { get; set; }
    public decimal TotalAmountProcessed { get; set; }
    public decimal AverageProcessingTimeHours { get; set; }
    public List<MonthlyPerformanceDto> MonthlyPerformance { get; set; } = new();
    public List<RegionPerformanceDto> RegionPerformance { get; set; } = new();
    public List<RecentActivityDto> RecentActivities { get; set; } = new();
}

public class RecentActivityDto
{
    public string Type { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public DateTime Date { get; set; }
    public decimal? Amount { get; set; }
}

public class RegionStatisticsDto
{
    public int RegionId { get; set; }
    public string RegionName { get; set; } = string.Empty;
    public int ApplicationCount { get; set; }
    public decimal TotalRequestedAmount { get; set; }
    public int ApprovalCount { get; set; }
    public decimal ApprovalRate { get; set; }
}

public class MonthlyStatisticsDto
{
    public int Year { get; set; }
    public int Month { get; set; }
    public string MonthName { get; set; } = string.Empty;
    public int ApplicationCount { get; set; }
    public decimal TotalAmount { get; set; }
    public int ApprovalCount { get; set; }
}

public class MonthlyPerformanceDto
{
    public int Year { get; set; }
    public int Month { get; set; }
    public string MonthName { get; set; } = string.Empty;
    public int ApplicationsProcessed { get; set; }
    public decimal AmountProcessed { get; set; }
}

public class RegionPerformanceDto
{
    public int RegionId { get; set; }
    public string RegionName { get; set; } = string.Empty;
    public int ApplicationsProcessed { get; set; }
    public decimal AmountProcessed { get; set; }
}
