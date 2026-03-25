using CRMBanks.Core.Dtos;
using CRMBanks.Core.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CRMBanks.Web.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class UserController(IUserService userService) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetAll() =>
        Ok(await userService.GetAllAsync());

    [HttpGet("paged")]
    public async Task<IActionResult> GetPaged([FromQuery] PagedRequestDto request)
    {
        var result = await userService.GetPagedAsync(request);
        return Ok(result);
    }

    [HttpGet("select-list")]
    public async Task<IActionResult> GetSelectList()
    {
        var result = await userService.GetSelectListAsync();
        return Ok(result);
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id)
    {
        var result = await userService.GetByIdAsync(id);
        return result == null ? NotFound() : Ok(result);
    }

    [HttpPost]
    [Authorize(Roles = "admin")]
    public async Task<IActionResult> Create([FromBody] UserDto dto)
    {
        var result = await userService.CreateAsync(dto);
        return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
    }

    [HttpPut("{id:int}")]
    [Authorize(Roles = "admin")]
    public async Task<IActionResult> Update(int id, [FromBody] UserDto dto)
    {
        var result = await userService.UpdateAsync(id, dto);
        return result ? NoContent() : NotFound();
    }

    [HttpPut("{id:int}/permissions")]
    [HttpPatch("{id:int}/permissions")]
    [Authorize(Roles = "boss,admin")]
    public async Task<IActionResult> UpdatePermissions(int id, [FromBody] UserPermissionsDto dto)
    {
        // TODO: Implement permissions update functionality
        return Ok("Permissions update not yet implemented");
    }

    [HttpDelete("{id:int}")]
    [Authorize(Roles = "admin")]
    public async Task<IActionResult> Delete(int id)
    {
        var result = await userService.DeleteAsync(id);
        return result ? NoContent() : NotFound();
    }
}
