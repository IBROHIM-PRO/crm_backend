namespace CRMBanks.Core.Dtos;

public class SignInDto
{
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}

public class VerifyCodeDto
{
    public int UserId { get; set; }
    public string Code { get; set; } = string.Empty;
}

public class AuthResponseDto
{
    public int UserId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Role { get; set; } = string.Empty;
    public int BankId { get; set; }
    public string Token { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
}

public class RecoverPasswordDto
{
    public string Email { get; set; } = string.Empty;
}

public class ChangePasswordDto
{
    public int UserId { get; set; }
    public string NewPassword { get; set; } = string.Empty;
}

public class PhoneLoginDto
{
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}

public class PhoneLoginResponseDto
{
    public string Token { get; set; } = string.Empty;
    public int UserId { get; set; }
    public string UserName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public DateTime ExpiresAt { get; set; }
}

/// <summary>
/// Simple login request with email and 6-digit code
/// </summary>
public class SimpleLoginRequest
{
    public string Email { get; set; } = string.Empty;
    public string ConfirmationCode { get; set; } = string.Empty;
}

/// <summary>
/// Simple login response containing JWT token
/// </summary>
public class SimpleLoginResponse
{
    public string Token { get; set; } = string.Empty;
    public DateTime ExpiresAt { get; set; }
    public UserInfo? User { get; set; }
}

/// <summary>
/// Basic user information
/// </summary>
public class UserInfo
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Role { get; set; } = string.Empty;
    public int BankId { get; set; }
}
