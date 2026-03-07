using CRMBanks.Core.Dtos;

namespace CRMBanks.Core.Services.Interfaces;

public interface IUserService
{
    Task<IEnumerable<UserDto>> GetAllAsync();
    Task<PagedResponseDto<UserDto>> GetPagedAsync(PagedRequestDto request);
    Task<UserDto?> GetByIdAsync(int id);
    Task<UserDto> CreateAsync(UserDto dto);
    Task<bool> UpdateAsync(int id, UserDto dto);
    Task<bool> DeleteAsync(int id);
    Task<IEnumerable<SelectItemDto>> GetSelectListAsync();
}
