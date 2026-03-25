using CRMBanks.Core.Dtos;
using CRMBanks.Core.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace CRMBanks.Web.Controllers;

[ApiController]
[Route("api/public/applications")]
public class PublicApplicationController(
    ILoanApplicationService loanApplicationService,
    IDepositApplicationService depositApplicationService,
    IBankService bankService,
    ICreditService creditService,
    IDepositService depositService) : ControllerBase
{
    [HttpPost("loan")]
    public async Task<IActionResult> SubmitLoanApplication([FromBody] PublicLoanApplicationDto dto)
    {
        try
        {
            // Validate bank selection
            if (!dto.SendToAllBanks && !dto.SelectedBankIds.Any())
            {
                return BadRequest("Either SendToAllBanks must be true or SelectedBankIds must contain at least one bank.");
            }

            // Validate credit exists
            var credit = await creditService.GetByIdAsync(dto.CreditId);
            if (credit == null)
            {
                return BadRequest("Invalid credit product selected.");
            }

            // Create internal DTO from public DTO
            var createLoanDto = new CreateLoanApplicationDto
            {
                ApplicantName = dto.ApplicantName,
                ApplicantPhone = dto.ApplicantPhone,
                ApplicantEmail = dto.ApplicantEmail,
                CreditId = dto.CreditId,
                RequestedAmount = dto.RequestedAmount,
                RequestedTermMonths = dto.RequestedTermMonths,
                RegionId = dto.RegionId,
                ApplicationPurpose = dto.ApplicationPurpose,
                MonthlyIncome = dto.MonthlyIncome,
                EmploymentStatus = dto.EmploymentStatus,
                SendToAllBanks = dto.SendToAllBanks,
                SelectedBankIds = dto.SelectedBankIds
            };

            // Create loan application (using system user ID 1 for public applications)
            var result = await loanApplicationService.CreateAsync(createLoanDto, 1);

            // Send confirmation email (this would be implemented in the service)
            // await emailService.SendApplicationConfirmation(dto.ApplicantEmail, result.Id);

            var confirmation = new ApplicationConfirmationDto
            {
                ApplicationId = result.Id,
                ApplicationNumber = $"LOAN-{result.Id:D6}",
                ApplicantName = dto.ApplicantName,
                Status = "Submitted",
                SubmissionDate = DateTime.UtcNow,
                SubmittedToBanks = dto.SendToAllBanks 
                    ? (await bankService.GetAllAsync()).Select(b => b.Name).ToList()
                    : await GetSelectedBankNames(dto.SelectedBankIds)
            };

            return Ok(confirmation);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"An error occurred while submitting your application: {ex.Message}");
        }
    }

    [HttpPost("deposit")]
    public async Task<IActionResult> SubmitDepositApplication([FromBody] PublicDepositApplicationDto dto)
    {
        try
        {
            // Validate bank selection
            if (!dto.SendToAllBanks && !dto.SelectedBankIds.Any())
            {
                return BadRequest("Either SendToAllBanks must be true or SelectedBankIds must contain at least one bank.");
            }

            // Validate deposit exists
            var deposit = await depositService.GetByIdAsync(dto.DepositId);
            if (deposit == null)
            {
                return BadRequest("Invalid deposit product selected.");
            }

            // Create internal DTO from public DTO
            var createDepositDto = new CreateDepositApplicationDto
            {
                ApplicantName = dto.ApplicantName,
                ApplicantPhone = dto.ApplicantPhone,
                ApplicantEmail = dto.ApplicantEmail,
                DepositId = dto.DepositId,
                DepositAmount = dto.DepositAmount,
                DepositTermMonths = dto.DepositTermMonths,
                RegionId = dto.RegionId,
                SendToAllBanks = dto.SendToAllBanks,
                SelectedBankIds = dto.SelectedBankIds
            };

            // Create deposit application (using system user ID 1 for public applications)
            var result = await depositApplicationService.CreateAsync(createDepositDto, 1);

            // Send confirmation email (this would be implemented in the service)
            // await emailService.SendApplicationConfirmation(dto.ApplicantEmail, result.Id);

            var confirmation = new ApplicationConfirmationDto
            {
                ApplicationId = result.Id,
                ApplicationNumber = $"DEPOSIT-{result.Id:D6}",
                ApplicantName = dto.ApplicantName,
                Status = "Submitted",
                SubmissionDate = DateTime.UtcNow,
                SubmittedToBanks = dto.SendToAllBanks 
                    ? (await bankService.GetAllAsync()).Select(b => b.Name).ToList()
                    : await GetSelectedBankNames(dto.SelectedBankIds)
            };

            return Ok(confirmation);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"An error occurred while submitting your application: {ex.Message}");
        }
    }

    [HttpGet("status/{applicationNumber}")]
    public async Task<IActionResult> GetApplicationStatus(string applicationNumber)
    {
        try
        {
            // Parse application number to get ID and type
            if (applicationNumber.StartsWith("LOAN-"))
            {
                var id = int.Parse(applicationNumber.Substring(5));
                var application = await loanApplicationService.GetByIdAsync(id);
                
                if (application == null) return NotFound();

                return Ok(new
                {
                    ApplicationNumber = applicationNumber,
                    Status = application.Status.ToString(),
                    ApplicantName = application.ApplicantName,
                    SubmissionDate = application.ApplicationDate,
                    LastUpdatedDate = application.LastUpdatedDate,
                    RejectionReason = application.RejectionReason
                });
            }
            else if (applicationNumber.StartsWith("DEPOSIT-"))
            {
                var id = int.Parse(applicationNumber.Substring(8));
                var application = await depositApplicationService.GetByIdAsync(id);
                
                if (application == null) return NotFound();

                return Ok(new
                {
                    ApplicationNumber = applicationNumber,
                    Status = application.Status.ToString(),
                    ApplicantName = application.ApplicantName,
                    SubmissionDate = application.ApplicationDate,
                    LastUpdatedDate = application.LastUpdatedDate,
                    RejectionReason = application.RejectionReason
                });
            }
            else
            {
                return BadRequest("Invalid application number format.");
            }
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"An error occurred while retrieving application status: {ex.Message}");
        }
    }

    private async Task<List<string>> GetSelectedBankNames(List<int> bankIds)
    {
        var allBanks = await bankService.GetAllAsync();
        return allBanks.Where(b => bankIds.Contains(b.Id)).Select(b => b.Name).ToList();
    }
}
