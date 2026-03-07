using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using CRMBanks.Core.Common;
using CRMBanks.Core.Dtos;
using CRMBanks.Core.Entities;
using CRMBanks.Core.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace CRMBanks.Core.Services;

public class AuthService(IRepository<User> userRepository,
    IRepository<Auth2F> auth2FRepository,
    IEmailService emailService,
    IConfiguration configuration) : IAuthService
{
    public async Task<(bool Success, string Message, int? UserId, string? Email)> SignInAsync(SignInDto dto)
    {
        var user = await userRepository.GetQuery()
            .Include(u => u.Role)
            .FirstOrDefaultAsync(u => u.Email == dto.Email && u.Password == dto.Password);

        if (user == null)
            return (false, "Email ё Password хатост!", null, null);

        var code = new Random().Next(100000, 999999).ToString();
        emailService.Send(dto.Email, "Коди тасдиқ", $"Коди воридшавӣ: {code}");

        await auth2FRepository.AddAsync(new Auth2F
        {
            UserId = user.Id,
            Code = int.Parse(code),
            DateTimeSendCode = DateTimeOffset.UtcNow,
            IsEnabled = true
        });
        await userRepository.SaveChangesAsync();

        return (true, "Код фиристода шуд", user.Id, user.Email);
    }

    public async Task<AuthResponseDto?> VerifyCodeAsync(VerifyCodeDto dto)
    {
        var auth = await auth2FRepository.GetQuery()
            .Include(a => a.Users)
            .ThenInclude(u => u!.Role)
            .Where(a => a.UserId == dto.UserId && a.IsEnabled)
            .OrderByDescending(a => a.DateTimeSendCode)
            .FirstOrDefaultAsync();

        if (auth == null) return null;
        if (auth.Code.ToString() != dto.Code) return null;
        if (DateTimeOffset.UtcNow > auth.DateTimeSendCode.AddMinutes(5)) return null;

        auth.IsEnabled = false;
        await userRepository.SaveChangesAsync();

        // Создаем JWT токен
        var token = GenerateJwtToken(auth.Users!);

        return new AuthResponseDto
        {
            UserId = auth.Users!.Id,
            Name = auth.Users.Name,
            Role = auth.Users.Role?.Name ?? string.Empty,
            BankId = auth.Users.BankId,
            Token = token,
            Email = auth.Users.Email,
            Phone = auth.Users.Phone
        };
    }

    private string GenerateJwtToken(User user)
    {
        var jwtSettings = configuration.GetSection("JwtSettings");
        var secretKey = jwtSettings["SecretKey"];
        if (secretKey == null) throw new Exception("Token secret key not found");
        var key = Encoding.UTF8.GetBytes(secretKey);

        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Name, user.Name),
            new Claim(ClaimTypes.Email, user.Email),
            new Claim(ClaimTypes.Role, user.Role?.Name ?? "User"),
            new Claim("BankId", user.BankId.ToString()),
            new Claim("Phone", user.Phone)
        };

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Expires = DateTime.UtcNow.AddMinutes(int.Parse(jwtSettings["ExpirationInMinutes"] ?? "60")),
            Issuer = jwtSettings["Issuer"] ?? "CRMBanks",
            Audience = jwtSettings["Audience"] ?? "CRMBanksUsers",
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
        };

        var tokenHandler = new JwtSecurityTokenHandler();
        var token = tokenHandler.CreateToken(tokenDescriptor);
        return tokenHandler.WriteToken(token);
    }

    public async Task<bool> ResendCodeAsync(int userId, string email)
    {
        var code = new Random().Next(100000, 999999).ToString();
        await auth2FRepository.AddAsync(new Auth2F
        {
            UserId = userId,
            Code = int.Parse(code),
            DateTimeSendCode = DateTimeOffset.UtcNow,
            IsEnabled = true
        });
        await userRepository.SaveChangesAsync();
        emailService.Send(email, "Тасдиқи Рамз", $"Коди тасдиқ: {code}");
        return true;
    }

    public async Task<int?> RecoverPasswordAsync(RecoverPasswordDto dto)
    {
        var user = await userRepository.GetQuery()
            .FirstOrDefaultAsync(u => u.Email == dto.Email);
        return user?.Id;
    }

    public async Task<bool> ChangePasswordAsync(ChangePasswordDto dto)
    {
        var user = await userRepository.GetQuery().FirstOrDefaultAsync(x=>x.Id==dto.UserId);
        if (user == null) return false;

        user.Password = dto.NewPassword;
        userRepository.Update(user);
        await userRepository.SaveChangesAsync();
        return true;
    }

    public async Task<PhoneLoginResponseDto?> LoginByPhoneAsync(PhoneLoginDto dto)
    {
        var user = await userRepository.GetQuery()
            .Include(u => u.Role)
            .FirstOrDefaultAsync(u => u.Email == dto.Email && u.Password == dto.Password);

        if (user == null) return null;

        var token = GenerateJwtToken(user);
        var expiresAt = DateTime.UtcNow.AddHours(24);

        return new PhoneLoginResponseDto
        {
            Token = token,
            UserId = user.Id,
            UserName = user.Name,
            Email = user.Email,
            ExpiresAt = expiresAt
        };
    }
}
