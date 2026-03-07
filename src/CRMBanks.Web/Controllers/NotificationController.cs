using System.Security.Claims;
using CRMBanks.Core.Services.Interfaces;
using CRMBanks.SharedKernel.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CRMBanks.Web.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class NotificationController(INotificationService notificationService) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetMine()
    {
        var userId = GetUserId();
        if (userId == null) return Unauthorized();
        var result = await notificationService.GetByUserIdAsync(userId.Value);
        return Ok(result);
    }

    [HttpGet("unseen-count")]
    public async Task<IActionResult> GetUnseenCount()
    {
        var userId = GetUserId();
        if (userId == null) return Unauthorized();
        var count = await notificationService.GetUnseenCountAsync(userId.Value);
        return Ok(new { count });
    }

    [HttpPatch("{id:int}/status")]
    public async Task<IActionResult> UpdateStatus(int id, [FromQuery] NotificationStatus status)
    {
        var result = await notificationService.UpdateStatusAsync(id, status);
        return result ? NoContent() : NotFound();
    }

    [HttpPatch("seen-all")]
    public async Task<IActionResult> MarkAllSeen()
    {
        var userId = GetUserId();
        if (userId == null) return Unauthorized();
        await notificationService.MarkAllSeenAsync(userId.Value);
        return NoContent();
    }

    private int? GetUserId()
    {
        var claim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        return int.TryParse(claim, out var id) ? id : null;
    }
}
