using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using CRMBanks.Core.Services.Interfaces;
using CRMBanks.Core.Dtos;

namespace CRMBanks.Web.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class BankController(IBankService bankService) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetAll() =>
        Ok(await bankService.GetAllAsync());

    [HttpGet("paged")]
    public async Task<IActionResult> GetPaged([FromQuery] PagedRequestDto request)
    {
        var result = await bankService.GetPagedAsync(request);
        return Ok(result);
    }

    [HttpGet("select-list")]
    public async Task<IActionResult> GetSelectList()
    {
        var result = await bankService.GetSelectListAsync();
        return Ok(result);
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id)
    {
        var result = await bankService.GetByIdAsync(id);
        return result == null ? NotFound() : Ok(result);
    }

    [HttpPost]
    [Authorize(Roles = "admin")]
    public async Task<IActionResult> Create([FromBody] BankDto dto)
    {
        var result = await bankService.CreateAsync(dto);
        return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
    }

    [HttpPut("{id:int}")]
    [Authorize(Roles = "admin")]
    public async Task<IActionResult> Update(int id, [FromBody] BankDto dto)
    {
        var result = await bankService.UpdateAsync(id, dto);
        return result ? NoContent() : NotFound();
    }

    [HttpDelete("{id:int}")]
    [Authorize(Roles = "admin")]
    public async Task<IActionResult> Delete(int id)
    {
        var result = await bankService.DeleteAsync(id);
        return result ? NoContent() : NotFound();
    }
}
