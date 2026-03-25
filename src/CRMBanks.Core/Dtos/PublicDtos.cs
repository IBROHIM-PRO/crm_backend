using CRMBanks.SharedKernel.Enums;

namespace CRMBanks.Core.Dtos;

public class PublicBankDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public List<PublicCreditDto> Credits { get; set; } = new();
    public List<PublicDepositDto> Deposits { get; set; } = new();
}

public class PublicCreditDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public double Foiz { get; set; }
    public int AzSum { get; set; }
    public int ToSum { get; set; }
    public string TypeSum { get; set; } = string.Empty;
    public int AzSana { get; set; }
    public int ToSana { get; set; }
    public string InfoProt { get; set; } = string.Empty;
    public string TypeCreditName { get; set; } = string.Empty;
    public List<PublicProductDto> Products { get; set; } = new();
}

public class PublicDepositDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public double Foiz { get; set; }
    public int AzSum { get; set; }
    public int ToSum { get; set; }
    public string TypeSum { get; set; } = string.Empty;
    public int AzSana { get; set; }
    public int ToSana { get; set; }
    public string InfoProt { get; set; } = string.Empty;
    public string TypeDepositName { get; set; } = string.Empty;
    public List<PublicProductDto> Products { get; set; } = new();
}

public class PublicProductDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public int AzSum { get; set; }
    public int ToSum { get; set; }
    public string TypeSum { get; set; } = string.Empty;
    public string TypeProductName { get; set; } = string.Empty;
}

public class PublicLoanApplicationDto
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

public class PublicDepositApplicationDto
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

public class ApplicationConfirmationDto
{
    public int ApplicationId { get; set; }
    public string ApplicationNumber { get; set; } = string.Empty;
    public string ApplicantName { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public DateTime SubmissionDate { get; set; }
    public List<string> SubmittedToBanks { get; set; } = new();
}
