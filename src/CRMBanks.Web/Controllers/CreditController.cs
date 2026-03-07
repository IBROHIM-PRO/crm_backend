using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using CRMBanks.Core.Dtos;
using CRMBanks.Core.Services.Interfaces;

namespace CRMBanks.Web.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class CreditController(ICreditService creditService) : ControllerBase
{
    private int BankId => int.Parse(User.FindFirstValue("BankId") ?? "0");

    [HttpGet]
    public async Task<IActionResult> GetAll() =>
        Ok(await creditService.GetAllByBankAsync(BankId));

    [HttpGet("paged")]
    public async Task<IActionResult> GetPaged([FromQuery] PagedRequestDto request)
    {
        var result = await creditService.GetPagedByBankAsync(BankId, request);
        return Ok(result);
    }

    [HttpGet("select-list")]
    public async Task<IActionResult> GetSelectList()
    {
        var result = await creditService.GetSelectListAsync(BankId);
        return Ok(result);
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id)
    {
        var result = await creditService.GetByIdAsync(id);
        return result == null ? NotFound() : Ok(result);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreditDto dto)
    {
        var result = await creditService.CreateAsync(dto, BankId);
        return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, [FromBody] CreditDto dto)
    {
        var result = await creditService.UpdateAsync(id, dto);
        return result ? NoContent() : NotFound();
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var result = await creditService.DeleteAsync(id);
        return result ? NoContent() : NotFound();
    }
}
