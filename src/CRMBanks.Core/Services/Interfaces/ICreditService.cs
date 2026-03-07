using CRMBanks.Core.Dtos;

namespace CRMBanks.Core.Services.Interfaces;

public interface ICreditService
{
    Task<IEnumerable<CreditDto>> GetAllByBankAsync(int bankId);
    Task<PagedResponseDto<CreditDto>> GetPagedByBankAsync(int bankId, PagedRequestDto request);
    Task<CreditDto?> GetByIdAsync(int id);
    Task<CreditDto> CreateAsync(CreditDto dto, int bankId);
    Task<bool> UpdateAsync(int id, CreditDto dto);
    Task<bool> DeleteAsync(int id);
    Task<IEnumerable<SelectItemDto>> GetSelectListAsync(int bankId);
}
