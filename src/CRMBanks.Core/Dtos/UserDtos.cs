using CRMBanks.Core.Entities;

namespace CRMBanks.Core.Dtos;

public class UserDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public string Email { get; set; } = "";
    public string Password { get; set; } = "";
    public int BankId { get; set; }
    public BankDto? Bank { get; set; }
    public int RoleId { get; set; }
    public RoleDto? Role { get; set; }
    public int AzSum { get; set; }
    public int ToSum { get; set; }
    public decimal MaxLoanAmount { get; set; } = 0;
    public string Token { get; set; } = "";
    public List<int> RegionIds { get; set; } = new();
    public List<RegionDto> Regions { get; set; } = new();
}

public class UserPermissionsDto
{
    public List<int> RegionIds { get; set; } = new();
    public decimal MaxLoanAmount { get; set; } = 0;
    public int BankId { get; set; }
}

