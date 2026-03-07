using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using CRMBanks.Core.Dtos;
using CRMBanks.Core.Services.Interfaces;

namespace CRMBanks.Web.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class DepositController(IDepositService depositService) : ControllerBase
{
    private int BankId => int.Parse(User.FindFirstValue("BankId") ?? "0");

    [HttpGet]
    public async Task<IActionResult> GetAll() =>
        Ok(await depositService.GetAllByBankAsync(BankId));

    [HttpGet("paged")]
    public async Task<IActionResult> GetPaged([FromQuery] PagedRequestDto request)
    {
        var result = await depositService.GetPagedByBankAsync(BankId, request);
        return Ok(result);
    }

    [HttpGet("select-list")]
    public async Task<IActionResult> GetSelectList()
    {
        var result = await depositService.GetSelectListAsync(BankId);
        return Ok(result);
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id)
    {
        var result = await depositService.GetByIdAsync(id);
        return result == null ? NotFound() : Ok(result);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] DepositDto dto)
    {
        var result = await depositService.CreateAsync(dto, BankId);
        return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, [FromBody] DepositDto dto)
    {
        var result = await depositService.UpdateAsync(id, dto);
        return result ? NoContent() : NotFound();
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var result = await depositService.DeleteAsync(id);
        return result ? NoContent() : NotFound();
    }
}
