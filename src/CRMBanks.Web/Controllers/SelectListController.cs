using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using CRMBanks.Core.Services.Interfaces;

namespace CRMBanks.Web.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class SelectListController(ISelectListService selectListService) : ControllerBase
{
    private int BankId => int.Parse(User.FindFirstValue("BankId") ?? "0");

    [HttpGet("roles")]
    public async Task<IActionResult> GetRoles() =>
        Ok(await selectListService.GetRolesAsync());

    [HttpGet("banks")]
    public async Task<IActionResult> GetBanks() =>
        Ok(await selectListService.GetBanksAsync());

    [HttpGet("regions")]
    public async Task<IActionResult> GetRegions() =>
        Ok(await selectListService.GetRegionsAsync());

    [HttpGet("type-credits")]
    public async Task<IActionResult> GetTypeCredits() =>
        Ok(await selectListService.GetTypeCreditsAsync());

    [HttpGet("type-deposits")]
    public async Task<IActionResult> GetTypeDeposits() =>
        Ok(await selectListService.GetTypeDepositsAsync());

    [HttpGet("users")]
    public async Task<IActionResult> GetUsers() =>
        Ok(await selectListService.GetUsersAsync(BankId));

    [HttpGet("credits")]
    public async Task<IActionResult> GetCredits() =>
        Ok(await selectListService.GetCreditsAsync(BankId));

    [HttpGet("deposits")]
    public async Task<IActionResult> GetDeposits() =>
        Ok(await selectListService.GetDepositsAsync(BankId));

    [HttpGet("cards")]
    public async Task<IActionResult> GetCards() =>
        Ok(await selectListService.GetCardsAsync(BankId));

    [HttpGet("requests")]
    public async Task<IActionResult> GetRequests() =>
        Ok(await selectListService.GetRequestsAsync(BankId));
}
