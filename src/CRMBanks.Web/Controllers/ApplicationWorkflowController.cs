using System.Security.Claims;
using CRMBanks.Core.Dtos;
using CRMBanks.Core.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CRMBanks.Web.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ApplicationWorkflowController(IApplicationWorkflowService workflowService) : ControllerBase
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

    // Claim & Lock Mechanism
    [HttpPost("claim/{applicationType}/{applicationId:int}")]
    [Authorize(Roles = "worker,boss,admin")]
    public async Task<IActionResult> ClaimApplication(int applicationId, string applicationType, [FromBody] ClaimApplicationDto dto)
    {
        if (dto.WorkerId != GetCurrentUserId() && GetUserRole() != "admin")
            return Forbid();

        var result = await workflowService.ClaimApplicationAsync(applicationId, dto.WorkerId, applicationType, dto.Comments);
        return Ok(result);
    }

    [HttpPost("release/{applicationType}/{applicationId:int}")]
    [Authorize(Roles = "worker,boss,admin")]
    public async Task<IActionResult> ReleaseApplication(int applicationId, string applicationType)
    {
        var workerId = GetCurrentUserId();
        var result = await workflowService.ReleaseApplicationAsync(applicationId, workerId, applicationType);
        return result ? Ok() : BadRequest("Failed to release application");
    }

    // Status Management
    [HttpPut("status/{applicationType}/{applicationId:int}")]
    [Authorize(Roles = "worker,boss,admin")]
    public async Task<IActionResult> UpdateApplicationStatus(int applicationId, string applicationType, [FromBody] UpdateApplicationStatusDto dto)
    {
        var workerId = GetCurrentUserId();
        var result = await workflowService.UpdateApplicationStatusAsync(applicationId, dto, workerId, applicationType);
        return result ? Ok() : BadRequest("Failed to update status");
    }

    [HttpPost("escalate/{applicationType}/{applicationId:int}")]
    [Authorize(Roles = "worker,boss,admin")]
    public async Task<IActionResult> EscalateApplication(int applicationId, string applicationType, [FromBody] EscalateApplicationDto dto)
    {
        if (dto.WorkerId != GetCurrentUserId() && GetUserRole() != "admin")
            return Forbid();

        var result = await workflowService.EscalateApplicationAsync(dto, applicationType);
        return result ? Ok() : BadRequest("Failed to escalate application");
    }

    // Internal Notes
    [HttpPost("notes/{applicationType}/{applicationId:int}")]
    [Authorize(Roles = "worker,boss,admin")]
    public async Task<IActionResult> AddInternalNote(int applicationId, string applicationType, [FromBody] AddInternalNoteDto dto)
    {
        if (dto.WorkerId != GetCurrentUserId() && GetUserRole() != "admin")
            return Forbid();

        dto.ApplicationId = applicationId;
        var result = await workflowService.AddInternalNoteAsync(dto);
        return result ? Ok() : BadRequest("Failed to add note");
    }

    [HttpGet("notes/{applicationType}/{applicationId:int}")]
    [Authorize(Roles = "worker,boss,admin")]
    public async Task<IActionResult> GetInternalNotes(int applicationId, string applicationType)
    {
        var notes = await workflowService.GetInternalNotesAsync(applicationId, applicationType);
        return Ok(notes);
    }

    [HttpPut("notes/{noteId:int}")]
    [Authorize(Roles = "worker,boss,admin")]
    public async Task<IActionResult> UpdateInternalNote(int noteId, [FromBody] UpdateNoteDto dto)
    {
        var workerId = GetCurrentUserId();
        var result = await workflowService.UpdateInternalNoteAsync(noteId, dto.Note, workerId);
        return result ? Ok() : BadRequest("Failed to update note");
    }

    [HttpDelete("notes/{noteId:int}")]
    [Authorize(Roles = "worker,boss,admin")]
    public async Task<IActionResult> DeleteInternalNote(int noteId)
    {
        var workerId = GetCurrentUserId();
        var result = await workflowService.DeleteInternalNoteAsync(noteId, workerId);
        return result ? Ok() : BadRequest("Failed to delete note");
    }

    // Verification Checklist
    [HttpGet("checklist/{applicationType}/{applicationId:int}")]
    [Authorize(Roles = "worker,boss,admin")]
    public async Task<IActionResult> GetVerificationChecklist(int applicationId, string applicationType)
    {
        var checklist = await workflowService.GetVerificationChecklistAsync(applicationId, applicationType);
        return Ok(checklist);
    }

    [HttpPost("checklist/{applicationType}/{applicationId:int}")]
    [Authorize(Roles = "worker,boss,admin")]
    public async Task<IActionResult> InitializeChecklist(int applicationId, string applicationType)
    {
        var result = await workflowService.InitializeDefaultChecklistAsync(applicationId, applicationType);
        return result ? Ok() : BadRequest("Failed to initialize checklist");
    }

    [HttpPut("checklist/items/{itemId:int}")]
    [Authorize(Roles = "worker,boss,admin")]
    public async Task<IActionResult> UpdateVerificationItem(int itemId, [FromBody] UpdateVerificationItemDto dto)
    {
        var workerId = GetCurrentUserId();
        var result = await workflowService.UpdateVerificationItemAsync(itemId, dto.IsCompleted, workerId, dto.Comments);
        return result ? Ok() : BadRequest("Failed to update checklist item");
    }

    // Audit Trail / History
    [HttpGet("history/{applicationType}/{applicationId:int}")]
    [Authorize(Roles = "worker,boss,admin")]
    public async Task<IActionResult> GetApplicationHistory(int applicationId, string applicationType)
    {
        var history = await workflowService.GetApplicationHistoryAsync(applicationId, applicationType);
        return Ok(history);
    }

    // Enhanced Application Retrieval
    [HttpGet("enhanced/loan/{id:int}")]
    [Authorize(Roles = "worker,boss,admin")]
    public async Task<IActionResult> GetEnhancedLoanApplication(int id)
    {
        var currentUserId = GetCurrentUserId();
        var userRole = GetUserRole();
        var application = await workflowService.GetEnhancedLoanApplicationAsync(id, currentUserId, userRole);
        return application == null ? NotFound() : Ok(application);
    }

    [HttpGet("enhanced/deposit/{id:int}")]
    [Authorize(Roles = "worker,boss,admin")]
    public async Task<IActionResult> GetEnhancedDepositApplication(int id)
    {
        var currentUserId = GetCurrentUserId();
        var userRole = GetUserRole();
        var application = await workflowService.GetEnhancedDepositApplicationAsync(id, currentUserId, userRole);
        return application == null ? NotFound() : Ok(application);
    }
}

// Supporting DTOs for controller
public class UpdateNoteDto
{
    public string Note { get; set; } = string.Empty;
}

public class UpdateVerificationItemDto
{
    public bool IsCompleted { get; set; }
    public string? Comments { get; set; }
}
