using CRMBanks.Core.Dtos;

namespace CRMBanks.Core.Services.Interfaces;

public interface IRoleService
{
    Task<IEnumerable<RoleDto>> GetAllAsync();
    Task<PagedResponseDto<RoleDto>> GetPagedAsync(PagedRequestDto request);
    Task<RoleDto?> GetByIdAsync(int id);
    Task<RoleDto> CreateAsync(RoleDto dto);
    Task<bool> UpdateAsync(int id, RoleDto dto);
    Task<bool> DeleteAsync(int id);
}
