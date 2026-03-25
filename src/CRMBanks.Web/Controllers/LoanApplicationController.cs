using System.Security.Claims;
using CRMBanks.Core.Dtos;
using CRMBanks.Core.Services.Interfaces;
using CRMBanks.SharedKernel.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CRMBanks.Web.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class LoanApplicationController(
    ILoanApplicationService loanApplicationService,
    IEmailService emailService) : ControllerBase
{
    [HttpGet("filtered")]
    public async Task<IActionResult> GetFiltered([FromQuery] LoanApplicationFilterDto filter)
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        var roleClaim = User.FindFirst(ClaimTypes.Role)?.Value;
        
        if (userIdClaim == null || roleClaim == null)
            return BadRequest("User not authenticated");

        var userId = int.Parse(userIdClaim);
        var result = await loanApplicationService.GetFilteredAsync(filter, userId, roleClaim);
        return Ok(result);
    }

    [HttpGet("worker-available")]
    [Authorize(Roles = "worker")]
    public async Task<IActionResult> GetAvailableForWorker()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (userIdClaim == null)
            return BadRequest("User not authenticated");

        var userId = int.Parse(userIdClaim);
        var result = await loanApplicationService.GetAvailableForWorkerAsync(userId);
        return Ok(result);
    }

    [HttpGet("boss-all")]
    [Authorize(Roles = "boss")]
    public async Task<IActionResult> GetAllForBoss()
    {
        var result = await loanApplicationService.GetAllForBossAsync();
        return Ok(result);
    }

    [HttpGet("user/{userId:int}")]
    [Authorize(Roles = "client,user,admin,boss")]
    public async Task<IActionResult> GetByUser(int userId)
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        var roleClaim = User.FindFirst(ClaimTypes.Role)?.Value;
        
        if (userIdClaim == null || roleClaim == null)
            return BadRequest("User not authenticated");

        var currentUserId = int.Parse(userIdClaim);
        var currentRole = roleClaim.ToLower();
        
        // Users can only see their own applications
        if (currentRole == "client" || currentRole == "user")
        {
            if (currentUserId != userId)
                return Forbid();
        }

        var result = await loanApplicationService.GetFilteredAsync(new LoanApplicationFilterDto { UserId = userId }, userId, roleClaim);
        return Ok(result);
    }

    [HttpGet("worker/{userId:int}")]
    [Authorize(Roles = "worker,admin,boss")]
    public async Task<IActionResult> GetForWorker(int userId)
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        var roleClaim = User.FindFirst(ClaimTypes.Role)?.Value;
        
        if (userIdClaim == null || roleClaim == null)
            return BadRequest("User not authenticated");

        var currentUserId = int.Parse(userIdClaim);
        var currentRole = roleClaim.ToLower();
        
        // Workers can only see their own assigned applications
        if (currentRole == "worker")
        {
            if (currentUserId != userId)
                return Forbid();
        }

        var result = await loanApplicationService.GetAvailableForWorkerAsync(userId);
        return Ok(result);
    }

    [HttpPost("{id:int}/action")]
    [Authorize(Roles = "worker,boss")]
    public async Task<IActionResult> ProcessAction(int id, [FromBody] LoanApplicationActionDto dto)
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (userIdClaim == null)
            return BadRequest("User not authenticated");

        var userId = int.Parse(userIdClaim);
        var result = await loanApplicationService.UpdateStatusAsync(id, new UpdateLoanApplicationStatusDto { Status = ApplicationStatus.Approved, Comments = dto.Comments }, userId);
        return result ? NoContent() : BadRequest("Failed to process action");
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id)
    {
        var result = await loanApplicationService.GetByIdAsync(id);
        return result == null ? NotFound() : Ok(result);
    }

    [HttpPost]
    [Authorize(Roles = "client,user")]
    public async Task<IActionResult> Create([FromBody] CreateLoanApplicationDto dto)
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (userIdClaim == null)
            return BadRequest("User not authenticated");

        var userId = int.Parse(userIdClaim);
        var result = await loanApplicationService.CreateAsync(dto, userId);
        return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
    }

    [HttpPatch("{id:int}/status")]
    [Authorize(Roles = "worker,boss,admin")]
    public async Task<IActionResult> UpdateStatus(int id, [FromBody] UpdateLoanApplicationStatusDto dto)
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (userIdClaim == null)
            return BadRequest("User not authenticated");

        var userId = int.Parse(userIdClaim);
        
        // Get the application before updating to check status change
        var oldApplication = await loanApplicationService.GetByIdAsync(id);
        if (oldApplication == null)
            return NotFound();

        var result = await loanApplicationService.UpdateStatusAsync(id, dto, userId);
        
        if (result)
        {
            // Get the updated application
            var updatedApplication = await loanApplicationService.GetByIdAsync(id);
            if (updatedApplication != null)
            {
                // Send email based on status change
                if (oldApplication.Status != ApplicationStatus.Approved && updatedApplication.Status == ApplicationStatus.Approved)
                {
                    // TODO: Implement email notification
                    // await emailService.SendLoanApplicationApprovedAsync(updatedApplication);
                }
                else if (oldApplication.Status != ApplicationStatus.Rejected && updatedApplication.Status == ApplicationStatus.Rejected)
                {
                    // TODO: Implement email notification
                    // await emailService.SendLoanApplicationRejectedAsync(updatedApplication);
                }
            }
            
            return NoContent();
        }
        
        return BadRequest("Failed to update status");
    }

    [HttpDelete("{id:int}")]
    [Authorize(Roles = "admin,boss")]
    public async Task<IActionResult> Delete(int id)
    {
        var result = await loanApplicationService.DeleteAsync(id);
        return result ? NoContent() : NotFound();
    }
}
