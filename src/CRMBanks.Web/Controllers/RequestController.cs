using System.Security.Claims;
using CRMBanks.Core.Dtos;
using CRMBanks.Core.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CRMBanks.Web.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class RequestController(IRequestService requestService) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetAll() =>
        Ok(await requestService.GetAllAsync());

    [HttpGet("paged")]
    public async Task<IActionResult> GetPaged([FromQuery] PagedRequestDto request)
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        var userId = int.Parse(userIdClaim!);
        if (userIdClaim == null) return BadRequest(new { message = "User ID not found" });
        var result = await requestService.GetPagedAsync(request, userId);
        return Ok(result);
    }

    [HttpGet("select-list")]
    public async Task<IActionResult> GetSelectList()
    {
        var result = await requestService.GetSelectListAsync();
        return Ok(result);
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id)
    {
        var result = await requestService.GetByIdAsync(id);
        return result == null ? NotFound() : Ok(result);
    }

    [HttpPost]
    [AllowAnonymous]
    public async Task<IActionResult> Create([FromBody] RequestDto dto)
    {
        var result = await requestService.CreateAsync(dto);
        return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
    }

    [HttpPatch("{id:int}/state")]
    public async Task<IActionResult> UpdateState(int id, [FromBody] UpdateStateWithReasonDto dto)
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        int? userId = int.TryParse(userIdClaim, out var uid) ? uid : null;
        var result = await requestService.UpdateStateAsync(id, dto.State, dto.Reason, userId);
        return result ? NoContent() : BadRequest(new { message = "Ҳолат нодуруст аст" });
    }

    [HttpDelete("{id:int}")]
    [Authorize(Roles = "admin")]
    public async Task<IActionResult> Delete(int id)
    {
        var result = await requestService.DeleteAsync(id);
        return result ? NoContent() : NotFound();
    }
}
