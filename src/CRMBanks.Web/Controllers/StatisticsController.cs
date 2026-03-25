using System.Security.Claims;
using CRMBanks.Core.Dtos;
using CRMBanks.Core.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CRMBanks.Web.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class StatisticsController(IStatisticsService statisticsService) : ControllerBase
{
    [HttpGet("global")]
    [Authorize(Roles = "boss,admin")]
    public async Task<IActionResult> GetGlobalStatistics([FromQuery] DateTime? from = null, [FromQuery] DateTime? to = null)
    {
        var result = await statisticsService.GetGlobalStatisticsAsync();
        return Ok(result);
    }

    [HttpGet("workers")]
    [Authorize(Roles = "boss")]
    public async Task<IActionResult> GetWorkerStatistics([FromQuery] DateTime? from = null, [FromQuery] DateTime? to = null)
    {
        // For now, return global stats as worker stats
        var result = await statisticsService.GetGlobalStatisticsAsync();
        return Ok(result.WorkerPerformances);
    }

    [HttpGet("personal/{userId:int}")]
    [Authorize]
    public async Task<IActionResult> GetPersonalStatistics(int userId, [FromQuery] DateTime? from = null, [FromQuery] DateTime? to = null)
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        var roleClaim = User.FindFirst(ClaimTypes.Role)?.Value;
        
        if (userIdClaim == null || roleClaim == null)
            return BadRequest("User not authenticated");

        var currentUserId = int.Parse(userIdClaim);
        var currentRole = roleClaim.ToLower();
        
        // Users can only see their own statistics
        if (currentRole == "client" || currentRole == "user" || currentRole == "worker")
        {
            if (currentUserId != userId)
                return Forbid();
        }

        var result = await statisticsService.GetPersonalStatisticsAsync(userId);
        return Ok(result);
    }

    [HttpGet("personal")]
    [Authorize(Roles = "worker,boss,admin")]
    public async Task<IActionResult> GetPersonalStatistics([FromQuery] DateTime? from = null, [FromQuery] DateTime? to = null)
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (userIdClaim == null)
            return BadRequest("User not authenticated");

        var userId = int.Parse(userIdClaim);
        var result = await statisticsService.GetPersonalStatisticsAsync(userId);
        return Ok(result);
    }
}
