using CRMBanks.Core.Dtos;

namespace CRMBanks.Core.Services.Interfaces;

public interface ICardService
{
    Task<IEnumerable<CardDto>> GetAllAsync();
    Task<PagedResponseDto<CardDto>> GetPagedAsync(PagedRequestDto request);
    Task<CardDto?> GetByIdAsync(int id);
    Task<CardDto> CreateAsync(CardDto dto);
    Task<bool> UpdateAsync(int id, CardDto dto);
    Task<bool> DeleteAsync(int id);
    Task<IEnumerable<SelectItemDto>> GetSelectListAsync();
}
