using CRMBanks.SharedKernel.Enums;

namespace CRMBanks.Core.Dtos;

public class DepositApplicationDto
{
    public int Id { get; set; }
    public string ApplicantName { get; set; } = string.Empty;
    public string ApplicantPhone { get; set; } = string.Empty;
    public string ApplicantEmail { get; set; } = string.Empty;
    public int DepositId { get; set; }
    public string DepositName { get; set; } = string.Empty;
    public decimal DepositAmount { get; set; }
    public int DepositTermMonths { get; set; }
    public int RegionId { get; set; }
    public string RegionName { get; set; } = string.Empty;
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
    
    public List<DepositApplicationActionDto> Actions { get; set; } = new();
}

public class CreateDepositApplicationDto
{
    public string ApplicantName { get; set; } = string.Empty;
    public string ApplicantPhone { get; set; } = string.Empty;
    public string ApplicantEmail { get; set; } = string.Empty;
    public int DepositId { get; set; }
    public decimal DepositAmount { get; set; }
    public int DepositTermMonths { get; set; }
    public int RegionId { get; set; }
    public bool SendToAllBanks { get; set; }
    public List<int> SelectedBankIds { get; set; } = new();
}

public class UpdateDepositApplicationStatusDto
{
    public ApplicationStatus Status { get; set; }
    public string? Comments { get; set; }
}

public class DepositApplicationActionDto
{
    public int Id { get; set; }
    public int DepositApplicationId { get; set; }
    public string UserName { get; set; } = string.Empty;
    public string Action { get; set; } = string.Empty;
    public string? Comments { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class DepositApplicationFilterDto
{
    public int? RegionId { get; set; }
    public decimal? MaxAmount { get; set; }
    public ApplicationStatus? Status { get; set; }
    public int? UserId { get; set; }
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 10;
}

public class PagedDepositApplicationDto
{
    public List<DepositApplicationDto> Items { get; set; } = new();
    public int TotalCount { get; set; }
    public int Page { get; set; }
    public int PageSize { get; set; }
    public int TotalPages { get; set; }
}
