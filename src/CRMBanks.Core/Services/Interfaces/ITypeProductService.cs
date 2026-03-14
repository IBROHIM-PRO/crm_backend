using CRMBanks.Core.Dtos;

namespace CRMBanks.Core.Services.Interfaces;

public interface ITypeProductService
{
    Task<IEnumerable<TypeProductDto>> GetAllAsync();
    Task<PagedResponseDto<TypeProductDto>> GetPagedAsync(PagedRequestDto request);
    Task<TypeProductDto?> GetByIdAsync(int id);
    Task<TypeProductDto> CreateAsync(TypeProductDto dto);
    Task<bool> UpdateAsync(int id, TypeProductDto dto);
    Task<bool> DeleteAsync(int id);
    Task<IEnumerable<SelectItemDto>> GetSelectListAsync();
}
