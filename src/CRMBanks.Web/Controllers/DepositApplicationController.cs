using System.Security.Claims;
using CRMBanks.Core.Dtos;
using CRMBanks.Core.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CRMBanks.Web.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class DepositApplicationController(IDepositApplicationService depositApplicationService) : ControllerBase
{
    [HttpGet("filtered")]
    public async Task<IActionResult> GetFiltered([FromQuery] DepositApplicationFilterDto filter)
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        var roleClaim = User.FindFirst(ClaimTypes.Role)?.Value;
        
        if (userIdClaim == null || roleClaim == null)
            return BadRequest("User not authenticated");

        var userId = int.Parse(userIdClaim);
        var result = await depositApplicationService.GetFilteredAsync(filter, userId, roleClaim);
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
        var result = await depositApplicationService.GetAvailableForWorkerAsync(userId);
        return Ok(result);
    }

    [HttpGet("boss-all")]
    [Authorize(Roles = "boss")]
    public async Task<IActionResult> GetAllForBoss()
    {
        var result = await depositApplicationService.GetAllForBossAsync();
        return Ok(result);
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id)
    {
        var result = await depositApplicationService.GetByIdAsync(id);
        return result == null ? NotFound() : Ok(result);
    }

    [HttpPatch("{id:int}/status")]
    [Authorize(Roles = "worker,boss,admin")]
    public async Task<IActionResult> UpdateStatus(int id, [FromBody] UpdateDepositApplicationStatusDto dto)
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (userIdClaim == null)
            return BadRequest("User not authenticated");

        var userId = int.Parse(userIdClaim);
        var result = await depositApplicationService.UpdateStatusAsync(id, dto, userId);
        return result ? NoContent() : BadRequest("Failed to update status");
    }

    [HttpDelete("{id:int}")]
    [Authorize(Roles = "admin,boss")]
    public async Task<IActionResult> Delete(int id)
    {
        var result = await depositApplicationService.DeleteAsync(id);
        return result ? NoContent() : NotFound();
    }
}
