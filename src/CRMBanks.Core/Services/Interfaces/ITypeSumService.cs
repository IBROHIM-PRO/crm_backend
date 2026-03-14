using CRMBanks.Core.Dtos;

namespace CRMBanks.Core.Services.Interfaces;

public interface ITypeSumService
{
    Task<IEnumerable<TypeSumDto>> GetAllAsync();
    Task<PagedResponseDto<TypeSumDto>> GetPagedAsync(PagedRequestDto request);
    Task<TypeSumDto?> GetByIdAsync(int id);
    Task<TypeSumDto> CreateAsync(TypeSumDto dto);
    Task<bool> UpdateAsync(int id, TypeSumDto dto);
    Task<bool> DeleteAsync(int id);
    Task<IEnumerable<SelectItemDto>> GetSelectListAsync();
}
