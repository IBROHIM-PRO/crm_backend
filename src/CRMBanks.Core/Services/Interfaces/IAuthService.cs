using CRMBanks.Core.Dtos;

namespace CRMBanks.Core.Services.Interfaces;

public interface IAuthService
{
    Task<(bool Success, string Message, int? UserId, string? Email)> SignInAsync(SignInDto dto);
    Task<AuthResponseDto?> VerifyCodeAsync(VerifyCodeDto dto);
    Task<bool> ResendCodeAsync(int userId, string email);
    Task<int?> RecoverPasswordAsync(RecoverPasswordDto dto);
    Task<bool> ChangePasswordAsync(ChangePasswordDto dto);
    Task<PhoneLoginResponseDto?> LoginByPhoneAsync(PhoneLoginDto dto);
}
