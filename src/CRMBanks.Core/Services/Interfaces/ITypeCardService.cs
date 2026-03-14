using CRMBanks.Core.Dtos;

namespace CRMBanks.Core.Services.Interfaces;

public interface ITypeCardService
{
    Task<IEnumerable<TypeCardDto>> GetAllAsync();
    Task<PagedResponseDto<TypeCardDto>> GetPagedAsync(PagedRequestDto request);
    Task<TypeCardDto?> GetByIdAsync(int id);
    Task<TypeCardDto> CreateAsync(TypeCardDto dto);
    Task<bool> UpdateAsync(int id, TypeCardDto dto);
    Task<bool> DeleteAsync(int id);
    Task<IEnumerable<SelectItemDto>> GetSelectListAsync();
}
