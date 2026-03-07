using CRMBanks.Core.Dtos;
using CRMBanks.Core.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace CRMBanks.Web.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController(IAuthService authService) : ControllerBase
{
    [HttpPost("sign-in")]
    public async Task<IActionResult> SignIn([FromBody] SignInDto dto)
    {
        var (success, message, userId, email) = await authService.SignInAsync(dto);
        if (!success)
            return Unauthorized(new { message });

        return Ok(new { message, userId, email });
    }

    [HttpPost("verify")]
    public async Task<IActionResult> Verify([FromBody] VerifyCodeDto dto)
    {
        var result = await authService.VerifyCodeAsync(dto);
        if (result == null)
            return Unauthorized(new { message = "Код нодуруст ё мӯҳлаташ гузаштааст" });

        return Ok(result);
    }

    [HttpPost("resend-code")]
    public async Task<IActionResult> ResendCode([FromQuery] int userId, [FromQuery] string email)
    {
        await authService.ResendCodeAsync(userId, email);
        return Ok(new { message = "Рамзи нав фиристода шуд" });
    }

    [HttpPost("recover-password")]
    public async Task<IActionResult> RecoverPassword([FromBody] RecoverPasswordDto dto)
    {
        var userId = await authService.RecoverPasswordAsync(dto);
        if (userId == null)
            return NotFound(new { message = "Email ёфт нашуд" });

        return Ok(new { userId });
    }

    [HttpPost("change-password")]
    public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordDto dto)
    {
        var result = await authService.ChangePasswordAsync(dto);
        if (!result)
            return NotFound(new { message = "Корбар ёфт нашуд" });

        return Ok(new { message = "Рамз иваз шуд" });
    }

    [HttpPost("login")]
    public async Task<IActionResult> LoginByPhone([FromBody] PhoneLoginDto dto)
    {
        var result = await authService.LoginByPhoneAsync(dto);
        if (result == null)
            return Unauthorized(new { message = "Email ё рамз нодуруст аст" });

        return Ok(result);
    }
}
