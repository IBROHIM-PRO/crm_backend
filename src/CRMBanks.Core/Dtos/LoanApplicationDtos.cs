using CRMBanks.SharedKernel.Enums;

namespace CRMBanks.Core.Dtos;

public class LoanApplicationDto
{
    public int Id { get; set; }
    public string ApplicantName { get; set; } = string.Empty;
    public string ApplicantPhone { get; set; } = string.Empty;
    public string ApplicantEmail { get; set; } = string.Empty;
    public int UserId { get; set; }
    public string UserName { get; set; } = string.Empty;
    public int CreditId { get; set; }
    public string CreditName { get; set; } = string.Empty;
    public decimal RequestedAmount { get; set; }
    public int RequestedTermMonths { get; set; }
    public int RegionId { get; set; }
    public string RegionName { get; set; } = string.Empty;
    public string ApplicationPurpose { get; set; } = string.Empty;
    public string MonthlyIncome { get; set; } = string.Empty;
    public string EmploymentStatus { get; set; } = string.Empty;
    public ApplicationStatus Status { get; set; }
    public string? RejectionReason { get; set; }
    public DateTime ApplicationDate { get; set; }
    public DateTime? LastUpdatedDate { get; set; }
    public bool SendToAllBanks { get; set; }
    public List<string> SelectedBankNames { get; set; } = new();
    
    // Workflow fields
    public int? AssignedWorkerId { get; set; }
    public string? AssignedWorkerName { get; set; }
    public DateTime? ClaimedAt { get; set; }
    public bool IsLocked => AssignedWorkerId.HasValue;
    
    public List<LoanApplicationActionDto> Actions { get; set; } = new();
}

public class CreateLoanApplicationDto
{
    public string ApplicantName { get; set; } = string.Empty;
    public string ApplicantPhone { get; set; } = string.Empty;
    public string ApplicantEmail { get; set; } = string.Empty;
    public int CreditId { get; set; }
    public decimal RequestedAmount { get; set; }
    public int RequestedTermMonths { get; set; }
    public int RegionId { get; set; }
    public string ApplicationPurpose { get; set; } = string.Empty;
    public string MonthlyIncome { get; set; } = string.Empty;
    public string EmploymentStatus { get; set; } = string.Empty;
    public bool SendToAllBanks { get; set; }
    public List<int> SelectedBankIds { get; set; } = new();
}

public class UpdateLoanApplicationStatusDto
{
    public ApplicationStatus Status { get; set; }
    public string? Comments { get; set; }
}

public class LoanApplicationActionRequestDto
{
    public string Action { get; set; } = string.Empty;
    public string? Comments { get; set; }
}

public class LoanApplicationActionDto
{
    public int Id { get; set; }
    public int LoanApplicationId { get; set; }
    public string UserName { get; set; } = string.Empty;
    public string Action { get; set; } = string.Empty;
    public string? Comments { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class LoanApplicationFilterDto
{
    public int? RegionId { get; set; }
    public decimal? MaxAmount { get; set; }
    public ApplicationStatus? Status { get; set; }
    public int? UserId { get; set; }
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 10;
}

public class PagedLoanApplicationDto
{
    public List<LoanApplicationDto> Items { get; set; } = new();
    public int TotalCount { get; set; }
    public int Page { get; set; }
    public int PageSize { get; set; }
    public int TotalPages { get; set; }
}
