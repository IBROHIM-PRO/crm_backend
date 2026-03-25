using CRMBanks.Core.Dtos;
using CRMBanks.Core.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace CRMBanks.Web.Controllers;

[ApiController]
[Route("api/public")]
public class PublicController(
    IBankService bankService,
    ICreditService creditService,
    IDepositService depositService,
    IRegionService regionService,
    ILoanApplicationService loanApplicationService,
    IEmailService emailService) : ControllerBase
{
    [HttpGet("banks")]
    public async Task<IActionResult> GetAllBanks()
    {
        var banks = await bankService.GetAllAsync();
        var credits = await GetCreditsFromAllBanks();
        var deposits = await GetDepositsFromAllBanks();

        var publicBanks = banks.Select(bank => new PublicBankDto
        {
            Id = bank.Id,
            Name = bank.Name,
            Phone = bank.Phone,
            Address = bank.Address,
            Email = bank.Email,
            Credits = credits.Where(c => c.BankId == bank.Id).Select(credit => new PublicCreditDto
            {
                Id = credit.Id,
                Name = credit.Name,
                Description = credit.Description,
                Foiz = credit.Foiz,
                AzSum = credit.AzSum,
                ToSum = credit.ToSum,
                TypeSum = credit.TypeSumId,
                AzSana = credit.AzSana,
                ToSana = credit.ToSana,
                InfoProt = credit.Infoprot,
                TypeCreditName = credit.TypeCredit?.Name ?? "",
                Products = new List<PublicProductDto>() // Empty for now
            }).ToList(),
            Deposits = deposits.Where(d => d.BankId == bank.Id).Select(deposit => new PublicDepositDto
            {
                Id = deposit.Id,
                Name = deposit.Name,
                Description = deposit.Description,
                Foiz = deposit.Foiz,
                AzSum = deposit.AzSum,
                ToSum = deposit.ToSum,
                TypeSum = deposit.TypeSumId,
                AzSana = deposit.AzSana,
                ToSana = deposit.ToSana,
                InfoProt = deposit.Infoprot,
                TypeDepositName = deposit.TypeDeposit?.Name ?? "",
                Products = new List<PublicProductDto>() // Empty for now
            }).ToList()
        }).ToList();

        return Ok(publicBanks);
    }

    [HttpGet("banks/{id:int}")]
    public async Task<IActionResult> GetBankById(int id)
    {
        var bank = await bankService.GetByIdAsync(id);
        if (bank == null) return NotFound();

        var credits = await creditService.GetAllByBankAsync(id);
        var deposits = await depositService.GetAllByBankAsync(id);

        var publicBank = new PublicBankDto
        {
            Id = bank.Id,
            Name = bank.Name,
            Phone = bank.Phone,
            Address = bank.Address,
            Email = bank.Email,
            Credits = credits.Where(c => c.BankId == bank.Id).Select(credit => new PublicCreditDto
            {
                Id = credit.Id,
                Name = credit.Name,
                Description = credit.Description,
                Foiz = credit.Foiz,
                AzSum = credit.AzSum,
                ToSum = credit.ToSum,
                TypeSum = credit.TypeSumId,
                AzSana = credit.AzSana,
                ToSana = credit.ToSana,
                InfoProt = credit.Infoprot,
                TypeCreditName = credit.TypeCredit?.Name ?? "",
                Products = new List<PublicProductDto>() // Empty for now
            }).ToList(),
            Deposits = deposits.Where(d => d.BankId == bank.Id).Select(deposit => new PublicDepositDto
            {
                Id = deposit.Id,
                Name = deposit.Name,
                Description = deposit.Description,
                Foiz = deposit.Foiz,
                AzSum = deposit.AzSum,
                ToSum = deposit.ToSum,
                TypeSum = deposit.TypeSumId,
                AzSana = deposit.AzSana,
                ToSana = deposit.ToSana,
                InfoProt = deposit.Infoprot,
                TypeDepositName = deposit.TypeDeposit?.Name ?? "",
                Products = new List<PublicProductDto>() // Empty for now
            }).ToList()
        };

        return Ok(publicBank);
    }

    [HttpGet("credits")]
    public async Task<IActionResult> GetAllCredits()
    {
        var credits = await GetCreditsFromAllBanks();
        var publicCredits = credits.Select(credit => new PublicCreditDto
        {
            Id = credit.Id,
            Name = credit.Name,
            Description = credit.Description,
            Foiz = credit.Foiz,
            AzSum = credit.AzSum,
            ToSum = credit.ToSum,
            TypeSum = credit.TypeSumId,
            AzSana = credit.AzSana,
            ToSana = credit.ToSana,
            InfoProt = credit.Infoprot,
            TypeCreditName = credit.TypeCredit?.Name ?? "",
            Products = new List<PublicProductDto>() // Empty for now
        }).ToList();

        return Ok(publicCredits);
    }

    [HttpGet("deposits")]
    public async Task<IActionResult> GetAllDeposits()
    {
        var deposits = await GetDepositsFromAllBanks();
        var publicDeposits = deposits.Select(deposit => new PublicDepositDto
        {
            Id = deposit.Id,
            Name = deposit.Name,
            Description = deposit.Description,
            Foiz = deposit.Foiz,
            AzSum = deposit.AzSum,
            ToSum = deposit.ToSum,
            TypeSum = deposit.TypeSumId,
            AzSana = deposit.AzSana,
            ToSana = deposit.ToSana,
            InfoProt = deposit.Infoprot,
            TypeDepositName = deposit.TypeDeposit?.Name ?? "",
            Products = new List<PublicProductDto>() // Empty for now
        }).ToList();

        return Ok(publicDeposits);
    }

    [HttpGet("regions")]
    public async Task<IActionResult> GetAllRegions()
    {
        var regions = await regionService.GetAllAsync();
        return Ok(regions);
    }

    [HttpGet("credits/{id:int}")]
    public async Task<IActionResult> GetCreditById(int id)
    {
        var credits = await GetCreditsFromAllBanks();
        var credit = credits.FirstOrDefault(c => c.Id == id);
        
        if (credit == null) return NotFound();
        
        var publicCredit = new PublicCreditDto
        {
            Id = credit.Id,
            Name = credit.Name,
            Description = credit.Description,
            Foiz = credit.Foiz,
            AzSum = credit.AzSum,
            ToSum = credit.ToSum,
            TypeSum = credit.TypeSumId,
            AzSana = credit.AzSana,
            ToSana = credit.ToSana,
            InfoProt = credit.Infoprot,
            TypeCreditName = credit.TypeCredit?.Name ?? "",
            Products = new List<PublicProductDto>()
        };
        
        return Ok(publicCredit);
    }

    [HttpGet("deposits/{id:int}")]
    public async Task<IActionResult> GetDepositById(int id)
    {
        var deposits = await GetDepositsFromAllBanks();
        var deposit = deposits.FirstOrDefault(d => d.Id == id);
        
        if (deposit == null) return NotFound();
        
        var publicDeposit = new PublicDepositDto
        {
            Id = deposit.Id,
            Name = deposit.Name,
            Description = deposit.Description,
            Foiz = deposit.Foiz,
            AzSum = deposit.AzSum,
            ToSum = deposit.ToSum,
            TypeSum = deposit.TypeSumId,
            AzSana = deposit.AzSana,
            ToSana = deposit.ToSana,
            InfoProt = deposit.Infoprot,
            TypeDepositName = deposit.TypeDeposit?.Name ?? "",
            Products = new List<PublicProductDto>()
        };
        
        return Ok(publicDeposit);
    }

    [HttpPost("loan-application")]
    public async Task<IActionResult> CreateLoanApplication([FromBody] CreateLoanApplicationDto dto)
    {
        try
        {
            // Create the loan application (without user ID for public applications)
            var application = await loanApplicationService.CreateAsync(dto, 1); // Use system user ID
            
            // Send confirmation email
            var applicationDto = await loanApplicationService.GetByIdAsync(application.Id);
            if (applicationDto != null)
            {
                // TODO: Implement email notification
                // await emailService.SendLoanApplicationReceivedAsync(applicationDto);
            }
            
            // Trigger real-time notifications to eligible workers
            // TODO: Implement notification service
            // await notificationService.CreateForLoanApplicationAsync(application.Id);
            
            return CreatedAtAction(nameof(GetLoanApplicationById), new { id = application.Id }, application);
        }
        catch (Exception ex)
        {
            return BadRequest(new { error = "Failed to create loan application", details = ex.Message });
        }
    }

    [HttpPost("deposit-application")]
    public async Task<IActionResult> CreateDepositApplication([FromBody] CreateDepositApplicationDto dto)
    {
        try
        {
            // Create the deposit application (without user ID for public applications)
            // TODO: Use depositApplicationService when available
            // var application = await depositApplicationService.CreateAsync(dto, 1); // Use system user ID
            
            // TODO: Send confirmation email (similar to loan application)
            // Note: You might want to create a separate email method for deposits
            
            return Ok("Deposit application creation not yet implemented");
        }
        catch (Exception ex)
        {
            return BadRequest(new { error = "Failed to create deposit application", details = ex.Message });
        }
    }

    [HttpGet("loan-application/{id:int}")]
    public async Task<IActionResult> GetLoanApplicationById(int id)
    {
        var application = await loanApplicationService.GetByIdAsync(id);
        return application == null ? NotFound() : Ok(application);
    }

    [HttpGet("deposit-application/{id:int}")]
    public async Task<IActionResult> GetDepositApplicationById(int id)
    {
        // This would need to be implemented in the service layer
        // For now, return a placeholder response
        return Ok(new { message = "Deposit application endpoint", id });
    }

    private async Task<List<CreditDto>> GetCreditsFromAllBanks()
    {
        var banks = await bankService.GetAllAsync();
        var allCredits = new List<CreditDto>();
        
        foreach (var bank in banks)
        {
            var bankCredits = await creditService.GetAllByBankAsync(bank.Id);
            allCredits.AddRange(bankCredits);
        }
        
        return allCredits;
    }

    private async Task<List<DepositDto>> GetDepositsFromAllBanks()
    {
        var banks = await bankService.GetAllAsync();
        var allDeposits = new List<DepositDto>();
        
        foreach (var bank in banks)
        {
            var bankDeposits = await depositService.GetAllByBankAsync(bank.Id);
            allDeposits.AddRange(bankDeposits);
        }
        
        return allDeposits;
    }
}
