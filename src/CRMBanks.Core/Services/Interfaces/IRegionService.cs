using CRMBanks.Core.Dtos;

namespace CRMBanks.Core.Services.Interfaces;

public interface IRegionService
{
    Task<IEnumerable<RegionDto>> GetAllAsync();
    Task<PagedResponseDto<RegionDto>> GetPagedAsync(PagedRequestDto request);
    Task<RegionDto?> GetByIdAsync(int id);
    Task<RegionDto> CreateAsync(RegionDto dto);
    Task<bool> UpdateAsync(int id, RegionDto dto);
    Task<bool> DeleteAsync(int id);
    Task<IEnumerable<SelectItemDto>> GetSelectListAsync();
}
